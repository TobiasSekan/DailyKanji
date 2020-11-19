using DailyKanjiLogic.Helper;
using System;
using System.Collections.Generic;
using System.Timers;
using System.Windows;
using System.Windows.Controls;

namespace DailyKanji.Mvvm.Model
{
    /// <summary>
    /// A data model that contains all data for the surface and the application
    /// (this model doesn't contain data for the program logic and doesn't contain Kanji data)
    /// </summary>
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

        /// <summary>
        /// A list with column widths of the answer buttons (always 10 entries)
        /// </summary>
        public IList<GridLength> AnswerButtonColumnWidth
        {
            get => _answerButtonColumnWidth;
            set
            {
                if(_answerButtonColumnWidth == value)
                {
                    return;
                }

                _answerButtonColumnWidth = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// A list with button visibility of the answer buttons (always 10 entries)
        /// </summary>
        public IList<Visibility> AnswerButtonVisibility
        {
            get => _answerButtonVisibility;
            set
            {
                if(_answerButtonVisibility == value)
                {
                    return;
                }

                _answerButtonVisibility = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// A list with all answer sings (always 10 entries)
        /// </summary>
        public IList<string> AnswerAnswerText
        {
            get => _answerAnswerText;
            set
            {
                if(_answerAnswerText == value)
                {
                    return;
                }

                _answerAnswerText = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// A list with all answer hints (always 10 entries)
        /// </summary>
        public IList<string> AnswerHintText
        {
            get => _answerHintText;
            set
            {
                if(_answerHintText == value)
                {
                    return;
                }

                _answerHintText = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// A list with all keyboard short cuts (always 10 entries)
        /// </summary>
        public IList<string> AnswerShortCutText
        {
            get => _answerShortCutText;
            set
            {
                if(_answerShortCutText == value)
                {
                    return;
                }

                _answerShortCutText = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The grid row height for the answer hints
        /// </summary>
        public GridLength AnswerHintTextHeight
        {
            get => _answerHintTextHeight;
            set
                {
                if(_answerHintTextHeight == value)
                {
                    return;
                }

                _answerHintTextHeight = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The grid row height for the keyboard short cuts
        /// </summary>
        public GridLength AnswerShortCutTextHeight
        {
            get => _answerShortCutTextHeight;
            set
                {
                if(_answerShortCutTextHeight == value)
                {
                    return;
                }

                _answerShortCutTextHeight = value;
                OnPropertyChanged();
            }
        }

        #endregion Public Properties

        #region Internal Properties

        /// <summary>
        /// Running timer for each test
        /// </summary>
        internal Timer TestTimer { get; set; }

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

        /// <summary>
        /// Backing-field for <see cref="AnswerButtonColumnWidth"/>
        /// </summary>
        private IList<GridLength> _answerButtonColumnWidth;

        /// <summary>
        /// Backing-field for <see cref="AnswerButtonVisibility"/>
        /// </summary>
        private IList<Visibility> _answerButtonVisibility;

        /// <summary>
        /// Backing-field for <see cref="AnswerAnswerText"/>
        /// </summary>
        private IList<string> _answerAnswerText;

        /// <summary>
        /// Backing-field for <see cref="AnswerHintText"/>
        /// </summary>
        private IList<string> _answerHintText;

        /// <summary>
        /// Backing-field for <see cref="AnswerShortCutText"/>
        /// </summary>
        private IList<string> _answerShortCutText;

        /// <summary>
        /// Backing-field for <see cref="AnswerShortCutTextHeight"/>
        /// </summary>
        private GridLength _answerShortCutTextHeight;

        /// <summary>
        /// Backing-field for <see cref="AnswerHintTextHeight"/>
        /// </summary>
        private GridLength _answerHintTextHeight;

        #endregion Private Backing-fields

        #region Internal Constructors

        /// <summary>
        /// Create a new <see cref="MainModel"/> with default values
        /// </summary>
        internal MainModel()
        {
            _answerShortCutTextHeight = GridLength.Auto;
            _answerHintTextHeight     = GridLength.Auto;
            ProgressPrefreshInterval  = new TimeSpan(0, 0, 0, 0, 15);
            TestTimer                 = new Timer(ProgressPrefreshInterval.TotalMilliseconds);
            _answerMenu               = new List<MenuItem>(10);
            _markMenu                 = new List<MenuItem>(10);
            _answerButtonColumnWidth  = new List<GridLength>(10);
            _answerButtonVisibility   = new List<Visibility>(10);
            _answerAnswerText         = new List<string>(10);
            _answerHintText           = new List<string>(10);
            _answerShortCutText       = new List<string>(10);

            for(var answerNumber = 0; answerNumber < 10; answerNumber++)
            {
                _answerButtonColumnWidth.Add(GridLength.Auto);
                _answerButtonVisibility.Add(Visibility.Collapsed);
                _answerAnswerText.Add(string.Empty);
                _answerHintText.Add(string.Empty);
                _answerShortCutText.Add(string.Empty);
            }
        }

        #endregion Internal Constructors

        #region IDisposable Implementation

        /// <inheritdoc/>
        public void Dispose()
        {
            _answerMenu.Clear();
            _markMenu.Clear();
            _answerButtonColumnWidth.Clear();
            _answerButtonVisibility.Clear();
            _answerAnswerText.Clear();
            _answerHintText.Clear();
            _answerShortCutText.Clear();
        }

        #endregion IDisposable Implementation
    }
}
