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
        
        PointLatLng startOfRoute;
        PointLatLng endOfRoute;
        IEnumerable<MapObject> besidedObjects;

        private string[] boxes = new string[5] {"Местоположение", "Человек", "Автомобиль", "Маршрут", "Область" }; 

        private Human h;
        private Car nearestCar;

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

            variantCb.Items.Add(boxes[0]);
            variantCb.Items.Add(boxes[1]);
            variantCb.Items.Add(boxes[2]);
            variantCb.Items.Add(boxes[3]);
            variantCb.Items.Add(boxes[4]);
            
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
            MapObject mapObject = null;

            if (CreatingObjectsRB.IsChecked == true)
            {
                if ((string)variantCb.SelectedItem == boxes[1])
                {
                    mapObject = new Human(TitleTB.Text, Map.Position);                    
                }

                if ((string)variantCb.SelectedItem == boxes[2])
                {
                    mapObject = new Car(TitleTB.Text, Map.Position);
                }

                if ((string)variantCb.SelectedItem == boxes[0])
                {
                    mapObject = new Location(TitleTB.Text, Map.Position);
                }

                if ((string)variantCb.SelectedItem == boxes[3])
                {
                    mapObject = new Route(TitleTB.Text, points);
                }

                if((string)variantCb.SelectedItem == boxes[4])
                {
                    mapObject = new Area(TitleTB.Text, points);
                }
                AddMarker(mapObject);
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
                if (obj.getTitle().Contains(SearchTB.Text))
                {
                    mapObject = obj;
                    Map.Position = obj.getFocus();
                    break;
                }
            }
            
            foreach (MapObject obj in objects)
            {
                
                    if ((mapObject.getDistance(obj.getFocus()) < 500) || (obj.getTitle().Contains(mapObject.getTitle())))
                    {
                        nearestObjects.Add(obj);
                    }
            }

            besidedObjects = nearestObjects.OrderBy(mapObj => mapObj.getDistance(mapObject.getFocus()));

            foreach (MapObject obj in besidedObjects)
            {
                    if ((mapObject.getDistance(obj.getFocus()) < 500) || (obj.getTitle().Contains(mapObject.getTitle())))
                    {
                        
                        NearestPointsLb.Items.Add(new MapPoint{ 
                            Title = obj.getTitle(), 
                            Distance = Math.Round(mapObject.getDistance(obj.getFocus()), 2)
                        });
                        nearestPointPosition.Add(obj.getFocus());
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
                
                    if ((point.getDistance(obj.getFocus()) < 500))
                    {
                        nearestObjects.Add(obj);
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

            nearestCar = null;
            h = null;

            foreach (MapObject obj in objects)
            {
                if ((obj is Human) && obj.getFocus() == startOfRoute)
                {
                    h = (Human)obj;
                    h.destinationPoint = endOfRoute;
                    break;
                }
            }

            foreach(MapObject obj in besidedObj)
            {
                if(obj is Car)
                {
                    nearestCar = (Car)obj;
                    break;
                }
            }
            
            nearestCar.MoveTo(startOfRoute);
            nearestCar.Arrived += h.CarArrived;
            h.seated += nearestCar.passengerSeated;
            nearestCar.Follow += Focus_Follow;
            nearestCar.ArrivedToLocate += h.PassengerArrived;
        }

        private void NearestPointsLb_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Map.Position = nearestPointPosition[NearestPointsLb.SelectedIndex + 1];
        }

        private void Focus_Follow(object sender, EventArgs args)
        {
            Car c = (Car)sender;
            nearestCar.Arrived -= h.CarArrived;

            if (c.getFocus() == c.route.Points[0])
            {
                Route route = new Route("SystemRoutForOurCar", c.route.Points, true);
                AddMarker(route);
            }

            CarProgressBar.Maximum = c.route.Points.Count;
            Map.Position = c.getFocus();

            if (CarProgressBar.Value != CarProgressBar.Maximum - 1)
                CarProgressBar.Value += 1;
            else
            {
                CarProgressBar.Value = 0;
                h.seated -= nearestCar.passengerSeated;
                nearestCar.Follow -= Focus_Follow;
                nearestCar.ArrivedToLocate -= h.PassengerArrived;
                MessageBox.Show("Вы приехали!");
                h.point = h.destinationPoint;
                NearestPointsLb.Items.Clear();

                foreach (MapObject obj in objects)
                {
                    if (obj is Route)
                    {
                        Route r = (Route)obj;
                        if (r.IsVisible)
                        {
                            objects.Remove(r);
                            Map.Markers.Remove(r.marker);
                            break;
                        }
                    }
                }
            }
        }
    }
}
