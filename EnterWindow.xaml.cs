using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BorDer_PR18_01._01
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Entities _db = new Entities();

        int _enterAttemptsCount = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void buttonEnter_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(textBoxLogin.Text)
                || String.IsNullOrEmpty(textBoxPassword.Text))
            {
                MessageBox.Show("Заполните все поля");
                return;
            }

            user user = _db.users.FirstOrDefault(_user => _user.login == textBoxLogin.Text);
            if (user == null)
            {
                MessageBox.Show("Такого пользователя в системе нет. Попробуйте снова, либо зарегистрируйтесь");
                return;
            }

            if (captchaBlock.Visibility == Visibility.Visible)
            {
                if (textBoxCaptcha.Text != captchaText.Text)
                {
                    Task.Run(() => MessageBox.Show("Капча введена неверно. \nВведите капчу повторно"));
                    loadCaptcha();
                    return;
                }
            }

            string password;

            if (textBoxPassword.Visibility == Visibility.Visible)
            {
                password = textBoxPassword.Text;
            }
            else
            {
                password = passwordBoxPassword.Password;
            }

            if (password == user.password)
            {
                Task.Run(() => MessageBox.Show($"Успешный вход под ролью \n{user.role.name}"));

                //Администратор
                if (user.roleId == 1)
                {
                    WindowForAdministrator window = new WindowForAdministrator(user, this);
                }
                //Пользователь
                else
                {

                }

                captchaBlock.Visibility = Visibility.Collapsed;
                _enterAttemptsCount = 0;
            }
            //Пароль введен неверно
            else
            {
                captchaBlock.Visibility = Visibility.Visible;
                _enterAttemptsCount++;
            }

            

        }

        private void buttonRegister_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonChangeCaptcha_Click(object sender, RoutedEventArgs e)
        {
            loadCaptcha();
        }

        private void loadCaptcha()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789";
            Random random = new Random();

            captchaText.Text = Enumerable.Repeat(chars, 6).Select(s => s[random.Next(s.Length)]).ToString();
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton button = (ToggleButton)sender;

            if (button.IsChecked == true)
            {
                textBoxPassword.Visibility = Visibility.Visible;
                passwordBoxPassword.Visibility = Visibility.Collapsed;

                textBoxPassword.Text = passwordBoxPassword.Password;
            }
            else
            {
                textBoxPassword.Visibility = Visibility.Collapsed;
                passwordBoxPassword.Visibility = Visibility.Visible;

                passwordBoxPassword.Password = textBoxPassword.Text;
            }
        }
    }
}
