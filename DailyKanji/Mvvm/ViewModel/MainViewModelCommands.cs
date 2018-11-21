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
                    Model.SelectedTestType = testType != null
                         ? (TestType)Convert.ToInt32(testType)
                         : TestType.HiraganaOrKatakanaToRoomaji;

                    CreateNewTest();
                });

        /// <summary>
        /// <see cref="ICommand"/> for change the error timeout
        /// </summary>
        public ICommand CommandChangeErrorTimeout
            => new CommandHelper(timeout => Model.ErrorTimeout = Convert.ToInt32(timeout));

        /// <summary>
        /// <see cref="ICommand"/> for change the answer count
        /// (answer button and answer menu entries)
        /// </summary>
        public ICommand CommandChangeAnswerCount
            => new CommandHelper(value
                =>
                {
                    Model.MaximumAnswers = Convert.ToByte(value);
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
                    Model.MaximumAnswerTimeout = Convert.ToDouble(value);

                    Model.TestTimer.Stop();
                    Model.ProgressBarColor = _progressBarColor;

                    Model.TestStartTime = DateTime.UtcNow;
                    Model.TestTimer.Start();
                });

        /// <summary>
        /// <see cref="ICommand"/> for change the answer mode
        /// </summary>
        public ICommand CommandChangeSimilarAnswerMode
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

                    CheckAnswer(Model.PossibleAnswers.ElementAtOrDefault(answerNumber - 1));
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
                if(Model.PreviousTest == null)
                {
                    return;
                }

                BuildTestPool();
                ChooseNewSign(Model.PreviousTest);

                ChooseNewPossibleAnswers();
                BuildAnswerMenuAndButtons();

                Model.IgnoreInput   = false;
                Model.PreviousTest  = null;
                Model.TestStartTime = DateTime.UtcNow;

                Model.TestTimer.Start();
            });

        /// <summary>
        /// <see cref="ICommand"/> for go to next test
        /// </summary>
        public ICommand CommandNextTest
            => new CommandHelper(() => CheckAnswer(new TestBaseModel(string.Empty, string.Empty, string.Empty)));

        #endregion Commands - Navigation
    }
}
