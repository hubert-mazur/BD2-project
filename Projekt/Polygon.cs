using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using System.Runtime.InteropServices;

namespace Projekt
{
    [Serializable]
    [Microsoft.SqlServer.Server.SqlUserDefinedType(Format.UserDefined, IsByteOrdered = true, MaxByteSize = -1)]
    [StructLayout(LayoutKind.Sequential)]
    public class Polygon: INullable, IBinarySerialize
    {

        private List<Point> polygon;
        private double area;

        public bool IsNull
        {
            get { return false; }
        }

        public static Polygon Null
        {
            get { return new Polygon(); }
        }

        // constructor
        public Polygon()
        {
            this.area = 0.0;
            this.polygon = new List<Point>();
        }

        // getter and setter for polygon area
        public double GetArea
        {
            get { return this.area; }
            set { this.area = value; }
        }

        // function that loads point to polygon
        public Point Load
        {
            set { this.polygon.Add(new Point(value.Latitude, value.Longitude)); }
        }

        // important function that ensures last point is same as first- required by algorithms
        public Polygon ClosePolygon
        {
            set { this.polygon.Add(this.polygon[0]); }
        }

        // function that creates polygon from string- required by sql server
        public static Polygon Parse(SqlString s)
        {
            return new Polygon();
        }

        // overriden ToString method
        public override string ToString()
        {
            string str = "";

            foreach (Point i in this.polygon)
            {
                str += i.ToString();
                str += "\n";
            }
            return str;
        }

        // function that returns number of points in polygon
        public int IncludedPoints
        {
            get {return this.polygon.Count;}
        }


        // serialization
        #region IBinarySerialize Members

        public void Read(System.IO.BinaryReader r)
        {
            int j = r.ReadInt32();
            for (int i = 0; i < j; i++)
            {
                Point tmp = new Point();
                tmp.Read(r);
                this.polygon.Add(tmp);
            }
            this.area = r.ReadDouble();
        }

        public void Write(System.IO.BinaryWriter writer)
        {
            writer.Write(this.polygon.Count);
            foreach (Point i in this.polygon)
            {
                i.Write(writer);
            }
            writer.Write(this.area);
        }
        #endregion

        // ALGORITHMS

        // Copyright 2000 softSurfer, 2012 Dan Sunday
        // This code may be freely used and modified for any purpose
        // providing that this copyright notice is included with it.
        // SoftSurfer makes no warranty for this code, and cannot be held
        // liable for any real or imagined damage resulting from its use.
        // Users of this code must verify correctness for their application.

        // C# adaptation by Hubert Mazur

        // check whether given point is inside or outside polygon
        private bool isInside(Point p)
        {
            int wn = 0; // the winding number counter

            for (int i = 0; i < this.polygon.Count - 1; i++)
            {
                if (this.polygon[i].Longitude <= p.Longitude)
                {
                    if (this.polygon[i + 1].Longitude > p.Longitude)
                        if (isLeft(this.polygon[i], this.polygon[i + 1], p) > 0)
                            ++wn;
                }
                else
                {
                    if (this.polygon[i + 1].Longitude <= p.Longitude)
                        if (isLeft(this.polygon[i], this.polygon[i + 1], p) < 0)
                            --wn;
                }
                }

            return !(wn == 0);
            }

        public bool isPointInside(double x, double y)
        {
            Point p = new Point(x, y);
            int wn = 0; // the winding number counter

            for (int i = 0; i < this.polygon.Count - 1; i++)
            {
                if (this.polygon[i].Longitude <= p.Longitude)
                {
                    if (this.polygon[i + 1].Longitude > p.Longitude)
                        if (isLeft(this.polygon[i], this.polygon[i + 1], p) > 0)
                            ++wn;
                }
                else
                {
                    if (this.polygon[i + 1].Longitude <= p.Longitude)
                        if (isLeft(this.polygon[i], this.polygon[i + 1], p) < 0)
                            --wn;
                }
            }

            return !(wn == 0);
        }
        
        // check position of point to line
        public double isLeft(Point p0, Point p1, Point p2)
        {
            return (((p1.Latitude - p0.Latitude) * (p2.Longitude - p0.Longitude) - (p2.Latitude - p0.Latitude) * (p1.Longitude - p0.Longitude)));
        }

        // function that calculates polygon area
        public double calculate_area()
        {
            if (this.polygon[this.polygon.Count - 1].Latitude != this.polygon[0].Latitude || this.polygon[this.polygon.Count - 1].Longitude != this.polygon[0].Longitude)
                this.polygon.Add(new Point(this.polygon[0].Latitude, this.polygon[0].Longitude));

            double up_corner = this.polygon[0].Longitude;
            double down_corner = this.polygon[0].Longitude;
            double left_corner = this.polygon[0].Latitude;
            double right_corner = this.polygon[0].Latitude;

            foreach (Point i in this.polygon)
            {
                if (i.Longitude > up_corner)
                    up_corner = i.Longitude;
                if (i.Longitude < down_corner)
                    down_corner = i.Longitude;
            }

            foreach (Point i in this.polygon)
            {
                if (i.Latitude > right_corner)
                    right_corner = i.Latitude;
                if (i.Latitude < left_corner)
                    left_corner= i.Latitude;
            }

            double minimalBoundingBoxArea = (up_corner - down_corner) * (right_corner - left_corner);

            const double tryNumber = 1000000;
            double inTarget = 0;
            Random rand = new Random();

            for (int i = 0; i < tryNumber; i++)
                if (isInside(new Point(rand.NextDouble() * (right_corner - left_corner) + left_corner, rand.NextDouble() * (up_corner - down_corner) + down_corner)))
                    inTarget++;

            this.area = minimalBoundingBoxArea * (inTarget / tryNumber);
            return this.area;
        }
    }
}

