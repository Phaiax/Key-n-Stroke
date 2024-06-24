using Microsoft.VisualStudio.TestTools.UnitTesting;
using KeyNStroke;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyNStroke.Tests
{
    [TestClass()]
    public class KeystrokeParserTests
    {
        [TestMethod()]
        public void KeystrokeParserTest()
        {
            var myKeyboardHook = new KeyboardHook();
            var myKeystrokeConverter = new KeystrokeParser(myKeyboardHook);
            Assert.IsNotNull(myKeystrokeConverter);
           
        }
    }
}