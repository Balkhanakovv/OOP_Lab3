using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using System.Device.Location;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Threading;

namespace MegaMap
{
    class Car : MapObject
    {
        private PointLatLng point;
        private List<Human> passengers = new List<Human>();
        private MapRoute route;
        private GMapMarker marker;

        public event EventHandler Arrived;

        public Car(string title, PointLatLng point) : base(title)
        {
            this.point = point;
        }

        public override double getDistance(PointLatLng point)
        {
            GeoCoordinate p1 = new GeoCoordinate(point.Lat, point.Lng);
            GeoCoordinate p2 = new GeoCoordinate(this.point.Lat, this.point.Lng);
            return p1.GetDistanceTo(p2);
        }

        public override GMapMarker getMarker()
        {
            marker = new GMapMarker(point)
            {
                Shape = new Image
                {
                    Width = 32,
                    Height = 32,
                    ToolTip = getTitle(),
                    Margin = new System.Windows.Thickness(-16, -16, 0, 0),
                    Source = new BitmapImage(new Uri("pack://application:,,,/resources/Car/car.png"))
                }
            };

            return marker;
        }

        public override PointLatLng getFocus()
        {
            return point;
        }

        public void MoveTo(PointLatLng endPoint)
        {
            RoutingProvider routingProvider = GMapProviders.OpenStreetMap;
            route = routingProvider.GetRoute(
                point,
                endPoint,
                false,
                false,
                15);

            Thread ridingCar = new Thread(MoveByRoute);
            ridingCar.Start();
        }

        private void MoveByRoute()
        {
            try
            {
                foreach (var point in route.Points)
                {
                    Application.Current.Dispatcher.Invoke(delegate
                    {
                        this.point = point;
                        marker.Position = point;
                    });

                    Thread.Sleep(1000);
                }
                
                Arrived?.Invoke(this, null);
            }
            catch
            {

            }
        }
          
        public void passengerSeated(object sender, EventArgs args)
        {
            Human h = (Human)sender;

            MoveTo(h.destinationPoint);
            h.point = point;
        }
    }
}
