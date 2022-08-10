using Microsoft.VisualStudio.TestTools.UnitTesting;
using KeyNStroke;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KeyNStroke.Tests
{
    [TestClass()]
    public class ImageResourcesTests
    {
        [TestMethod()]
        public void ApplyScalingFactorTest()
        {
            KeyNStroke.SettingsStore mySettings = new KeyNStroke.SettingsStore();

            mySettings.WindowLocationDefault = new Point(
                System.Windows.SystemParameters.PrimaryScreenWidth - mySettings.WindowSizeDefault.Width - 20,
                System.Windows.SystemParameters.PrimaryScreenHeight - mySettings.WindowSizeDefault.Height - 40);

            //mySettings.ResetAll(); // test defaults
            mySettings.LoadAll();

            ImageResources.Init(mySettings.ButtonIndicatorCustomIconsFolder);
            ImageResources.ApplyScalingFactor(1.0);
            ImageResources.ApplyScalingFactor(1.4f);
        }
    }
}