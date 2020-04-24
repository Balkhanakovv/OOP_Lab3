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
using System.Windows.Shapes;
using System.Windows.Media;

namespace MegaMap
{
    class Route : MapObject
    {
        private List<PointLatLng> points = new List<PointLatLng>();
        public GMapRoute marker = null;

        public Route(string title, List<PointLatLng> points) : base(title)
        {
            foreach (PointLatLng point in points)
            {
                this.points.Add(point);
            }
        }

        public override double getDistance(PointLatLng point)
        {
            GeoCoordinate p1 = new GeoCoordinate(point.Lat, point.Lng);
            GeoCoordinate p2 = new GeoCoordinate(points[0].Lat, points[0].Lng);
            return p1.GetDistanceTo(p2);
        }

        public override GMapMarker getMarker()
        {
            marker = new GMapRoute(points)
            {
                Shape = new Path
                {
                    Stroke = Brushes.DarkBlue,
                    Fill = Brushes.DarkBlue,
                    StrokeThickness = 4
                }
            };

            return marker;
        }

        public override PointLatLng getFocus()
        {
            return points[0];
        }
    }
}
