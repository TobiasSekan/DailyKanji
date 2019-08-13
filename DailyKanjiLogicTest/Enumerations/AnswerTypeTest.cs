using DailyKanjiLogic.Enumerations;
using NUnit.Framework;

namespace DailyKanjiLogicTest.Enumerations
{
    [TestFixture]
    public sealed class AnswerTypeTest
    {
        [Test]
        public void EnumerationNumberTest()
        {
            Assert.That((byte)AnswerType.Unknown,     Is.EqualTo(0));
            Assert.That((byte)AnswerType.Roomaji,     Is.EqualTo(1));
            Assert.That((byte)AnswerType.Hiragana,    Is.EqualTo(2));
            Assert.That((byte)AnswerType.Katakana,    Is.EqualTo(3));
        }
    }
}
