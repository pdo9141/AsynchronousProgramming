using System.Net;
using System.Windows;

namespace ReliableWPFApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int _count = 1;

        public MainWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
        }

        private void RssButton_Click(object sender, RoutedEventArgs e)
        {
            RssButton.IsEnabled = false;

            var client = new WebClient();
            client.DownloadStringAsync(new System.Uri("http://www.filipekberg.se/rss/"));
            client.DownloadStringCompleted += Client_DownloadStringCompleted;
        }

        private void Client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            RssText.Text = e.Result;
            RssButton.IsEnabled = true;
        }

        private void CounterButton_Click(object sender, RoutedEventArgs e)
        {
            CounterText.Text = $"Counter: { _count++ }";
        }
    }
}
