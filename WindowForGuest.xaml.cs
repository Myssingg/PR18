using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Media;
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

namespace BorDer_PR18_01._01
{
    /// <summary>
    /// Логика взаимодействия для WindowForGuest.xaml
    /// </summary>
    public partial class WindowForGuest : Window
    {
        public WindowForGuest()
        {
            InitializeComponent();

            loadList();
        }

        private async void loadList()
        {
            using (Entities db = new Entities())
            {
                var animalsList = db.animals.Include(a => a.image).ToList();
                listView.ItemsSource = animalsList;
            }
        }

        private async void TabItem_Click(object sender, RoutedEventArgs e)
        {
            EnterWindow window = new EnterWindow();
            this.Hide();
            window.ShowDialog();

            
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

                // Используем то же преобразование для большой картинки
                imgAnimal.Source = selected.image.Image;
            
            }
        }
    }
}
