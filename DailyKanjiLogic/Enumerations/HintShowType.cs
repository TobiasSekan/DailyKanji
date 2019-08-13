using System;

namespace DailyKanjiLogic.Enumerations
{
    /// <summary>
    /// The kind of hints that should show on wrong/right answers
    /// </summary>
    [Flags]
    public enum HintShowType : byte
    {
        /// <summary>
        /// Show no hint
        /// </summary>
        ShowOnNoAnswers = 0x_00,

        /// <summary>
        /// Show the hint on the wrong answer
        /// </summary>
        ShowOnWrongAnswer = 0x_01,

        /// <summary>
        /// Show the hint on the right answer
        /// </summary>
        ShowOnRightAnswer = 0x_02,

        /// <summary>
        /// Show the hint on all marked answers
        /// </summary>
        ShowOnMarkedAnswers = 0x_04,

        /// <summary>
        /// Show the hint on all other answers (not marked answers, not right, not wrong)
        /// </summary>
        ShowOnOtherAnswers = 0x_08
    }
}
