using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

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

            tree.ItemsSource = new ObservableCollection<TreeListViewNode>() {
                new TreeListViewNode() { Data = "123423-2354254-345235434", Level= 1 },
                new TreeListViewNode() { Data = "223423-2354254-345235434", Level= 2 },
                new TreeListViewNode() { Data = "323423-2354254-345235434", Level= 3 },
                new TreeListViewNode() { Data = "423423-2354254-345235434", Level= 4 },
            };
        }
    }

    public class User
    {
        public User(string name, int age)
        {
            Name = name;
            Age = age;
        }

        public string Name { get; set; }
        public int Age { get; set; }
    }

    /*public class Node
    {
        public User User { get; set; } = new User(string.Empty, default);
        public bool IsRed { get; set; } = false;
        public ObservableCollection<Node> Nodes { get; set; } = new();
    }*/

    public class Model : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private SecureString _password;

        public Model()
        {
            _password = new SecureString();
            Click = new Command(OnClick);

            /*_nodes = new() {
                new Node
                {
                    User = new User("Вика", 19),
                    Nodes = new ObservableCollection<Node>
                    {
                        new Node { User = new User("Катя", 7), IsRed = true },
                        new Node { User = new User("Денис", 23) },
                        new Node {
                            User = new User("Яна", 30),
                            Nodes = new ObservableCollection<Node> {
                                new Node { User = new User("Виталик", 15) },
                                new Node { User = new User("Антон", 31), IsRed = true },
                                new Node { User = new User("Ира", 32), IsRed = true },
                                new Node { User = new User("Аня", 18) },
                            }
                        }
                    }
                },
                new Node {
                    User = new User("Лера", 20),
                    Nodes = new ObservableCollection<Node> {
                        new Node { User = new User("Петя", 9) },
                        new Node { User = new User("Вася", 24), IsRed = true },
                        new Node { User = new User("Серега", 17) }
                    }
                },
                new Node { User = new User("Коля", 15), IsRed = true },
                new Node { User = new User("Кирилл", 11) },
                new Node { User = new User("Иван", 10) }
            };*/
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

    public class BoolToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isRed = (bool)value;

            if (isRed)
                return Brushes.Red;

            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => DependencyProperty.UnsetValue;
    }

    public class NodeToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var user = (User)value;
            return $"{user.Name} - {user.Age}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => DependencyProperty.UnsetValue;
    }

    public class Command : ICommand
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