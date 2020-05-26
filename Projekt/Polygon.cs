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


        public Polygon()
        {
            this.area = 0.0;
            this.polygon = new List<Point>();
        }

        public double GetArea
        {
            get { return this.area; }
            set { this.area = value; }
        }

        public Point Load
        {
            set { this.polygon.Add(new Point(value.Latitude, value.Longitude)); }
        }

        public Polygon ClosePolygon
        {
            set { this.polygon.Add(this.polygon[0]); }
        }

        public static Polygon Parse(SqlString s)
        {
            return new Polygon();
        }

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

        public int IncludedPoints
        {
            get {return this.polygon.Count;}
        }


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
        // check whether given point is inside or outside polygon
        public bool isInside(Point p)
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
        

        public int isLeft(Point p0, Point p1, Point p2)
        {
            return Convert.ToInt32(((p1.Latitude - p0.Latitude) * (p2.Longitude - p0.Longitude) - (p2.Latitude - p0.Latitude) * (p1.Longitude - p0.Longitude)));
        }

        public double calculate_area()
        {
            if (this.polygon[this.polygon.Count - 1] != this.polygon[0])
                this.ClosePolygon = this;

            double up_corner = this.polygon[0].Latitude;
            double down_corner = this.polygon[0].Latitude;
            double left_corner = this.polygon[0].Longitude;
            double right_corner = this.polygon[0].Longitude;

            foreach (Point i in this.polygon)
            {
                if (i.Latitude > up_corner)
                    up_corner = i.Latitude;
                if (i.Latitude < down_corner)
                    down_corner = i.Latitude;
            }

            foreach (Point i in this.polygon)
            {
                if (i.Longitude > left_corner)
                    left_corner = i.Longitude;
                if (i.Longitude < right_corner)
                    right_corner = i.Longitude;
            }

            double minimalBoundingBoxArea = (up_corner - down_corner) * (left_corner - right_corner);

            const double tryNumber = 1000000;
            double inTarget = 0;
            Random rand = new Random();

            for (int i = 0; i < tryNumber; i++)
                if (isInside(new Point(rand.NextDouble() * (up_corner - down_corner + down_corner), rand.NextDouble() * (left_corner - right_corner) + right_corner)))
                    inTarget++;

            this.area = minimalBoundingBoxArea * (inTarget / tryNumber);
            return this.area;
        }
    }
}

