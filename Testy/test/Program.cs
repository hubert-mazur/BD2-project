using System;
using System.Collections.Generic;
using System.Text;
using Projekt;
using System.Collections;
using System.Data.SqlClient;

namespace Projekt
{
    public class Polygon_new
    {
        public static void Main(string[] args)
        {
            Polygon_new p = new Polygon_new();
            p.Load("points");
            Console.ReadKey();
        }


        private ArrayList polygon;
        private string connectionString = "SERVER=MSSQLServer; " +
                                          "INITIAL CATALOG=projekt; INTEGRATED SECURITY=SSPI; " +
                                          "MULTIPLEACTIVERESULTSETS=true;";

        private SqlCommand command;
        private SqlConnection connection;
        private SqlDataReader reader;

        public Polygon_new()
        {
            this.polygon = new ArrayList();
        }

        public void Load(string tableName)
        {
            try
            {
                string command_string = "SELECT point FROM " + tableName;
                this.connection = new SqlConnection(connectionString);
                this.connection.Open();
                this.command = new SqlCommand(command_string);
                this.reader = this.command.ExecuteReader();
                foreach (string i in this.reader)
                {
                    Console.WriteLine(i);
                }
            }
            finally
            {
                Console.WriteLine("Finally");
                this.connection.Close();
            }
        }

        public void ClosePolygon()
        {
            this.polygon.Add(this.polygon[0]);
        }

    //    public override string ToString()
    //    {
    //        string str = "";

    //        foreach (Point i in this.polygon)
    //        {
    //            str += i.ToString();
    //            str += "\n";
    //        }
    //        return str;
    //    }
    }
}
