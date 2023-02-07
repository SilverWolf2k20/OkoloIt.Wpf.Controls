using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OkoloIt.Wpf.Controls
{
    /// <summary>
    /// Поле для ввода пароля.
    /// </summary>
    public sealed class PasswordBox : TextBox
    {
        #region Public Fields

        /// <summary>
        /// Настройка пароля.
        /// </summary>
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register(
                nameof(Password),
                typeof(SecureString),
                typeof(PasswordBox),
                new PropertyMetadata(new SecureString(), new PropertyChangedCallback(OnPasswordChange))
            );

        #endregion Public Fields

        #region Public Constructors

        /// <summary>
        /// Создает экземпляр поля для ввода пароля.
        /// </summary>
        public PasswordBox()
        {
            PreviewTextInput += OnPreviewTextInput;
            PreviewKeyDown += OnPreviewKeyDown;

            CommandManager.AddPreviewExecutedHandler(this, PreviewExecutedHandler);
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Пароль.
        /// </summary>
        public SecureString Password {
            get { return (SecureString)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        #endregion Public Properties

        #region Private Methods

        private static void OnPasswordChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var passwordBox = (PasswordBox)d;
            passwordBox.Text = new string('●', ((SecureString)e.NewValue).Length);
        }

        private void AddText(string text)
        {
            // Удаление выделенного участка.
            if (SelectionLength > 0)
                RemoveText(SelectionStart, SelectionLength);

            int caretIndex = CaretIndex;

            foreach (char c in text) {
                // Обновление пароля.
                SecureString password = Password.Copy();
                password.InsertAt(caretIndex++, c);
                Password = password;
            }

            CaretIndex = caretIndex;
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs args)
        {
            Key pressedKey = args.Key == Key.System ? args.SystemKey : args.Key;

            if (pressedKey == Key.Enter) {
                args.Handled = true;
                return;
            }

            if (pressedKey == Key.Space) {
                AddText(" ");
                args.Handled = true;
                return;
            }

            if ((pressedKey == Key.Delete || pressedKey == Key.Back) && SelectionLength > 0) {
                RemoveText(SelectionStart, SelectionLength);
                args.Handled = true;
                return;
            }

            if (pressedKey == Key.Back && CaretIndex > 0) {
                int caretIndex = CaretIndex;

                if (CaretIndex > 0 && CaretIndex < Text.Length)
                    --caretIndex;

                RemoveText(CaretIndex - 1, 1);
                CaretIndex = caretIndex;

                args.Handled = true;
                return;
            }

            if (pressedKey == Key.Delete && CaretIndex < Text.Length) {
                RemoveText(CaretIndex, 1);
                args.Handled = true;
                return;
            }
        }

        private void OnPreviewTextInput(object sender, TextCompositionEventArgs args)
        {
            AddText(args.Text);
            args.Handled = true;
        }

        private void PreviewExecutedHandler(object sender, ExecutedRoutedEventArgs args)
        {
            if (args.Command == ApplicationCommands.Copy || args.Command == ApplicationCommands.Cut)
                args.Handled = true;

            if (args.Command == ApplicationCommands.Paste && Clipboard.ContainsText()) {
                AddText(Clipboard.GetText());
                args.Handled = true;
            }
        }
        private void RemoveText(int startIndex, int count)
        {
            int caretIndex = CaretIndex;

            for (int i = 0; i < count; ++i) {
                SecureString password = Password.Copy();
                password.RemoveAt(startIndex);
                Password = password;
            }

            CaretIndex = caretIndex;
        }

        #endregion Private Methods
    }
}