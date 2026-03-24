using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using PlacesToVisit.Models;

namespace PlacesToVisit.Pages
{
    /// <summary>
    /// Interaction logic for AddPlacePage.xaml
    /// </summary>
    public partial class AddPlacePage : Page
    {
        // Змінна для збереження шляху вибраного файлу
        private string _selectedFilePath = "";

        public AddPlacePage()
        {
            InitializeComponent();
        }

        // Метод для кнопки "Вибрати фото"
        private void SelectImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png";

            if (openFileDialog.ShowDialog() == true)
            {
                _selectedFilePath = openFileDialog.FileName;
                ImageInput.Text = _selectedFilePath; // Показуємо шлях у текстовому полі
            }
        }

        private void SavePlace_Click(object sender, RoutedEventArgs e)
        {
            string newName = NameInput.Text;
            string newOpis = DescriptionInput.Text;

            if (string.IsNullOrEmpty(newName) || string.IsNullOrEmpty(newOpis))
            {
                MessageBox.Show("Proszę wypełnić przynajmniej nazwę i opis", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // Зупиняємо код, щоб не зберігати пусту форму!
            }

            string finalImagePath = "";

            // Логіка копіювання картинки (якщо користувач її вибрав)
            if (!string.IsNullOrEmpty(_selectedFilePath))
            {
                // Створюємо папку Images біля запущеної програми
                string targetFolder = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Images");
                Directory.CreateDirectory(targetFolder);

                // Беремо ім'я файлу і створюємо новий шлях
                string fileName = Path.GetFileName(_selectedFilePath);
                string destinationPath = Path.Combine(targetFolder, fileName);

                // Копіюємо файл
                File.Copy(_selectedFilePath, destinationPath, true);

                // Записуємо правильний відносний шлях для JSON
                finalImagePath = destinationPath;
            }
            else
            {
                // Якщо картинку не вибрали, записуємо те, що ввели вручну (або залишаємо пустим)
                finalImagePath = ImageInput.Text;
            }

            Place newPlace = new Place()
            {
                Name = newName,
                Opis = newOpis,
                ImageUrl = finalImagePath
            };

            string filePath = "Data/places.json";
            List<Place>? places = new List<Place>();

            if (File.Exists(filePath))
            {
                string jsonString = File.ReadAllText(filePath);
                if (!string.IsNullOrWhiteSpace(jsonString))
                {
                    places = JsonSerializer.Deserialize<List<Place>>(jsonString) ?? new List<Place>();
                }
            }

            places.Add(newPlace);

            var options = new JsonSerializerOptions { WriteIndented = true };
            string newJsonString = JsonSerializer.Serialize(places, options);
            File.WriteAllText(filePath, newJsonString);

            MessageBox.Show("Success", "Success", MessageBoxButton.OK);

            // Очищаємо форму після успішного збереження
            NameInput.Clear();
            DescriptionInput.Clear();
            ImageInput.Clear();
            _selectedFilePath = "";
        }
    }
}