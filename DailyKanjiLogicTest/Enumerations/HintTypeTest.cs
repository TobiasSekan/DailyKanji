using DailyKanjiLogic.Enumerations;
using NUnit.Framework;

namespace DailyKanjiLogicTest.Enumerations
{
    [TestFixture]
    public sealed class HintTypeTest
    {
        [Test]
        public void EnumerationNumberTest()
        {
            Assert.That((byte)HintType.BasedOnAskSign,    Is.EqualTo(0));
            Assert.That((byte)HintType.AlwaysInRoomaji,   Is.EqualTo(1));
            Assert.That((byte)HintType.AlwaysInHiragana,  Is.EqualTo(2));
            Assert.That((byte)HintType.AlwaysInKatakana,  Is.EqualTo(3));
        }
    }
}
