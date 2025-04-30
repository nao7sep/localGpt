using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace localGpt.App.Logging.ViewModels
{
    /// <summary>
    /// ViewModel for the error dialog.
    /// </summary>
    public class ErrorDialogViewModel : INotifyPropertyChanged
    {
        private string _errorMessage;

        /// <summary>
        /// Gets or sets the error message to display.
        /// </summary>
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                if (_errorMessage != value)
                {
                    _errorMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorDialogViewModel"/> class.
        /// </summary>
        /// <param name="errorMessage">The error message to display</param>
        public ErrorDialogViewModel(string errorMessage)
        {
            _errorMessage = errorMessage;
        }

        /// <summary>
        /// Event raised when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">Name of the property that changed</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}