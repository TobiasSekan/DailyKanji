using DailyKanji.Enumerations;

namespace DailyKanji.Mvvm.Model
{
    public sealed class TestModel : TestBaseModel
    {
        internal TestType TestType { get; }

        internal TestModel(TestBaseModel testBaseModel, TestType testType) : base(testBaseModel)
            => TestType = testType;
    }
}
