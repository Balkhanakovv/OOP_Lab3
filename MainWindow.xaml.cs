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
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using System.Device.Location;

namespace MegaMap
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<MapObject> objects = new List<MapObject>();
        
        private void AddMarker(MapObject marker)
        {
            objects.Add(marker);
            Map.Markers.Add(marker.getMarker());
            TitleTB.Text = "";
        }

        public MainWindow()
        {
            InitializeComponent();

            variantCb.Items.Add("Местоположение");
            variantCb.Items.Add("Человек");
            variantCb.Items.Add("Автомобиль");
            variantCb.Items.Add("Маршрут");
            variantCb.Items.Add("Область");
        }

        private void MapLoaded(object sender, RoutedEventArgs e)
        {      
            GMaps.Instance.Mode = AccessMode.ServerAndCache;          
                 
            Map.MapProvider = OpenStreetMapProvider.Instance;          
            
            Map.MinZoom = 2;    
            Map.MaxZoom = 17;
            Map.Zoom = 15; 

            Map.Position = new PointLatLng(55.012823, 82.950359);
            
            Map.MouseWheelZoomType = MouseWheelZoomType.MousePositionAndCenter; 
            Map.CanDragMap = true; 
            Map.DragButton = MouseButton.Left;
        }

        private void AddPointBt_Click(object sender, RoutedEventArgs e)
        {
            if (CreatingObjectsRB.IsChecked == true)
            {
                if ((string)variantCb.SelectedItem == "Человек")
                {
                    Human human = new Human(TitleTB.Text, Map.Position);
                    AddMarker(human);
                }

                if ((string)variantCb.SelectedItem == "Автомобиль")
                {
                    Car car = new Car(TitleTB.Text, Map.Position);
                    AddMarker(car);
                }

                if ((string)variantCb.SelectedItem == "Местоположение")
                {
                    Location location = new Location(TitleTB.Text, Map.Position);
                    AddMarker(location);
                }
            }
        }

        private void ClearBt_Click(object sender, RoutedEventArgs e)
        {
            Map.Markers.Clear();
            objects.Clear();
        }

        private void CreatingObjectsRB_Checked(object sender, RoutedEventArgs e)
        {
            CreateGrid.Visibility = Visibility.Visible;
            SearchGrid.Visibility = Visibility.Hidden;
        }

        private void SearchNearObjectsRB_Checked(object sender, RoutedEventArgs e)
        {
            SearchGrid.Visibility = Visibility.Visible;
            CreateGrid.Visibility = Visibility.Hidden;
        }

        private void SearchPointBt_Click(object sender, RoutedEventArgs e)
        {
            MapObject mapObject = null;

            foreach (MapObject obj in objects)
            {
                if (obj.getTitle() == SearchTB.Text)
                {
                    mapObject = obj;
                    Map.Position = obj.getFocus();
                    break;
                }
            }

            foreach (MapObject obj in objects)
            {
                NearestPointsLb.Items.Add(obj.getTitle());
            }
        }
    }
}
