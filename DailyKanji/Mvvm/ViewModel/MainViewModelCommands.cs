using DailyKanji.Mvvm.View;
using DailyKanjiLogic.Enumerations;
using DailyKanjiLogic.Helper;
using DailyKanjiLogic.Mvvm.Model;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace DailyKanji.Mvvm.ViewModel
{
    public sealed partial class MainViewModel
    {
        #region Commands - File Menu

        /// <summary>
        /// <see cref="ICommand"/> for close the program (same as ALT+F4)
        /// </summary>
        public ICommand CommandCloseProgram
            => new CommandHelperSlim(_mainWindow.Close);

        #endregion Commands - File Menu

        #region Commands - View Menu

        public ICommand CommandOpenSignStatistics
            => new CommandHelperSlim(new StatisticsWindow(this).Show);

        #endregion Commands - View Menu

        #region Commands - Settings Menu

        /// <summary>
        /// <see cref="ICommand"/> for change hint type
        /// </summary>
        public ICommand CommandChangeHintType
            => new CommandHelper(value
                =>
                {
                    if(!Enum.IsDefined(typeof(HintType), value))
                    {
                        return;
                    }

                    BaseModel.SelectedHintType = (HintType)value;
                    CreateNewTest();
                });

        /// <summary>
        /// <see cref="ICommand"/> for change the error timeout
        /// </summary>
        public ICommand CommandChangeErrorTimeout
            => new CommandHelper(value
                =>
                {
                    if(!int.TryParse(value?.ToString(), out var timeout))
                    {
                        return;
                    }

                    BaseModel.ErrorTimeout = timeout;
                });

        /// <summary>
        /// <see cref="ICommand"/> for change the answer count
        /// (answer button and answer menu entries)
        /// </summary>
        public ICommand CommandChangeAnswerCount
            => new CommandHelper(value
                =>
                {
                    if(!byte.TryParse(value?.ToString(), out var maximumAnswers))
                    {
                        return;
                    }

                    BaseModel.MaximumAnswers = maximumAnswers;

                    ChooseNewPossibleAnswers();
                    BuildAnswerMenuAndButtons();
                });

        /// <summary>
        /// <see cref="ICommand"/> for change the maximum (running) answer timeout
        /// </summary>
        public ICommand CommandChangeMaximumAswerTimeout
            => new CommandHelper(value
                =>
                {
                    if(!double.TryParse(value?.ToString(), out var maximumAnswerTimeout))
                    {
                        return;
                    }

                    BaseModel.MaximumAnswerTimeout = maximumAnswerTimeout;

                    Model.TestTimer.Stop();
                    BaseModel.ProgressBarColor = _progressBarColor;

                    StartTestTimer();
                });

        /// <summary>
        /// <see cref="ICommand"/> for change the answer mode
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
                        StartTestTimer();
                    }
                    else
                    {
                        Model.TestTimer.Stop();
                    }
                });

        #endregion Commands - Settings Menu

        #region Commands - Tests Menu

        /// <summary>
        /// <see cref="ICommand"/> for change test type (test direction)
        /// </summary>
        public ICommand CommandChangeTestType
            => new CommandHelper(value
                =>
                {
                    if(!Enum.IsDefined(typeof(TestType), value))
                    {
                        return;
                    }

                    BaseModel.SelectedTestType = (TestType)value;
                    CreateNewTest();
                });

        /// <summary>
        /// <see cref="ICommand"/> for change kana type
        /// </summary>
        public ICommand CommandChangeKanaType
            => new CommandHelper(value
                =>
                {
                    if(!Enum.IsDefined(typeof(KanaType), value))
                    {
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
        /// <see cref="ICommand"/> for select a answer by a <see cref="TestBaseModel"/> object
        /// </summary>
        public ICommand CommandAnswerTest
            => new CommandHelper(value => CheckSelectedAnswer(value as TestBaseModel));

        /// <summary>
        /// <see cref="ICommand"/> for select a answer by a number value
        /// </summary>
        public ICommand CommandAnswerTestNumber
            => new CommandHelper(value
                =>
                {
                    if(!byte.TryParse(value?.ToString(), out var answerNumber))
                    {
                        return;
                    }

                    CheckSelectedAnswer(BaseModel.PossibleAnswers.ElementAtOrDefault(answerNumber - 1));
                });

        #endregion Commands - Answer Menu

        #region Commands - Statistics Menu

        /// <summary>
        /// <see cref="ICommand"/> for reset the statistic
        /// </summary>
        public ICommand CommandRestStatistic
            => new CommandHelper(value
                =>
                {
                    if(!Enum.IsDefined(typeof(ResetType), value))
                    {
                        return;
                    }

                    Model.TestTimer.Stop();

                    if(MessageBox.Show(
                        $"Do you really want to delete the statistics?{Environment.NewLine}{Environment.NewLine}Reset type: {value}",
                        "Delete statistics",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question) != MessageBoxResult.Yes)
                    {
                        StartTestTimer();
                        return;
                    }

                    ResetCompleteStatistic((ResetType)value);
                    CreateNewTest();
                });

        #endregion Commands - Statistics Menu

        #region Commands - Navigation

        /// <summary>
        /// <see cref="ICommand"/> for go to previous test
        /// </summary>
        public ICommand CommandPreviousTest
            => new CommandHelperSlim(() =>
            {
                if(BaseModel.PreviousTest == null)
                {
                    return;
                }

                BuildTestPool();
                ChooseNewSign(BaseModel.PreviousTest);

                ChooseNewPossibleAnswers();
                BuildAnswerMenuAndButtons();
                StartTestTimer();

                BaseModel.IgnoreInput  = false;
                BaseModel.PreviousTest = null;
            });

        /// <summary>
        /// <see cref="ICommand"/> for go to next test
        /// </summary>
        public ICommand CommandNextTest
            => new CommandHelperSlim(() => CheckSelectedAnswer(TestBaseModel.EmptyTest));

        #endregion Commands - Navigation

        #region Commands - Help

        public ICommand OpenInfoWindow
            => new CommandHelperSlim(new InfoWindow(this).Show);

        #endregion Commands - Help
    }
}
