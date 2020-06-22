using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Projekt;

namespace unitTests
{
    [TestClass]
    public class UnitTest
    {
        private DBConnection fig1;
        public UnitTest()
        {
            this.fig1 = new DBConnection();
            List<Point> p = new List<Point>();
            p.Add(new Point(3, 8));
            p.Add(new Point(10, 14));
            p.Add(new Point(12, 2));
            p.Add(new Point(11, -6));
            p.Add(new Point(-3, -9));
            p.Add(new Point(-13, 0));
            p.Add(new Point(-9, 10));
            p.Add(new Point(3, 8));
            fig1.Init();
            fig1.InsertPoints(p);
            fig1.Create_Polygon();
        }

        private TestContext testContextInstance;

        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }


        [TestMethod]
        public void AreaTest()
        {
            double expected_area = 373;
            double calculated_area = fig1.PolygonArea();
            Console.WriteLine("area: {0}", calculated_area);
            fig1.Clear();
            Assert.IsTrue(0.99 * expected_area <= calculated_area && calculated_area <= 1.01 * expected_area);
        }

        [TestMethod]
        public void LenghtTest1()
        {
            double expected_length = 26.41;
            double calculated_length = fig1.CalculateDistance(2, 5);
            fig1.Clear();
            Console.WriteLine("LengthTest1 calculated: {0}", calculated_length);
            Assert.IsTrue(0.99 * expected_length < calculated_length && 1.01 * expected_length > calculated_length);
        }

        [TestMethod]
        public void LenghtTest2()
        {
            double expected_length = 9.22;
            double calculated_length = fig1.CalculateDistance(1, 2);
            fig1.Clear();
            Console.WriteLine("LengthTest2 calculated: {0}", calculated_length);
            Assert.IsTrue(0.99 * expected_length < calculated_length && 1.01 * expected_length > calculated_length);
        }

        [TestMethod]
        public void LenghtTest3()
        {
            double expected_length = 24.74;
            double calculated_length = fig1.CalculateDistance(4, 6);
            fig1.Clear();
            Console.WriteLine("LengthTest3 calculated: {0}", calculated_length);
            Assert.IsTrue(0.99 * expected_length < calculated_length && 1.01 * expected_length > calculated_length);
        }

        [TestMethod]
        public void PointInclusionTest1()
        {
            bool result = fig1.IsInside(0, 0);
            fig1.Clear();
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void PointInclusionTest2()
        {
            bool result = fig1.IsInside(-22, 0);
            fig1.Clear();
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void PointInclusionTest3()
        {
            bool result = fig1.IsInside(165, 9);
            fig1.Clear();
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void PointInclusionTest4()
        {
            bool result = fig1.IsInside(8, -3);
            fig1.Clear();
            Assert.IsTrue(result);
        }
    }
}
