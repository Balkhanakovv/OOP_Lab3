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
    class Area : MapObject
    {
        private List<PointLatLng> points;

        public Area(string title, List<PointLatLng> points) : base(title)
        {
            this.points = points;
        }

        public override double getDistance(PointLatLng point)
        {
            GeoCoordinate p1 = new GeoCoordinate(point.Lat, point.Lng);
            GeoCoordinate p2 = new GeoCoordinate(points[0].Lat, points[0].Lng);
            return p1.GetDistanceTo(p2);
        }

        public override GMapMarker getMarker()
        {
            GMapMarker marker = new GMapPolygon(points)
            {
                Shape = new Path
                {
                    Stroke = Brushes.Black, 
                    Fill = Brushes.Violet,
                    Opacity = 0.7
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
