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
using System.Reflection;

namespace VPKSoft.RichTextEdit
{
    /// <summary>
    /// A control for selecting a font family from a ToolStrip.
    /// </summary>
    [ToolboxItem(false)]
    public partial class ToolStripFontComboBox : ToolStripComboBox
    {
        /// <summary>
        /// Initializes a new instance of the ToolStripFontComboBox class.
        /// </summary>
        public ToolStripFontComboBox(): base()
        {
            Init(); // call the common constructor init code
        }

        /// <summary>
        /// Initializes a new instance of the ToolStripFontComboBox class with a given name.
        /// </summary>
        /// <param name="name">The name of the ToolStripFontComboBox.</param>
        public ToolStripFontComboBox(string name)
            : base(name)
        {
            Init(); // call the common constructor init code
        }

        /// <summary>
        /// A value indicating if the ToolStripFontComboBox can raise FontFamilyChanged event.
        /// </summary>
        private bool raiseEvent = false;

        /// <summary>
        /// Gets or sets the value of the selected FontFamily of the ToolStripFontComboBox.
        /// </summary>
        public FontFamily FontFamily
        {
            get
            {
                return (FontFamily)SelectedItem;
            }

            set
            {
                raiseEvent = false;
                if (value != null)
                {
                    if (!value.Equals(SelectedItem))
                    {
                        SelectedItem = value;
                    }
                }
                else
                {
                    SelectedIndex = -1;
                }
                raiseEvent = true;
            }
        }

        /// <summary>
        /// Sets the controls default values, populates the internal ComboBox, eg..
        /// </summary>
        internal void Init()
        {
            InitializeComponent();
            AutoSize = false;
            ComboBox cmb = (ComboBox)Control;
            cmb.DrawMode = DrawMode.OwnerDrawFixed;
            System.Drawing.Text.InstalledFontCollection fontCollection = new System.Drawing.Text.InstalledFontCollection();
            IEnumerable<FontFamily> families = fontCollection.Families.OrderBy(f => f.Name);
            cmb.Items.AddRange(families.ToArray());
            cmb.DisplayMember = "Name";
            cmb.DrawItem += cmb_DrawItem;
            cmb.DropDownStyle = ComboBoxStyle.DropDownList;
            cmb.SelectedIndexChanged += cmb_SelectedIndexChanged;
            raiseEvent = true;
        }

        /// <summary>
        /// Rises the FontFamilyChanged event if there are no preventing conditions.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">EventArgs class instance.</param>
        void cmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cmb = (ComboBox)Control;
            if (!raiseEvent) // dont't raise the FontFamilyChanged event
            {
                return;
            }

            if (cmb.SelectedItem != null && FontFamilyChanged != null)
            {
                FontFamilyChanged(this, new FontFamilyEventArgs(FontFamily));
            }
        }

        /// <summary>
        /// Draws the ToolStripFontComboBox contents with font families using a font created from the FontFamily item in the Items list.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">DrawItemEventArgs class instance.</param>
        void cmb_DrawItem(object sender, DrawItemEventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;
            if (e.Index != -1)
            {
                e.Graphics.FillRectangle(new SolidBrush(e.BackColor), e.Bounds);
                e.Graphics.DrawString(((FontFamily)cmb.Items[e.Index]).Name, new Font((FontFamily)cmb.Items[e.Index], cmb.Font.Size), new SolidBrush(e.ForeColor), e.Bounds.Location);
            }
        }

        /// <summary>
        /// A delegate for the FontFamilyChanged event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">FontFamilyEventArgs class instance.</param>
        public delegate void OnFontFamilyChanged(object sender, FontFamilyEventArgs e);

        /// <summary>
        /// An event that is raised if the user changes the SelectedItem property of the ToolStripFontComboBox class instance.
        /// </summary>
        public event OnFontFamilyChanged FontFamilyChanged;
    }

    /// <summary>
    /// A class which instance is passed with the ToolStripFontComboBox.FontFamilyChanged event.
    /// </summary>
    public class FontFamilyEventArgs : EventArgs
    {
        /// <summary>
        /// The selected FontFamily.
        /// </summary>
        private FontFamily _family;

        /// <summary>
        /// Initializes a new instance of the FontFamilyEventArgs class.
        /// </summary>
        /// <param name="family">A selected FontFamily.</param>
        public FontFamilyEventArgs(FontFamily family)
            : base()
        {
            _family = family;
        }

        /// <summary>
        /// Gets the selected FontFamily.
        /// </summary>
        public FontFamily FontFamily
        {
            get
            {
                return _family;
            }
        }
    }
}
