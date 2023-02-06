using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows;
using System.Windows.Input;

namespace OkoloIt.Wpf.Controls.Samples
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            /*tlistProducts.ItemsSource = new List<Node>() {
                new Node() { IsExpandable = true, Data = new User("Ivan 228", "14"), Level = 1 },
                new Node() { IsExpandable = true, Data = new User("Ivan 337", "16"), Level = 2 },
                new Node() { IsExpandable = true, Data = new User("Ivan 322", "15"), Level = 1 },
                new Node() { IsExpandable = true, Data = new User("Ivan 123", "11"), Level = 2 },
            };*/
        }

        public class User
        {
            public User(string name, string age)
            {
                Name = name;
                Age = age;
            }

            public string Name { get; set; }
            public string Age { get; set; }
        }

    }

    public class Model : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private SecureString _password;

        public Model()
        {
            _password = new SecureString();
            Click = new Command(OnClick);
        }

        public SecureString Password {
            get => _password;
            set {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        private void OnClick(object? sender)
        {
            Trace.WriteLine($"[PBOX]{ConvertToString(Password)}");

            var password = new SecureString();

            password.AppendChar('0');
            password.AppendChar('1');
            password.AppendChar('2');
            password.AppendChar('3');

            Password = password;
        }

        public ICommand Click { get; init; }

        public void OnPropertyChanged(string propertyName)
           => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public static string ConvertToString(SecureString text)
        {
            IntPtr ptr = IntPtr.Zero;
            string result = string.Empty;
            try {
                ptr = Marshal.SecureStringToGlobalAllocUnicode(text);
                result = Marshal.PtrToStringUni(ptr)!;
            }
            finally {
                if (ptr != IntPtr.Zero)
                    Marshal.ZeroFreeGlobalAllocUnicode(ptr);
            }
            return result;
        }
    }

    class Command : ICommand
    {
        private readonly Predicate<object?>? _canExecute;
        private readonly Action<object?>? _execute;

        public Command(Action<object?>? execute)
            : this(execute, null)
        {
        }

        public Command(Action<object?>? execute, Predicate<object?>? canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }


        public bool CanExecute(object? parameter)
            => _canExecute == null || _canExecute(parameter);

        public void Execute(object? parameter)
        {
            if (_execute is null)
                return;

            _execute(parameter);
        }
    }

}