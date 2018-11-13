using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DailyKanji.Helper
{
    /// <summary>
    /// Helper class to easier work with <see cref="INotifyPropertyChanged"/>
    /// </summary>
    public abstract class PropertyChangedHelper : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
