using DailyKanjiLogic.Enumerations;
using NUnit.Framework;

namespace DailyKanjiLogicTest.Enumerations
{
    [TestFixture]
    public sealed class ResetTypeTest
    {
        [Test]
        public void EnumerationNumberTest()
        {
            Assert.That((byte)ResetType.All,                  Is.EqualTo(0));
            Assert.That((byte)ResetType.OnlyCorrectAll,       Is.EqualTo(1));
            Assert.That((byte)ResetType.OnlyCorrectHiragana,  Is.EqualTo(2));
            Assert.That((byte)ResetType.OnlyCorrectKatakana,  Is.EqualTo(3));
            Assert.That((byte)ResetType.OnlyWrongAll,         Is.EqualTo(4));
            Assert.That((byte)ResetType.OnlyWrongHiragana,    Is.EqualTo(5));
            Assert.That((byte)ResetType.OnlyWrongKatakana,    Is.EqualTo(6));
        }
    }
}
