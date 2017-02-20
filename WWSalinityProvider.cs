using System;
using System.Collections.Generic;


namespace UCNLSalinity
{
    public class WWSalinityProvider
    {
        #region Properties

        SettingsProviderXML<List<MSDE>> dataProvider;

        #endregion

        #region Constructor

        public WWSalinityProvider(string salinityDBFileName)
        {
            dataProvider = new SettingsProviderXML<List<MSDE>>();
            dataProvider.Load(salinityDBFileName);
        }

        #endregion

        #region Methods

        public double GetNearestSalinity(double lat, double lon, out double nlat, out double nlon)
        {
            double dLat = double.MaxValue;
            double dLon = double.MaxValue;
            int idx = 0;
            double minD = double.MaxValue;

            nlat = 0;
            nlon = 0;

            for (int i = 0; i < dataProvider.Data.Count; i++)
            {
                dLat = Math.Abs(lat - dataProvider.Data[i].Lat);
                dLon = Math.Abs(lon - dataProvider.Data[i].Lon);

                if (dLon + dLat < minD)
                {
                    minD = dLon + dLat;
                    idx = i;

                    nlat = dataProvider.Data[i].Lat;
                    nlon = dataProvider.Data[i].Lon;
                }
            }            

            return dataProvider.Data[idx].Sal;
        }

        #endregion
    }
}
