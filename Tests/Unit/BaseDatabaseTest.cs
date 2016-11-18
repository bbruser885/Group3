﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using LiteDB;
using ChocAn;

namespace Tests
{
    // All tests that use database functionality should inherit from this base test
    [TestClass]
    public class BaseDatabaseTest
    {
        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            BaseModel.InitializeDatabase();
        }

        [ClassCleanup]
        public static void ClassClean(TestContext context)
        {
            File.Delete("data.db");
        }
    }
}