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
using System.Threading;

namespace Cinema
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Page
    {
        public Login()
        {
            InitializeComponent();
        }
        private void BlockInputs()
        {
            this.Dispatcher.Invoke(() =>
            {
                LoginTextBox.IsEnabled = false;
                PasswordBox.IsEnabled = false;
                PasswordBox.Password = "";
                LoginButton.IsEnabled = false;
            });
            for(var i = Properties.Settings.Default.LoginBlockTimeout; i>0; i--)
            {
                this.Dispatcher.Invoke(() => { LoginTextBox.Text = $"До конца блокировки осталось: {i} секунд."; });
                Thread.Sleep(1000);
            }
            this.Dispatcher.Invoke(() =>
            {
                LoginTextBox.IsEnabled = true;
                LoginTextBox.Text = "";
                LoginTextBox.IsEnabled = true;
                LoginButton.IsEnabled = true;
            });
            Properties.Settings.Default.LoginTries = 0;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {            
            string username = LoginTextBox.Text;
            string password = PasswordBox.Password;
            //WIP: anti-injection input tests
            string userRole = "";
            try
            {
                userRole = DBInteraction.LoginCheck(username, password);
            }
            catch(Exception ex)
            {
                if (ex.Message == Properties.Resources.UserNotFoundQueryResponse)
                {
                    MessageBox.Show($"Пользователь не найден. Проверьте логин и пароль.", "Ошибка");
                    Properties.Settings.Default.LoginTries += 1;
                    if (Properties.Settings.Default.LoginTries >= Properties.Settings.Default.LoginBlockLimit)
                    {
                        Thread t = new Thread(new ThreadStart(BlockInputs));
                        t.Start();
                        return;
                    }
                    return;
                }
                else
                {
                    MessageBox.Show($"Произошла ошибка при попытке входа в систему: {ex.Message}. Обратитесь к администратору и попробуйте позже.", "Ошибка");
                    return;
                }
            }

            userRole = userRole.ToLower();
            switch (userRole)
            {
                case "client":
                    ClientInterface clientInterface = new ClientInterface();
                    NavigationService.Navigate(clientInterface);
                    break;
                case "manager":
                    ManagerInterface managerInterface = new ManagerInterface();
                    NavigationService.Navigate(managerInterface);
                    break;
                case "admin":
                    AdminInterface adminInterface = new AdminInterface();
                    NavigationService.Navigate(adminInterface);
                    break;

                default:
                    MessageBox.Show("Произошла ошибка при попытке входа в систему: Ваша роль неизвестна системе.", "Ошибка");
                    break;
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
