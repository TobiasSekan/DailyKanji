using DailyKanji.Mvvm.View;
using DailyKanjiLogic.Enumerations;
using DailyKanjiLogic.Helper;
using DailyKanjiLogic.Mvvm.Model;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace DailyKanji.Mvvm.ViewModel
{
    internal sealed partial class MainViewModel
    {
        #region Commands - File Menu

        /// <summary>
        /// Command for close the program (same as ALT+F4)
        /// </summary>
        public ICommand CommandCloseProgram
            => new CommandHelperSlim(() => MainWindow.Close());

        #endregion Commands - File Menu

        #region Commands - View Menu

        /// <summary>
        /// Command for show the sign statistic
        /// </summary>
        public ICommand CommandShowSignStatistics
            => new CommandHelperSlim(() => new StatisticsWindow(this).Show());

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
                        Debug.Fail($"[{value}] is not defined in the [{nameof(HintType)}] enumeration");
                        return;
                    }

                    BaseModel.SelectedHintType = (HintType)value;
                    CreateNewTest();
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
                        Debug.Fail($"can't parse [{maximumAnswers}] into [byte] value");
                        return;
                    }

                    BaseModel.MaximumAnswers = maximumAnswers;

                    ChooseNewPossibleAnswers();
                    BuildAnswerMenuAndButtons();
                });

        /// <summary>
        /// Command for change the answer mode
        /// </summary>
        public ICommand CommandBuildNewAnswers
            => new CommandHelperSlim(()
                =>
                {
                    ChooseNewPossibleAnswers();
                    BuildAnswerMenuAndButtons();
                });

        public ICommand CommandUseAnswerTimer
            => new CommandHelperSlim(()
                =>
                {
                    BaseModel.ShowRunningAnswerTimer = BaseModel.UseAnswerTimer;

                    if(BaseModel.UseAnswerTimer)
                    {
                        RestartTestTimer();
                    }
                    else
                    {
                        Model.TestTimer.Stop();
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
                        Debug.Fail($"[{value}] is not defined in the [{nameof(TestType)}] enumeration");
                        return;
                    }

                    BaseModel.SelectedTestType = (TestType)value;
                    CreateNewTest();
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
                        Debug.Fail($"[{value}] is not defined in the [{nameof(KanaType)}] enumeration");
                        return;
                    }

                    BaseModel.SelectedKanaType ^= (KanaType)value;

                    if(BaseModel.SelectedKanaType == KanaType.None)
                    {
                        Model.TestTimer.Stop();

                        BaseModel.SelectedKanaType = (KanaType)value;

                        MessageBox.Show("At least one Kana type must be selected.",
                                        "Daily Kanji - Information",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Information);
                    }

                    CreateNewTest();
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
                        Debug.Fail($"can't parse [{answerNumber}] into a [byte] value");
                        return;
                    }

                    CheckSelectedAnswer(BaseModel.PossibleAnswers.ElementAtOrDefault(answerNumber - 1));
                });

        #endregion Commands - Answer Menu

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
                        Debug.Fail($"[{value}] is not defined in the [{nameof(ResetType)}] enumeration");
                        return;
                    }

                    Model.TestTimer.Stop();

                    if(MessageBox.Show($"Do you really want to delete the statistics?{Environment.NewLine}{Environment.NewLine}Reset type: {value}",
                                       "Delete statistics",
                                       MessageBoxButton.YesNo,
                                       MessageBoxImage.Question)
                       != MessageBoxResult.Yes)
                    {
                        RestartTestTimer();
                        return;
                    }

                    ResetCompleteStatistic((ResetType)value);
                    CreateNewTest();
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
                    Model.TestTimer.Stop();

                    var infoWindow = new InfoWindow(this);

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
                if(BaseModel.PreviousTest is null)
                {
                    Debug.Fail($"[{nameof(BaseModel.PreviousTest)}] is [null]");
                    return;
                }

                BuildTestPool();
                ChooseNewSign(BaseModel.PreviousTest);

                ChooseNewPossibleAnswers();
                BuildAnswerMenuAndButtons();
                RestartTestTimer();

                BaseModel.IgnoreInput  = false;
                BaseModel.PreviousTest = null;
            });

        /// <summary>
        /// Command for go to next test
        /// </summary>
        public ICommand CommandNextTest
            => new CommandHelperSlim(() => CheckSelectedAnswer(TestBaseModel.EmptyTest));

        /// <summary>
        /// Command to highlight a (possible) wrong answer
        /// </summary>
        public ICommand CommandHighlightAnswer
            => new CommandHelper(value
                =>
                {
                    if(!byte.TryParse(value?.ToString(), out var answerNumber))
                    {
                        Debug.Fail($"can't parse [{answerNumber}] into a [byte] value");
                        return;
                    }

                    HighlightAnswer(BaseModel.PossibleAnswers.ElementAtOrDefault(answerNumber - 1));
                });

        #endregion Commands - Navigation
    }
}
