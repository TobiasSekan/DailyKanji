using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DailyKanjiLogic.Helper
{
    /// <summary>
    /// Helper class to easier work with changed properties for/in WPF
    /// </summary>
    public abstract class PropertyChangedHelper : INotifyPropertyChanged
    {
        /// <summary>
        /// Event is called when a property has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notify the WPF subsystem that a property has changed
        /// </summary>
        /// <param name="name">The name of the property</param>
        protected void OnPropertyChanged([CallerMemberName] in string name = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
