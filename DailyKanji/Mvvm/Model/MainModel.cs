using DailyKanjiLogic.Helper;
using System;
using System.Collections.Generic;
using System.Timers;
using System.Windows.Controls;

#nullable enable

namespace DailyKanji.Mvvm.Model
{
    public sealed class MainModel : PropertyChangedHelper, IDisposable
    {
        #region Public Properties

        /// <summary>
        /// Return the version and target framework of this program
        /// </summary>
        public string GetProgramVersion
            => $"{AssemblyHelper.GetAssemblyVersion(this)} ({AssemblyHelper.GetTargetFramework(this)})";

        /// <summary>
        /// The refresh interval for the visual progress bar (Note: The progress bar show the running timer)
        /// </summary>
        public TimeSpan ProgressPrefreshInterval
        {
            get => _progressPrefreshInterval;
            set
            {
                if(_progressPrefreshInterval == value)
                {
                    return;
                }

                _progressPrefreshInterval = value;
                OnPropertyChanged();

                if(TestTimer is null)
                {
                    return;
                }

                TestTimer.Interval = _progressPrefreshInterval.Milliseconds;
            }
        }

        /// <summary>
        /// List that contains all entries for the "Answer" menu
        /// </summary>
        public IList<MenuItem> AnswerMenu
        {
            get => _answerMenu;
            set
            {
                if(_answerMenu == value)
                {
                    return;
                }

                _answerMenu = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// List that contains all entries for the "Mark" menu
        /// </summary>
        public IList<MenuItem> MarkMenu
        {
            get => _markMenu;
            set
            {
                if(_markMenu == value)
                {
                    return;
                }

                _markMenu = value;
                OnPropertyChanged();
            }
        }

        #endregion Public Properties

        #region Internal Properties

        /// <summary>
        /// Running timer for each test
        /// </summary>
        internal Timer TestTimer { get; set; }

        /// <summary>
        /// List that contains all values for the visible answers
        /// </summary>
        internal IList<AnswerViewElement> AnswerElements { get; }

        #endregion Internal Properties

        #region Private Backing-fields

        /// <summary>
        /// Backing-field for <see cref="ProgressPrefreshInterval"/>
        /// </summary>
        private TimeSpan _progressPrefreshInterval;

        /// <summary>
        /// Backing-field for <see cref="AnswerMenu"/>
        /// </summary>
        private IList<MenuItem> _answerMenu;

        /// <summary>
        /// Backing-field for <see cref="MarkMenu"/>
        /// </summary>
        private IList<MenuItem> _markMenu;

        #endregion Private Backing-fields

        #region Internal Constructors

        /// <summary>
        /// Create a new <see cref="MainModel"/> with default values
        /// </summary>
        internal MainModel()
        {
            ProgressPrefreshInterval = new TimeSpan(0, 0, 0, 0, 15);
            TestTimer                = new Timer(ProgressPrefreshInterval.TotalMilliseconds);
            _answerMenu              = new List<MenuItem>(10);
            _markMenu                = new List<MenuItem>(10);
            AnswerElements           = new List<AnswerViewElement>(10);
        }

        #endregion Internal Constructors

        #region IDisposable Implementation

        public void Dispose()
        {
            AnswerElements.Clear();
            _answerMenu.Clear();
            _markMenu.Clear();
        }

        #endregion IDisposable Implementation
    }
}
