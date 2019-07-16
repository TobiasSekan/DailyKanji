using System.Windows.Media;

namespace DailyKanji.Helper
{
    /// <summary>
    /// Helper class to easier work with <see cref="Colors"/>
    /// </summary>
    internal static class ColorHelper
    {
        /// <summary>
        /// The color string for the progress bar (<see cref="Colors.LightBlue"/> - #FFADD8E6)
        /// </summary>
        internal static string ProgressBarColor
            => Colors.LightBlue.ToString();

        /// <summary>
        /// The color string for the error highlight (<see cref="Colors.LightCoral"/> - #FFF08080)
        /// </summary>
        internal static string ErrorColor
            => Colors.LightCoral.ToString();

        /// <summary>
        /// The color string for none selected answers (<see cref="Colors.LightGoldenrodYellow"/> - #FFFAFAD2)
        /// </summary>
        internal static string NoneSelectedColor
            => Colors.LightGoldenrodYellow.ToString();

        /// <summary>
        /// The color string for the correct answer (<see cref="Colors.LightGreen"/> - FF90EE90)
        /// </summary>
        internal static string CorrectColor
            => Colors.LightGreen.ToString();

        /// <summary>
        /// The color string for invisible text and elements (<see cref="Colors.Transparent"/> - #00FFFFFF)
        /// </summary>
        internal static string TransparentColor
            => Colors.Transparent.ToString();

        /// <summary>
        /// The color string for the answer hints (<see cref="Colors.Black"/> - #FF000000)
        /// </summary>
        internal static string AnswerHintTextColor
            => Colors.Black.ToString();
    }
}
