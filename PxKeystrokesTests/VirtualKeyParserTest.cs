using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Forms;
using PxKeystrokesUi;

namespace PxKeystrokesTests
{
    [TestClass]
    public class VirtualKeyParserTest
    {
        [TestMethod]
        public void ParseVirtualKey()
        {
            KeyboardRawEventArgs V = new KeyboardRawEventArgs(auml());
            Assert.AreEqual(V.vkCode, (int)Keys.Oem7);
            string R = KeyboardLayoutParser.Parse(V);
            Assert.AreEqual("ä", R, false);
        }

        [TestMethod]
        public void ParseVirtualKeyWithShift()
        {
            KeyboardRawEventArgs V = new KeyboardRawEventArgs(Auml());
            //Assert.AreEqual(V.vkCode, (int)Keys.Oem7);
            string R = KeyboardLayoutParser.Parse(V);
            Assert.AreEqual("Ä", R, false);
        }

        [TestMethod]
        public void ParseVirtualKeyViaMapKeyCode()
        {
            KeyboardRawEventArgs V = new KeyboardRawEventArgs(auml());
            Assert.AreEqual(V.vkCode, (int)Keys.Oem7);
            string R = KeyboardLayoutParser.ParseViaMapKeycode(V);
            Assert.AreEqual("ä", R, false);
            }

        [TestMethod]
        public void ParseVirtualKeyViaMapKeyCodeWithShift()
        {
            KeyboardRawEventArgs V = new KeyboardRawEventArgs(Auml());
            //Assert.AreEqual(V.vkCode, (int)Keys.Oem7);
            string R = KeyboardLayoutParser.ParseViaMapKeycode(V);
            Assert.AreEqual("Ä", R, false);
        }

        private NativeMethodsKeyboard.KBDLLHOOKSTRUCT auml()
        {
            // ä
            NativeMethodsKeyboard.KBDLLHOOKSTRUCT S = new NativeMethodsKeyboard.KBDLLHOOKSTRUCT();
            S.scanCode = 40;
            S.time = 153304890;
            S.vkCode = 222;
            S.flags = 0;
            S.dwExtraInfo = 0;
            return S;
        }

        private NativeMethodsKeyboard.KBDLLHOOKSTRUCT Auml()
        {
            // Ä
            NativeMethodsKeyboard.KBDLLHOOKSTRUCT S = new NativeMethodsKeyboard.KBDLLHOOKSTRUCT();
            S.scanCode = 42;
            S.time = 153304890;
            S.vkCode = 160;
            S.flags = 0;
            S.dwExtraInfo = 0;
            return S;
        }

    }
}
