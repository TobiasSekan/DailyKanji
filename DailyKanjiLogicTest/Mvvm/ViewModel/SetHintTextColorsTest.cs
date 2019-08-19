using DailyKanjiLogic.Enumerations;
using DailyKanjiLogic.Mvvm.Model;
using DailyKanjiLogic.Mvvm.ViewModel;
using NUnit.Framework;
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

        private string _transparentColor;

        private string _progressBarColor;

        private string _hintColor;

        private string _markedColor;

        private int _correctAnswerNumber;

        private int _wrongAnswerNumber;

#pragma warning restore CS8618

        [SetUp]
        public void SetUp()
        {
            _transparentColor= "transparentColor";
            _progressBarColor= "progressBarColor";
            _markedColor     = "markedColor";
            _hintColor       = "hintColor";

            _model     = new MainBaseModel();
            _viewModel = new MainBaseViewModel(_model, _transparentColor, _progressBarColor);

            _viewModel.PrepareNewTest();

            // TODO ???: PossibleAnswers should always 10 ???
            Assert.That(_model.PossibleAnswers.Count, Is.EqualTo(7));

            var answerList = _model.PossibleAnswers.ToList();

            _correctAnswer = _model.CurrentTest;
            _wrongAnswer   = answerList.Find(found => found.Roomaji != _correctAnswer.Roomaji);

            _correctAnswerNumber = answerList.IndexOf(_correctAnswer);
            _wrongAnswerNumber   = answerList.IndexOf(_wrongAnswer);

            Assert.That(_correctAnswer, Is.Not.Null);
        }

        [Test]
        public void Test_HintShowType_ShowOnNoAnswers_WithCorrectAnswer()
        {
            _model.SelectedHintShowType = HintShowType.ShowOnNoAnswers;

            for(var answerNumber = 0; answerNumber < 10; answerNumber++)
            {
                _viewModel.SetHintTextColors(answerNumber, _correctAnswer, _markedColor, _hintColor);
            }

            Assert.That(_model.HintTextColor.Count, Is.EqualTo(10));
            Assert.That(_model.HintTextColor.Count(found => found == _transparentColor), Is.EqualTo(10));
        }

        [Test]
        public void Test_HintShowType_ShowOnNoAnswers_WithWrongAnswer()
        {
            _model.SelectedHintShowType = HintShowType.ShowOnNoAnswers;

            for(var answerNumber = 0; answerNumber < 10; answerNumber++)
            {
                _viewModel.SetHintTextColors(answerNumber, _wrongAnswer, _markedColor, _hintColor);
            }

            Assert.That(_model.HintTextColor.Count, Is.EqualTo(10));
            Assert.That(_model.HintTextColor.Count(found => found == _transparentColor), Is.EqualTo(10));
        }

        [Test]
        public void Test_HintShowType_ShowOnCorrectAnswer_WithCorrectAnswer()
        {
            _model.SelectedHintShowType = HintShowType.ShowOnCorrectAnswer;

            for(var answerNumber = 0; answerNumber < 10; answerNumber++)
            {
                _viewModel.SetHintTextColors(answerNumber, _correctAnswer, _markedColor, _hintColor);
            }

            Assert.That(_model.HintTextColor.Count, Is.EqualTo(10));

            Assert.That(_model.HintTextColor.Count(found => found == _transparentColor), Is.EqualTo(9));
            Assert.That(_model.HintTextColor.Count(found => found == _hintColor), Is.EqualTo(1));

            Assert.That(_model.HintTextColor[_correctAnswerNumber], Is.EqualTo(_hintColor));
        }

        [Test]
        public void Test_HintShowType_ShowOnCorrectAnswer_WithWrongAnswer()
        {
            _model.SelectedHintShowType = HintShowType.ShowOnCorrectAnswer;

            for(var answerNumber = 0; answerNumber < 10; answerNumber++)
            {
                _viewModel.SetHintTextColors(answerNumber, _correctAnswer, _markedColor, _hintColor);
            }

            Assert.That(_model.HintTextColor.Count, Is.EqualTo(10));

            Assert.That(_model.HintTextColor.Count(found => found == _transparentColor), Is.EqualTo(9));
            Assert.That(_model.HintTextColor.Count(found => found == _hintColor), Is.EqualTo(1));

            Assert.That(_model.HintTextColor[_correctAnswerNumber], Is.EqualTo(_hintColor));
        }

        [Test]
        public void Test_HintShowType_ShowOnWrongAnswer_WithCorrectAnswer()
        {
            _model.SelectedHintShowType = HintShowType.ShowOnWrongAnswer;

            for(var answerNumber = 0; answerNumber < 10; answerNumber++)
            {
                _viewModel.SetHintTextColors(answerNumber, _correctAnswer, _markedColor, _hintColor);
            }

            Assert.That(_model.HintTextColor.Count, Is.EqualTo(10));

            Assert.That(_model.HintTextColor.Count(found => found == _transparentColor), Is.EqualTo(10));
            Assert.That(_model.HintTextColor.Count(found => found == _hintColor), Is.EqualTo(0));
        }

        [Test]
        public void Test_HintShowType_ShowOnWrongAnswer_WithWrongAnswer()
        {
            _model.SelectedHintShowType = HintShowType.ShowOnWrongAnswer;

            for(var answerNumber = 0; answerNumber < 10; answerNumber++)
            {
                _viewModel.SetHintTextColors(answerNumber, _wrongAnswer, _markedColor, _hintColor);
            }

            Assert.That(_model.HintTextColor.Count, Is.EqualTo(10));

            Assert.That(_model.HintTextColor.Count(found => found == _transparentColor), Is.EqualTo(9));
            Assert.That(_model.HintTextColor.Count(found => found == _hintColor), Is.EqualTo(1));

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

            Assert.That(_model.HintTextColor.Count, Is.EqualTo(10));

            Assert.That(_model.HintTextColor.Count(found => found == _transparentColor), Is.EqualTo(9));
            Assert.That(_model.HintTextColor.Count(found => found == _hintColor), Is.EqualTo(1));

            Assert.That(_model.HintTextColor[_correctAnswerNumber], Is.EqualTo(_hintColor));
        }

        [Test]
        public void Test_HintShowType_ShowOnCorrectAndWrongAnswer_WithWrongAnswer()
        {
            _model.SelectedHintShowType = HintShowType.ShowOnCorrectAnswer | HintShowType.ShowOnWrongAnswer;

            for(var answerNumber = 0; answerNumber < 10; answerNumber++)
            {
                _viewModel.SetHintTextColors(answerNumber, _wrongAnswer, _markedColor, _hintColor);
            }

            Assert.That(_model.HintTextColor.Count, Is.EqualTo(10));

            Assert.That(_model.HintTextColor.Count(found => found == _transparentColor), Is.EqualTo(8));
            Assert.That(_model.HintTextColor.Count(found => found == _hintColor), Is.EqualTo(2));

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

            Assert.That(_model.HintTextColor.Count, Is.EqualTo(10));

            Assert.That(_model.HintTextColor.Count(found => found == _transparentColor), Is.EqualTo(4));
            Assert.That(_model.HintTextColor.Count(found => found == _hintColor), Is.EqualTo(6));

            Assert.That(_model.HintTextColor[_correctAnswerNumber], Is.EqualTo(_transparentColor));
        }

        [Test]
        public void Test_HintShowType_ShowOnOtherAnswers_WithWrongAnswer()
        {
            _model.SelectedHintShowType = HintShowType.ShowOnOtherAnswers;

            for(var answerNumber = 0; answerNumber < 10; answerNumber++)
            {
                _viewModel.SetHintTextColors(answerNumber, _wrongAnswer, _markedColor, _hintColor);
            }

            Assert.That(_model.HintTextColor.Count, Is.EqualTo(10));

            Assert.That(_model.HintTextColor.Count(found => found == _transparentColor), Is.EqualTo(5));
            Assert.That(_model.HintTextColor.Count(found => found == _hintColor), Is.EqualTo(5));

            Assert.That(_model.HintTextColor[_correctAnswerNumber], Is.EqualTo(_transparentColor));
            Assert.That(_model.HintTextColor[_wrongAnswerNumber], Is.EqualTo(_transparentColor));
        }

        // TODO: Add tests for all other HintShowTypes

        [Test]
        [Ignore("TODO")]
        public void Test_HintShowType_ShowOnMarkedAnswers_WithCorrectAnswer()
        {
            _model.SelectedHintShowType = HintShowType.ShowOnMarkedAnswers;

            _model.AnswerButtonColor[0] = _markedColor;
            _model.AnswerButtonColor[2] = _markedColor;
            _model.AnswerButtonColor[4] = _markedColor;
            _model.AnswerButtonColor[6] = _markedColor;
            _model.AnswerButtonColor[8] = _markedColor;

            for(var answerNumber = 0; answerNumber < 10; answerNumber++)
            {
                _viewModel.SetHintTextColors(answerNumber, _model.CurrentTest, _markedColor, _hintColor);
            }

            // TODO:

            // Assert.That(_model.HintTextColor.Count,                                      Is.EqualTo(5));
            // Assert.That(_model.HintTextColor.Count(found => found == _transparentColor), Is.EqualTo(5));
            // Assert.That(_model.HintTextColor.Count(found => found == _hintColor),        Is.EqualTo(5));

            // Assert.That(_model.HintTextColor[0], Is.EqualTo(_hintColor));
            // Assert.That(_model.HintTextColor[2], Is.EqualTo(_hintColor));
            // Assert.That(_model.HintTextColor[4], Is.EqualTo(_hintColor));

            // Assert.That(_model.HintTextColor[6], Is.EqualTo(_hintColor));
            // Assert.That(_model.HintTextColor[8], Is.EqualTo(_hintColor));

            // Assert.That(_model.HintTextColor[1], Is.EqualTo(_transparentColor));
            // Assert.That(_model.HintTextColor[3], Is.EqualTo(_transparentColor));
            // Assert.That(_model.HintTextColor[5], Is.EqualTo(_transparentColor));
            // Assert.That(_model.HintTextColor[7], Is.EqualTo(_transparentColor));
            // Assert.That(_model.HintTextColor[9], Is.EqualTo(_transparentColor));
        }
    }
}
