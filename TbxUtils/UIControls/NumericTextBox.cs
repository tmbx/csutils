/******************************************************/
/*          NULLFX FREE SOFTWARE LICENSE              */
/******************************************************/
/*  NumericTextBox Library                            */
/*  by: Steve Whitley                                 */
/*  © 2005 NullFX Software                            */
/*                                                    */
/* NULLFX SOFTWARE DISCLAIMS ALL WARRANTIES,          */
/* RESPONSIBILITIES, AND LIABILITIES ASSOCIATED WITH  */
/* USE OF THIS CODE IN ANY WAY, SHAPE, OR FORM        */
/* REGARDLESS HOW IMPLICIT, EXPLICIT, OR OBSCURE IT   */
/* IS. IF THERE IS ANYTHING QUESTIONABLE WITH REGARDS */
/* TO THIS SOFTWARE BREAKING AND YOU GAIN A LOSS OF   */
/* ANY NATURE, WE ARE NOT THE RESPONSIBLE PARTY. USE  */
/* OF THIS SOFTWARE CREATES ACCEPTANCE OF THESE TERMS */
/*                                                    */
/* USE OF THIS CODE MUST RETAIN ALL COPYRIGHT NOTICES */
/* AND LICENSES (MEANING THIS TEXT).                  */
/*                                                    */
/******************************************************/

namespace NullFX.Controls
{
    using System;
    using System.Windows.Forms;
    public class NumericTextBox : TextBox
    {
        int WM_KEYDOWN = 0x0100,
            WM_PASTE = 0x0302;
        public override bool PreProcessMessage(ref Message msg)
        {
            if (msg.Msg == WM_KEYDOWN)
            {
                Keys keys = (Keys)msg.WParam.ToInt32();
                bool numbers = ((keys >= Keys.D0 && keys <= Keys.D9)
                    || (keys >= Keys.NumPad0 && keys <= Keys.NumPad9)) && ModifierKeys != Keys.Shift;
                bool ctrl = keys == Keys.Control;
                bool ctrlZ = keys == Keys.Z && ModifierKeys == Keys.Control,
                    ctrlX = keys == Keys.X && ModifierKeys == Keys.Control,
                    ctrlC = keys == Keys.C && ModifierKeys == Keys.Control,
                    ctrlV = keys == Keys.V && ModifierKeys == Keys.Control,
                    del = keys == Keys.Delete,
                    bksp = keys == Keys.Back,
                    home = keys == Keys.Home,
                    end = keys == Keys.End,
                    arrows = (keys == Keys.Up)
                    | (keys == Keys.Down)
                    | (keys == Keys.Left)
                    | (keys == Keys.Right);
                if (numbers | ctrl | del | bksp | arrows |
                    ctrlC | ctrlX | ctrlZ | home | end)
                    return false;
                else if (ctrlV)
                {
                    IDataObject obj = Clipboard.GetDataObject();
                    string input = (string)obj.GetData(typeof(string));
                    foreach (char c in input)
                    {
                        if (!char.IsDigit(c)) return true;
                    }
                    return false;
                }
                else
                    return true;
            }
            else


                return base.PreProcessMessage(ref msg);
        }
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_PASTE)
            {
                IDataObject obj = Clipboard.GetDataObject();
                string input = (string)obj.GetData(typeof(string));
                foreach (char c in input)
                {
                    if (!char.IsDigit(c))
                    {
                        m.Result = (IntPtr)0;
                        return;
                    }
                }
            }
            base.WndProc(ref m);
        }
    }
}
