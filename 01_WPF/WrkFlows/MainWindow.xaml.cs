using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WrkFlows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {

            var services1 = new ServiceCollection();
            services1.AddWpfBlazorWebView();
            services1.AddBlazorWebViewDeveloperTools();
            var serviceProvider = services1.BuildServiceProvider();
            Resources.Add("services1", serviceProvider);

            InitializeComponent();

        }
    }
}
