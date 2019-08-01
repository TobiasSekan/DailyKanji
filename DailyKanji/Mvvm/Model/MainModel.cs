﻿using DailyKanjiLogic.Helper;
using System;
using System.Collections.Generic;
using System.Timers;
using System.Windows;
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

        public IList<Visibility> AnswerShortCutTextVisibility
        {
            get => _answerShortCutTextVisibility;
            set
            {
                if(_answerShortCutTextVisibility == value)
                {
                    return;
                }

                _answerShortCutTextVisibility = value;
                OnPropertyChanged();
            }
        }

        public IList<Visibility> AnswerHintTextVisibility
        {
            get => _answerHintTextVisibility;
            set
            {
                if(_answerHintTextVisibility == value)
                {
                    return;
                }

                _answerHintTextVisibility = value;
                OnPropertyChanged();
            }
        }

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

        private IList<GridLength> _answerButtonColumnWidth;

        private IList<Visibility>_answerButtonVisibility;

        private IList<Visibility>_answerShortCutTextVisibility;

        private IList<Visibility>_answerHintTextVisibility;

        private IList<string>_answerAnswerText;

        private IList<string> _answerHintText;

        private IList<string>_answerShortCutText;

        #endregion Private Backing-fields

        #region Internal Constructors

        /// <summary>
        /// Create a new <see cref="MainModel"/> with default values
        /// </summary>
        internal MainModel()
        {
            ProgressPrefreshInterval      = new TimeSpan(0, 0, 0, 0, 15);
            TestTimer                     = new Timer(ProgressPrefreshInterval.TotalMilliseconds);
            _answerMenu                   = new List<MenuItem>(10);
            _markMenu                     = new List<MenuItem>(10);
            _answerButtonColumnWidth      = new List<GridLength>(10);
            _answerButtonVisibility       = new List<Visibility>(10);
            _answerShortCutTextVisibility = new List<Visibility>(10);
            _answerHintTextVisibility     = new List<Visibility>(10);
            _answerAnswerText             = new List<string>(10);
            _answerHintText               = new List<string>(10);
            _answerShortCutText           = new List<string>(10);

            for(var answerNumber = 0; answerNumber < 10; answerNumber++)
            {
                _answerButtonColumnWidth.Add(GridLength.Auto);
                _answerButtonVisibility.Add(Visibility.Collapsed);
                _answerShortCutTextVisibility.Add(Visibility.Collapsed);
                _answerHintTextVisibility.Add(Visibility.Collapsed);
                _answerAnswerText.Add(string.Empty);
                _answerHintText.Add(string.Empty);
                _answerShortCutText.Add(string.Empty);
            }
        }

        #endregion Internal Constructors

        #region IDisposable Implementation

        public void Dispose()
        {
            _answerMenu.Clear();
            _markMenu.Clear();
            _answerButtonColumnWidth.Clear();
            _answerButtonVisibility.Clear();
            _answerShortCutTextVisibility.Clear();
            _answerHintTextVisibility.Clear();
            _answerAnswerText.Clear();
            _answerHintText.Clear();
            _answerShortCutText.Clear();
        }

        #endregion IDisposable Implementation
    }
}
