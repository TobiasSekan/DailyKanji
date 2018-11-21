using DailyKanjiLogic.Mvvm.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DailyKanjiLogic.Helper
{
    public static class KanaHelper
    {
        public static IEnumerable<TestBaseModel> GetKanaList()
            => new Collection<TestBaseModel>
            {
                // Kana Signs, Think Now How Much You Really Want (to learn them).
                new TestBaseModel("a", "あ", "ア"),
                new TestBaseModel("i", "い", "イ"),
                new TestBaseModel("u", "う", "ウ"),
                new TestBaseModel("e", "え", "エ"),
                new TestBaseModel("o", "お", "オ"),

                new TestBaseModel("ka", "か", "カ"),
                new TestBaseModel("ki", "き", "キ"),
                new TestBaseModel("ku", "く", "ク"),
                new TestBaseModel("ke", "け", "ケ"),
                new TestBaseModel("ko", "こ", "コ"),

                new TestBaseModel("sa", "さ", "サ"),
                new TestBaseModel("shi","し", "シ"),
                new TestBaseModel("su", "す", "ス"),
                new TestBaseModel("se", "せ", "セ"),
                new TestBaseModel("so", "そ", "ソ"),

                new TestBaseModel("ta", "た", "タ"),
                new TestBaseModel("chi","ち", "チ"),
                new TestBaseModel("tsu","つ", "ツ"),
                new TestBaseModel("te", "て", "テ"),
                new TestBaseModel("to", "と", "ト"),

                new TestBaseModel("na", "な", "ナ"),
                new TestBaseModel("ni", "に", "ニ"),
                new TestBaseModel("nu", "ぬ", "ヌ"),
                new TestBaseModel("ne", "ね", "ネ"),
                new TestBaseModel("no", "の", "ノ"),

                new TestBaseModel("ha", "は", "ハ"),
                new TestBaseModel("hi", "ひ", "ヒ"),
                new TestBaseModel("fu", "ふ", "フ"),
                new TestBaseModel("he", "へ", "ヘ"),
                new TestBaseModel("ho", "ほ", "ホ"),

                new TestBaseModel("ma", "ま", "マ"),
                new TestBaseModel("mi", "み", "ミ"),
                new TestBaseModel("mu", "む", "ム"),
                new TestBaseModel("me", "め", "メ"),
                new TestBaseModel("mo", "も", "モ"),

                new TestBaseModel("ya", "や", "ヤ"),
                new TestBaseModel("yu", "ゆ", "ユ"),
                new TestBaseModel("yo", "よ", "ヨ"),

                new TestBaseModel("ra", "ら", "ラ"),
                new TestBaseModel("ri", "り", "リ"),
                new TestBaseModel("ru", "る", "ル"),
                new TestBaseModel("re", "れ", "レ"),
                new TestBaseModel("ro", "ろ", "ロ"),

                new TestBaseModel("wa", "わ", "ワ"),
                new TestBaseModel("wo", "を", "ヲ"),

                new TestBaseModel("n",  "ん", "ン")
            };
    }
}
