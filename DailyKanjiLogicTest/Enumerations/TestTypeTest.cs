using DailyKanjiLogic.Enumerations;
using NUnit.Framework;

namespace DailyKanjiLogicTest.Enumerations
{
    [TestFixture]
    public sealed class TestTypeTest
    {
        [Test]
        public void EnumerationNumberTest()
        {
            Assert.That((byte)TestType.HiraganaOrKatakanaToRoomaji,               Is.EqualTo(0));
            Assert.That((byte)TestType.HiraganaToRoomaji,                         Is.EqualTo(1));
            Assert.That((byte)TestType.KatakanaToRoomaji,                         Is.EqualTo(2));
            Assert.That((byte)TestType.RoomajiToHiraganaOrKatakana,               Is.EqualTo(3));
            Assert.That((byte)TestType.RoomajiToHiragana,                         Is.EqualTo(4));
            Assert.That((byte)TestType.RoomajiToKatakana,                         Is.EqualTo(5));
            Assert.That((byte)TestType.HiraganaToKatakanaOrKatakanaToHiragana,    Is.EqualTo(6));
            Assert.That((byte)TestType.HiraganaToKatakana,                        Is.EqualTo(7));
            Assert.That((byte)TestType.KatakanaToHiragana,                        Is.EqualTo(8));
            Assert.That((byte)TestType.AllToAll,                                  Is.EqualTo(9));
        }
    }
}
