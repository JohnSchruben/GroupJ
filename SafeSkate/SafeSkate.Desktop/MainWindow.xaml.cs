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

namespace SafeSkate.Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            //ServiceTypeProvider.ServerIp = "safeskate2.eastus.cloudapp.azure.com";
            ServiceTypeProvider.ServerIp =  "20.83.148.72";
            ServiceTypeProvider.UpdatePort = 9000;
            ServiceTypeProvider.QueryPort = 9001;
            InitializeComponent();
            var test = ServiceTypeProvider.Instance.MapMarkerInfoCollectionProxy;
            Console.WriteLine(test.MapMarkerInfos);
        }
    }
}