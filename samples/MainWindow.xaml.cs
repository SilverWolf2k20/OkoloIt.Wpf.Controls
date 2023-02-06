using System.Collections.Generic;
using System.Windows;

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

            tlistProducts.ItemsSource = new List<Node>() {
                new Node() { IsExpandable = true, Data = new User("Ivan 228", "14"), Level = 1 },
                new Node() { IsExpandable = true, Data = new User("Ivan 337", "16"), Level = 2 },
                new Node() { IsExpandable = true, Data = new User("Ivan 322", "15"), Level = 1 },
                new Node() { IsExpandable = true, Data = new User("Ivan 123", "11"), Level = 2 },
            };

            UpdateLayout();
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
}