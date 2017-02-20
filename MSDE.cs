using System;

namespace UCNLSalinity
{
    [Serializable]
    public class MSDE
    {
        
        public double Lat;
        public double Lon;
        public double Sal;

        public MSDE()
        {
            Lat = 0;
            Lon = 0;
            Sal = 0;
        }

        public MSDE(double lat, double lon, double salinity)
        {
            Lat = lat;
            Lon = lon;
            Sal = salinity;
        }
    }
}
