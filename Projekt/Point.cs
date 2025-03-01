﻿using System;
using System.Data;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using System.Text;
using System.Runtime.InteropServices;

namespace Projekt
{
    [Serializable]
    [Microsoft.SqlServer.Server.SqlUserDefinedType(Format.UserDefined, ValidationMethodName="validate", IsByteOrdered = true, MaxByteSize = 64)]
    [StructLayout(LayoutKind.Sequential)]
    public class Point: INullable, IBinarySerialize {

        private double latitude;
        private double longitude;

        public bool IsNull
        {
            get { return (Double.IsNaN(this.Latitude) || Double.IsNaN(this.Longitude)); }
        }

        public static Point Null
        {
            get
            { return new Point(); }
        }

        // constructor, takes two parameters
        public Point(double latitude, double longitude)
        {
            this.latitude = latitude;
            this.longitude = longitude;
        }

        public Point()
        {
            this.latitude = Double.NaN;
            this.longitude = Double.NaN;
        }

        // getter and setter for latitude (x- coordinate)
        public double Latitude
        {
            get { return this.latitude; }
            set { this.latitude = value; }
        }

        // getter and setter for longitude (y- coordinate)
        public double Longitude
        {
            get { return this.longitude; }
            set { this.longitude = value; }
        }

        // overriden ToString method
        public override string ToString()
        {
            return "(" + this.Latitude.ToString() + ", " + this.Longitude.ToString() + ")";
        }

        // String parser, required by sql server
        public static Point Parse(SqlString s)
        {
            if (s.IsNull)
                return new Point();
            
            string[] xy = s.Value.Split(",".ToCharArray());
            Double X = Double.Parse(xy[0]);
            Double Y = Double.Parse(xy[1]);
            return new Point(X,Y);
        }

        // function calculating distance between points
        public SqlDouble distance(Point p)
        {
            return Math.Sqrt(Math.Pow(this.Latitude - p.Latitude, 2) + Math.Pow(this.Longitude - p.Longitude,2));
        }

        // check whether point is legit
        private bool validate()
        {
            return (!Double.IsNaN(this.Latitude) && !Double.IsNaN(this.Longitude));
        }

        // serialization
        #region IBinarySerialize Members

        public void Write(System.IO.BinaryWriter writer)
        {
            writer.Write(this.latitude);
            writer.Write(this.longitude);
        }

        public void Read(System.IO.BinaryReader reader)
        {
            this.latitude = reader.ReadDouble();
            this.longitude = reader.ReadDouble();
        }
        #endregion
    }
}
