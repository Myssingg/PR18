using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
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

namespace BorDer_PR18_01._01
{
    /// <summary>
    /// Логика взаимодействия для EditAnimal.xaml
    /// </summary>
    public partial class EditAnimal : Window
    {
        private animal _animal;
        user _user;
        private bool _create;
        public EditAnimal(animal animal, user user, bool create)
        {
            InitializeComponent();

            this._animal = animal;
            this._create = create;
            if (!create)
            {
                button_create_edit.Content = "Редактировать";
                SizeToContent = SizeToContent.Height;
            }
            else
            {
                button_create_edit.Content = "Создать";
            }
            Entities db = new Entities();
            this.DataContext = animal;
            Photo.ItemsSource = db.images.ToList();
            image.DataContext = animal.image;
        }

        private void Photo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            image.DataContext = Photo.SelectedItem;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            using (Entities db = new Entities())
            {
                if (!_create)
                {


                    animal animal = db.animals.FirstOrDefault(a => a.id == _animal.id);

                    animal.name = nameq.Text;
                    animal.description = description.Text;
                    animal.facts = facts.Text;
                    animal.imageId = Photo.SelectedIndex + 1;

                    try
                    {
                        db.SaveChanges();
                    }
                    catch (DbUpdateException ex)
                    {
                        Task.Run(() => MessageBox.Show("Ошибка сохранения базы. Проверьте Debug, либо заполненность всех полей"));
                        Debug.WriteLine(ex);
                        return;
                    }
                    catch (Exception ex)
                    {
                        Task.Run(() => MessageBox.Show("Ошибка! " + ex.Message));
                        return;
                    }
                    MessageBox.Show("Изменения сохранены");
                    WindowForAdministrator windowForAdministrator = new WindowForAdministrator(_user);
                    DialogResult = true;
                }
                else
                {
                    animal animal = new animal();
                    animal.name = nameq.Text;
                    animal.description = description.Text;
                    animal.facts = facts.Text;
                    animal.imageId = Photo.SelectedIndex + 1;

                    db.animals.Add(animal);
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (DbUpdateException ex)
                    {
                        Task.Run(() => MessageBox.Show("Ошибка сохранения базы. Проверьте Debug, либо заполненность всех полей"));
                        Debug.WriteLine(ex);
                        return;
                    }
                    catch (Exception ex)
                    {
                        Task.Run(() => MessageBox.Show("Ошибка! " + ex.Message));
                        return;
                    }
                    MessageBox.Show("Животное добавлено");
                    WindowForAdministrator windowForAdministrator = new WindowForAdministrator(_user);
                    DialogResult = true;
                }
            }
        }

        private void description_TextChanged(object sender, TextChangedEventArgs e)
        {
            SizeToContent = SizeToContent.Height;
        }

        private void facts_TextChanged(object sender, TextChangedEventArgs e)
        {
            SizeToContent = SizeToContent.Height;
        }
    }
}
