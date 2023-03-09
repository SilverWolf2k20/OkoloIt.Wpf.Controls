using System.Globalization;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.ComponentModel;
using System.Collections;
using OkoloIt.Collections;

namespace OkoloIt.Wpf.Controls
{
    public class TreeListView : ListView
    {
        public static new readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                nameof(ItemsSource),
                typeof(IEnumerable),
                typeof(TreeListView),
                new PropertyMetadata(null, new PropertyChangedCallback(OnItemsSourceChange))
            );

        public new IEnumerable ItemsSource {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        private static void OnItemsSourceChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not TreeListView treeListView || e.NewValue is not ITreeNode node)
                return;

            treeListView.GetNode(treeListView.Items, node);
        }

        private void GetNode(ItemCollection items, ITreeNode node)
        {
            foreach (var child in node) {
                if (child is not ITreeNode treeNode)
                    continue;

                var newNode = new TreeListViewNode() { 
                    Data = treeNode.GetData(), 
                    Level = treeNode.Level,
                    IsExpandable = treeNode.IsLeaf == false
                };

                items.Add(newNode);

                if (treeNode.IsLeaf == false)
                    GetNode(items, treeNode);
            }
        }

        protected override DependencyObject GetContainerForItemOverride()
            => new TreeListViewItem();

        protected override bool IsItemItsOwnContainerOverride(object? item)
            => item is TreeListViewItem;

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            if (element is not TreeListViewItem treeItem || item is not TreeListViewNode node)
                return;

            treeItem.Node = item as TreeListViewNode;
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
            => new Thickness((int)o * IndentSize, 0, 0, 0);

        public object ConvertBack(object o, Type type, object parameter, CultureInfo culture)
            => DependencyProperty.UnsetValue;
    }

    public class CanExpandConverter : IValueConverter
    {
        public object Convert(object o, Type type, object parameter, CultureInfo culture)
            => (bool)o ? Visibility.Visible : Visibility.Hidden;

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
        public bool IsExpandable { get; set; }
    }
}