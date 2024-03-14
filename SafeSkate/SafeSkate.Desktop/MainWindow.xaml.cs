using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SafeSkate.Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //this.Loaded += async (sender, e) => await Window_LoadedAsync(sender, e);
        }

        private async Task Window_LoadedAsync(object sender, RoutedEventArgs e)
        {
            //await ViewModelLocator.Instance.InitializeAsync();
            DataContext = ViewModelLocator.Instance.MainWindowViewModel;
        }
    }
}