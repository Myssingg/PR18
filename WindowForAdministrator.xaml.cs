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
using System.Windows.Shapes;
using System.Data.Entity;
using System.Windows.Controls.Primitives;

namespace BorDer_PR18_01._01
{
    /// <summary>
    /// Логика взаимодействия для WindowForAdministrator.xaml
    /// </summary>
    public partial class WindowForAdministrator : Window
    {
        user _user;

        public WindowForAdministrator(user user)
        {
            InitializeComponent();
            _user = user;
            loadList();
            loadUserList();
        }

        private async void loadList()
        {
            using (Entities db = new Entities())
            {
                var animalsList = await db.animals.Include(a => a.image).ToListAsync();
                listView.ItemsSource = animalsList;
            }
        }

        private async void loadUserList()
        {
            using (Entities db = new Entities())
            {
                var userList = await db.users.Include(a => a.role).Include(b => b.userStatus).ToListAsync();
                usersGrid.ItemsSource = userList;
                
            }
        }

        //Кнопка "Выход" в верхней панели
        private async void TabItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private MediaPlayer mediaPlayer = new MediaPlayer();
        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = listView.SelectedItem as animal;
            if (selected != null)
            {
                gridDescription.Visibility = Visibility.Visible;

                // Заполняем данные из выбранного объекта
                txtName.Text = selected.name;
                textDescription.Text = selected.description;
                txtFacts.Text = selected.facts;

                // Используем то же преобразование для большой картинки
                imgAnimal.Source = selected.image.Image;
                var id = selected.id;
                string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sounds", $"{id}.wav");
                mediaPlayer.Open(new Uri(path, UriKind.Absolute));
                mediaPlayer.Play();
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            var selected = listView.SelectedItem as animal;
            EditAnimal editAnimal = new EditAnimal(selected, _user, false);
            Hide();
            editAnimal.ShowDialog();
            Show();
            loadList();
        }

        private void Button_Click_Add_Animal(object sender, RoutedEventArgs e)
        {
            var selected = new animal();
            EditAnimal editAnimal = new EditAnimal(selected, _user, true);
            Hide();
            var selectedIndex = listView.SelectedIndex;
            editAnimal.ShowDialog();
            Show();
            listView.SelectedIndex = selectedIndex;
            loadList();
        }

        private void usersGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            user item = (sender as DataGrid).SelectedItem as user;
            if (item == null) return;

            var selected = usersGrid.SelectedItem as user;
            EditUsers editUsers = new EditUsers(selected);
            this.Hide();
            var selectedIndex = listView.SelectedIndex;
            editUsers.ShowDialog();
            this.Show();
            listView.SelectedIndex = selectedIndex;
            loadList();
        }


    }
}
