using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PlacesToVisit.Pages;


namespace PlacesToVisit;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        MainFrame.Navigate(new HomePage());
    }
    private void AllPlaces_Click(object sender, RoutedEventArgs e) {
        MainFrame.Navigate(new Pages.HomePage());
    }
    private void AddPlaces_Click(object sender, RoutedEventArgs e)
    {
        MainFrame.Navigate(new Pages.AddPlacePage());
    }
}