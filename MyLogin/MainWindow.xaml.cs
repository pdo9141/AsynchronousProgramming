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

        private void BasicButton_Click(object sender, RoutedEventArgs e)
        {
            CallBigImportantMethod();
            BasicButton.Content = "Waiting...";
        }

        private async void CallBigImportantMethod()
        {
            var result = await BigLongImportantMethodAsync("Phillip");
            BasicButton.Content = result;
        }

        private Task<string> BigLongImportantMethodAsync(string name)
        {
            return Task.Factory.StartNew(() => BigLongImportantMethod(name));
        }

        private string BigLongImportantMethod(string name)
        {
            Thread.Sleep(3000);
            return "Hello, " + name;
        }
        
        private async void AsyncAwaitLoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = await LoginAsync();

                // if you must block using Wait or Result
                // run it on a seperate thread to avoid deadlocking your application using below code
                //var result = Task.Run(() => LoginAsync()).Result;

                // everything after the await keyword is your continuation code
                AsyncAwaitLoginButton.Content = result;
            }
            catch (Exception)
            {
                AsyncAwaitLoginButton.Content = "Login failed!";
            }
        }

        private async Task<string> LoginAsync()
        {
            try
            {
                // continuation will run on UI thread when using async/await keywords
                var loginTask = Task.Run(() => {
                    Thread.Sleep(2000);
                    return "Login Successful!";
                });

                var logTask = Task.Delay(2000); // Log the login

                var purchaseTask = Task.Delay(1000); // Fetch purchases

                await Task.WhenAll(loginTask, logTask, purchaseTask);

                // everything after the await keyword is your continuation code
                return loginTask.Result;
            }
            catch (Exception)
            {
                return "Login failed!";
            }
        }

        /// <summary>
        /// marking methods with "async void" is evil and should be avoided at all cost
        /// but since event handlers must comply to an exact delegate signature
        /// our hands are tied and we must do so
        /// we must mark this event handler with the async keyword and await BadLoginAsync
        /// to handle UnauthorizedAccessException thrown by BadLoginAsync
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AsyncAwaitBadLoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = await BadLoginAsync();

                // everything after the await keyword is your continuation code
                AsyncAwaitBadLoginButton.Content = result;
            }
            catch (Exception)
            {
                AsyncAwaitBadLoginButton.Content = "Login Failed!";
            }
        }

        private async Task<string> BadLoginAsync()
        {
            try
            {
                // continuation will run on UI thread when using async/await keywords
                var result = await Task.Run(() => {
                    throw new UnauthorizedAccessException();

                    Thread.Sleep(2000);
                    return "Login Successful!";
                });

                return result;
            }
            catch (Exception)
            {
                return "Login failed!";
            }
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
