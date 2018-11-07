using DailyKanji.Enumerations;
using Newtonsoft.Json;
using System;

namespace DailyKanji.Mvvm.Model
{
    public sealed class TestModel : TestBaseModel
    {
        internal TestType TestType { get; }

        internal TestModel(TestBaseModel testBaseModel, TestType testType) : base(testBaseModel)
            => TestType = testType;

        [JsonIgnore]
        public TimeSpan AverageAnswerTime
            => TestType == TestType.HiraganaToRoomaji ? AverageAnswerTimeForHiragana : AverageAnswerTimeForKatakana;

        [JsonIgnore]
        public int CorrectCount
            => TestType == TestType.HiraganaToRoomaji ? CorrectHiraganaCount : CorrectKatakanaCount;

        [JsonIgnore]
        public int WrongCount
            => TestType == TestType.HiraganaToRoomaji ? WrongHiraganaCount : WrongKatakanaCount;
    }
}
