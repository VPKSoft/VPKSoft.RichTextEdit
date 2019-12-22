#region License
/*
VPKSoft.RichTextEdit

A library containing a rich text edit component with a tool strip.
Copyright © 2019 VPKSoft, Petteri Kautonen

Contact: vpksoft@vpksoft.net

This file is part of VPKSoft.RichTextEdit.

VPKSoft.RichTextEdit is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

VPKSoft.RichTextEdit is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with VPKSoft.RichTextEdit.  If not, see <http://www.gnu.org/licenses/>.
*/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VPKSoft.RichTextEdit
{
    /// <summary>
    /// A control for selecting a font size from a ToolStrip.
    /// </summary>
    [ToolboxItem(false)]
    public partial class ToolStripFontSizeComboBox : ToolStripComboBox
    {
        /// <summary>
        /// Initializes a new instance of the ToolStripFontSizeComboBox class.
        /// </summary>
        public ToolStripFontSizeComboBox(): base()
        {
            Init(); // default values for all constructors
        }

        /// <summary>
        /// Initializes a new instance of the ToolStripFontSizeComboBox class with a given name.
        /// </summary>
        /// <param name="name">The name of the ToolStripFontSizeComboBox.</param>
        public ToolStripFontSizeComboBox(string name)
            : base(name)
        {
            Init(); // default values for all constructors
        }

        /// <summary>
        /// The internal ComboBox from a ToolStripComboBox class instance.
        /// </summary>
        ComboBox cmb;

        /// <summary>
        /// Common logic for all the constructors.
        /// </summary>
        internal void Init()
        {
            InitializeComponent();
            cmb = (ComboBox)Control; // get the internal ComboBox control

            // populate the ComboBox with font sizes
            Items.Clear();

            for (int i = 8; i <= 12; i++)
            {
                Items.Add(i.ToString());
            }

            for (int i = 14; i <= 28; i+=2)
            {
                Items.Add(i.ToString());
            }
            Items.Add(36.ToString());
            Items.Add(48.ToString());
            Items.Add(72.ToString());

            // Assign a size
            cmb.MinimumSize = new Size(40, cmb.MinimumSize.Height);

            // No autosize
            base.AutoSize = false;
        }

        /// <summary>
        /// Gets the default size of the control.
        /// </summary>
        protected override Size DefaultSize
        {
            get
            {
                return new Size(40, base.DefaultSize.Width);
            }
        }

        /// <summary>
        /// The selected font size.
        /// </summary>
        private float? _FontSize = 9F;

        /// <summary>
        /// Indicates whether to raise FontSizeChanged event.
        /// </summary>
        private bool suspendChange = false;

        /// <summary>
        /// Gets or sets the selected font size.
        /// </summary>
        public float? FontSize
        {
            get
            {
                return _FontSize;
            }
            
            set
            {
                suspendChange = true; // no events are raised..
                _FontSize = value;
                Text = value == null ? string.Empty : _FontSize.ToString();
                cmb.SelectionLength = 0;
                cmb.SelectionStart = cmb.Text.Length;
                suspendChange = false; // allow events to be raised
            }
        }

        /// <summary>
        /// A delegate for the FontSizeChanged event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">FontSizeEventArgs class instance.</param>
        public delegate void OnFontSizeChanged(object sender, FontSizeEventArgs e);

        /// <summary>
        /// An event that is raised if the user changes the FontSize property of the ToolStripFontSizeComboBox class instance.
        /// </summary>
        public event OnFontSizeChanged FontSizeChanged;


        /// <summary>
        /// Raises the System.Windows.Forms.ToolStripItem.TextChanged event.
        /// </summary>
        /// <param name="e">EventArgs class instance.</param>     
        protected override void OnTextChanged(EventArgs e)
        {
            if (suspendChange) // don't raise events if disabled
            {
                return;
            }

            float val;
            if (float.TryParse(cmb.Text, out val)) // try to parse the text in the ComboBox to a float value
            {
                if (val > 0 && _FontSize != val) // if the given size was > 0 and the value was actually changed raise the FontSizeChanged event
                {
                    FontSize = val;

                    if (FontSizeChanged != null) // only if assigned
                    {
                        FontSizeChanged(this, new FontSizeEventArgs(val)); // raise the event
                    }
                }
            }

        }
    }

    /// <summary>
    /// A class which instance is passed with the ToolStripFontSizeComboBox.FontSizeChanged event.
    /// </summary>
    public class FontSizeEventArgs: EventArgs
    {
        /// <summary>
        /// The font size.
        /// </summary>
        private float _size;

        /// <summary>
        /// Initializes a new instance of the FontSizeEventArgs class.
        /// </summary>
        /// <param name="size">A selected font size.</param>
        public FontSizeEventArgs(float size)
            : base()
        {
            _size = size;
        }

        /// <summary>
        /// Gets the font size.
        /// </summary>
        public float FontSize
        {
            get
            {
                return _size;
            }
        }
    }
}
