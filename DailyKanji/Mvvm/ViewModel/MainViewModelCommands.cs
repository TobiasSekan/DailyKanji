using DailyKanji.Helper;
using System;
using System.Linq;
using System.Windows.Input;

namespace DailyKanji.Mvvm.ViewModel
{
    public sealed partial class MainViewModel
    {
        public ICommand AnswerNumber
            => new CommandHelper((parameter) =>
            {
                CheckAnswer(Model.PossibleAnswers.ElementAtOrDefault(Convert.ToInt32(parameter) - 1)?.Roomaji);
            });

        public ICommand ChangeAnswerCount
            => new CommandHelper((value) =>
            {
                Model.MaximumAnswer = Convert.ToByte(value);
                ChooseNewPossibleAnswers();
                BuildAnswerButtons();
            });

        public ICommand ChangeAnswerMode
            => new CommandHelper((_) =>
            {
                ChooseNewPossibleAnswers();
                BuildAnswerButtons();
            });

        public ICommand CloseProgram
            => new CommandHelper((_) => Environment.Exit(0));
    }
}
