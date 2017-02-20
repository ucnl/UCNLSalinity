using System;

namespace UCNLSalinity
{
    [Serializable]
    public class SDE
    {
        public double Latitude;
        public double Longitude;
        public double Depth;
        public double Salinity;

        public SDE()
        {
            Latitude = 0;
            Longitude = 0;
            Depth = 0;
            Salinity = 0;
        }

        public SDE(double lat, double lon, double depth, double salinity)
        {
            Latitude = lat;
            Longitude = lon;
            Depth = depth;
            Salinity = salinity;
        }
    }
}
