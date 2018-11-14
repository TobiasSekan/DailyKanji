using DailyKanji.Enumerations;
using DailyKanji.Helper;
using DailyKanji.Mvvm.Model;
using System;
using System.Linq;
using System.Windows.Input;

namespace DailyKanji.Mvvm.ViewModel
{
    public sealed partial class MainViewModel
    {
        public ICommand CloseProgram
            => new CommandHelper(() => _mainWindow.Close());

        public ICommand ChangeHintVisibility
            => new CommandHelper(() => Model.ShowHints = !Model.ShowHints);

        public ICommand ChangeTestType
            => new CommandHelper(testType
                =>
                {
                    Model.MainTestType = testType != null ? (TestType)Convert.ToInt32(testType) : TestType.HiraganaOrKatakanaToRoomaji;
                    CreateNewTest();
                });

        public ICommand ChangeErrorTimeout
            => new CommandHelper(timeout => Model.ErrorTimeout = Convert.ToInt32(timeout));

        public ICommand ChangeAnswerCount
            => new CommandHelper(value
                =>
                {
                    Model.MaximumAnswer = Convert.ToByte(value);
                    ChooseNewPossibleAnswers();
                    BuildAnswerMenuAndButtons();
                });

        public ICommand ChangeAnswerMode
            => new CommandHelper(()
                =>
                {
                    ChooseNewPossibleAnswers();
                    BuildAnswerMenuAndButtons();
                });

        public ICommand AnswerTest
            => new CommandHelper(parameter => CheckAnswer(parameter as TestBaseModel));

        public ICommand AnswerNumber
            => new CommandHelper(parameter
                =>
                {
                    if(!int.TryParse(parameter.ToString(), out var answerNumber))
                    {
                        return;
                    }

                    CheckAnswer(Model.PossibleAnswers.ElementAtOrDefault(answerNumber - 1));
                });

        public ICommand RestCompleteStatistic
            => new CommandHelper(() =>
            {
                ResetCompleteStatistic();
                CreateNewTest();
            });

        public ICommand PreviousTest
            => new CommandHelper(() =>
            {
                if(Model.LastTest == null)
                {
                    return;
                }

                BuildNewQuestionList();
                ChooseNewSign(Model.LastTest);

                ChooseNewPossibleAnswers();
                BuildAnswerMenuAndButtons();

                Model.IgnoreInput   = false;
                Model.LastTest      = null;
                Model.TestStartTime = DateTime.UtcNow;
            });

        public ICommand NextTest
            => new CommandHelper(() => CheckAnswer(new TestBaseModel(string.Empty, string.Empty, string.Empty)));
    }
}
