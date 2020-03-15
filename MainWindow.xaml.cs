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
        List<PointLatLng> points = new List<PointLatLng>();
        List<PointLatLng> nearestPointPosition = new List<PointLatLng>(); 
        
        private void AddMarker(MapObject marker)
        {
            objects.Add(marker);
            Map.Markers.Add(marker.getMarker());
            TitleTB.Text = "";
        }

        private void ClearPoints()
        {
            points.Clear();
            ClearPointsBt.Content = points.Count;
        }

        public MainWindow()
        {
            InitializeComponent();

            variantCb.Items.Add("Местоположение");
            variantCb.Items.Add("Человек");
            variantCb.Items.Add("Автомобиль");
            variantCb.Items.Add("Маршрут");
            variantCb.Items.Add("Область");
            
            ClearPointsBt.Content = points.Count;
        }

        private void MapLoaded(object sender, RoutedEventArgs e)
        {      
            GMaps.Instance.Mode = AccessMode.ServerAndCache;          
                 
            Map.MapProvider = GMapProviders.GoogleMap;          
            
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

                if ((string)variantCb.SelectedItem == "Маршрут")
                {
                    Route route = new Route(TitleTB.Text, points);
                    AddMarker(route);
                }

                if((string)variantCb.SelectedItem == "Область")
                {
                    Area area = new Area(TitleTB.Text, points);
                    AddMarker(area);
                }
            }

            ClearPoints();
        }

        private void ClearBt_Click(object sender, RoutedEventArgs e)
        {
            Map.Markers.Clear();
            objects.Clear();
            ClearPoints();
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
            NearestPointsLb.Items.Clear();
            nearestPointPosition.Clear();

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
                if ((mapObject.getDistance(obj.getFocus()) < 500) || (mapObject.getTitle() == obj.getTitle()))
                {
                    NearestPointsLb.Items.Add(obj.getTitle());
                    nearestPointPosition.Add(obj.getFocus());
                }
            }
        }

        private void Map_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            points.Add(Map.Position);
            ClearPointsBt.Content = points.Count;
        }

        private void ClearPointsBt_Click(object sender, RoutedEventArgs e)
        {
            ClearPoints();
        }

        private void FocusBt_Click(object sender, RoutedEventArgs e)
        {
            Map.Position = nearestPointPosition[NearestPointsLb.SelectedIndex];            
        }
    }
}
