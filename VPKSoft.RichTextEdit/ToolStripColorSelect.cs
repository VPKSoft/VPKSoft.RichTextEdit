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
using VPKSoft.TextEditIcons;

namespace VPKSoft.RichTextEdit
{
    /// <summary>
    /// A class for selecting colors for text from a ToolStripButton's drop down menu.
    /// </summary>
    [ToolboxItem(false)]
    public partial class ToolStripColorSelect : Panel
    {
        /// <summary>
        /// Initializes a new instance of ToolStripColorSelect class.
        /// </summary>
        /// <param name="background">Indicates whether to use default foreground or default background color for default color button.</param>
        public ToolStripColorSelect(bool background): base()
        {
            InitializeComponent();
            Background = background;

            InitColorButtons();
            SelectedColor = background ? SystemColors.Window : SystemColors.WindowText;
            this.MouseMove += ToolStripColorSelect_MouseMove;
            this.MouseClick += ToolStripColorSelect_MouseClick;
        }

        /// <summary>
        /// Internal logic to handle mouse clicks for the color buttons.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">MouseEventArgs class instance.</param>
        void ToolStripColorSelect_MouseClick(object sender, MouseEventArgs e)
        {
            bool shouldInvalidate = false, found = false;
            for (int i = 0; i < colorButtons.Count; i++) // check if some of the custom drawn color buttons is clicked
            {
                if (!shouldInvalidate) // indication if the control should be re-drawn
                {
                    shouldInvalidate = colorButtons[i].Selected != colorButtons[i].Rectangle.Contains(e.Location);
                }
                colorButtons[i].Selected = colorButtons[i].Rectangle.Contains(e.Location); // set the property of some color is clicked (selected)
                if (!found) // indication that the color button was selected
                {
                    found = colorButtons[i].Selected;
                    if (found) // if a color button was selected fire ColorChanged event
                    {
                        SelectedColor = colorButtons[i].Color;
                        if (ColorChanged != null)
                        {
                            ColorChanged(this, new ColorChangeEventArgs(SelectedColor));
                        }
                    }
                }
            }

            if (shouldInvalidate && found) // Invalidate the control if it should be re-drawn
            {
                Invalidate();
            }
        }

        /// <summary>
        /// Internal logic to handle mouse moving on the control.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">MouseEventArgs class instance.</param>
        void ToolStripColorSelect_MouseMove(object sender, MouseEventArgs e)
        {
            bool shouldInvalidate = false; // indication if the control should be re-drawn
            for (int i = 0; i < colorButtons.Count; i++)
            {
                if (!shouldInvalidate) // set the re-draw indicator
                {
                    shouldInvalidate = colorButtons[i].Floating != colorButtons[i].Rectangle.Contains(e.Location);
                }
                colorButtons[i].Floating = colorButtons[i].Rectangle.Contains(e.Location); // check if the mouse cursor is on a color button
            }

            if (shouldInvalidate) // Invalidate the control if it should be re-drawn
            {
                Invalidate();
            }
        }

        /// <summary>
        /// Colors for the custom color buttons in a HTML notation
        /// </summary>
        private static readonly string[] colors = new string[] { "#ffffff", "#ff0000", "#c0504d", "#d16349", "#dd8484", 
                                                                 "#cccccc", "#ffc000", "#f79646", "#d19049", "#f3a447",
                                                                 "#a5a5a5", "#ffff00", "#9bbb59", "#ccb400", "#dfce04",
                                                                 "#666666", "#00b050", "#4bacc6", "#8fb08c", "#a5b592",
                                                                 "#333333", "#004dbb", "#4f81bd", "#646b86", "#809ec2",
                                                                 "#000000", "#9b00d3", "#8064a2", "#9e7c7c", "#9c85c0" };


        /// <summary>
        /// Custom color select buttons in a list
        /// </summary>
        private List<CheckRect> colorButtons = new List<CheckRect>();

        // Other rectangles that shoulb be drawn to the control
        private List<ColorRect> backRects = new List<ColorRect>();
        private List<ColorRect> foreRects = new List<ColorRect>();
        private List<ColorRect> boundRects = new List<ColorRect>();

        /// <summary>
        /// Gets or set the value whether to use default foreground or default background color for default color button.
        /// </summary>
        private bool Background { get; set; }

        // Controls used by ToolStripColorSelect
        private CheckBox cbDefaultColor = new CheckBox();
        private Button btSelectCustomColor = new Button();
        private ColorDialog cdSelectCustomColor = new ColorDialog();

        /// <summary>
        /// Creates the custom color buttons and other controls used by the ToolStripColorSelect
        /// </summary>
        private void InitColorButtons()
        {
            Size = new Size(103, 147);
            int yOffset = 26;
            for (int i = 0; i < 5; i++)
            {
                backRects.Add(new ColorRect(20 * i + 1, 1 + yOffset, 17, 92, SystemColors.ControlLight));
                for (int j = 0; j < 6; j++)
                {
                    colorButtons.Add(new CheckRect(20 * i + 2, 15 * j + 2 + yOffset, 15, 15, false, System.Drawing.ColorTranslator.FromHtml(colors[j * 5 + i])));
                }
            }

            cbDefaultColor = new CheckBox();
            cbDefaultColor.Appearance = Appearance.Button;
            cbDefaultColor.Text = AutomaticColorText;
            cbDefaultColor.Image = IconsTextEdit.DefaultColorIcon(Background);
            cbDefaultColor.ImageAlign = ContentAlignment.MiddleLeft;
            cbDefaultColor.TextAlign = ContentAlignment.MiddleRight;
            Controls.Add(cbDefaultColor);
            cbDefaultColor.Bounds = new Rectangle(0, 2, 99, 23);
            cbDefaultColor.Checked = true;
            cbDefaultColor.Click += cbDefaultColor_Click;

            btSelectCustomColor = new Button();
            btSelectCustomColor.Text = MoreColorsText;
            btSelectCustomColor.Image = IconsTextEdit.PaletteIcon();
            btSelectCustomColor.ImageAlign = ContentAlignment.MiddleLeft;
            btSelectCustomColor.TextAlign = ContentAlignment.MiddleRight;
            Controls.Add(btSelectCustomColor);
            btSelectCustomColor.Bounds = new Rectangle(0, 121, 99, 23);
            btSelectCustomColor.Click += btSelectCustomColor_Click;
        }

        /// <summary>
        /// Internal logic to handle the default color button click.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">EventArgs class instance.</param>
        void cbDefaultColor_Click(object sender, EventArgs e)
        {
            if (cbDefaultColor.Checked)
            {
                SelectedColor = Background ? SystemColors.Window : SystemColors.WindowText;
                ColorChanged(this, new ColorChangeEventArgs(SelectedColor));
            }
            else
            {
                cbDefaultColor.Checked = (Background && SelectedColor == SystemColors.Window) ||
                                         (!Background && SelectedColor == SystemColors.WindowText);
            }
        }

        /// <summary>
        /// Internal logic to handle the custom color button click.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">EventArgs class instance.</param>
        void btSelectCustomColor_Click(object sender, EventArgs e)
        {
            cdSelectCustomColor.AllowFullOpen = true;
            cdSelectCustomColor.Color = SelectedColor;
            if (cdSelectCustomColor.ShowDialog(this.FindForm()) == DialogResult.OK)
            {
                SelectedColor = cdSelectCustomColor.Color;
                ColorChanged(this, new ColorChangeEventArgs(SelectedColor));
            }
        }

        /// <summary>
        /// An internal extension class to Rectangle. An instance of this class is used to draw a color select button.
        /// </summary>
        internal class CheckRect
        {
            /// <summary>
            /// Initializes a new instance of the CheckRect class.
            /// </summary>
            /// <param name="x">The x-coordinate of the upper-left corner of the rectangle.</param>
            /// <param name="y">The y-coordinate of the upper-left corner of the rectangle.</param>
            /// <param name="width">The width of the rectangle.</param>
            /// <param name="height">The height of the rectangle.</param>
            /// <param name="selected">A value indicating if the rectangle is seleted (checked) or not.</param>
            /// <param name="color">A color of the the rectangle.</param>
            public CheckRect(int x, int y, int width, int height, bool selected, Color color)
            {
                Rectangle = new Rectangle(x, y, width, height);
                BoundRectangle = new Rectangle(x - 1, y - 1, width + 1, height + 1);
                Selected = selected;
                Color = color;
                Floating = false;
            }

            /// <summary>
            /// An area where the CheckRect should be drawn.
            /// </summary>
            public Rectangle Rectangle { get; set; }

            /// <summary>
            /// An area where the selected or floating indicator rectangle should be drawn.
            /// </summary>
            public Rectangle BoundRectangle { get; set; }

            /// <summary>
            /// Gets or sets the value if CheckRect class instance selected/checked.
            /// </summary>
            public bool Selected { get; set; }

            /// <summary>
            /// Gets or sets the color to be used to draw the rectangle.
            /// </summary>
            public Color Color { get; set; }

            /// <summary>
            /// Gets or set a value indicating if mouse cursor is floating over the rectangle.
            /// </summary>
            public bool Floating { get; set; }
        }

        /// <summary>
        /// An internal extension class to Rectangle. An instance of this class is used to draw either filled or not-filled rectangles on the control.
        /// </summary>
        internal class ColorRect
        {
            /// <summary>
            /// Initializes a new instance of the ColorRect class.
            /// </summary>
            /// <param name="x">The x-coordinate of the upper-left corner of the rectangle.</param>
            /// <param name="y">The y-coordinate of the upper-left corner of the rectangle.</param>
            /// <param name="width">The width of the rectangle.</param>
            /// <param name="height">The height of the rectangle.</param>
            /// <param name="color">A color of the the rectangle.</param>
            public ColorRect(int x, int y, int width, int height, Color color)
            {
                Rectangle = new Rectangle(x, y, width, height);
                Color = color;
            }

            /// <summary>
            /// An area where the ColorRect should be drawn.
            /// </summary>
            public Rectangle Rectangle { get; set; }

            /// <summary>
            /// Gets or sets the color to be used to draw the rectangle.
            /// </summary>
            public Color Color { get; set; }
        }

        /// <summary>
        /// Raises the Paint event.
        /// </summary>
        /// <param name="e">A PaintEventArgs that contains the event data.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e); // First call the base class OnPaint methdod

            // Draw background rectangles
            for (int i = 0; i < backRects.Count; i++)
            {
                e.Graphics.FillRectangle(new SolidBrush(backRects[i].Color), backRects[i].Rectangle);
            }

            // Draw the color buttons
            foreach (CheckRect rect in colorButtons)
            {
                e.Graphics.FillRectangle(new SolidBrush(rect.Color), rect.Rectangle);
            }

            // Highlight selected or floating(mouse cursor is over the button) color buttons
            foreach (CheckRect rect in colorButtons)
            {
                if (rect.Selected || rect.Floating)
                {
                    e.Graphics.DrawRectangle(new Pen(SystemColors.MenuHighlight), rect.BoundRectangle);
                }
            }

            // Draw foreground rectangles
            for (int i = 0; i < foreRects.Count; i++)
            {
                e.Graphics.FillRectangle(new SolidBrush(foreRects[i].Color), foreRects[i].Rectangle);
            }

            // Draw bounding rectangles
            for (int i = 0; i < boundRects.Count; i++)
            {
                e.Graphics.DrawRectangle(new Pen(boundRects[i].Color), boundRects[i].Rectangle);
            }
        }

        /// <summary>
        /// The text used for automatic color button.
        /// </summary>
        private string _AutomaticColorText = "Automatic";

        /// <summary>
        /// Gets or sets the text used for automatic color button.
        /// </summary>
        public string AutomaticColorText
        {
            get
            {
                return _AutomaticColorText;
            }

            set
            {
                _AutomaticColorText = value;
                cbDefaultColor.Text = value;
            }
        }

        /// <summary>
        /// The text used for more colors button.
        /// </summary>
        private string _MoreColorsText = "More colors...";

        /// <summary>
        /// Gets or sets the text used for more colors button.
        /// </summary>
        public string MoreColorsText
        {
            get
            {
                return _MoreColorsText;
            }

            set
            {
                _MoreColorsText = value;
                btSelectCustomColor.Text = value;
            }
        }


        /// <summary>
        /// Gets or set the value of the selected color.
        /// </summary>
        [Browsable(false)]
        public Color SelectedColor 
        {
            get
            {
                return _SelectedColor;
            }

            set
            {
                _SelectedColor = value;

                // first disable selected state for all color buttons
                for (int i = 0; i < colorButtons.Count; i++)
                {
                    if (colorButtons[i].Selected)
                    {
                        Invalidate();
                        colorButtons[i].Selected = false;
                    }
                }

                // Disable the checked state of the default color button
                cbDefaultColor.Checked = false;

                // Check the default button if the given value is a default color.
                if (Background && value == SystemColors.Window)
                {
                    cbDefaultColor.Checked = true;
                    return;
                }
                else if (!Background && value == SystemColors.WindowText)
                {
                    cbDefaultColor.Checked = true;
                    return;
                }

                // enable the selected state for color button if it matches the given color value
                for (int i = 0; i < colorButtons.Count; i++)
                {
                    if (colorButtons[i].Color.ToArgb() == value.ToArgb())
                    {
                        Invalidate(); // Re-draw the control
                        colorButtons[i].Selected = true;
                    }
                }
            }
        }

        /// <summary>
        /// The value of the selected color.
        /// </summary>
        private Color _SelectedColor;

        /// <summary>
        /// A delegate for the ColorChanged event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">ColorChangeEventArgs class instance.</param>
        public delegate void OnColorChanged(object sender, ColorChangeEventArgs e);

        /// <summary>
        /// An event that is raised if the user changes the SelectedColor property of the ToolStripColorSelect class instance.
        /// </summary>
        public event OnColorChanged ColorChanged;
    }

    /// <summary>
    /// A class which instance is passed with the ToolStripColorSelect.ColorChanged event.
    /// </summary>
    public class ColorChangeEventArgs: EventArgs
    {
        /// <summary>
        /// The selected color value.
        /// </summary>
        private Color _color;

        /// <summary>
        /// Initializes a new instance of the ColorChangeEventArgs class.
        /// </summary>
        /// <param name="color">A selected color value.</param>
        public ColorChangeEventArgs(Color color): base()
        {
            _color = color;
        }

        /// <summary>
        /// Gets the selected color value.
        /// </summary>
        public Color Color
        {
            get
            {
                return _color;
            }
        }
    }
}
