using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics; // Потрібно для відкриття посилань у браузері
using PlacesToVisit.Pages;
using PlacesToVisit.Models;

namespace PlacesToVisit.Pages
{
    public partial class PropertiesPage : Page
    {
        public PropertiesPage()
        {
            InitializeComponent();
            LoadStatistics();
        }

        private void LoadStatistics()
        {
            string filePath = "Data/places.json";
            List<Place> places = new List<Place>();
            if (File.Exists(filePath))
            {
                string jsonString = File.ReadAllText(filePath);
                if (!string.IsNullOrWhiteSpace(jsonString))
                {
                    places = JsonSerializer.Deserialize<List<Place>>(jsonString) ?? new List<Place>();
                }
            }
            int total = places.Count;
            int visited = places.Count(p => p.Status == "Visited");
            int wishes = places.Count(p => p.Status == "Wish");
            int saved = places.Count(p => p.Status == "Saved");

            TotalPlacesText.Text = $"Total places: {total}";
            VisitedPlacesText.Text = $"Visited: {visited}";
            WishPlacesText.Text = $"⭐ Wishes: {wishes}";
            SavedPlacesText.Text = $"🔖 In the archive (Saved): {saved}";

            // Математика для Прогрес-бару
            // Рахуємо відсоток відвіданих відносно тих, куди ти збираєшся (Visited + Wish)
            int targetPlaces = visited + wishes;
            if (targetPlaces > 0)
            {
                // Формула відсотка: (Відвідані / (Відвідані + У планах)) * 100
                double percentage = ((double)visited / targetPlaces) * 100;
                TravelProgress.Value = percentage;
            }
            else
            {
                TravelProgress.Value = 0;
            }
        }

        private void GitHub_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string myGitHubUrl = "https://github.com/oleksandrperemybida-hub";

                Process.Start(new ProcessStartInfo
                {
                    FileName = myGitHubUrl,
                    UseShellExecute = true
                });
            }
            catch
            {
                MessageBox.Show("Failed to open browser", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}