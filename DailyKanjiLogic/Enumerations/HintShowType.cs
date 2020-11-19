using System;

namespace DailyKanjiLogic.Enumerations
{
    /// <summary>
    /// The kind of hints that should show on wrong/right answers
    /// </summary>
    [Flags]
    public enum HintShowType
    {
        /// <summary>
        /// Show no hint
        /// </summary>
        ShowOnNoAnswers = 0,

        /// <summary>
        /// Show the hint on the wrong answer
        /// </summary>
        ShowOnWrongAnswer = 1,

        /// <summary>
        /// Show the hint on the correct answer
        /// </summary>
        ShowOnCorrectAnswer = 1 << 1,

        /// <summary>
        /// Show the hint on all marked answers
        /// </summary>
        ShowOnMarkedAnswers = 1 << 2,

        /// <summary>
        /// Show the hint on all other answers (not marked answers, not right, not wrong)
        /// </summary>
        ShowOnOtherAnswers = 1 << 3
    }
}
