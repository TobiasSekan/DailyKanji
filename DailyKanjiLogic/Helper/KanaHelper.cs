﻿using DailyKanjiLogic.Enumerations;
using DailyKanjiLogic.Mvvm.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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
                new TestBaseModel("a",  "あ", "ア", KanaType.Gojuuon),
                new TestBaseModel("i",  "い", "イ", KanaType.Gojuuon),
                new TestBaseModel("u",  "う", "ウ", KanaType.Gojuuon),
                new TestBaseModel("e",  "え", "エ", KanaType.Gojuuon),
                new TestBaseModel("o",  "お", "オ", KanaType.Gojuuon),

                new TestBaseModel("ka",  "か", "カ", KanaType.Gojuuon),
                new TestBaseModel("ki",  "き", "キ", KanaType.Gojuuon),
                new TestBaseModel("ku",  "く", "ク", KanaType.Gojuuon),
                new TestBaseModel("ke",  "け", "ケ", KanaType.Gojuuon),
                new TestBaseModel("ko",  "こ", "コ", KanaType.Gojuuon),

                new TestBaseModel("sa",  "さ", "サ", KanaType.Gojuuon),
                new TestBaseModel("shi", "し", "シ", KanaType.Gojuuon),
                new TestBaseModel("su",  "す", "ス", KanaType.Gojuuon),
                new TestBaseModel("se",  "せ", "セ", KanaType.Gojuuon),
                new TestBaseModel("so",  "そ", "ソ", KanaType.Gojuuon),

                new TestBaseModel("ta",  "た", "タ", KanaType.Gojuuon),
                new TestBaseModel("chi", "ち", "チ", KanaType.Gojuuon),
                new TestBaseModel("tsu", "つ", "ツ", KanaType.Gojuuon),
                new TestBaseModel("te",  "て", "テ", KanaType.Gojuuon),
                new TestBaseModel("to",  "と", "ト", KanaType.Gojuuon),

                new TestBaseModel("na",  "な", "ナ", KanaType.Gojuuon),
                new TestBaseModel("ni",  "に", "ニ", KanaType.Gojuuon),
                new TestBaseModel("nu",  "ぬ", "ヌ", KanaType.Gojuuon),
                new TestBaseModel("ne",  "ね", "ネ", KanaType.Gojuuon),
                new TestBaseModel("no",  "の", "ノ", KanaType.Gojuuon),

                new TestBaseModel("ha",  "は", "ハ", KanaType.Gojuuon),
                new TestBaseModel("hi",  "ひ", "ヒ", KanaType.Gojuuon),
                new TestBaseModel("fu",  "ふ", "フ", KanaType.Gojuuon),
                new TestBaseModel("he",  "へ", "ヘ", KanaType.Gojuuon),
                new TestBaseModel("ho",  "ほ", "ホ", KanaType.Gojuuon),

                new TestBaseModel("ma",  "ま", "マ", KanaType.Gojuuon),
                new TestBaseModel("mi",  "み", "ミ", KanaType.Gojuuon),
                new TestBaseModel("mu",  "む", "ム", KanaType.Gojuuon),
                new TestBaseModel("me",  "め", "メ", KanaType.Gojuuon),
                new TestBaseModel("mo",  "も", "モ", KanaType.Gojuuon),

                new TestBaseModel("ya",  "や", "ヤ", KanaType.Gojuuon),
                new TestBaseModel("yu",  "ゆ", "ユ", KanaType.Gojuuon),
                new TestBaseModel("yo",  "よ", "ヨ", KanaType.Gojuuon),

                new TestBaseModel("ra",  "ら", "ラ", KanaType.Gojuuon),
                new TestBaseModel("ri",  "り", "リ", KanaType.Gojuuon),
                new TestBaseModel("ru",  "る", "ル", KanaType.Gojuuon),
                new TestBaseModel("re",  "れ", "レ", KanaType.Gojuuon),
                new TestBaseModel("ro",  "ろ", "ロ", KanaType.Gojuuon),

                new TestBaseModel("wa",  "わ", "ワ", KanaType.Gojuuon),
                new TestBaseModel("wo",  "を", "ヲ", KanaType.Gojuuon),

                new TestBaseModel("n",   "ん", "ン", KanaType.Gojuuon),

                new TestBaseModel("ga",  "が", "ガ", KanaType.GojuuonWithDakuten),
                new TestBaseModel("gi",  "ぎ", "ギ", KanaType.GojuuonWithDakuten),
                new TestBaseModel("gu",  "ぐ", "グ", KanaType.GojuuonWithDakuten),
                new TestBaseModel("ge",  "げ", "ゲ", KanaType.GojuuonWithDakuten),
                new TestBaseModel("go",  "ご", "ゴ", KanaType.GojuuonWithDakuten),

                new TestBaseModel("za",  "ざ", "ザ", KanaType.GojuuonWithDakuten),
                new TestBaseModel("ji",  "じ", "ジ", KanaType.GojuuonWithDakuten),
                new TestBaseModel("zu",  "ず", "ズ", KanaType.GojuuonWithDakuten),
                new TestBaseModel("ze",  "ぜ", "ゼ", KanaType.GojuuonWithDakuten),
                new TestBaseModel("zo",  "ぞ", "ゾ", KanaType.GojuuonWithDakuten),

                new TestBaseModel("da",  "だ", "ダ", KanaType.GojuuonWithDakuten),
                new TestBaseModel("ji",  "ぢ", "ヂ", KanaType.GojuuonWithDakuten),
                new TestBaseModel("zu",  "づ", "ヅ", KanaType.GojuuonWithDakuten),
                new TestBaseModel("de",  "で", "デ", KanaType.GojuuonWithDakuten),
                new TestBaseModel("do",  "ど", "ド", KanaType.GojuuonWithDakuten),

                new TestBaseModel("ba",  "ば", "バ", KanaType.GojuuonWithDakuten),
                new TestBaseModel("bi",  "び", "ビ", KanaType.GojuuonWithDakuten),
                new TestBaseModel("bu",  "ぶ", "ブ", KanaType.GojuuonWithDakuten),
                new TestBaseModel("be",  "べ", "ベ", KanaType.GojuuonWithDakuten),
                new TestBaseModel("bo",  "ぼ", "ボ", KanaType.GojuuonWithDakuten),

                new TestBaseModel("pa",  "ぱ", "パ", KanaType.GojuuonWithHandakuten),
                new TestBaseModel("pi",  "ぴ", "ピ", KanaType.GojuuonWithHandakuten),
                new TestBaseModel("pu",  "ぷ", "プ", KanaType.GojuuonWithHandakuten),
                new TestBaseModel("pe",  "ぺ", "ペ", KanaType.GojuuonWithHandakuten),
                new TestBaseModel("po",  "ぽ", "ポ", KanaType.GojuuonWithHandakuten),

                new TestBaseModel("kya", "きゃ", "キャ", KanaType.Yooon),
                new TestBaseModel("kyu", "きゅ", "キュ", KanaType.Yooon),
                new TestBaseModel("kyo", "きょ", "キョ", KanaType.Yooon),

                new TestBaseModel("sha", "しゃ", "シャ", KanaType.Yooon),
                new TestBaseModel("shu", "しゅ", "シュ", KanaType.Yooon),
                new TestBaseModel("sho", "しょ", "ショ", KanaType.Yooon),

                new TestBaseModel("cha", "ちゃ", "チャ", KanaType.Yooon),
                new TestBaseModel("chu", "ちゅ", "チュ", KanaType.Yooon),
                new TestBaseModel("cho", "ちょ", "チョ", KanaType.Yooon),

                new TestBaseModel("nya", "にゃ", "ニャ", KanaType.Yooon),
                new TestBaseModel("nyu", "にゅ", "ニュ", KanaType.Yooon),
                new TestBaseModel("nyo", "にょ", "ニョ", KanaType.Yooon),

                new TestBaseModel("hya", "ひゃ", "ヒャ", KanaType.Yooon),
                new TestBaseModel("hyu", "ひゅ", "ヒュ", KanaType.Yooon),
                new TestBaseModel("hyo", "ひょ", "ヒョ", KanaType.Yooon),

                new TestBaseModel("mya", "みゃ", "ミャ", KanaType.Yooon),
                new TestBaseModel("myu", "みゅ", "ミュ", KanaType.Yooon),
                new TestBaseModel("myo", "みょ", "ミョ", KanaType.Yooon),

                new TestBaseModel("rya", "りゃ", "リャ", KanaType.Yooon),
                new TestBaseModel("ryu", "りゅ", "リュ", KanaType.Yooon),
                new TestBaseModel("ryo", "りょ", "リョ", KanaType.Yooon),

                new TestBaseModel("gya", "ぎゃ", "ギャ", KanaType.YooonWithDakuten),
                new TestBaseModel("gyu", "ぎゅ", "ギュ", KanaType.YooonWithDakuten),
                new TestBaseModel("gyo", "ぎょ", "ギョ", KanaType.YooonWithDakuten),

                new TestBaseModel("ja",  "じゃ", "ジャ", KanaType.YooonWithDakuten),
                new TestBaseModel("ju",  "じゅ", "ジュ", KanaType.YooonWithDakuten),
                new TestBaseModel("jo",  "じょ", "ジョ", KanaType.YooonWithDakuten),

                new TestBaseModel("bya", "びゃ", "ビャ", KanaType.YooonWithDakuten),
                new TestBaseModel("byu", "びゅ", "ビュ", KanaType.YooonWithDakuten),
                new TestBaseModel("byo", "びょ", "ビョ", KanaType.YooonWithDakuten),

                new TestBaseModel("pya", "ぴゃ", "ピャ", KanaType.YooonWithHandakuten),
                new TestBaseModel("pyu", "ぴゅ", "ピュ", KanaType.YooonWithHandakuten),
                new TestBaseModel("pyo", "ぴょ", "ピョ", KanaType.YooonWithHandakuten),
            };

        /// <summary>
        /// Return a list with tests that have the same Kana type as the given test
        /// </summary>
        /// <param name="testList">A list with all tests</param>
        /// <param name="test">The test that contains the kana type for the new list</param>
        /// <returns>A list with tests that have the same Kana type</returns>
        public static IEnumerable<TestBaseModel> GetSameKana(in IEnumerable<TestBaseModel> testList, in TestBaseModel test)
        {
            var tempTest = test;
            return testList.Where(found => found.Type == tempTest.Type);
        }

        /// <summary>
        /// Return a tests with a similar Kana (similar look) from the given list, based on the given test
        /// </summary>
        /// <param name="testList">A list with all tests</param>
        /// <param name="test">A test with a Kana, need to find a similar Kana</param>
        /// <param name="answerType">The type of the answer (Roomaji, Hiragana, Katakana)</param>
        /// <returns>A list with similar test</returns>
        public static IEnumerable<TestBaseModel> GetSimilarKana(in IEnumerable<TestBaseModel> testList, in TestBaseModel test, in AnswerType answerType)
        {
            return answerType switch
            {
                AnswerType.Roomaji => GetSimilarRoomaji(testList, test),
                AnswerType.Hiragana => GetSimilarHiragana(testList, test),
                AnswerType.Katakana => GetSimilarKatakana(testList, test),

                _ => GetSimilarRoomaji(testList, test),

                // TODO
                // throw new ArgumentOutOfRangeException(nameof(answerType), $"{nameof(answerType)} can't be {nameof(AnswerType.Unknown)}");
            };
        }

        /// <summary>
        /// Return a test with a similar Roomaji (similar look) from the given list, based on the given test
        /// </summary>
        /// <param name="testList">A list with all tests</param>
        /// <param name="test">A test with a Roomaji, need to find a similar Roomaji</param>
        /// <returns>A list with similar test</returns>
        public static IEnumerable<TestBaseModel> GetSimilarRoomaji(in IEnumerable<TestBaseModel> testList, in TestBaseModel test)
        {
            var similarTestList = new Collection<TestBaseModel>();

            foreach(var testInList in testList)
            {
                // don't add the given test
                if(testInList.Roomaji == test.Roomaji)
                {
                    continue;
                }

                if(!testInList.Roomaji.Contains(test.Roomaji.FirstOrDefault())
                && !testInList.Roomaji.Contains(test.Roomaji.ElementAtOrDefault(1))
                && !testInList.Roomaji.Contains(test.Roomaji.ElementAtOrDefault(2)))
                {
                    continue;
                }

                similarTestList.Add(testInList);
            }

            return similarTestList;
        }

        /// <summary>
        /// Return a test with a similar Hiragana (similar look) from the given list, based on the given test
        /// </summary>
        /// <param name="testList">A list with all tests</param>
        /// <param name="test">A test with a Hiragana, need to find a similar Hiragana</param>
        /// <returns>A list with similar test</returns>
        public static IEnumerable<TestBaseModel> GetSimilarHiragana(in IEnumerable<TestBaseModel> testList, in TestBaseModel test)
        {
            // TODO
            return testList;
        }

        /// <summary>
        /// Return a test with a similar Katakana (similar look) from the given list, based on the given test
        /// </summary>
        /// <param name="testList">A list with all tests</param>
        /// <param name="test">A test with a Katakana, need to find a similar Katakana</param>
        /// <returns>A list with similar test</returns>
        public static IEnumerable<TestBaseModel> GetSimilarKatakana(in IEnumerable<TestBaseModel> testList, in TestBaseModel test)
        {
            // TODO
            return testList;
        }
    }
}
