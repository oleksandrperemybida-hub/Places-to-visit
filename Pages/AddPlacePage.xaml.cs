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
        private string _selectedFilePath = "";

        public AddPlacePage()
        {
            InitializeComponent();
        }

        private void SelectImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png";

            if (openFileDialog.ShowDialog() == true)
            {
                _selectedFilePath = openFileDialog.FileName;
                ImageInput.Text = _selectedFilePath; 
            }
        }

        private void SavePlace_Click(object sender, RoutedEventArgs e)
        {
            string newName = NameInput.Text;
            string newOpis = DescriptionInput.Text;

            try
            {
                if (string.IsNullOrEmpty(newName) || string.IsNullOrEmpty(newOpis))
                {
                    MessageBox.Show("Proszę wypełnić przynajmniej nazwę i opis", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return; 
                }

                string finalImagePath = "";

                if (!string.IsNullOrEmpty(_selectedFilePath))
                {
                    string targetFolder = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Images");
                    Directory.CreateDirectory(targetFolder);

                    string fileName = Path.GetFileName(_selectedFilePath);
                    string destinationPath = Path.Combine(targetFolder, fileName);

                    File.Copy(_selectedFilePath, destinationPath, true);

                    finalImagePath = destinationPath;
                }
                else
                {
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


                NameInput.Clear();
                DescriptionInput.Clear();
                ImageInput.Clear();
                _selectedFilePath = "";
            }
            
            catch 
            {
                MessageBox.Show("Something went wrong", "Problem", MessageBoxButton.OK);
            }
        }
    }
}