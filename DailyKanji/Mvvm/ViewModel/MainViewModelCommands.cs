using DailyKanji.Mvvm.View;
using DailyKanjiLogic.Enumerations;
using DailyKanjiLogic.Helper;
using DailyKanjiLogic.Mvvm.Model;
using System;
using System.Linq;
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
            => new CommandHelper(() => _mainWindow.Close());

        #endregion Commands - File Menu

        #region Commands - Settings Menu

        /// <summary>
        /// <see cref="ICommand"/> for change test type (test direction)
        /// </summary>
        public ICommand CommandChangeTestType
            => new CommandHelper(testType
                =>
                {
                    BaseModel.SelectedTestType = testType != null
                         ? (TestType)Convert.ToInt32(testType)
                         : TestType.HiraganaOrKatakanaToRoomaji;

                    CreateNewTest();
                });

        /// <summary>
        /// <see cref="ICommand"/> for change the error timeout
        /// </summary>
        public ICommand CommandChangeErrorTimeout
            => new CommandHelper(timeout => BaseModel.ErrorTimeout = Convert.ToInt32(timeout));

        /// <summary>
        /// <see cref="ICommand"/> for change the answer count
        /// (answer button and answer menu entries)
        /// </summary>
        public ICommand CommandChangeAnswerCount
            => new CommandHelper(value
                =>
                {
                    BaseModel.MaximumAnswers = Convert.ToByte(value);
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
                    BaseModel.MaximumAnswerTimeout = Convert.ToDouble(value);

                    Model.TestTimer.Stop();
                    Model.ProgressBarColor = _progressBarColor;

                    BaseModel.TestStartTime = DateTime.UtcNow;
                    Model.TestTimer.Start();
                });

        /// <summary>
        /// <see cref="ICommand"/> for change the answer mode
        /// </summary>
        public ICommand CommandBuildNewAnswers
            => new CommandHelper(()
                =>
                {
                    ChooseNewPossibleAnswers();
                    BuildAnswerMenuAndButtons();
                });

        #endregion Commands - Settings Menu

        #region Commands - Answer Menu

        /// <summary>
        /// <see cref="ICommand"/> for select a answer by a <see cref="TestBaseModel"/> object
        /// </summary>
        public ICommand CommandAnswerTest
            => new CommandHelper(parameter => CheckAnswer(parameter as TestBaseModel));

        /// <summary>
        /// <see cref="ICommand"/> for select a answer by a number value
        /// </summary>
        public ICommand CommandAnswerTestNumber
            => new CommandHelper(parameter
                =>
                {
                    if(!int.TryParse(parameter.ToString(), out var answerNumber))
                    {
                        return;
                    }

                    CheckAnswer(BaseModel.PossibleAnswers.ElementAtOrDefault(answerNumber - 1));
                });

        #endregion Commands - Answer Menu

        #region Commands - Statistics Menu

        /// <summary>
        /// <see cref="ICommand"/> for reset the complete (all data) statistics
        /// </summary>
        public ICommand CommandRestCompleteStatistic
            => new CommandHelper(() =>
            {
                ResetCompleteStatistic();
                CreateNewTest();
            });

        #endregion Commands - Statistics Menu

        #region Commands - Navigation

        /// <summary>
        /// <see cref="ICommand"/> for go to previous test
        /// </summary>
        public ICommand CommandPreviousTest
            => new CommandHelper(() =>
            {
                if(BaseModel.PreviousTest == null)
                {
                    return;
                }

                BuildTestPool();
                ChooseNewSign(BaseModel.PreviousTest);

                ChooseNewPossibleAnswers();
                BuildAnswerMenuAndButtons();

                BaseModel.IgnoreInput   = false;
                BaseModel.PreviousTest  = null;
                BaseModel.TestStartTime = DateTime.UtcNow;

                Model.TestTimer.Start();
            });

        /// <summary>
        /// <see cref="ICommand"/> for go to next test
        /// </summary>
        public ICommand CommandNextTest
            => new CommandHelper(() => CheckAnswer(new TestBaseModel(string.Empty, string.Empty, string.Empty)));

        #endregion Commands - Navigation

        public ICommand OpenInfoWindow
            => new CommandHelper(() => new InfoWindow(this).Show());
    }
}
