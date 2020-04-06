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

namespace MegaMap
{
    class Human : MapObject
    {
        public PointLatLng point { get; set; }
        public PointLatLng destinationPoint { get; set; }
        public GMapMarker marker { get; private set; }
        public event EventHandler seated;

        public Human(string title, PointLatLng point) : base(title)
        {
            this.point = point;
        }

        public override double getDistance(PointLatLng point)
        {
            GeoCoordinate p1 = new GeoCoordinate(point.Lat, point.Lng);
            GeoCoordinate p2 = new GeoCoordinate(this.point.Lat, this.point.Lng);

            return p1.GetDistanceTo(p2);
        }

        public override PointLatLng getFocus()
        {
            return point;
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
                    Source = new BitmapImage(new Uri("pack://application:,,,/resources/Human/human.png"))                    
                }
            };

            return marker;
        }  
        
        public void CarArrived(object sender, EventArgs args)
        {            
            try
            {
                var tmp = getDistance(destinationPoint);

                if (tmp > 30)
                {
                    MessageBox.Show("Водитель прибыл на место назначения.");
                    seated?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    MessageBox.Show("Вы на месте.");
                }
            }
            catch
            {
                
            }
        }
    }
}
