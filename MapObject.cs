using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using System.Device.Location;

namespace MegaMap
{
    abstract class MapObject
    {
        private string title;

        private DateTime creationTime;

        public MapObject(string title)
        {
            this.title = title;
            creationTime = DateTime.Now;
        }

        public string getTitle()
        {
            return title;
        }

        public DateTime getCreationDate()
        {
            return creationTime;
        }

        abstract public double getDistance(PointLatLng point);

        abstract public PointLatLng getFocus();

        abstract public GMapMarker getMarker();
    }
}
