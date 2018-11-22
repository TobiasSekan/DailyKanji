using DailyKanjiLogic.Helper;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Timers;
using System.Windows.Media;

namespace DailyKanji.Mvvm.Model
{
    public sealed class MainModel : PropertyChangedHelper
    {
        #region Public Properties

        /// <summary>
        /// The current colour of all answer buttons
        /// </summary>
        [JsonIgnore]
        public ObservableCollection<Brush> AnswerButtonColor
        {
            get => _buttonColor;
            set
            {
                _buttonColor = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The current colour of all answer buttons
        /// </summary>
        [JsonIgnore]
        public ObservableCollection<Brush> HintTextColor
        {
            get => _hintTextColor;
            set
            {
                _hintTextColor = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The current colour of the ask sign
        /// </summary>
        [JsonIgnore]
        public Brush CurrentAskSignColor
        {
            get => _currentAskSignColor;
            set
            {
                _currentAskSignColor = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The colour for the progress bar (running answer time)
        /// </summary>
        [JsonIgnore]
        public Brush ProgressBarColor
        {
            get => _progressBarColor;
            set
            {
                _progressBarColor = value;
                OnPropertyChanged();
            }
        }

        #endregion Public Properties

        #region Internal Properties

        internal Timer TestTimer { get; set; }

        #endregion Internal Properties

        #region Private Backing-Fields

        /// <summary>
        /// Backing-field for <see cref="AnswerButtonColor"/>
        /// </summary>
        private ObservableCollection<Brush> _buttonColor;

        /// <summary>
        /// Backing-field for <see cref="HintTextColor"/>
        /// </summary>
        private ObservableCollection<Brush> _hintTextColor;

        /// <summary>
        /// Backing-field for <see cref="CurrentAskSignColor"/>
        /// </summary>
        private Brush _currentAskSignColor;

        /// <summary>
        /// Backing-field for <see cref="ProgressBarColor"/>
        /// </summary>
        private Brush _progressBarColor;

        #endregion Private Backing-Fields
    }
}
