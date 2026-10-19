using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
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
using System.Windows.Shapes;
using Mail_LIB;

namespace BorDer_PR18_01._01
{
    /// <summary>
    /// Логика взаимодействия для RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        user _user;

        public RegisterWindow()
        {
            InitializeComponent();
        }

        public user GetRegisteredUser
        {
            get
            {
                return _user;   
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            //Проверка заполненности полей
            if (String.IsNullOrEmpty(textBoxSurname.Text)
                || String.IsNullOrEmpty(textBoxName.Text)
                || String.IsNullOrEmpty(textBoxPatronymic.Text)
                || String.IsNullOrEmpty(textBoxEmail.Text)
                || String.IsNullOrEmpty(textBoxLogin.Text)
                || String.IsNullOrEmpty(textBoxPassword.Text)
                || String.IsNullOrEmpty(textBoxConfirmPassword.Text))
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }

            string surname = textBoxSurname.Text.Substring(0, 1).ToUpper() + textBoxSurname.Text.Substring(1).ToLower();
            string name = textBoxName.Text.Substring(0, 1).ToUpper() + textBoxName.Text.Substring(1).ToLower();
            string patronymic = textBoxPatronymic.Text.Substring(0, 1).ToUpper() + textBoxPatronymic.Text.Substring(1).ToLower();


            if (!Mail_Class.check_mail(textBoxEmail.Text))
            {
                MessageBox.Show("Адрес электронной почты не корректен!");
                return;
            }

            if (!Mail_Class.check_login(textBoxLogin.Text))
            {
                MessageBox.Show("Логин не соответствует нормам. \n(Длина не менее 6 символов + одна цифра)");
                return;
            }

            if (!Mail_Class.check_password(textBoxPassword.Text))
            {
                MessageBox.Show("Пароль не соответствует нормам. \n(Длина не менее 8 символов. Не менее 1 заглавной буквы и 1 цифры)");
                return;
            }

            if (textBoxPassword.Text != textBoxConfirmPassword.Text)
            {
                MessageBox.Show("Пароли не совпадают!");
                return;
            }

            using (Entities db = new Entities())
            {
                string _login = textBoxLogin.Text;
                if (await db.users.FirstOrDefaultAsync(_user => _user.login == _login) != null)
                {
                    MessageBox.Show("Пользователь с таким логином уже занят!");
                    return;
                }

                try
                {
                    _user = new user()
                    {
                        surname = surname,
                        name = name,
                        patronymic = patronymic,
                        email = textBoxEmail.Text,
                        login = _login,
                        password = textBoxPassword.Text,
                    };

                    db.users.Add(_user);
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    Task.Run(() => MessageBox.Show($"{ex.Message} \nПодробности смотрите в выводе Debug"));
                    DialogResult = false;
                    return;
                }
            }


            DialogResult = true;
        }
    }
}
