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
using System.Text.Json;
using PlacesToVisit.Models;
using System.IO;

namespace PlacesToVisit.Pages
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
            LoadData();

        }
        private void LoadData()
        {
            string filePath = "Data/places.json";

            if (File.Exists(filePath))
            {
                string jsonString = File.ReadAllText(filePath);
                // десеріалізація - перетворюємо текст у список об'єктів list<place>
                List<Place> places = JsonSerializer.Deserialize<List<Place>>(jsonString);

                PlacesList.ItemsSource = places;
            }
        }
        private void DeletePlace_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            if (menuItem == null) return;

            Place placeToDelete = menuItem.DataContext as Place;
            if (placeToDelete == null) return;

            MessageBoxResult result = MessageBox.Show(
                $"Are you sure you want to delete '{placeToDelete.Name}'?",
                "Delete confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                string filePath = "Data/places.json";

                if (File.Exists(filePath))
                {
                    string jsonString = File.ReadAllText(filePath);
                    List<Place> places = JsonSerializer.Deserialize<List<Place>>(jsonString) ?? new List<Place>();
                    Place itemToRemove = places.FirstOrDefault(p => p.Name == placeToDelete.Name);

                    if (itemToRemove != null)
                    {
                        places.Remove(itemToRemove);
                        var options = new JsonSerializerOptions { WriteIndented = true };
                        File.WriteAllText(filePath, JsonSerializer.Serialize(places, options));

                        LoadData();
                    }
                }
            }
        }
    }
}
