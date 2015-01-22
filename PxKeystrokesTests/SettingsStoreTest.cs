using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PxKeystrokesUi;
using System.Drawing;


namespace PxKeystrokesTests
{
    [TestClass]
    public class SettingsStoreTest
    {
        [TestMethod]
        public void TestSavingAndLoadingInt()
        {
            SettingsStore s1 = new SettingsStore();
            s1.LoadAll();
            s1.LabelAnimation = Style.NoAnimation;
            s1.WindowSize = new Size(200, 211);
            s1.WindowLocation = new Point(233, 234);
            s1.SaveAll();

            SettingsStore s2 = new SettingsStore();
            s2.LoadAll();
            Assert.AreEqual(s2.LabelAnimation, Style.NoAnimation);
            Assert.AreEqual(s2.WindowSize.Width, 200);
            Assert.AreEqual(s2.WindowSize.Height, 211);
            Assert.AreEqual(s2.WindowLocation.X, 233);
            Assert.AreEqual(s2.WindowLocation.Y, 234);
            s2.LabelAnimation = Style.Slide;
            s2.WindowSize = new Size(201, 212);
            s2.WindowLocation = new Point(234, 235);
            s2.SaveAll();

            s1.LoadAll();
            Assert.AreEqual(s1.LabelAnimation, Style.Slide);
            Assert.AreEqual(s1.WindowSize.Width, 201);
            Assert.AreEqual(s1.WindowSize.Height, 212);
            Assert.AreEqual(s1.WindowLocation.X, 234);
            Assert.AreEqual(s1.WindowLocation.Y, 235);
        }

    }
}
