using System.Globalization;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.ComponentModel;

namespace OkoloIt.Wpf.Controls
{
    public class TreeListView : ListView
    {
        protected override DependencyObject GetContainerForItemOverride()
            => new TreeListViewItem();

        protected override bool IsItemItsOwnContainerOverride(object? item)
            => item is TreeListViewItem;

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            if (element is not TreeListViewItem ti || item is not TreeListViewNode node)
                return;

            ti.Node = item as TreeListViewNode;
            base.PrepareContainerForItemOverride(element, node);
        }

        static TreeListView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(TreeListView), 
                new FrameworkPropertyMetadata(typeof(TreeListView))
            );
        }
    }

    public class LevelToIndentConverter : IValueConverter
    {
        private const double IndentSize = 19.0;

        public object Convert(object o, Type type, object parameter, CultureInfo culture)
        {
            return new Thickness((int)o * IndentSize, 0, 0, 0);
        }

        public object ConvertBack(object o, Type type, object parameter, CultureInfo culture)
            => DependencyProperty.UnsetValue;
    }

    public class TreeListViewItem : ListViewItem, INotifyPropertyChanged
    {
        private TreeListViewNode? _node;

        public event PropertyChangedEventHandler? PropertyChanged;

        public TreeListViewNode? Node {
            get => _node;
            internal set {
                _node = value;
                OnPropertyChanged(nameof(Node));
            }
        }

        private void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class TreeListViewNode
    {
        public object Data { get; set; } = new object();
        public int Level { get; set; } = default;
    }
}