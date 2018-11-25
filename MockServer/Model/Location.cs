using System;

namespace MockServer.Model
{
    public class Location
    {
        public double longitude { get; set; }
        public double lattitude { get; set; }

        internal static double GetDistanceBetween(Location l1, Location l2)
        {
            return Math.Pow(l1.lattitude - l2.lattitude, 2) 
                    + Math.Pow(l1.longitude - l2.longitude, 2);
        }
    }
}
