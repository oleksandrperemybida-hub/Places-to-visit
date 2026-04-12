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
using PlacesToVisit.Pages;


namespace PlacesToVisit.Pages
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        private string _currentFilter;
        private List<Place> _allData;
        public HomePage(string filter = "All")
        {
            InitializeComponent();
            _currentFilter = filter;
            TitleChange();
            LoadData();

        }
        private void TitleChange()
        {
            if (_currentFilter == "Visited")
            {
                PageTitle.Text = "Visited places";
            }
            else if (_currentFilter == "Wish")
            {
                PageTitle.Text = "My Wishlist";
            }
            else if (_currentFilter == "Saved")
            {
                PageTitle.Text = "Saved places";
            }
            else
            {
                PageTitle.Text = "All places";
            }
        }
        private void LoadData()
        {
            string filePath = "Data/places.json";

            if (File.Exists(filePath))
            {
                string jsonString = File.ReadAllText(filePath);
                // десеріалізація - перетворюємо текст у список об'єктів list<place>
                _allData = JsonSerializer.Deserialize<List<Place>>(jsonString);

                if (_currentFilter == "Visited")
                {
                    PlacesList.ItemsSource = _allData.Where(p => p.Status == "Visited").ToList();
                }
                else if (_currentFilter == "Wish")
                {
                    PlacesList.ItemsSource = _allData.Where(p => p.Status == "Wish").ToList();
                }
                else if (_currentFilter == "Saved")
                {
                    PlacesList.ItemsSource = _allData.Where(p => p.Status == "Saved").ToList();
                }
                else
                {
                    PlacesList.ItemsSource = _allData;
                }
            }

            var currentList = PlacesList.ItemsSource as List<Place>;

            if (currentList == null || currentList.Count == 0)
            {
                PlacesList.Visibility = Visibility.Collapsed;
                EmptyStatePanel.Visibility = Visibility.Visible;

                if (_currentFilter == "Visited")
                    EmptyStateText.Text = "You haven't visited any places yet.";
                else if (_currentFilter == "Wish")
                    EmptyStateText.Text = "Your wish list is empty.";
                else if (_currentFilter == "Saved")
                    EmptyStateText.Text = "No saved places";
                else
                    EmptyStateText.Text = "Add your first place";
            }
            else
            {
                PlacesList.Visibility = Visibility.Visible;
                EmptyStatePanel.Visibility = Visibility.Collapsed;
            }
        }
        private void DeletePlace_Click(object sender, RoutedEventArgs e)
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
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchBox.Text.ToLower();
            string filePath = "Data/places.json";

            if (File.Exists(filePath)) 
            {
                string jsonString = File.ReadAllText(filePath);
                List<Place> allPlaces = JsonSerializer.Deserialize<List<Place>>(jsonString) ?? new List<Place>();

                var filtredPlaces = allPlaces.Where(p => p.Name.ToLower().Contains(searchText)).ToList();

                PlacesList.ItemsSource = filtredPlaces;
            }
        }
        private void PlaceClick_Card(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            if (element == null) return;

            Place clickedPlace = element.DataContext as Place;
            if (clickedPlace == null) return;

            this.NavigationService.Navigate(new DetailsPage(clickedPlace));
        }
        private void MarkVisited_Click(object sender, RoutedEventArgs e) {
            var menuItem = sender as MenuItem;
            var place = menuItem.DataContext as Place;

            if (place != null)
            {
                place.Status = "Visited";

                SavePlacesToJson();

                RefreshList();
            }
        }

        private void SavePlacesToJson()
        {
            if (_allData != null)
            {
                string filePath = "Data/places.json";
                string jsonString = JsonSerializer.Serialize(_allData, new JsonSerializerOptions { WriteIndented = true });

                File.WriteAllText(filePath, jsonString);
            }
        }
        private void RefreshList()
        {
            var allPlaces = PlacesList.ItemsSource as List<Place>;

            PlacesList.ItemsSource = null;
            PlacesList.ItemsSource = allPlaces;
        }
        private void AddWish_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            var place = menuItem.DataContext as Place;

            if (place != null)
            {
                place.Status = "Wish";

                SavePlacesToJson();
                RefreshList();
            }
        }
        private void MarkSaved_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            var place = menuItem.DataContext as Place;

            if (place != null)
            {
                place.Status = "Saved";
                SavePlacesToJson();
                RefreshList();
            }
        }
        private void EditPlace_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            var place = menuItem.DataContext as Place;

            if (place != null)
            {
                NavigationService.Navigate(new AddPlacePage(place));
            }
        }

    }
}
