using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChocAn;

namespace Tests
{
    [TestClass]
    public class ViewInputTests
    {
        [TestMethod]
        public void Read_Datetime_Reads_Valid_Datetime()
        {
            var sr = new System.IO.StringReader("1234567abc\n20-20-2020\n12-31-1999");
            System.Console.SetIn(sr);
            Assert.AreEqual("12-31-1999", View.ReadDateTime().ToString("MM-dd-yyyy"));
        }
    }
}
