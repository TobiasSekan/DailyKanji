using DailyKanjiLogic.Enumerations;
using NUnit.Framework;

namespace DailyKanjiLogicTest.Enumerations
{
    [TestFixture]
    public sealed class KanaTypeTest
    {
        [Test]
        public void EnumerationNumberTest()
        {
            Assert.That((byte)KanaType.None,                  Is.EqualTo(00));
            Assert.That((byte)KanaType.Gojuuon,               Is.EqualTo(01));
            Assert.That((byte)KanaType.GojuuonWithDakuten,    Is.EqualTo(02));
            Assert.That((byte)KanaType.GojuuonWithHandakuten, Is.EqualTo(04));
            Assert.That((byte)KanaType.Yooon,                 Is.EqualTo(08));
            Assert.That((byte)KanaType.YooonWithDakuten,      Is.EqualTo(16));
            Assert.That((byte)KanaType.YooonWithHandakuten,   Is.EqualTo(32));
        }
    }
}
