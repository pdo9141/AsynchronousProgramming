using System;
using System.Windows;
using System.Threading;
using System.Threading.Tasks;

namespace MyLogin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            LoginButton.IsEnabled = false;

            var task = Task.Run(() => {
                Thread.Sleep(2000);
                return "Login Successful";
            });

            task.ContinueWith((t) => {
                // Must use Dispatcher here since task thread cannot update UI thread components directly
                Dispatcher.Invoke(() => {
                    LoginButton.Content = t.Result;
                    LoginButton.IsEnabled = true;
                });
            });
        }

        private void LoginAwaitButton_Click(object sender, RoutedEventArgs e)
        {
            LoginAwaitButton.IsEnabled = false;

            var task = Task.Run(() => {                
                Thread.Sleep(2000);
                return "Login Successful";
            });

            // Configure continuation to continue on UI thread, the original context
            task.ConfigureAwait(true)
                .GetAwaiter()
                .OnCompleted(() =>
                {
                    // No longer need to use Dispatcher since the continuation will happen on the original UI thread
                    LoginAwaitButton.Content = task.Result;
                    LoginAwaitButton.IsEnabled = true;
                });
        }

        private void BadLoginButton_Click(object sender, RoutedEventArgs e)
        {
            BadLoginButton.IsEnabled = false;

            var task = Task.Run(() => {
                throw new UnauthorizedAccessException();

                Thread.Sleep(2000);
                return "Login Successful";
            });

            task.ContinueWith((t) => {
                if (t.IsFaulted)
                {
                    // Must use Dispatcher here since task thread cannot update UI thread components directly
                    Dispatcher.Invoke(() =>
                    {
                        BadLoginButton.Content = "Login failed!";
                        BadLoginButton.IsEnabled = true;
                    });
                }
                else
                {
                    // Must use Dispatcher here since task thread cannot update UI thread components directly
                    Dispatcher.Invoke(() =>
                    {
                        BadLoginButton.Content = t.Result;
                        BadLoginButton.IsEnabled = true;
                    });
                }
            });
        }
    }
}
