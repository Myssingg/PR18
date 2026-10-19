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
using System.Windows.Threading;

namespace BorDer_PR18_01._01
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class EnterWindow : Window
    {
        Entities _db = new Entities();

        int _enterAttemptsCount = 0;
        DispatcherTimer _timer = new DispatcherTimer();
        int _timerTime = 180; //180 секунд таймера

        public EnterWindow()
        {
            InitializeComponent();

            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += timerTick;
        }

        //Действия таймера
        void timerTick(object sender, EventArgs e)
        {
            _timerTime--;
            if(_timerTime == 0)
            {
                tmrTime.Visibility = Visibility.Collapsed;
                buttonEnter.IsEnabled = true;
                _timer.Stop();
                _timerTime = 180;
            }
            else
            {
                tmrTime.Visibility = Visibility.Visible;
                buttonEnter.IsEnabled = false;
                tmrTime.Text = TimeSpan.FromSeconds(_timerTime).ToString(@"mm\:ss");
            }
        }

        private async void buttonEnter_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(textBoxLogin.Text)
                || (String.IsNullOrEmpty(textBoxPassword.Text)
                && String.IsNullOrEmpty(passwordBoxPassword.Password)))
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
                    WindowForAdministrator window = new WindowForAdministrator(user);
                    this.Hide();
                    window.ShowDialog();
                    this.Show();
                }
                //Пользователь
                else 
                {
                    WindowForUser window = new WindowForUser(user);
                    this.Hide();
                    window.ShowDialog();
                    this.Show();
                }

                captchaBlock.Visibility = Visibility.Collapsed;
                if (this.Height > 350) this.Height -= 120;
                _enterAttemptsCount = 0;
            }
            //Пароль введен неверно
            else
            {
                captchaBlock.Visibility = Visibility.Visible;
                this.Height += 120;
                _enterAttemptsCount++;
            }

            if (_enterAttemptsCount == 3)
            {
                _enterAttemptsCount = 0;

                _timer.Start();
            }

        }

        private void buttonRegister_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow window = new RegisterWindow();
            this.Hide();
            window.ShowDialog();
            

            if (window.DialogResult == true)
            {
                WindowForUser windowForUser = new WindowForUser(window.GetRegisteredUser);
                this.Hide();
                windowForUser.ShowDialog();
            }
            else
            {
                Task.Run(() => MessageBox.Show("Ошибка регистрации. Попробуйте еще раз"));
            }
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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
