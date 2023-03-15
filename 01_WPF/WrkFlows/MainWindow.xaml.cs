using Microsoft.AspNetCore.Builder;
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
using VxFormGenerator.Settings.Bootstrap;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace WrkFlows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {

            var services = new ServiceCollection();
            services.AddWpfBlazorWebView();
            services.AddBlazorWebView();
            services.AddBlazorWebViewDeveloperTools();

            //FxCore
            services.AddVxFormGenerator();
                                 
            var serviceProvider = services.BuildServiceProvider();
            Resources.Add("services", serviceProvider);
     
            InitializeComponent();

        }
    }
}
