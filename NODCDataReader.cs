﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace UCNLSalinity
{
    public static class NODCDataReader
    {
        #region Properties

        static int[] annualProfileDepths = new int[] {    0,   10,   20,   30,   50,   75,  100,  125,  150,  200,  250, 
                                                        300,  400,  500,  600,  700,  800,  900, 1000, 1100, 1200, 1300,
                                                       1400, 1500, 1750, 2000, 2500, 3000, 3500, 4000, 4500, 5000, 5500 };


        #endregion

        #region Methods

        public static List<MSDE> ReadSalinityData(string fileName)
        {
            List<MSDE> result = new List<MSDE>();

            try
            {
                using (StreamReader sr = new StreamReader(fileName))
                {
                    String line;                   
                    while ((line = sr.ReadLine()) != null)
                    {
                        var splits = line.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                        if (splits.Length == 3)
                        {
                            MSDE item = new MSDE();

                            item.Lat = double.Parse(splits[0]);
                            item.Lon = double.Parse(splits[1]);
                            item.Sal = double.Parse(splits[2]);

                            result.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public static List<SDE> Read1x1DegreeSalinityData(string fileName, int[] profileDepths)
        {
            List<SDE> result = new List<SDE>();
            try
            {                
                double latIdx = 0;
                double lonIdx = 0;

                double lon = lonIdx + 0.5;
                if (lon > 180)
                    lon = lon - 360;

                double lat = latIdx - 89.5;
                
                int depthLevelIdx = 0;

                using (StreamReader sr = new StreamReader(fileName))
                {
                    String line;                   
                    while ((line = sr.ReadLine()) != null)
                    {
                        var splits = line.Split(" -".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                        for (int i = 0; i < splits.Length; i++)
                        {

                            double salinity = double.Parse(splits[i]);
                            if (salinity < 99)
                            {
                                result.Add(new SDE(lat, lon, profileDepths[depthLevelIdx], salinity));
                            }

                            depthLevelIdx++;

                            if (depthLevelIdx >= profileDepths.Length)
                            {
                                depthLevelIdx = 0;
                                lonIdx++;                         

                                if (lonIdx >= 360)
                                {
                                    lonIdx = 0;
                                    latIdx++;
                                }

                                lon = lonIdx + 0.5;
                                if (lon > 180)
                                    lon = lon - 360;

                                lat = latIdx - 89.5;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {

            }

            return result;
        }

        public static List<SDE> ReadQuarterDegreeSalinityData(string fileName, int[] profileDepths)
        {
            List<SDE> result = new List<SDE>();
            try
            {
                double latIdx = 0;
                double lonIdx = 0;

                double lon = lonIdx + 0.5;
                if (lon > 180)
                    lon = lon - 360;

                double lat = latIdx - 89.5;

                int depthLevelIdx = 0;

                using (StreamReader sr = new StreamReader(fileName))
                {
                    String line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        var splits = line.Split(" -".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                        for (int i = 0; i < splits.Length; i++)
                        {

                            double salinity = double.Parse(splits[i]);
                            if (salinity < 99)
                            {
                                result.Add(new SDE(lat, lon, profileDepths[depthLevelIdx], salinity));
                            }

                            depthLevelIdx++;

                            if (depthLevelIdx >= profileDepths.Length)
                            {
                                depthLevelIdx = 0;
                                lonIdx += 0.25;

                                if (lonIdx >= 360)
                                {
                                    lonIdx = 0;
                                    latIdx++;
                                }

                                lon = lonIdx + 0.25;
                                if (lon > 180)
                                    lon = lon - 360;

                                lat = latIdx - 89.875;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {

            }

            return result;
        }


        public static Dictionary<double, Dictionary<double, Dictionary<double, double>>> ConvertToDictionary(List<SDE> salinityList)
        {
            Dictionary<double, Dictionary<double, Dictionary<double, double>>> result = new Dictionary<double, Dictionary<double, Dictionary<double, double>>>();

            // lon -> lat -> depth -> salinity

            foreach (var item in salinityList)
            {
                if (!result.ContainsKey(item.Longitude))
                    result.Add(item.Longitude, new Dictionary<double, Dictionary<double, double>>());

                if (!result[item.Longitude].ContainsKey(item.Latitude))
                    result[item.Longitude].Add(item.Latitude, new Dictionary<double, double>());

                if (!result[item.Longitude][item.Latitude].ContainsKey(item.Depth))
                    result[item.Longitude][item.Latitude].Add(item.Depth, item.Salinity);
            }

            return result;
        }

        public static List<MSDE> BuildMeanSalinityData(Dictionary<double, Dictionary<double, Dictionary<double, double>>> source)
        {
            // lon -> lat -> depth -> salinity
            List<MSDE> result = new List<MSDE>();

            foreach (var lonEntry in source)
            {
                foreach (var latEntry in source[lonEntry.Key])
                {
                    double mean = 0.0;
                    int count = 0;
                    foreach (var dptEntry in source[lonEntry.Key][latEntry.Key])
                    {
                        mean += dptEntry.Value;
                        count++;
                    }

                    if (count > 0)
                    {
                        mean /= count;
                        mean = Math.Round(mean, 4);

                        result.Add(new MSDE(latEntry.Key, lonEntry.Key, mean));
                    }
                }
            }

            return result;
        }



        private static int WOD09_GetColNumber(string line)
        {
            int pIdx = line.IndexOf('.');
            int sIdx = line.IndexOf('#');
            int result = -1;

            if ((pIdx > sIdx) && (sIdx >= 0))
            {
                result = int.Parse(line.Substring(sIdx + 1, pIdx - sIdx - 1)) - 1;
            }

            return result;
        }

        public static List<MSDE> WOD09_SalinityData_Read(string fileName)
        {
            List<MSDE> result = new List<MSDE>();
            try
            {

                //#ANNUAL SALINITY (unitless) at    0 meters depth
                //# 1 degree lat/lon grid
                //#Column definitions:
                //#1. Latitude (degrees,negative=south),
                //#2. Longitude (degrees,negative=west),
                //#3. Depth (meters),
                //#4. Objectively analyzed mean,
                //#5. statistical mean,
                //#6. standard deviation of statistical mean,
                //#7. standard error of statistical mean,
                //#8. objectively analyzed mean minus statistical mean,
                //#9. objectively analyzed mean minus objectively analyzed annual mean,
                //#10. number of grids with statistical means within radius of influence,
                //#11. number of data used to calculate statistical mean

                int latCol = -1;
                int lonCol = -1;
                int oanCol = -1;

                double lat, lon, salinity;

                using (StreamReader sr = new StreamReader(fileName))
                {
                    String line;
                    while (((line = sr.ReadLine()) != null) && (line.Contains("#"))) // read header
                    {
                        if (line.Contains("Latitude"))
                            latCol = WOD09_GetColNumber(line);
                        else if (line.Contains("Longitude"))
                            lonCol = WOD09_GetColNumber(line);
                        else if (line.Contains("Objectively analyzed mean"))
                            oanCol = WOD09_GetColNumber(line);
                    }

                    if ((latCol >= 0) &&
                        (lonCol >= 0) &&
                        (oanCol >= 0))
                    {
                        while ((line = sr.ReadLine()) != null)
                        {
                            var splits = line.Split(",".ToCharArray());

                            lat = double.Parse(splits[latCol], CultureInfo.InvariantCulture);
                            lon = double.Parse(splits[lonCol], CultureInfo.InvariantCulture);
                            salinity = double.Parse(splits[oanCol], CultureInfo.InvariantCulture);

                            result.Add(new MSDE(lat, lon, salinity));
                        }

                    }
                    else
                    {
                        throw new InvalidDataException("Error parsing header of the file");
                    }
                }

            }
            catch (Exception e)
            {
                /// TODO: 
                throw e; 
            }

            return result;
        }



        #endregion
    }
}
