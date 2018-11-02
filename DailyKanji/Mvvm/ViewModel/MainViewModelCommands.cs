using DailyKanji.Enumerations;
using DailyKanji.Helper;
using System;
using System.Linq;
using System.Windows.Input;

namespace DailyKanji.Mvvm.ViewModel
{
    public sealed partial class MainViewModel
    {
        public ICommand NewQuestion
            => new CommandHelper(CreateNewTest);

        public ICommand CloseProgram
            => new CommandHelper(() => _mainWindow.Close());

        public ICommand ChangeTestType
            => new CommandHelper((testType) =>
            {
                Model.MainTestType = testType != null ? (TestType)Convert.ToInt32(testType) : TestType.HiraganaOrKatakanaToRomaji;
                CreateNewTest();
            });

        public ICommand ChangeErrorTimeout
            => new CommandHelper((timeout) => Model.ErrorTimeout = Convert.ToInt32(timeout));

        public ICommand ChangeAnswerCount
            => new CommandHelper((value) =>
            {
                Model.MaximumAnswer = Convert.ToByte(value);
                ChooseNewPossibleAnswers();
                BuildAnswerButtons();
            });

        public ICommand ChangeAnswerMode
            => new CommandHelper(() =>
            {
                ChooseNewPossibleAnswers();
                BuildAnswerButtons();
            });

        public ICommand AnswerNumber
            => new CommandHelper((parameter) => CheckAnswer(Model.PossibleAnswers.ElementAtOrDefault(Convert.ToInt32(parameter) - 1)?.Roomaji));
    }
}
