using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KeyNStroke
{
    class SpecialkeysParser
    {
        public static string ToString(Key k)
        {
            switch(k){
                case Key.LeftShift:
                case Key.RightShift:
                    return "⇧";
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
                    return "⇪";
                case Key.LineFeed:
                case Key.Return:
                    return " ⏎";
                case Key.Back:
                    return " ⌫ ";
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
                    return "↹";
                case Key.Space:
                    return "␣";
                case Key.PageUp: 
                    return " ↖ ";
                case Key.PageDown:
                     return " ↘ ";
                case Key.End:
                    return " ⇲ ";
                case Key.Home:
                    return " ⇱ ";
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

            
           
                case Key.OemSemicolon: //  Key.Oem1
                    return ";";
                case Key.OemComma:
                    return ",";
                case Key.OemQuestion: // Key.Oem2
                    return "/";
                case Key.OemTilde: // Key.Oem3
                    return "`";
                //case Key.AbntC1:
                //case Key.AbntC2:
                //case Key.OemOpenBrackets:
                //case Key.OemCloseBrackets:
                case Key.OemQuotes: // Key.Oem7
                    return "'";

                //case Key.Oem102:
                case Key.OemPipe: // Key.Oem5
                    return "\\";
                case Key.OemBackslash:
                    return "\\";


                case Key.Multiply:
                    return "*";
                case Key.Add:
                    return "+";
                case Key.Separator:
                    return " [Seperator] ";
                case Key.Subtract:
                case Key.OemMinus:
                    return "-";
                case Key.OemPeriod:
                case Key.Decimal:
                    return ".";
                case Key.Divide:
                    return "/";
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
