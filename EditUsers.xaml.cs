using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Infrastructure;
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
using System.Windows.Shapes;
using Mail_LIB;

namespace BorDer_PR18_01._01
{
    /// <summary>
    /// Логика взаимодействия для EditUsers.xaml
    /// </summary>
    public partial class EditUsers : Window
    {
        private user _user;
        public EditUsers(user user)
        {
            InitializeComponent();
            this._user = user;
            using (Entities db = new Entities())
            {

                this.DataContext = user;
                surname.Text = user.surname;
                name.Text = user.name;
                patronymic.Text = user.patronymic;
                email.Text = user.email;
                login.Text = user.login;
                password.Text = user.password;



                roleId.ItemsSource = db.roles.ToList();
                status.ItemsSource = db.userStatuses.ToList();


                roleId.SelectedIndex = user.role.id - 1;
                status.SelectedIndex = user.userStatus.id - 1;


            }
        }

        private void button_edit_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(surname.Text)
                || String.IsNullOrEmpty(name.Text)
                || String.IsNullOrEmpty(patronymic.Text)
                || String.IsNullOrEmpty(login.Text)
                || String.IsNullOrEmpty(email.Text)
                || String.IsNullOrEmpty(password.Text))
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }


            string _surname = surname.Text.Substring(0, 1).ToUpper() + surname.Text.Substring(1).ToLower();
            string _name = name.Text.Substring(0, 1).ToUpper() + name.Text.Substring(1).ToLower();
            string _patronymic = patronymic.Text.Substring(0, 1).ToUpper() + patronymic.Text.Substring(1).ToLower();

            if (!Mail_Class.check_mail(email.Text))
            {
                MessageBox.Show("Введите корректный email!");
                return;
            }

            if (!Mail_Class.check_login(login.Text))
            {
                MessageBox.Show("Введите безопасный логин!");
                return;
            }

            if (!Mail_Class.check_password(password.Text))
            {
                MessageBox.Show("Введите безопасный пароль!");
                return;
            }

            using (Entities db = new Entities())
            {
                user editingUser = db.users.FirstOrDefault(us => us.id == _user.id);

                if (editingUser != null)
                {
                    editingUser.surname = _surname;
                    editingUser.name = _name;
                    editingUser.patronymic = _patronymic;
                    editingUser.email = email.Text;
                    editingUser.login = login.Text;
                    editingUser.password = password.Text;
                    editingUser.roleId = ((role)roleId.SelectedItem).id;
                    editingUser.status = ((userStatus)status.SelectedItem).id;

                    try
                    {
                        Task.Run(() => db.SaveChanges());
                    }
                    catch (DbUpdateException ex)
                    {
                        Task.Run(() => MessageBox.Show($"{ex.Message} \nПодробности смотрите в выводе Debug"));
                        return;
                    }
                }
                else
                {
                    Task.Run(() => MessageBox.Show("Такого пользователя нет. Ошибка в системе"));
                    return;
                }
            }
        }
    }
}
