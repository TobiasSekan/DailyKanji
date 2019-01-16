using DailyKanji.Helper;
using DailyKanji.Mvvm.Model;
using DailyKanji.Mvvm.View;
using DailyKanjiLogic.Helper;
using DailyKanjiLogic.Mvvm.Model;
using DailyKanjiLogic.Mvvm.ViewModel;
using SharpDX.DirectInput;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DailyKanji.Mvvm.ViewModel
{
    // BUG
    // ---
    // Fix correct counting for wrong answers on test type "RoomajiToHiraganaOrKatakana"

    // Version 1.x
    // -----------
    // TODO: Add test type for all -> "Hiragana, Katakana or Roomaji to Hiragana, Katakana or Roomaji"
    // TODO: Prevent double-click and multi-click on correct answers to avoid wrong next answer
    //       Note: Prevent it direct inside the command handlers
    //
    // TODO: On similar answers, in some circumstance it is easy to direct find the correct answer
    //       we need a prevention for this
    //
    //       Maybe: Only the first character or last character must are the same on less then five answers
    //
    // TODO: Add similar list for each Hiragana and each Katakana character for option "Similar answers"
    // TODO: Change test order so that all tests will be ask (based on ask counter)
    // TODO: Add more menu underscores (for menu keyboard navigation)
    // TODO: Make refresh interval for timer changeable via menu

    // Version 2.x
    // -----------
    // TODO: Add command line project in .Net Core 2.1 (usable under Windows, Linux, macOS)
    // TODO: Add German language and language selector in menu
    // TODO: Add tooltips for each menu entries
    // TODO: Make colours choose-able
    // TODO: Export statistics (CSV, JSON, XML)

    // Version 3.x
    // -----------
    // TODO: Start with integration of Kanji tests

    // Ideas
    // -----
    // TODO: Export statistics (XLSX)
    // TODO: Import statistics (XLSX, CSV, JSON, XML)
    // TODO: Gamepad support
    // TODO: Ribbon menu
    // TODO: Investigate in WPF - FlowDocument (for integrated zooming features)
    // TODO: Auto update program
    // TODO: .Net Xamarin version for Andorid and iOS

    public sealed partial class MainViewModel : MainBaseViewModel
    {
        #region Private Properties

        /// <summary>
        /// The name of the settings file (this file contains all settings and statistics)
        /// </summary>
        private string _settingFileName
            => "settings.json";

        /// <summary>
        /// The colour string for the progress bar
        /// </summary>
        private string _progressBarColor
            => Colors.LightBlue.ToString();

        /// <summary>
        /// The colour string for the error highlight
        /// </summary>
        private string _errorColor
            => Colors.LightCoral.ToString();

        /// <summary>
        /// The colour string for the correct answer (on error highlight)
        /// </summary>
        private string _correctColor
            => Colors.LightGreen.ToString();

        /// <summary>
        /// The colour string for invisible text and elements
        /// </summary>
        private string _transparentColor
            => Colors.Transparent.ToString();

        /// <summary>
        /// The colour string for the answer hints (on error highlight)
        /// </summary>
        private string _hintColor
            => Colors.Black.ToString();

        /// <summary>
        /// The main viewable window of this application
        /// </summary>
        private MainWindow _mainWindow { get; }

        #endregion Private Properties

        #region Public Properties

        public MainModel Model { get; }

        #endregion Public Properties

        #region Public Constructors

        internal MainViewModel()
        {
            //GamepadTest();

            if(!TryLoadSettings(_settingFileName, out var loadException) && !(loadException is FileNotFoundException))
            {
                MessageBox.Show($"Can't load settings{Environment.NewLine}{Environment.NewLine}{loadException}",
                                $"Error on save {_settingFileName}",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }

            Model = new MainModel
            {
                TestTimer           = new Timer(15),
                ErrorHighlightTimer = new Timer(BaseModel.ErrorTimeout)
                {
                    AutoReset = false
                }
            };

            InitalizieBaseModel(_transparentColor, _progressBarColor);

            Model.TestTimer.Elapsed += (_, __) =>
            {
                if(!BaseModel.UseAnswerTimer)
                {
                    return;
                }

                BaseModel.CurrentAnswerTime = (DateTime.UtcNow - BaseModel.TestStartTime).TotalMilliseconds;

                if(BaseModel.CurrentAnswerTime < BaseModel.MaximumAnswerTimeout)
                {
                    return;
                }

                Model.TestTimer.Stop();
                CheckSelectedAnswer(TestBaseModel.EmptyTest);
            };

            Model.ErrorHighlightTimer.Elapsed += (_, __) =>
            {
                Model.ErrorHighlightTimer.Stop();
                _mainWindow.Dispatcher.Invoke(new Action(() => SetNormalColors(_transparentColor, _progressBarColor)));
                CreateNewTest();
                return;
            };

            _mainWindow = new MainWindow(this);

            CheckForNewVersion();

            CreateNewTest();
            SetNormalColors(_transparentColor, _progressBarColor);

            _mainWindow.Closed += (_, __) =>
            {
                if(TrySaveSettings(_settingFileName, out var saveException))
                {
                    return;
                }

                MessageBox.Show($"Can't save settings{Environment.NewLine}{Environment.NewLine}{saveException}",
                                $"Error on save {_settingFileName}",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            };

            _mainWindow.Show();
        }

        #endregion Public Constructors

        #region Internal Methods

        /// <summary>
        /// Create a new test with new question and new possible answers
        /// </summary>
        internal void CreateNewTest()
        {
            OrderAllTests();
            BuildTestPool();
            ChooseNewSign(GetRandomKanaTest());
            ChooseNewPossibleAnswers();
            BuildAnswerMenuAndButtons();
            StartTestTimer();

            BaseModel.IgnoreInput   = false;
        }

        /// <summary>
        /// Check if the given answer is correct
        /// </summary>
        /// <param name="answer">The answer to check</param>
        /// <exception cref="ArgumentNullException"></exception>
        internal void CheckSelectedAnswer(in TestBaseModel answer)
        {
            Debug.Assert(answer != null, "Answer can't be null for check selected answer");

            if(BaseModel.IgnoreInput)
            {
                return;
            }

            Model.TestTimer.Stop();

            if(CheckAnswer(answer))
            {
                CreateNewTest();
                return;
            }

            if(!BaseModel.HighlightOnErrors)
            {
                CreateNewTest();
                return;
            }

            _mainWindow.Dispatcher.Invoke(new Action(() =>
            {
                SetHighlightColors(_correctColor, _errorColor, _hintColor);
                BuildAnswerMenuAndButtons();
                Model.ErrorHighlightTimer.Start();
            }));
        }

        /// <summary>
        /// Build all answer menu entries and buttons
        /// </summary>
        internal void BuildAnswerMenuAndButtons()
        {
            _mainWindow?.Dispatcher?.Invoke(new Action(() =>
            {
                _mainWindow.AnswerMenu.Items.Clear();

                for(byte answerNumber = 0; answerNumber < 10; answerNumber++)
                {
                    if(answerNumber < BaseModel.MaximumAnswers)
                    {
                        _mainWindow._answerButtonColumn[answerNumber].Width           = new GridLength(1, GridUnitType.Star);

                        _mainWindow._buttonList[answerNumber].Visibility              = Visibility.Visible;
                        _mainWindow._answerShortCutTextBlock[answerNumber].Visibility = Visibility.Visible;
                        _mainWindow._answerHintTextBlock[answerNumber].Visibility     = Visibility.Visible;

                        _mainWindow._answerTextList[answerNumber].Text                = GetAnswerText(answerNumber);
                        _mainWindow._answerHintTextBlock[answerNumber].Text           = GetAnswerHint(answerNumber);
                        _mainWindow._answerShortCutTextBlock[answerNumber].Text       = BaseModel.ShowAnswerShortcuts
                                                                                            ? $"{answerNumber + 1}"
                                                                                            : string.Empty;

                        _mainWindow.AnswerMenu.Items.Add(new MenuItem
                        {
                            Command          = CommandAnswerTest,
                            CommandParameter = BaseModel.PossibleAnswers.ElementAtOrDefault(answerNumber),
                            Header           = GetAnswerText(answerNumber),
                            InputGestureText = $"{answerNumber + 1}"
                        });
                    }
                    else
                    {
                        _mainWindow._answerButtonColumn[answerNumber].Width           = GridLength.Auto;

                        _mainWindow._buttonList[answerNumber].Visibility              = Visibility.Collapsed;
                        _mainWindow._answerShortCutTextBlock[answerNumber].Visibility = Visibility.Collapsed;
                        _mainWindow._answerHintTextBlock[answerNumber].Visibility     = Visibility.Collapsed;

                        _mainWindow._answerTextList[answerNumber].Text                = string.Empty;
                        _mainWindow._answerHintTextBlock[answerNumber].Text           = string.Empty;
                        _mainWindow._answerShortCutTextBlock[answerNumber].Text       = string.Empty;
                    }
                }
            }));
        }

        /// <summary>
        /// Start the test timer (Start time is <see cref="DateTime.UtcNow"/>)
        /// </summary>
        internal void StartTestTimer()
        {
            if(!BaseModel.UseAnswerTimer)
            {
                return;
            }

            BaseModel.TestStartTime = DateTime.UtcNow;
            Model.TestTimer.Start();
        }

        /// <summary>
        /// Check if a new version online
        /// </summary>
        internal void CheckForNewVersion()
        {
            if(!BaseModel.CheckForNewVersionOnStartUp)
            {
                return;
            }

            try
            {
                var yourVersion = AssemblyHelper.GetAssemblyVersion(this);

                var onlineVersion = OnlineResourceHelper.GetVersion(
                                        "https://raw.githubusercontent.com/TobiasSekan/DailyKanji/master/DailyKanji/Properties/AssemblyInfo.cs");

                if(yourVersion.Equals(onlineVersion))
                {
                    return;
                }

                if(MessageBox.Show($"A new version of Daily Kanji is available.{Environment.NewLine}{Environment.NewLine}"
                                   + $"Your version:\t{yourVersion}{Environment.NewLine}"
                                   + $"Online version:\t{onlineVersion}{Environment.NewLine}{Environment.NewLine}"
                                   + "Do you want to go to website to download it?",
                                   "Version check",
                                   MessageBoxButton.YesNo,
                                   MessageBoxImage.Information) != MessageBoxResult.Yes)
                {
                    return;
                }

                Process.Start("https://github.com/TobiasSekan/DailyKanji/releases");
            }
            catch(Exception exception)
            {
                MessageBox.Show($"Can't check for updates{Environment.NewLine}{Environment.NewLine}{exception}",
                                "Error on check for updates",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        private void GamepadTest()
        {
            var directInput = new DirectInput();

            var gamepad = directInput.GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly).FirstOrDefault();
            if(gamepad == null)
            {
                Debug.WriteLine("No gamepad found");
                return;
            }

            Debug.WriteLine("Found gamepad");
            Debug.WriteLine($"Instance: {gamepad.InstanceGuid} - {gamepad.InstanceName}");
            Debug.WriteLine($" Product: {gamepad.ProductGuid} - {gamepad.ProductName}");
            Debug.WriteLine($" Type: {gamepad.Type} - {gamepad.Subtype}");
            Debug.WriteLine($" Usage: {gamepad.Usage} - {gamepad.UsagePage}");

            var joystick = new Joystick(directInput, gamepad.InstanceGuid);
            joystick.Acquire();

            Task.Run(() =>
            {
                bool button1;
                bool button2;
                bool button3;
                bool button4;
                bool button5;
                bool button6;
                bool button7;
                bool button8;

                var button1Last = false;
                var button2Last = false;
                var button3Last = false;
                var button4Last = false;
                var button5Last = false;
                var button6Last = false;
                var button7Last = false;
                var button8Last = false;

                while(true)
                {
                    System.Threading.Thread.Sleep(200);

                    var joystickState = joystick.GetCurrentState();

                    button1 = joystickState.Buttons.ElementAtOrDefault(0);
                    button2 = joystickState.Buttons.ElementAtOrDefault(1);
                    button3 = joystickState.Buttons.ElementAtOrDefault(2);
                    button4 = joystickState.Buttons.ElementAtOrDefault(3);
                    button5 = joystickState.Buttons.ElementAtOrDefault(4);
                    button6 = joystickState.Buttons.ElementAtOrDefault(5);
                    button7 = joystickState.Buttons.ElementAtOrDefault(6);
                    button8 = joystickState.Buttons.ElementAtOrDefault(7);

                    if(button1 == button1Last
                    && button2 == button2Last
                    && button3 == button3Last
                    && button4 == button4Last
                    && button5 == button5Last
                    && button6 == button6Last
                    && button7 == button7Last
                    && button8 == button8Last)
                    {
                        continue;
                    }

                    button1Last = button1;
                    button2Last = button2;
                    button3Last = button3;
                    button4Last = button4;
                    button5Last = button5;
                    button6Last = button6;
                    button7Last = button7;
                    button8Last = button8;

                    Debug.WriteLine($"Button 1: {button1}");
                    Debug.WriteLine($"Button 2: {button2}");
                    Debug.WriteLine($"Button 3: {button3}");
                    Debug.WriteLine($"Button 4: {button4}");
                    Debug.WriteLine($"Button 5: {button5}");
                    Debug.WriteLine($"Button 6: {button6}");
                    Debug.WriteLine($"Button 7: {button7}");
                    Debug.WriteLine($"Button 8: {button8}");
                }
            });
        }

        #endregion Internal Methods
    }
}
