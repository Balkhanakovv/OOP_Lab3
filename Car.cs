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

namespace MegaMap
{
    class Car : MapObject
    {
        private PointLatLng point;

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
            GMapMarker marker = new GMapMarker(point)
            {
                Shape = new Image
                {
                    Width = 32,
                    Height = 32,
                    ToolTip = getTitle(),
                    Source = new BitmapImage(new Uri("pack://application:,,,/resources/Car/car.png"))
                }
            };

            return marker;
        }

        public override PointLatLng getFocus()
        {
            return point;
        }
    }
}
