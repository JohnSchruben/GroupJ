﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeSkate
{
    [Serializable]
    public class Coordinate : IEquatable<Coordinate>
    {
        double latitude, longitude, elevation;
        public Coordinate() { }
        public Coordinate(double latitude, double longitude, double elevation)
        {
            this.latitude = latitude;
            this.longitude = longitude;
            this.elevation = elevation;
        }

        public double Latitude
        {
            get { return latitude; }
            set { latitude = value; }
        }

        public double Longitude
        {
            get { return longitude; }
            set { longitude = value; }
        }

        public double Elevation
        {
            get { return elevation; }
            set { elevation = value; }
        }

        public bool Equals(Coordinate? other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            return this.Latitude == other.Latitude &&
                this.Longitude == other.Longitude &&
                this.Elevation == other.Elevation;
        }

        public override string ToString()
        {
            return $"Latitude: {this.Latitude}\nLongitude: {this.Longitude}";
        }
    }
}
