using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common;
using Common.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class FileTests
    {
        [TestMethod]
        public void FindFileTest()
        {
            var file = CSVFile.FindFile("testdata.csv");

            Assert.AreNotEqual(null, file);

            var info = new FileInfo(file);

            Assert.IsTrue(info.Exists);
        }

        [TestMethod]
        public void FindWrongFileTest()
        {
            var file = CSVFile.FindFile("wrongtestdata.csv");

            Assert.AreEqual(null, file);
        }

        [TestMethod]
        public void ConvertDateTest1()
        {
            var dt = CSVFile.ConvertDate("30Mar2006 0:00:00");

            Assert.AreEqual(dt.Year, 2006);
            Assert.AreEqual(dt.Month, 3);
            Assert.AreEqual(dt.Day, 30);
        }


        [TestMethod]
        public void ConvertDateTest2()
        {
            var dt = CSVFile.ConvertDate("09Jan2014 0:00:00");

            Assert.AreEqual(dt.Year, 2014);
            Assert.AreEqual(dt.Month, 1);
            Assert.AreEqual(dt.Day, 9);
        }

        [TestMethod]
        public void LoadTestDataTest()
        {
            var data = new List<Customer>(CSVFile.ReadFile(CSVFile.FindFile("testdata.csv")));

            Assert.AreEqual(49, data.Count());
        }
    }
}
