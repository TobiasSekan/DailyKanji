using DailyKanjiLogic.Helper;
using System.Timers;

namespace DailyKanji.Mvvm.Model
{
    public sealed class MainModel
    {
        #region Public Properties

        /// <summary>
        /// Return the version and target framework of this program
        /// </summary>
        public string GetVersion
            => $"{AssemblyHelper.GetAssemblyVersion(this)} ({AssemblyHelper.GetTargetFramework(this)})";

        #endregion Public Properties

        #region Internal Properties

        /// <summary>
        /// Running timer for each test
        /// </summary>
        internal Timer TestTimer { get; set; }

        /// <summary>
        /// Timer for the error highlight, when a test was wrong answered
        /// </summary>
        internal Timer ErrorHighlightTimer { get; set; }

        #endregion Internal Properties
    }
}
