using System;
using System.Collections.Generic;
using System.Text;
using Projekt;
using System.Collections;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Data.SqlTypes;

namespace Projekt
{
    public class DBConnection
    {
        public static void Main(string[] args)
        {
            DBConnection p = new DBConnection();
        }

        private List<Point> points;
        private string connectionString = "SERVER=MSSQLServer; " +
                                          "INITIAL CATALOG=testDB; INTEGRATED SECURITY=SSPI; " +
                                          "MULTIPLEACTIVERESULTSETS=true;";
        private SqlCommand command;
        private SqlConnection connection;
        private SqlDataReader reader;

        // constructor, sets up connection with database engine
        public DBConnection()
        {
            this.points = new List<Point>();
            try
            {
                this.connection = new SqlConnection(connectionString);
                this.connection.Open();
                this.command = new SqlCommand("", this.connection);
                //this.Init();
                Console.WriteLine("-- DB Connection OK --");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                this.connection.Close();
                System.Environment.Exit(-1);
            }
        }

        // function that initializes database structures
        public void Init()
        {
            try
            {
                string command_string = "EXEC create_points_table";
                //this.command = new SqlCommand(command_string);
                this.command.CommandText = command_string;
                this.command.ExecuteNonQuery();
                
                command_string = "EXEC create_polygon_table";
                this.command.CommandText = command_string;
                this.command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        // function that wipes out created tables in database
        public void Clear()
        {
            try
            {
                this.command.CommandText = "EXEC clear_points_table";
                int rows = this.command.ExecuteNonQuery();
                this.command.CommandText = "EXEC clear_polygons_table";
                this.command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        // function that inserts points in database structures
        public void InsertPoints(List<Point> p)
        {
            try
            {
                this.points = p;
                string s = "INSERT INTO points VALUES ";
                foreach (Point i in this.points)
                {
                    if (this.points.IndexOf(i) != this.points.Count - 1)
                        s += "('" + i.Latitude + "," + i.Longitude + "'),";
                    else
                        s += "('" + i.Latitude + "," + i.Longitude + "');";
                }
                Console.WriteLine(s);
                this.command.CommandText = s;
                this.command.ExecuteNonQuery();
                
                Console.WriteLine("LOADED points and their indexes: ");

                this.command.CommandText = "SELECT id, point.ToString() as p FROM points";
                this.reader =  this.command.ExecuteReader();
                
                while (this.reader.Read())
                    Console.WriteLine("{0}, {1}",this.reader["id"], this.reader["p"]);
                this.reader.Close();
                

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                this.connection.Close();
                System.Environment.Exit(-1);
            }
        }

        // function creating polygon from points
        public void Create_Polygon()
        {
            try
            {
                this.command.CommandText = "INSERT INTO polygon VALUES ('')";
                this.command.ExecuteNonQuery();

                this.command.CommandText = "EXEC fill_polygon";
                this.command.ExecuteNonQuery();
                Console.WriteLine("Polygon created");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                this.connection.Close();
                System.Environment.Exit(-1);
            }
        }

        // function that executes area counting in database
        public double PolygonArea()
        {
            if (this.points.Count < 3)
            {
                Console.WriteLine("Can't calculate area");
                return 0.0;
            }

            try
            {
                this.command.CommandText = "SELECT pol.calculate_area() FROM polygon";
                double ar = (double)this.command.ExecuteScalar();
                Console.WriteLine("Polygon area: {0}", ar);
                return ar;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                this.connection.Close();
                System.Environment.Exit(-1);
            }
            return -1;
        }

        // function that corresponds in point inclusion
        public Boolean IsInside(double x, double y)
        {
            this.command.CommandText = "SELECT pol.isPointInside(@x,@y) FROM polygon;";
            this.command.Parameters.Add("@x", System.Data.SqlDbType.Float);
            this.command.Parameters.Add("@y", System.Data.SqlDbType.Float);
            this.command.Parameters["@x"].Value = x;
            this.command.Parameters["@y"].Value = y;
            //this.command.CommandText = "SELECT pol.isPointInside(" + x.ToString() + "," + y.ToString() + ") FROM polygon";
            return (Boolean)this.command.ExecuteScalar();
        }

        // function that corresponds in distance calculating
        public double CalculateDistance(int id1, int id2)
        {
            try
            {
                this.command.CommandText = "SELECT point.distance((SELECT point FROM points where id = " + id2.ToString() + ")) FROM points  WHERE id = " + id1.ToString();
                return (double)this.command.ExecuteScalar();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                this.connection.Close();
                System.Environment.Exit(-1);
            } 
            return 0.0;
        }
    }
}
