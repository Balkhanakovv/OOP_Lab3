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
using System.Threading;

namespace MegaMap
{
    public class MapPoint
    {
        public string Title { get; set; }
        public double Distance { get; set; }
    }


    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<MapObject> objects = new List<MapObject>();
        List<PointLatLng> points = new List<PointLatLng>();
        List<PointLatLng> nearestPointPosition = new List<PointLatLng>();        
        List<MapObject> nearestObjects = new List<MapObject>();
        
        static PointLatLng startOfRoute;
        static PointLatLng endOfRoute;

        static IEnumerable<MapObject> besidedObjects;

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
            nearestObjects.Clear();

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
                try
                {
                    if ((mapObject.getDistance(obj.getFocus()) < 500) || (mapObject.getTitle() == obj.getTitle()))
                    {
                        nearestObjects.Add(obj);
                    }
                }
                catch
                {
                    break;
                }
            }

            besidedObjects = nearestObjects.OrderBy(mapObj => mapObj.getDistance(mapObject.getFocus()));

            foreach (MapObject obj in besidedObjects)
            {
                try
                {
                    if ((mapObject.getDistance(obj.getFocus()) < 500) || (mapObject.getTitle() == obj.getTitle()))
                    {
                        
                        NearestPointsLb.Items.Add(new MapPoint{ 
                            Title = obj.getTitle(), 
                            Distance = Math.Round(mapObject.getDistance(obj.getFocus()), 2)
                        });
                        nearestPointPosition.Add(obj.getFocus());
                    }
                }
                catch
                {
                    break;
                }
            }

            foreach (MapPoint point in NearestPointsLb.Items)
            {
                if (point.Distance == 0)
                {
                    NearestPointsLb.Items.Remove(point);
                    break;
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
            MapObject point = new Location(TitleTB.Text, Map.Position);
            NearestPointsLb.Items.Clear();
            nearestPointPosition.Clear();
            nearestObjects.Clear();

            foreach (MapObject obj in objects)
            {
                try
                {
                    if ((point.getDistance(obj.getFocus()) < 500))
                    {
                        nearestObjects.Add(obj);
                    }
                }
                catch
                {
                    break;
                }
            }

            besidedObjects = nearestObjects.OrderBy(mapObj => mapObj.getDistance(point.getFocus()));

            foreach (MapObject obj in besidedObjects)
            {
                try
                {
                    if ((point.getDistance(obj.getFocus()) < 500) || (point.getTitle() == obj.getTitle()))
                    {

                        NearestPointsLb.Items.Add(new { Title = obj.getTitle(), Distance = Math.Round(point.getDistance(obj.getFocus()), 2) });
                        nearestPointPosition.Add(obj.getFocus());
                    }
                }
                catch
                {
                    break;
                }
            }
        }

        private void AddressBt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                endOfRoute = nearestPointPosition[NearestPointsLb.SelectedIndex];
            }
            catch
            {
                MessageBox.Show("Выберите конец маршрута!");
            }
        }

        private void GoBt_Click(object sender, RoutedEventArgs e)
        {
            CarProgressBar.Value = 0;
            startOfRoute = nearestPointPosition[NearestPointsLb.SelectedIndex];            

            var besidedObj = objects.OrderBy(mapObject => mapObject.getDistance(startOfRoute));

            Car nearestCar = null;
            Human h = null;

            foreach (MapObject obj in objects)
            {
                if (obj.GetType().ToString() == "MegaMap.Human" && obj.getFocus() == startOfRoute)
                {
                    h = (Human)obj;
                    h.destinationPoint = endOfRoute;
                    break;
                }
            }

            foreach(MapObject obj in besidedObj)
            {
                if(obj.GetType().ToString() == "MegaMap.Car")
                {
                    nearestCar = (Car)obj;
                    break;
                }
            }
            
            nearestCar.MoveTo(startOfRoute);
            nearestCar.Arrived += h.CarArrived;
            h.seated += nearestCar.passengerSeated;
            nearestCar.Follow += Focus_Follow;
        }

        private void NearestPointsLb_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Map.Position = Map.Position = nearestPointPosition[NearestPointsLb.SelectedIndex];
        }

        private void Focus_Follow(object sender, EventArgs args)
        {
            Car c = (Car)sender;
            CarProgressBar.Maximum = c.route.Points.Count;
            Map.Position = c.getFocus();

            if (CarProgressBar.Value != CarProgressBar.Maximum)
                CarProgressBar.Value += 1;
            else
            {
                CarProgressBar.Value = 0;
            }
        }
    }
}
