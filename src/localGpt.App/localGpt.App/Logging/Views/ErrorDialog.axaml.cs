using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using localGpt.App.Logging.ViewModels;

namespace localGpt.App.Logging.Views
{
    /// <summary>
    /// Dialog for displaying error messages.
    /// </summary>
    public partial class ErrorDialog : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorDialog"/> class.
        /// </summary>
        public ErrorDialog()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorDialog"/> class.
        /// </summary>
        /// <param name="errorMessage">The error message to display</param>
        public ErrorDialog(string errorMessage) : this()
        {
            DataContext = new ErrorDialogViewModel(errorMessage);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorDialog"/> class.
        /// </summary>
        /// <param name="errorMessage">The error message to display</param>
        /// <param name="owner">The owner window</param>
        public ErrorDialog(string errorMessage, Window owner) : this(errorMessage)
        {
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        /// <summary>
        /// Handles the OK button click event.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void OnOkButtonClick(object? sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}