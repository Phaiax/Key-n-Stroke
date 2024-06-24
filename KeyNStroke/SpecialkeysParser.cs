﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KeyNStroke
{
    class SpecialkeysParser
    {
        public static string ToString(Key k, bool enableTextOverSymbol)
        {
            switch(k){
                case Key.LeftShift:
                case Key.RightShift:
                    return enableTextOverSymbol ? " [Shift] " : "⇧";
                case Key.LeftCtrl:
                case Key.RightCtrl:
                    return "Ctrl";
                case Key.LWin:
                case Key.RWin:
                    return "Win";
                case Key.LeftAlt:
                case Key.RightAlt:
                    return "Alt";
                case Key.CapsLock:
                    return enableTextOverSymbol ? " [CapsLock] " : "⇪";
                case Key.LineFeed:
                case Key.Return:
                    return enableTextOverSymbol ? " [Return] " : " ⏎";
                case Key.Back:
                    return enableTextOverSymbol ? " [Backspace] " : " ⌫ ";
                case Key.Left:
                    return " ← ";
                case Key.Right:
                    return " → ";
                case Key.Down:
                    return " ↓ ";
                case Key.Up:
                    return " ↑ ";
                case Key.Escape:
                    return " [Esc] ";
                case Key.PrintScreen:
                    return " [Print] ";
                case Key.Pause:
                    return " [Pause] ";
                case Key.Insert:
                    return " [Insert] ";
                case Key.Delete:
                    return " [Delete] ";


                case Key.Tab:
                    return enableTextOverSymbol ? " [Tab] " : "↹";
                case Key.Space:
                    return "␣";
                case Key.PageUp: 
                    return enableTextOverSymbol ? " [PageUp] " : " ↖ ";
                case Key.PageDown:
                     return enableTextOverSymbol ? " [PageDown] " : " ↘ ";
                case Key.End:
                    return enableTextOverSymbol ? " [End] " : " ⇲ ";
                case Key.Home:
                    return enableTextOverSymbol ? " [Home] " : " ⇱ ";
                case Key.Print:
                    return " ⎙ ";

                case Key.Clear:
                case Key.ImeProcessed:
                case Key.Attn:
                case Key.CrSel:
                case Key.ExSel:
                case Key.EraseEof:
                case Key.Cancel:
                case Key.Select:
                case Key.Execute:
                case Key.Help:
                case Key.Apps:
                case Key.Pa1:
                case Key.Sleep:
                    return " [" + k.ToString() + "] ";

            
           
                //case Key.OemSemicolon: //  Key.Oem1 // Let ToUnicode handle this character
                //    return ";";
                //case Key.OemComma: // Let ToUnicode handle this character
                //    return ",";
                //case Key.OemQuestion: // Key.Oem2 // Let ToUnicode handle this character
                //    return "?";
                //case Key.OemTilde: // Key.Oem3   // Let ToUnicode handle this character
                //    return "~";
                //case Key.AbntC1:
                //case Key.AbntC2:
                //case Key.OemOpenBrackets:
                //case Key.OemCloseBrackets:
                //case Key.OemQuotes: // Key.Oem7 // Let ToUnicode handle this character
                //    return "\"";

                //case Key.Oem102:
                //case Key.OemPipe: // Key.Oem5  // Let ToUnicode handle this character
                //    return "|";
                // case Key.OemBackslash: // Let ToUnicode handle this character
                //    return "\\";


                //case Key.Multiply:  // Let ToUnicode handle this character
                //    return "*";
                //case Key.Add:  // Let ToUnicode handle this character
                //    return "+";
                case Key.Separator:
                    return " [Seperator] ";
                //case Key.Subtract:
                //case Key.OemMinus: // Let ToUnicode handle this character
                //    return "-";
                //case Key.OemPeriod:
                //case Key.Decimal: // Let ToUnicode handle this character
                //    return ".";
                //case Key.Divide: // Let ToUnicode handle this character
                //    return "/";
                case Key.NumLock:
                    return " [NumLock] ";
                case Key.Scroll:
                    return " [ScrollLock] ";

                case Key.BrowserBack:
                    return " [🌐⇦] ";
                case Key.BrowserForward:
                    return " [🌐⇨] ";
                case Key.BrowserRefresh:
                    return " [🌐↻] ";
                case Key.BrowserStop:
                    return " [🌐✋] ";
                case Key.BrowserSearch:
                    return " [🌐🔎] ";
                case Key.BrowserFavorites:
                    return " [🌐★] ";
                case Key.BrowserHome:
                    return " [🌐⌂] ";

          
                case Key.VolumeMute:
                    return " 🔇 ";
                case Key.VolumeDown:
                    return " 🔉⏬ ";
                case Key.VolumeUp:
                    return " 🔊⏫ ";
                case Key.MediaNextTrack:
                    return " ⏭ ";
                case Key.MediaPreviousTrack:
                    return " ⏮ ";
                case Key.MediaStop:
                    return " ◼ ";
                case Key.MediaPlayPause:
                    return " ⏯ ";
                case Key.LaunchMail:
                    return " 📧 ";
                case Key.SelectMedia:
                    return " ♪ ";
                case Key.LaunchApplication1:
                    return " ① ";
                case Key.LaunchApplication2:
                    return " ② ";

                case Key.Play:
                    return " ▶ ";
                case Key.Zoom:
                    return " [🔎±] ";
                

            }
            if(Key.F1 <= k && k <= Key.F24)
                return " " + k.ToString() + " ";

            throw new NotImplementedException();
        }
    }
}
