using DailyKanjiLogic.Enumerations;
using DailyKanjiLogic.Mvvm.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DailyKanjiLogic.Helper
{
    /// <summary>
    /// Helper class to easier work with kana (Hiragana, Katakana)
    /// </summary>
    public static class KanaHelper
    {
        /// <summary>
        /// Return a list with all possible kana tests (Hiragana, Katakana)
        /// </summary>
        /// <returns>A list with all possible tests</returns>
        public static IEnumerable<TestBaseModel> GetKanaList()
            => new Collection<TestBaseModel>
            {
                // Kana Signs, Think Now How Much You Really Want (to learn them).
                new TestBaseModel("a", "あ", "ア", KanaType.Gojuuon),
                new TestBaseModel("i", "い", "イ", KanaType.Gojuuon),
                new TestBaseModel("u", "う", "ウ", KanaType.Gojuuon),
                new TestBaseModel("e", "え", "エ", KanaType.Gojuuon),
                new TestBaseModel("o", "お", "オ", KanaType.Gojuuon),

                new TestBaseModel("ka", "か", "カ", KanaType.Gojuuon),
                new TestBaseModel("ki", "き", "キ", KanaType.Gojuuon),
                new TestBaseModel("ku", "く", "ク", KanaType.Gojuuon),
                new TestBaseModel("ke", "け", "ケ", KanaType.Gojuuon),
                new TestBaseModel("ko", "こ", "コ", KanaType.Gojuuon),

                new TestBaseModel("sa", "さ", "サ", KanaType.Gojuuon),
                new TestBaseModel("shi","し", "シ", KanaType.Gojuuon),
                new TestBaseModel("su", "す", "ス", KanaType.Gojuuon),
                new TestBaseModel("se", "せ", "セ", KanaType.Gojuuon),
                new TestBaseModel("so", "そ", "ソ", KanaType.Gojuuon),

                new TestBaseModel("ta", "た", "タ", KanaType.Gojuuon),
                new TestBaseModel("chi","ち", "チ", KanaType.Gojuuon),
                new TestBaseModel("tsu","つ", "ツ", KanaType.Gojuuon),
                new TestBaseModel("te", "て", "テ", KanaType.Gojuuon),
                new TestBaseModel("to", "と", "ト", KanaType.Gojuuon),

                new TestBaseModel("na", "な", "ナ", KanaType.Gojuuon),
                new TestBaseModel("ni", "に", "ニ", KanaType.Gojuuon),
                new TestBaseModel("nu", "ぬ", "ヌ", KanaType.Gojuuon),
                new TestBaseModel("ne", "ね", "ネ", KanaType.Gojuuon),
                new TestBaseModel("no", "の", "ノ", KanaType.Gojuuon),

                new TestBaseModel("ha", "は", "ハ", KanaType.Gojuuon),
                new TestBaseModel("hi", "ひ", "ヒ", KanaType.Gojuuon),
                new TestBaseModel("fu", "ふ", "フ", KanaType.Gojuuon),
                new TestBaseModel("he", "へ", "ヘ", KanaType.Gojuuon),
                new TestBaseModel("ho", "ほ", "ホ", KanaType.Gojuuon),

                new TestBaseModel("ma", "ま", "マ", KanaType.Gojuuon),
                new TestBaseModel("mi", "み", "ミ", KanaType.Gojuuon),
                new TestBaseModel("mu", "む", "ム", KanaType.Gojuuon),
                new TestBaseModel("me", "め", "メ", KanaType.Gojuuon),
                new TestBaseModel("mo", "も", "モ", KanaType.Gojuuon),

                new TestBaseModel("ya", "や", "ヤ", KanaType.Gojuuon),
                new TestBaseModel("yu", "ゆ", "ユ", KanaType.Gojuuon),
                new TestBaseModel("yo", "よ", "ヨ", KanaType.Gojuuon),

                new TestBaseModel("ra", "ら", "ラ", KanaType.Gojuuon),
                new TestBaseModel("ri", "り", "リ", KanaType.Gojuuon),
                new TestBaseModel("ru", "る", "ル", KanaType.Gojuuon),
                new TestBaseModel("re", "れ", "レ", KanaType.Gojuuon),
                new TestBaseModel("ro", "ろ", "ロ", KanaType.Gojuuon),

                new TestBaseModel("wa", "わ", "ワ", KanaType.Gojuuon),
                new TestBaseModel("wo", "を", "ヲ", KanaType.Gojuuon),

                new TestBaseModel("n",  "ん", "ン", KanaType.Gojuuon)
            };
    }
}
