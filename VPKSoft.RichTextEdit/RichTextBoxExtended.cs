using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VPKSoft.RichTextEdit
{
    /// <summary>
    /// An extended rich text box based on the article @: https://tutel.me/c/programming/questions/32304388/cursor+flickers+when+trying+to+resize+image+in+richtextbox
    /// Implements the <see cref="System.Windows.Forms.RichTextBox" />
    /// </summary>
    /// <seealso cref="System.Windows.Forms.RichTextBox" />
    public class RichTextBoxExtended: RichTextBox
    {
        private const int WM_SETCURSOR = 0x20;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr SetCursor(IntPtr hCursor);

        protected override void WndProc(ref Message m) {
            if (m.Msg == WM_SETCURSOR) 
            {
                if (SelectionType == RichTextBoxSelectionTypes.Object) 
                {
                    // Necessary to avoid recursive calls
                    if (Cursor != Cursors.Cross) 
                    {
                        Cursor = Cursors.Cross;
                    }
                }
                else 
                {
                    // Necessary to avoid recursive calls
                    if (Cursor != Cursors.IBeam) 
                    {
                        Cursor = Cursors.IBeam;
                    }
                }

                SetCursor(Cursor.Handle);
                return;
            }

            base.WndProc(ref m);
        }

        private CreateParams createParams;
        /*
        /// <summary>
        /// The Lower property CreateParams is being used to reduce flicker
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                if (createParams == null)
                {
                    const int WS_EX_COMPOSITED = 0x02000000;
                    createParams = base.CreateParams;
                    createParams.ExStyle |= WS_EX_COMPOSITED;
                    return createParams;
                }

                return createParams;
            }
        }
        */
    }
}
