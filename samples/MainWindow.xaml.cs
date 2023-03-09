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

using OkoloIt.Collections.Generic;

namespace OkoloIt.Wpf.Controls.Samples
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
            => InitializeComponent();
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
        private ITreeNode<User> _nodes;

        public ITreeNode<User> Nodes {
            get => _nodes;
            set {
                _nodes = value;
                OnPropertyChanged(nameof(Nodes));
            }
        }

        public Model()
        {
            _password = new SecureString();

            TreeNode<User> tree = new(new User("Иван", 12));

            TreeNode<User> node1 = new(new User("Николай", 11), tree);

            node1.AddNode(new TreeNode<User>(new User("Оля", 14), node1));
            node1.AddNode(new TreeNode<User>(new User("Виктор", 10), node1));

            TreeNode<User> node11 = new(new User("Катя", 17), node1);

            node11.AddNode(new TreeNode<User>(new User("Андрей", 15), node11));
            node11.AddNode(new TreeNode<User>(new User("Стас", 20), node11));

            node1.AddNode(node11);

            node1.AddNode(new TreeNode<User>(new User("Вика", 16), node1));

            TreeNode<User> node2 = new(new User("Сеня", 16), tree);

            node2.AddNode(new TreeNode<User>(new User("Петя", 16), node2));

            tree.AddNode(node1);
            tree.AddNode(node2);

            _nodes = tree;
        }

        public SecureString Password {
            get => _password;
            set {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

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