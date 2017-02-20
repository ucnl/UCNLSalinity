using System;

namespace UCNLSalinity
{
    public static class SaltWaterSoSProvider
    {
        /// <summary>
        /// Calculates speed of sound in water according to Wilson formula
        /// </summary>
        /// <param name="t">Water temperature in °C</param>
        /// <param name="p">Hydrostatic pressure in MPa</param>
        /// <param name="s">Salinity</param>
        /// <returns></returns>
        public static double CalcSoundSpeed(double t, double p, double s)
        {
            if ((t < -4) || (t > 30))
                throw new ArgumentOutOfRangeException("t");

            if ((p < 0.1) || (p > 100))
                throw new ArgumentOutOfRangeException("p");

            if ((s < 0) || (s > 40))
                throw new ArgumentOutOfRangeException("s");

            //temperature from -4° to 30°;
            //salinity from 0 to 37 per mille;
            //hydrostatic pressure from 0.1 MPa to 100 MPa. 
            // where c(S,T,P) - speed of sound, m/s; T - temperature, °C; S - salinity, per mille; P - hydrostatic pressure, MPa. 

            double c0 = 1449.14;
            double Dct = 4.5721 * t - 4.4532E-2 * t * t - 2.6045E-4 * t * t * t + 7.9851E-6 * t * t * t * t;
            double Dcs = 1.39799 * (s - 35) - 1.69202E-3 * (s - 35) * (s - 35);
            double Dcp = 1.63432 * p - 1.06768E-3 * p * p + 3.73403E-6 * p * p * p - 3.6332E-8 * p * p * p * p;
            double Dcstp = (s - 35) * (-1.1244E-2 * t + 7.7711E-7 * t * t + 7.85344E-4 * p -
                            1.3458E-5 * p * p + 3.2203E-7 * p * t + 1.6101E-8 * t * t * p) +
                            p * (-1.8974E-3 * t + 7.6287E-5 * t * t + 4.6176E-7 * t * t * t) +
                            p * p * (-2.6301E-5 * t + 1.9302E-7 * t * t) + p * p * p * (-2.0831E-7 * t);

            double result = c0 + Dct + Dcs + Dcp + Dcstp;

            return result;
        }
    }
}
