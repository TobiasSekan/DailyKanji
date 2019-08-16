using DailyKanji.Mvvm.View;
using DailyKanjiLogic.Enumerations;
using DailyKanjiLogic.Helper;
using DailyKanjiLogic.Mvvm.Model;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;

#nullable enable

namespace DailyKanji.Mvvm.ViewModel
{
    /// <summary>
    /// Partial class of the <see cref="MainViewModel"/> that contains all <see cref="ICommand"/>
    /// </summary>
    internal sealed partial class MainViewModel
    {
        #region Commands - File Menu

        /// <summary>
        /// Command for close the program (same as ALT+F4)
        /// </summary>
        public ICommand CommandCloseProgram
            => new CommandHelperSlim(() => MainWindow?.Close());

        #endregion Commands - File Menu

        #region Commands - View Menu

        /// <summary>
        /// Command for show the sign statistic
        /// </summary>
        public ICommand CommandShowAllSignStatistics
            => new CommandHelperSlim(() => new StatisticsWindow(_baseModel, this).Show());

        /// <summary>
        /// Command for change the hint show type
        /// </summary>
        public ICommand CommandChangeHintShowType
            => new CommandHelper(value
                =>
                {
                    if(!Enum.IsDefined(typeof(HintShowType), value))
                    {
                        Debug.Fail($"{nameof(CommandChangeHintShowType)}: [{value}] is not defined in the [{nameof(HintShowType)}] enumeration");
                        return;
                    }

                    _baseModel.SelectedHintShowType ^= (HintShowType)value;
                });

        #endregion Commands - View Menu

        #region Commands - Settings Menu

        /// <summary>
        /// Command for change hint type
        /// </summary>
        public ICommand CommandChangeHintType
            => new CommandHelper(value
                =>
                {
                    if(!Enum.IsDefined(typeof(HintType), value))
                    {
                        Debug.Fail($"CommandChangeHintType: [{value}] is not defined in the [{nameof(HintType)}] enumeration");
                        return;
                    }

                    _baseModel.SelectedHintType = (HintType)value;
                    _baseViewModel.PrepareNewTest();
                    ShowAndStartNewTest();
                });

        /// <summary>
        /// Command for change the answer count (answer button and answer menu entries)
        /// </summary>
        public ICommand CommandChangeAnswerCount
            => new CommandHelper(value
                =>
                {
                    if(!byte.TryParse(value?.ToString(), out var maximumAnswers))
                    {
                        Debug.Fail($"CommandChangeAnswerCount: Can't parse [{value}] into [byte] value");
                        return;
                    }

                    _baseModel.MaximumAnswers = maximumAnswers;

                    _baseViewModel.ChooseNewPossibleAnswers();
                    BuildAnswerMenuAndButtons();
                });

        /// <summary>
        /// Command for change the answer mode
        /// </summary>
        public ICommand CommandBuildNewAnswers
            => new CommandHelperSlim(()
                =>
                {
                    _baseViewModel.ChooseNewPossibleAnswers();
                    BuildAnswerMenuAndButtons();
                });

        public ICommand CommandUseAnswerTimer
            => new CommandHelperSlim(()
                =>
                {
                    _baseModel.ShowRunningAnswerTimer = _baseModel.UseAnswerTimer;

                    if(_baseModel.UseAnswerTimer)
                    {
                        RestartTestTimer();
                    }
                    else
                    {
                        _model.TestTimer.Stop();
                    }
                });

        #endregion Commands - Settings Menu

        #region Commands - Tests Menu

        /// <summary>
        /// Command for change test type (test direction)
        /// </summary>
        public ICommand CommandChangeTestType
            => new CommandHelper(value
                =>
                {
                    if(!Enum.IsDefined(typeof(TestType), value))
                    {
                        Debug.Fail($"CommandChangeTestType: [{value}] is not defined in the [{nameof(TestType)}] enumeration");
                        return;
                    }

                    _baseModel.SelectedTestType = (TestType)value;
                    _baseViewModel.PrepareNewTest();
                    ShowAndStartNewTest();
                });

        /// <summary>
        /// Command for change kana type
        /// </summary>
        public ICommand CommandChangeKanaType
            => new CommandHelper(value
                =>
                {
                    if(!Enum.IsDefined(typeof(KanaType), value))
                    {
                        Debug.Fail($"CommandChangeKanaType: [{value}] is not defined in the [{nameof(KanaType)}] enumeration");
                        return;
                    }

                    _baseModel.SelectedKanaType ^= (KanaType)value;

                    if(_baseModel.SelectedKanaType == KanaType.None)
                    {
                        _model.TestTimer.Stop();

                        _baseModel.SelectedKanaType = (KanaType)value;

                        MessageBox.Show("At least one Kana type must be selected.",
                                        "Daily Kanji - Information",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Information);
                    }

                    _baseViewModel.PrepareNewTest();
                    ShowAndStartNewTest();
                });

        #endregion Commands - Tests Menu

        #region Commands - Answer Menu

        /// <summary>
        /// Command for select a answer by a number value
        /// </summary>
        public ICommand CommandAnswerTestNumber
            => new CommandHelper(value
                =>
                {
                    if(!byte.TryParse(value?.ToString(), out var answerNumber))
                    {
                        Debug.Fail($"CommandAnswerTestNumber: Can't parse [{value}] into a [byte] value");
                        return;
                    }

                    CheckSelectedAnswer(_baseModel.PossibleAnswers.ElementAtOrDefault(answerNumber - 1));
                });

        #endregion Commands - Answer Menu

        #region Commands - Mark Menu

        /// <summary>
        /// Command to highlight a (possible) wrong answer
        /// </summary>
        public ICommand CommandHighlightAnswer
            => new CommandHelper(value
                =>
                {
                    if(!byte.TryParse(value?.ToString(), out var answerNumber))
                    {
                        Debug.Fail($"CommandHighlightAnswer: Can't parse [{value}] into a [byte] value");
                        return;
                    }

                    HighlightAnswer(_baseModel.PossibleAnswers.ElementAtOrDefault(answerNumber - 1));
                });

        #endregion Commands - Mark Menu

        #region Commands - Statistics Menu

        /// <summary>
        /// Command for reset the statistic
        /// </summary>
        public ICommand CommandRestStatistic
            => new CommandHelper(value
                =>
                {
                    if(!Enum.IsDefined(typeof(ResetType), value))
                    {
                        Debug.Fail($"CommandRestStatistic: [{value}] is not defined in the [{nameof(ResetType)}] enumeration");
                        return;
                    }

                    _model.TestTimer.Stop();

                    if(MessageBox.Show($"Do you really want to delete the statistics?{Environment.NewLine}{Environment.NewLine}Reset type: {value}",
                                       "Delete statistics",
                                       MessageBoxButton.YesNo,
                                       MessageBoxImage.Question)
                       != MessageBoxResult.Yes)
                    {
                        RestartTestTimer();
                        return;
                    }

                    _baseViewModel.ResetCompleteStatistic((ResetType)value);
                    _baseViewModel.PrepareNewTest();
                    ShowAndStartNewTest();
                });

        #endregion Commands - Statistics Menu

        #region Commands - Help

        /// <summary>
        /// Command for show info window
        /// </summary>
        public ICommand CommandShowInfoWindow
            => new CommandHelperSlim(()
                =>
                {
                    _model.TestTimer.Stop();

                    var infoWindow = new InfoWindow(_baseModel, _model);

                    infoWindow.Closed += (_, __) => RestartTestTimer();
                    infoWindow.Show();
                });

        #endregion Commands - Help

        #region Commands - Navigation

        /// <summary>
        /// Command for go to previous test
        /// </summary>
        public ICommand CommandPreviousTest
            => new CommandHelperSlim(() =>
            {
                _baseViewModel.BuildTestPool();
                _baseViewModel.ChooseNewSign(_baseModel.PreviousTest);

                _baseViewModel.ChooseNewPossibleAnswers();
                BuildAnswerMenuAndButtons();
                RestartTestTimer();

                _baseModel.IgnoreInput  = false;
                _baseModel.PreviousTest = TestBaseModel.EmptyTest;
            });

        /// <summary>
        /// Command for go to next test
        /// </summary>
        public ICommand CommandNextTest
            => new CommandHelperSlim(() => CheckSelectedAnswer(TestBaseModel.EmptyTest));

        #endregion Commands - Navigation
    }
}
