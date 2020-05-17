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

namespace WaveEchoRGB
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public RGBHandler handler;

        public MainWindow()
        {
            InitializeComponent();
            handler = new RGBHandler();
            handler.StatusChanged += OnHandlerStatusChanged;
        }

        private void OnHandlerStatusChanged(string status)
        {
            textbox.AppendText(DateTime.Now.ToString("o") + " - " + status + Environment.NewLine);
            textbox.ScrollToEnd();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            handler.Start();
            checkbox.Content = "Started";
        }

        private void checkbox_Unchecked(object sender, RoutedEventArgs e)
        {
            handler.Pause();
            checkbox.Content = "Stopped";
        }

        private void checkbox_Copy_Checked(object sender, RoutedEventArgs e)
        {
            handler.Initialize();
            checkbox_Copy.Content = "On";
            checkbox.IsEnabled = true;
        }

        private void checkbox_Copy_Unchecked(object sender, RoutedEventArgs e)
        {

            handler.Stop();
            checkbox_Copy.Content = "Off";
            checkbox.IsEnabled = false;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            handler.Stop();
        }
    }
}
