using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DailyKanji.Helper
{
    public abstract class PropertyChangedHelper : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
