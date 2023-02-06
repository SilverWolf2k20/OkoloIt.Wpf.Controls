using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace OkoloIt.Wpf.Controls
{
    public class TreeListView : ListView
    {
        #region Good

        protected override DependencyObject GetContainerForItemOverride()
            => new TreeListViewItem();

        protected override bool IsItemItsOwnContainerOverride(object? item)
            => item is TreeListViewItem;

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            if (element is not TreeListViewItem ti || item is not Node node)
                return;

            ti.Node = item as Node;
            base.PrepareContainerForItemOverride(element, node.Data);
        }

        #endregion Good

        static TreeListView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(TreeListView),
                new FrameworkPropertyMetadata(typeof(TreeListView))
            );
        }
    }

    public class TreeListViewItem : ListViewItem, INotifyPropertyChanged
    {
        private Node? _node;

        public event PropertyChangedEventHandler? PropertyChanged;

        public Node? Node {
            get => _node;
            internal set {
                _node = value;
                OnPropertyChanged(nameof(Node));
            }
        }

        private void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class Node
    {
        public bool IsExpandable { get; set; } = default;
        public object Data { get; set; } = new object();
        public int Level { get; set; } = default;
    }

    internal class CanExpandConverter : IValueConverter
    {
        public object Convert(object o, Type type, object parameter, CultureInfo culture)
        {
            if ((bool)o)
                return Visibility.Visible;

            return Visibility.Hidden;
        }

        public object ConvertBack(object o, Type type, object parameter, CultureInfo culture)
            => DependencyProperty.UnsetValue;
    }

    internal class LevelToIndentConverter : IValueConverter
    {
        private const double IndentSize = 19.0;

        public object Convert(object o, Type type, object parameter, CultureInfo culture)
        {
            Trace.WriteLine($"########################## Level {(int)o * IndentSize}");
            return new Thickness((int)o * IndentSize, 0, 0, 0);
        }

        public object ConvertBack(object o, Type type, object parameter, CultureInfo culture)
            => DependencyProperty.UnsetValue;
    }
}