using DailyKanjiLogic.Enumerations;
using NUnit.Framework;

namespace DailyKanjiLogicTest.Enumerations
{
    [TestFixture]
    public sealed class HintShowTypeTest
    {
        [Test]
        public void EnumerationNumberTest()
        {
            Assert.That((byte)HintShowType.ShowOnNoAnswers,       Is.EqualTo(0));
            Assert.That((byte)HintShowType.ShowOnWrongAnswer,     Is.EqualTo(1));
            Assert.That((byte)HintShowType.ShowOnRightAnswer,     Is.EqualTo(2));
            Assert.That((byte)HintShowType.ShowOnMarkedAnswers,   Is.EqualTo(4));
            Assert.That((byte)HintShowType.ShowOnOtherAnswers,    Is.EqualTo(8));
        }
    }
}
