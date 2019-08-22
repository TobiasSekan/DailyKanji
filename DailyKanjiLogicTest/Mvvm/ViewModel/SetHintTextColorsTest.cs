using DailyKanjiLogic.Enumerations;
using DailyKanjiLogic.Mvvm.Model;
using DailyKanjiLogic.Mvvm.ViewModel;
using NUnit.Framework;
using System;
using System.Linq;

namespace DailyKanjiLogicTest.Mvvm.ViewModel
{
    [TestFixture]
    public sealed class SetHintTextColorsTest
    {
#pragma warning disable CS8618

        private MainBaseModel _model;

        private MainBaseViewModel _viewModel;

        private TestBaseModel _correctAnswer;

        private TestBaseModel _wrongAnswer;

        private int _correctAnswerNumber;

        private int _wrongAnswerNumber;

#pragma warning restore CS8618

        private const string _transparentColor = "transparentColor";

        private const string _hintColor = "hintColor";

        private const string _markedColor = "markedColor";

        [SetUp]
        public void SetUp()
        {
            _model = new MainBaseModel
            {
                MaximumAnswers = 10
            };

            _viewModel = new MainBaseViewModel(_model, _transparentColor, "progressBarColor");
            _viewModel.PrepareNewTest();

            Assert.That(_model.HintTextColor.Count, Is.EqualTo(10));
            Assert.That(_model.AnswerButtonColor.Count, Is.EqualTo(10));
            Assert.That(_model.PossibleAnswers.Count, Is.EqualTo(10));

            var randomCounter  = 0;
            var random         = new Random();
            var fullAnswerList = _model.PossibleAnswers.ToList();

            _correctAnswer       = _model.CurrentTest;
            _correctAnswerNumber = fullAnswerList.IndexOf(_correctAnswer);

            do
            {
                _wrongAnswerNumber = random.Next(fullAnswerList.Count);
                randomCounter++;
            }
            while(randomCounter < 10 || _wrongAnswerNumber == _correctAnswerNumber);

            _wrongAnswer = fullAnswerList.ElementAtOrDefault(_wrongAnswerNumber);

            Assert.That(_correctAnswerNumber, Is.GreaterThanOrEqualTo(0));
            Assert.That(_wrongAnswerNumber, Is.GreaterThanOrEqualTo(0));

            Assert.That(_correctAnswerNumber, Is.LessThanOrEqualTo(9));
            Assert.That(_wrongAnswerNumber, Is.LessThanOrEqualTo(9));

            Assert.That(_correctAnswer, Is.Not.Null);
            Assert.That(_wrongAnswer, Is.Not.Null);

            _model.AnswerButtonColor[0] = _markedColor;
            _model.AnswerButtonColor[2] = _markedColor;
            _model.AnswerButtonColor[4] = _markedColor;
            _model.AnswerButtonColor[6] = _markedColor;
            _model.AnswerButtonColor[8] = _markedColor;

            Assert.That(_model.AnswerButtonColor.Count(found => found == _markedColor), Is.EqualTo(5));
            Assert.That(_model.AnswerButtonColor.Count(found => found == _transparentColor), Is.EqualTo(5));
        }

        [Test]
        public void Test_HintShowType_ShowOnNoAnswers_WithCorrectAnswer()
        {
            _model.SelectedHintShowType = HintShowType.ShowOnNoAnswers;

            for(var answerNumber = 0; answerNumber < 10; answerNumber++)
            {
                _viewModel.SetHintTextColors(answerNumber, _correctAnswer, _markedColor, _hintColor);
            }

            Assert.That(_model.HintTextColor.Count(found => found == _transparentColor), Is.EqualTo(10), nameof(_transparentColor));
            Assert.That(_model.HintTextColor.Count(found => found == _hintColor), Is.Zero, nameof(_hintColor));

            Assert.That(_model.HintTextColor[_correctAnswerNumber], Is.EqualTo(_transparentColor));
            Assert.That(_model.HintTextColor[_wrongAnswerNumber], Is.EqualTo(_transparentColor));
        }

        [Test]
        public void Test_HintShowType_ShowOnNoAnswers_WithWrongAnswer()
        {
            _model.SelectedHintShowType = HintShowType.ShowOnNoAnswers;

            for(var answerNumber = 0; answerNumber < 10; answerNumber++)
            {
                _viewModel.SetHintTextColors(answerNumber, _wrongAnswer, _markedColor, _hintColor);
            }

            Assert.That(_model.HintTextColor.Count(found => found == _transparentColor), Is.EqualTo(10), nameof(_transparentColor));
            Assert.That(_model.HintTextColor.Count(found => found == _hintColor), Is.Zero, nameof(_hintColor));

            Assert.That(_model.HintTextColor[_correctAnswerNumber], Is.EqualTo(_transparentColor));
            Assert.That(_model.HintTextColor[_wrongAnswerNumber], Is.EqualTo(_transparentColor));
        }

        [Test]
        public void Test_HintShowType_ShowOnCorrectAnswer_WithCorrectAnswer()
        {
            _model.SelectedHintShowType = HintShowType.ShowOnCorrectAnswer;

            for(var answerNumber = 0; answerNumber < 10; answerNumber++)
            {
                _viewModel.SetHintTextColors(answerNumber, _correctAnswer, _markedColor, _hintColor);
            }

            Assert.That(_model.HintTextColor.Count(found => found == _transparentColor), Is.EqualTo(9), nameof(_transparentColor));
            Assert.That(_model.HintTextColor.Count(found => found == _hintColor), Is.EqualTo(1), nameof(_hintColor));

            Assert.That(_model.HintTextColor[_correctAnswerNumber], Is.EqualTo(_hintColor));
            Assert.That(_model.HintTextColor[_wrongAnswerNumber], Is.EqualTo(_transparentColor));
        }

        [Test]
        public void Test_HintShowType_ShowOnCorrectAnswer_WithWrongAnswer()
        {
            _model.SelectedHintShowType = HintShowType.ShowOnCorrectAnswer;

            for(var answerNumber = 0; answerNumber < 10; answerNumber++)
            {
                _viewModel.SetHintTextColors(answerNumber, _correctAnswer, _markedColor, _hintColor);
            }

            Assert.That(_model.HintTextColor.Count(found => found == _transparentColor), Is.EqualTo(9), nameof(_transparentColor));
            Assert.That(_model.HintTextColor.Count(found => found == _hintColor), Is.EqualTo(1), nameof(_hintColor));

            Assert.That(_model.HintTextColor[_correctAnswerNumber], Is.EqualTo(_hintColor));
            Assert.That(_model.HintTextColor[_wrongAnswerNumber], Is.EqualTo(_transparentColor));
        }

        [Test]
        public void Test_HintShowType_ShowOnWrongAnswer_WithCorrectAnswer()
        {
            _model.SelectedHintShowType = HintShowType.ShowOnWrongAnswer;

            for(var answerNumber = 0; answerNumber < 10; answerNumber++)
            {
                _viewModel.SetHintTextColors(answerNumber, _correctAnswer, _markedColor, _hintColor);
            }

            Assert.That(_model.HintTextColor.Count(found => found == _transparentColor), Is.EqualTo(10), nameof(_transparentColor));
            Assert.That(_model.HintTextColor.Count(found => found == _hintColor), Is.Zero, nameof(_hintColor));

            Assert.That(_model.HintTextColor[_correctAnswerNumber], Is.EqualTo(_transparentColor));
            Assert.That(_model.HintTextColor[_wrongAnswerNumber], Is.EqualTo(_transparentColor));
        }

        [Test]
        public void Test_HintShowType_ShowOnWrongAnswer_WithWrongAnswer()
        {
            _model.SelectedHintShowType = HintShowType.ShowOnWrongAnswer;

            for(var answerNumber = 0; answerNumber < 10; answerNumber++)
            {
                _viewModel.SetHintTextColors(answerNumber, _wrongAnswer, _markedColor, _hintColor);
            }

            Assert.That(_model.HintTextColor.Count(found => found == _transparentColor), Is.EqualTo(9), nameof(_transparentColor));
            Assert.That(_model.HintTextColor.Count(found => found == _hintColor), Is.EqualTo(1), nameof(_hintColor));

            Assert.That(_model.HintTextColor[_correctAnswerNumber], Is.EqualTo(_transparentColor));
            Assert.That(_model.HintTextColor[_wrongAnswerNumber], Is.EqualTo(_hintColor));
        }

        [Test]
        public void Test_HintShowType_ShowOnCorrectAndWrongAnswer_WithCorrectAnswer()
        {
            _model.SelectedHintShowType = HintShowType.ShowOnCorrectAnswer | HintShowType.ShowOnWrongAnswer;

            for(var answerNumber = 0; answerNumber < 10; answerNumber++)
            {
                _viewModel.SetHintTextColors(answerNumber, _correctAnswer, _markedColor, _hintColor);
            }

            Assert.That(_model.HintTextColor.Count(found => found == _transparentColor), Is.EqualTo(9), nameof(_transparentColor));
            Assert.That(_model.HintTextColor.Count(found => found == _hintColor), Is.EqualTo(1), nameof(_hintColor));

            Assert.That(_model.HintTextColor[_correctAnswerNumber], Is.EqualTo(_hintColor));
            Assert.That(_model.HintTextColor[_wrongAnswerNumber], Is.EqualTo(_transparentColor));
        }

        [Test]
        public void Test_HintShowType_ShowOnCorrectAndWrongAnswer_WithWrongAnswer()
        {
            _model.SelectedHintShowType = HintShowType.ShowOnCorrectAnswer | HintShowType.ShowOnWrongAnswer;

            for(var answerNumber = 0; answerNumber < 10; answerNumber++)
            {
                _viewModel.SetHintTextColors(answerNumber, _wrongAnswer, _markedColor, _hintColor);
            }

            Assert.That(_model.HintTextColor.Count(found => found == _transparentColor), Is.EqualTo(8), nameof(_transparentColor));
            Assert.That(_model.HintTextColor.Count(found => found == _hintColor), Is.EqualTo(2), nameof(_hintColor));

            Assert.That(_model.HintTextColor[_correctAnswerNumber], Is.EqualTo(_hintColor));
            Assert.That(_model.HintTextColor[_wrongAnswerNumber], Is.EqualTo(_hintColor));
        }

        [Test]
        public void Test_HintShowType_ShowOnOtherAnswers_WithCorrectAnswer()
        {
            _model.SelectedHintShowType = HintShowType.ShowOnOtherAnswers;

            for(var answerNumber = 0; answerNumber < 10; answerNumber++)
            {
                _viewModel.SetHintTextColors(answerNumber, _correctAnswer, _markedColor, _hintColor);
            }

            Assert.That(_model.HintTextColor.Count(found => found == _transparentColor), Is.EqualTo(5), nameof(_transparentColor));
            Assert.That(_model.HintTextColor.Count(found => found == _hintColor), Is.EqualTo(5), nameof(_hintColor));

            Assert.That(_model.HintTextColor[_correctAnswerNumber], Is.EqualTo(_hintColor));
            Assert.That(_model.HintTextColor[_wrongAnswerNumber], Is.EqualTo(_hintColor));
        }

        [Test]
        public void Test_HintShowType_ShowOnOtherAnswers_WithWrongAnswer()
        {
            _model.SelectedHintShowType = HintShowType.ShowOnOtherAnswers;

            for(var answerNumber = 0; answerNumber < 10; answerNumber++)
            {
                _viewModel.SetHintTextColors(answerNumber, _wrongAnswer, _markedColor, _hintColor);
            }

            Assert.That(_model.HintTextColor.Count(found => found == _transparentColor), Is.EqualTo(4), nameof(_transparentColor));
            Assert.That(_model.HintTextColor.Count(found => found == _hintColor), Is.EqualTo(6), nameof(_hintColor));

            Assert.That(_model.HintTextColor[_correctAnswerNumber], Is.EqualTo(_transparentColor));
            Assert.That(_model.HintTextColor[_wrongAnswerNumber], Is.EqualTo(_hintColor));
        }

        [Test]
        public void Test_HintShowType_ShowOnMarkedAnswers_WithCorrectAnswer()
        {
            _model.SelectedHintShowType = HintShowType.ShowOnMarkedAnswers;

            for(var answerNumber = 0; answerNumber < 10; answerNumber++)
            {
                _viewModel.SetHintTextColors(answerNumber, _correctAnswer, _markedColor, _hintColor);
            }

            Assert.That(_model.HintTextColor.Count(found => found == _transparentColor), Is.EqualTo(5), nameof(_transparentColor));
            Assert.That(_model.HintTextColor.Count(found => found == _hintColor), Is.EqualTo(5), nameof(_hintColor));

            Assert.That(_model.HintTextColor[_correctAnswerNumber], Is.EqualTo(_correctAnswerNumber % 2 == 0 ? _hintColor : _transparentColor));
            Assert.That(_model.HintTextColor[_wrongAnswerNumber], Is.EqualTo(_wrongAnswerNumber % 2 == 0 ? _hintColor : _transparentColor));
        }

        [Test]
        public void Test_HintShowType_ShowOnMarkedAnswers_WithWrongAnswer()
        {
            _model.SelectedHintShowType = HintShowType.ShowOnMarkedAnswers;

            for(var answerNumber = 0; answerNumber < 10; answerNumber++)
            {
                _viewModel.SetHintTextColors(answerNumber, _wrongAnswer, _markedColor, _hintColor);
            }

            Assert.That(_model.HintTextColor.Count(found => found == _transparentColor), Is.EqualTo(5), nameof(_transparentColor));
            Assert.That(_model.HintTextColor.Count(found => found == _hintColor), Is.EqualTo(5), nameof(_hintColor));

            Assert.That(_model.HintTextColor[_correctAnswerNumber], Is.EqualTo(_correctAnswerNumber % 2 == 0 ? _hintColor : _transparentColor));
            Assert.That(_model.HintTextColor[_wrongAnswerNumber], Is.EqualTo(_wrongAnswerNumber % 2 == 0 ? _hintColor : _transparentColor));
        }

        [Test]
        public void Test_HintShowType_ShowOnMarkedAndCorrectAnswers_WithCorrectAnswer()
        {
            _model.SelectedHintShowType = HintShowType.ShowOnMarkedAnswers | HintShowType.ShowOnCorrectAnswer;

            for(var answerNumber = 0; answerNumber < 10; answerNumber++)
            {
                _viewModel.SetHintTextColors(answerNumber, _correctAnswer, _markedColor, _hintColor);
            }

            Assert.That(_model.HintTextColor.Count(found => found == _transparentColor), Is.EqualTo(4), nameof(_transparentColor));
            Assert.That(_model.HintTextColor.Count(found => found == _hintColor), Is.EqualTo(6), nameof(_hintColor));

            Assert.That(_model.HintTextColor[_correctAnswerNumber], Is.EqualTo(_hintColor));
            Assert.That(_model.HintTextColor[_wrongAnswerNumber], Is.EqualTo(_hintColor));
        }

        [Test]
        public void Test_HintShowType_ShowOnMarkedAndCorrectAnswers_WithWrongAnswer()
        {
            _model.SelectedHintShowType = HintShowType.ShowOnMarkedAnswers | HintShowType.ShowOnCorrectAnswer;

            for(var answerNumber = 0; answerNumber < 10; answerNumber++)
            {
                _viewModel.SetHintTextColors(answerNumber, _wrongAnswer, _markedColor, _hintColor);
            }

            Assert.That(_model.HintTextColor.Count(found => found == _transparentColor), Is.EqualTo(5), nameof(_transparentColor));
            Assert.That(_model.HintTextColor.Count(found => found == _hintColor), Is.EqualTo(5), nameof(_hintColor));

            Assert.That(_model.HintTextColor[_correctAnswerNumber], Is.EqualTo(_hintColor));
            Assert.That(_model.HintTextColor[_wrongAnswerNumber], Is.EqualTo(_wrongAnswerNumber % 2 == 0 ? _hintColor : _transparentColor));
        }

        [Test]
        public void Test_HintShowType_ShowOnMarkedAndWrongAnswers_WithCorrectAnswer()
        {
            _model.SelectedHintShowType = HintShowType.ShowOnMarkedAnswers | HintShowType.ShowOnWrongAnswer;

            for(var answerNumber = 0; answerNumber < 10; answerNumber++)
            {
                _viewModel.SetHintTextColors(answerNumber, _correctAnswer, _markedColor, _hintColor);
            }

            Assert.That(_model.HintTextColor.Count(found => found == _transparentColor), Is.EqualTo(4), nameof(_transparentColor));
            Assert.That(_model.HintTextColor.Count(found => found == _hintColor), Is.EqualTo(6), nameof(_hintColor));

            Assert.That(_model.HintTextColor[_correctAnswerNumber], Is.EqualTo(_transparentColor));
            Assert.That(_model.HintTextColor[_wrongAnswerNumber], Is.EqualTo(_hintColor));
        }

        [Test]
        public void Test_HintShowType_ShowOnMarkedAndWrongAnswers_WithWrongAnswer()
        {
            _model.SelectedHintShowType = HintShowType.ShowOnMarkedAnswers | HintShowType.ShowOnWrongAnswer;

            for(var answerNumber = 0; answerNumber < 10; answerNumber++)
            {
                _viewModel.SetHintTextColors(answerNumber, _wrongAnswer, _markedColor, _hintColor);
            }

            Assert.That(_model.HintTextColor.Count(found => found == _transparentColor), Is.EqualTo(_wrongAnswerNumber % 2 == 0 ? 5 : 4), nameof(_transparentColor));
            Assert.That(_model.HintTextColor.Count(found => found == _hintColor), Is.EqualTo(_wrongAnswerNumber % 2 == 0 ? 5 : 6), nameof(_hintColor));

            Assert.That(_model.HintTextColor[_correctAnswerNumber], Is.EqualTo(_correctAnswerNumber % 2 == 0 ? _hintColor : _transparentColor));
            Assert.That(_model.HintTextColor[_wrongAnswerNumber], Is.EqualTo(_hintColor));
        }

        [Test]
        public void Test_HintShowType_ShowOnMarkedAndOtherAnswers_WithCorrectAnswer()
        {
            _model.SelectedHintShowType = HintShowType.ShowOnMarkedAnswers | HintShowType.ShowOnOtherAnswers;

            for(var answerNumber = 0; answerNumber < 10; answerNumber++)
            {
                _viewModel.SetHintTextColors(answerNumber, _correctAnswer, _markedColor, _hintColor);
            }

            Assert.That(_model.HintTextColor.Count(found => found == _transparentColor), Is.EqualTo(1), nameof(_transparentColor));
            Assert.That(_model.HintTextColor.Count(found => found == _hintColor), Is.EqualTo(9), nameof(_hintColor));

            Assert.That(_model.HintTextColor[_correctAnswerNumber], Is.EqualTo(_transparentColor));
            Assert.That(_model.HintTextColor[_wrongAnswerNumber], Is.EqualTo(_hintColor));
        }

        [Test]
        public void Test_HintShowType_ShowOnMarkedAndOtherAnswers_WithWrongAnswer()
        {
            _model.SelectedHintShowType = HintShowType.ShowOnMarkedAnswers | HintShowType.ShowOnOtherAnswers;

            for(var answerNumber = 0; answerNumber < 10; answerNumber++)
            {
                _viewModel.SetHintTextColors(answerNumber, _wrongAnswer, _markedColor, _hintColor);
            }

            Assert.That(_model.HintTextColor.Count(found => found == _transparentColor), Is.EqualTo(2), nameof(_transparentColor));
            Assert.That(_model.HintTextColor.Count(found => found == _hintColor), Is.EqualTo(8), nameof(_hintColor));

            Assert.That(_model.HintTextColor[_correctAnswerNumber], Is.EqualTo(_transparentColor));
            Assert.That(_model.HintTextColor[_wrongAnswerNumber], Is.EqualTo(_transparentColor));
        }

        // TODO: Add tests for all other HintShowTypes
    }
}
