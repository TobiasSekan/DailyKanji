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
#pragma warning disable CS8618 // Das Nicht-Nullable-Feld wurde nicht initialisiert.

        private MainBaseModel _model;

        private MainBaseViewModel _viewModel;

        private TestBaseModel _answer;

        private string _baseColor;

        private string _correctColor;

        private string _errorColor;

        private string _progressBarColor;

        private string _hintColor;

        private string _markedColor;

#pragma warning restore CS8618 // Das Nicht-Nullable-Feld wurde nicht initialisiert.

        [SetUp]
        public void SetUp()
        {
            _baseColor        = "baseColor";
            _progressBarColor = "progressBarColor";
            _correctColor     = "correctColor";
            _errorColor       = "errorColor";
            _markedColor      = "markedColor";
            _hintColor        = "hintColor";

            _model       = new MainBaseModel();
            _viewModel   = new MainBaseViewModel(_model, _baseColor, _progressBarColor);

            _viewModel.PrepareNewTest();

            Assert.That(_model.PossibleAnswers, Has.Exactly(10).Items);

            _answer   = _model.PossibleAnswers.FirstOrDefault();

            Assert.That(_answer, Is.Not.Null);

            _viewModel.SetHighlightColors(_answer, _correctColor, _errorColor, _markedColor, _hintColor);
        }

        [Test]
        public void Test_HintShowType_ShowOnNoAnswers()
        {
            _model.SelectedHintShowType = HintShowType.ShowOnNoAnswers;

            for(var answerNumber = 0; answerNumber < 10; answerNumber++)
            {
                _viewModel.SetHintTextColors(answerNumber, _model.CurrentTest, _markedColor, _hintColor);
            }

            // TODO

            Assert.That(_model.HintTextColor[0], Is.EqualTo(_baseColor));
            Assert.That(_model.HintTextColor[1], Is.EqualTo(_baseColor));
            Assert.That(_model.HintTextColor[2], Is.EqualTo(_baseColor));
            Assert.That(_model.HintTextColor[3], Is.EqualTo(_baseColor));
            Assert.That(_model.HintTextColor[4], Is.EqualTo(_baseColor));
            Assert.That(_model.HintTextColor[5], Is.EqualTo(_baseColor));
            Assert.That(_model.HintTextColor[6], Is.EqualTo(_baseColor));
            Assert.That(_model.HintTextColor[7], Is.EqualTo(_baseColor));
            Assert.That(_model.HintTextColor[8], Is.EqualTo(_baseColor));
            Assert.That(_model.HintTextColor[9], Is.EqualTo(_baseColor));
        }

        // TODO: Add tests for all other HintShowTypes
    }
}
