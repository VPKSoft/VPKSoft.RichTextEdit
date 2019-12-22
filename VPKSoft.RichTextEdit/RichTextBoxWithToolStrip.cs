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
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using VPKSoft.TextEditIcons;
using System.IO;

namespace VPKSoft.RichTextEdit
{
    /// <summary>
    /// A Windows Forms control with a text edit ToolStrip and a RichTextEdit
    /// </summary>
    [ToolboxItem(true)]
    public partial class RichTextBoxWithToolStrip : Panel
    {
        /// <summary>
        /// Initializes a new instance of the VPKSoft.RichTextEdit class.
        /// </summary>
        public RichTextBoxWithToolStrip()
        {
            InitializeComponent();

            SuspendLayout();

            // The main component creation
            ts = new ToolStrip {Dock = DockStyle.Top};

            rb = new RichTextBoxExtended
            {
                Dock = DockStyle.Fill,
                SelectionBackColor = SystemColors.Window,
                SelectionColor = SystemColors.WindowText,
                AcceptsTab = true
            };
            // The default is to accept a Tab key
            rb.LinkClicked += rb_LinkClicked; // start a browser or whatever when a hyperlink is clicked

            components.Add(ts);
            components.Add(rb);

            Controls.Add(rb); // the order keeps the right layout !!
            Controls.Add(ts);

            ResumeLayout(false);
            PerformLayout();
            rb.SelectionChanged += rb_SelectionChanged;
            rb.HideSelection = false;
            InitToolsStrip(); // Create the ToolStrip buttons and other ToolStrip objects
        }

        [SuppressMessage("ReSharper", "IdentifierTypo")]
        protected override CreateParams CreateParams
        {
            get
            {
                // ReSharper disable once InconsistentNaming
                const int WS_EX_COMPOSITED = 0x02000000;
                var cp = base.CreateParams;
                cp.ExStyle |= WS_EX_COMPOSITED;
                return cp;
            }
        }

        /// <summary>
        /// Try to start a process with the clicked link text.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">LinkClickedEventArgs class instance.</param>
        void rb_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(e.LinkText);
            }
            catch
            {
                // ignored
            }
        }

        /// <summary>
        /// Occurs when the internal RichTextBox selection is changed.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">EventArgs class instance.</param>
        void rb_SelectionChanged(object sender, EventArgs e)
        {
            RefreshButtons();  // Refresh the ToolStrip's buttons and other ToolStrip objects to match the selection in the internal RichTextBox
            SelectionChanged?.Invoke(this,
                new SelectionChangedEventArgs(rb.GetLineFromCharIndex(rb.SelectionStart),
                    rb.SelectionStart - rb.GetFirstCharIndexOfCurrentLine(), rb.SelectionLength));
        }

        /// <summary>
        /// Gets or set the value indicating whether the word wrap is enabled in the internal RichTextBox.
        /// </summary>
        [Category("Misc"), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Description("Indicates whether the internal RichTextBox automatically wraps words to the beginning of the next line when necessary")]
        public bool WordWrap
        {
            get => rb.WordWrap;

            set => rb.WordWrap = value;
        }

        /// <summary>
        /// Gets or sets a value indicating if tab characters are accepted as input for the internal RichTextBox.
        /// </summary>
        [Category("Behavior"), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Description("Indicates if tab characters are accepted as input for the internal RichTextBox.")]
        public bool AcceptsTab
        {
            get => rb.AcceptsTab;

            set => rb.AcceptsTab = value;
        }


        #region LoadSave

        /// <summary>
        /// Loads a rich text format (RTF) or standard ASCII text file into the System.Windows.Forms.RichTextBox control.
        /// </summary>
        /// <param name="path">The name and location of the file to load into the control.</param>
        public void Load(string path)
        {
            Load(path, RichTextBoxStreamType.RichText);
        }

        /// <summary>
        /// Loads a specific type of file into the System.Windows.Forms.RichTextBox control.
        /// </summary>
        /// <param name="path">The name and location of the file to load into the control.</param>
        /// <param name="fileType">One of the System.Windows.Forms.RichTextBoxStreamType values.</param>
        public void Load(string path, RichTextBoxStreamType fileType)
        {
            Clear();
            rb.LoadFile(path, fileType);
            ContentsChanged = false;
            RefreshButtons();
        }

        /// <summary>
        /// Loads a specific type of file into the System.Windows.Forms.RichTextBox control.
        /// </summary>
        /// <param name="data">A stream of data to load into the System.Windows.Forms.RichTextBox control.</param>
        /// <param name="fileType">One of the System.Windows.Forms.RichTextBoxStreamType values.</param>
        public void Load(Stream data, RichTextBoxStreamType fileType)
        {
            Clear();
            rb.LoadFile(data, fileType);
            ContentsChanged = false;
            RefreshButtons();
        }

        /// <summary>
        /// Saves the contents of the System.Windows.Forms.RichTextBox to a rich text format (RTF) file.
        /// </summary>
        /// <param name="path">The name and location of the file to save.</param>
        public void Save(string path)
        {
            Save(path, RichTextBoxStreamType.RichText);
            ContentsChanged = false;
        }

        /// <summary>
        /// Saves the contents of the System.Windows.Forms.RichTextBox to a specific type of file.
        /// </summary>
        /// <param name="path">The name and location of the file to save.</param>
        /// <param name="fileType">One of the System.Windows.Forms.RichTextBoxStreamType values.</param>
        public void Save(string path, RichTextBoxStreamType fileType)
        {
            rb.SaveFile(path, fileType);
            ContentsChanged = false;
        }

        /// <summary>
        /// Saves the contents of a System.Windows.Forms.RichTextBox control to an open data stream.
        /// </summary>
        /// <param name="data">The data stream that contains the file to save to.</param>
        /// <param name="fileType">One of the System.Windows.Forms.RichTextBoxStreamType values.</param>
        public void Save(Stream data, RichTextBoxStreamType fileType)
        {
            rb.SaveFile(data, fileType);
            ContentsChanged = false;
        }

        #endregion

        /// <summary>
        /// Clears the contents of the internal RichTextBox, it's undo buffer and sets the ContentsChanged property to false.
        /// </summary>
        public void Clear()
        {
            rb.Clear();
            rb.ClearUndo();
            ContentsChanged = false;
            RefreshButtons();
        }

        /// <summary>
        /// Processes a command key.
        /// </summary>
        /// <param name="msg">A Message, passed by reference, that represents the window message to process.</param>
        /// <param name="keyData">One of the Keys values that represents the key to process.</param>
        /// <returns>true if the character was processed by the control; otherwise, false.</returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys)Shortcut.CtrlB) // process the bold, underline and italic shortcut keys
            {
                tsbBold.PerformClick();
                return true;
            }
            else if (keyData == (Keys)Shortcut.CtrlI)
            {
                tsItalic.PerformClick();
                return true;
            }
            else if (keyData == (Keys)Shortcut.CtrlU)
            {
                tsbUnderline.PerformClick();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        /// <summary>
        /// Refreshes the ToolStrip buttons and other ToolStrip objects to match the selection in the internal RichTextBox.
        /// </summary>
        private void RefreshButtons()
        {
            if (SelectionMultiFont) // Some Selection* propertied of the RichTextBox will have a null value.
            {
                cmbFont.FontFamily = null;
                cmbFontSize.FontSize = null;

                tsbBold.Checked = false;
                tsItalic.Checked = false;
                tsbUnderline.Checked = false;
                tsbStrikeThrough.Checked = false;
            }
            else // The SelectionFont has always a non-null value
            {
                cmbFont.FontFamily = SelectionFont.FontFamily;
                cmbFontSize.FontSize = SelectionFont.SizeInPoints;

                tsbBold.Checked = SelectionFont.Style.HasFlag(FontStyle.Bold);
                tsItalic.Checked = SelectionFont.Style.HasFlag(FontStyle.Italic);
                tsbUnderline.Checked = SelectionFont.Style.HasFlag(FontStyle.Underline);
                tsbStrikeThrough.Checked = SelectionFont.Style.HasFlag(FontStyle.Strikeout);
            }

            // Some Selection* properties of the RichTextBox will always have a value..
            tsbBulletList.Checked = rb.SelectionBullet;

            switch (rb.SelectionAlignment)
            {
                case HorizontalAlignment.Center:
                    tsbLeftAlign.Checked = false;
                    tsbCenter.Checked = true;
                    tsbRightAlign.Checked = false;
                    break;
                case HorizontalAlignment.Left:
                    tsbLeftAlign.Checked = true;
                    tsbCenter.Checked = false;
                    tsbRightAlign.Checked = false;
                    break;
                case HorizontalAlignment.Right:
                    tsbLeftAlign.Checked = false;
                    tsbCenter.Checked = false;
                    tsbRightAlign.Checked = true;
                    break;
            }

            tsbSuperscript.Checked = rb.SelectionCharOffset > 0;
            tsbSubscript.Checked = rb.SelectionCharOffset < 0;
        }

        /// <summary>
        /// Creates or recreates all the controls in the internal ToolStrip. Also all event handlers are attached. 
        /// <para/>The ToolStrip buttons and other ToolStrip objects are also matched to the selection of the internal RichTextBox.
        /// </summary>
        private void InitToolsStrip()
        {
            RemoveHandlers();
            ts.Items.Clear();
            tsbBold = new ToolStripButton(IconsTextEdit.BoldIcon(ColorButtonForeground));
            tsItalic = new ToolStripButton(IconsTextEdit.ItalicIcon(ColorButtonForeground));
            tsbUnderline = new ToolStripButton(IconsTextEdit.UnderlineIcon(ColorButtonForeground));
            tsbStrikeThrough = new ToolStripButton(IconsTextEdit.StrikeThroughIcon(ColorButtonForeground));
            tsbBold.Tag = FontStyle.Bold;
            tsItalic.Tag = FontStyle.Italic;
            tsbUnderline.Tag = FontStyle.Underline;
            tsbStrikeThrough.Tag = FontStyle.Strikeout;

            tsbSuperscript = new ToolStripButton(IconsTextEdit.SuperscriptIcon(ColorButtonForeground, ColorGlyph));
            tsbSubscript = new ToolStripButton(IconsTextEdit.SubscriptIcon(ColorButtonForeground, ColorGlyph));
            tsbHighlight = new ToolStripSplitButton(IconsTextEdit.HighlightIcon(ColorButtonForeground, SelectionBackColor));
            tsbTextColor = new ToolStripSplitButton(IconsTextEdit.TextColorIcon(ColorGlyph, SelectionColor));


            tsbHighlightDrop = new ToolStripColorSelect(true);
            tsbTextColorDrop = new ToolStripColorSelect(false);

            tsbHighlight.DropDownItems.Add(new ToolStripControlHost(tsbHighlightDrop));
            tsbTextColor.DropDownItems.Add(new ToolStripControlHost(tsbTextColorDrop));

            tsbLeftAlign = new ToolStripButton(IconsTextEdit.LeftAlignIcon(ColorButtonForeground));
            tsbCenter = new ToolStripButton(IconsTextEdit.CenterIcon(ColorButtonForeground));
            tsbRightAlign = new ToolStripButton(IconsTextEdit.RightAlignIcon(ColorButtonForeground));

            tsbBulletList = new ToolStripButton(IconsTextEdit.BulletListIcon(ColorButtonForeground, ColorGlyph));

            tsbDecreaseIndent = new ToolStripButton(IconsTextEdit.DecreaseIndentIcon(ColorButtonForeground, ColorGlyph));
            tsbIncreaseIndent = new ToolStripButton(IconsTextEdit.IncreaseIndentIcon(ColorButtonForeground, ColorGlyph));

            tsbDecreaseFontSize = new ToolStripButton(IconsTextEdit.DecreaseFontSizeIcon(ColorButtonForeground, ColorGlyph));
            tsbIncreaseFontSize = new ToolStripButton(IconsTextEdit.IncreaseFontSizeIcon(ColorButtonForeground, ColorGlyph));
            tsbInsertImage = new ToolStripButton(IconsTextEdit.InsertImageIcon());
            tsbUndo = new ToolStripButton(IconsTextEdit.UndoIcon(ColorButtonForeground));
            tsbRedo = new ToolStripButton(IconsTextEdit.RedoIcon(ColorButtonForeground));


            cmbFontSize = new ToolStripFontSizeComboBox();
            cmbFont = new ToolStripFontComboBox();

            ts.Items.Add(tsbBold);
            ts.Items.Add(tsItalic);
            ts.Items.Add(tsbUnderline);
            ts.Items.Add(tsbStrikeThrough);

            ts.Items.Add(tsbSuperscript);
            ts.Items.Add(tsbSubscript);
            ts.Items.Add(tsbHighlight);
            ts.Items.Add(tsbTextColor);
            ts.Items.Add(cmbFont);
            ts.Items.Add(new ToolStripSeparator());

            ts.Items.Add(tsbLeftAlign);
            ts.Items.Add(tsbCenter);
            ts.Items.Add(tsbRightAlign);
            ts.Items.Add(new ToolStripSeparator());

            ts.Items.Add(tsbBulletList);
            ts.Items.Add(new ToolStripSeparator());

            ts.Items.Add(tsbDecreaseIndent);
            ts.Items.Add(tsbIncreaseIndent);
            ts.Items.Add(new ToolStripSeparator());

            ts.Items.Add(tsbDecreaseFontSize);
            ts.Items.Add(tsbIncreaseFontSize);

            ts.Items.Add(cmbFontSize);
            cmbFontSize.Width = 50;

            ts.Items.Add(new ToolStripSeparator());

            ts.Items.Add(tsbInsertImage);

            ts.Items.Add(new ToolStripSeparator());

            ts.Items.Add(tsbUndo);
            ts.Items.Add(tsbRedo);

            tsbUndo.Enabled = rb.CanUndo;
            tsbRedo.Enabled = rb.CanRedo;

            AddHandlers();
            RefreshButtons();
        }

        /// <summary>
        /// The internal ToolsTrip control contains two ToolStripColorSelect components as a drop-down for the color of the selected text.
        /// <para/>This property gets or sets the text used for "Automatic" text.
        /// </summary>
        [Category("TextProps"), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Description("A ToolStrip color selection drop down text for \"Automatic\"")]
        public string AutomaticColorText
        {
            get => tsbHighlightDrop.AutomaticColorText;

            set
            {
                tsbHighlightDrop.AutomaticColorText = value;
                tsbTextColorDrop.AutomaticColorText = value;
            }
        }

        /// <summary>
        /// The internal ToolsTrip control contains two ToolStripColorSelect components as a drop-down for the color of the selected text.
        /// <para/>This property gets or sets the text used for "More colors..." text.
        /// </summary>
        [Category("TextProps"), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Description("A ToolStrip color selection drop down text for \"More colors...\"")]
        public string MoreColorsText
        {
            get => tsbHighlightDrop.MoreColorsText;

            set
            {
                tsbHighlightDrop.MoreColorsText = value;
                tsbTextColorDrop.MoreColorsText = value;
            }
        }

        #region HandlersAttach

        /// <summary>
        /// Removes all the internally attached handlers from the control.
        /// </summary>
        private void RemoveHandlers()
        {
            tsbBold.Click -= tsbFontStyle_Click;
            tsItalic.Click -= tsbFontStyle_Click;
            tsbUnderline.Click -= tsbFontStyle_Click;
            tsbStrikeThrough.Click -= tsbFontStyle_Click;
            cmbFont.FontFamilyChanged -= cmbFont_FontFamilyChanged;
            cmbFontSize.FontSizeChanged -= cmbFontSize_FontSizeChanged;
            tsbHighlightDrop.ColorChanged -= tsbHighlightDrop_ColorChanged;
            tsbTextColorDrop.ColorChanged -= tsbTextColorDrop_ColorChanged;
            tsbTextColor.Click -= tsbTextColor_Click;
            tsbHighlight.Click -= tsbHighlight_Click;
            tsbInsertImage.Click -= tsbInsertImage_Click;
            tsbLeftAlign.Click -= tsbAlign_Click;
            tsbRightAlign.Click -= tsbAlign_Click;
            tsbCenter.Click -= tsbAlign_Click;
            tsbBulletList.Click -= tsbBulletList_Click;
            tsbIncreaseIndent.Click -= tsbIncreaseIndent_Click;
            tsbDecreaseIndent.Click -= tsbDecreaseIndent_Click;
            tsbIncreaseFontSize.Click -= tsbIncDecFontSize_Click;
            tsbDecreaseFontSize.Click -= tsbIncDecFontSize_Click;
            tsbSubscript.Click -= tsbSubscript_Click;
            tsbSuperscript.Click -= tsbSuperscript_Click;
            rb.TextChanged -= rb_TextChanged;
            tsbUndo.Click -= tsbUndo_Click;
            tsbRedo.Click -= tsbRedo_Click;
        }

        /// <summary>
        /// Attaches all the internal handlers to the control.
        /// </summary>
        private void AddHandlers()
        {
            tsbBold.Click += tsbFontStyle_Click;
            tsItalic.Click += tsbFontStyle_Click;
            tsbUnderline.Click += tsbFontStyle_Click;
            tsbStrikeThrough.Click += tsbFontStyle_Click;
            cmbFont.FontFamilyChanged += cmbFont_FontFamilyChanged;
            cmbFontSize.FontSizeChanged += cmbFontSize_FontSizeChanged;
            tsbHighlightDrop.ColorChanged += tsbHighlightDrop_ColorChanged;
            tsbTextColorDrop.ColorChanged += tsbTextColorDrop_ColorChanged;
            tsbTextColor.Click += tsbTextColor_Click;
            tsbHighlight.Click += tsbHighlight_Click;
            tsbInsertImage.Click += tsbInsertImage_Click;
            tsbLeftAlign.Click += tsbAlign_Click;
            tsbRightAlign.Click += tsbAlign_Click;
            tsbCenter.Click += tsbAlign_Click;
            tsbBulletList.Click += tsbBulletList_Click;
            tsbIncreaseIndent.Click += tsbIncreaseIndent_Click;
            tsbDecreaseIndent.Click += tsbDecreaseIndent_Click;
            tsbIncreaseFontSize.Click += tsbIncDecFontSize_Click;
            tsbDecreaseFontSize.Click += tsbIncDecFontSize_Click;
            tsbSubscript.Click += tsbSubscript_Click;
            tsbSuperscript.Click += tsbSuperscript_Click;
            rb.TextChanged += rb_TextChanged;
            tsbUndo.Click += tsbUndo_Click;
            tsbRedo.Click += tsbRedo_Click;
        }

        #endregion


        #region Handers

        /// <summary>
        /// When the text is changed in the RichTextBox control also it's Undo and Redo capabilities change.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">EventArgs class instance.</param>
        void rb_TextChanged(object sender, EventArgs e)
        {
            tsbUndo.Enabled = rb.CanUndo;
            tsbRedo.Enabled = rb.CanRedo;
            ContentsChanged = true;
            TextChanged?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Gets a value indicating if the contents of the internal RichTextBox have been changed.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ContentsChanged { get; private set; }

        /// <summary>
        /// If the RichTextBox can redo, Redo() is called and the Redo and Undo button states are refreshed.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">EventArgs class instance.</param>
        void tsbRedo_Click(object sender, EventArgs e)
        {
            if (rb.CanRedo)
            {
                rb.Redo();
                tsbUndo.Enabled = rb.CanUndo;
                tsbRedo.Enabled = rb.CanRedo;
            }
        }

        /// <summary>
        /// If the RichTextBox can undo, Undo() is called and the Redo and Undo button states are refreshed.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">EventArgs class instance.</param>
        void tsbUndo_Click(object sender, EventArgs e)
        {
            if (rb.CanUndo)
            {
                rb.Undo();
                tsbUndo.Enabled = rb.CanUndo;
                tsbRedo.Enabled = rb.CanRedo;
            }
        }

        /// <summary>
        /// Sets the selected text as superscript changing the RichTextBox.SelectionCharOffset property to half of the font size.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">EventArgs class instance.</param>
        void tsbSuperscript_Click(object sender, EventArgs e)
        {
            rb.SelectionCharOffset = (int)(SelectionFont.Size / 2);
            RefreshButtons();
        }

        /// <summary>
        /// Sets the selected text as subscript changing the RichTextBox.SelectionCharOffset property to negative half of the font size.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">EventArgs class instance.</param>
        void tsbSubscript_Click(object sender, EventArgs e)
        {
            rb.SelectionCharOffset = -(int)(SelectionFont.Size / 2);
            RefreshButtons();
        }

        /// <summary>
        /// Increases or decreases the selection font size by one point depending on the <paramref name="sender"/>.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">EventArgs class instance.</param>
        void tsbIncDecFontSize_Click(object sender, EventArgs e)
        {
            if ((SelectionFont.Size > 1 && sender == tsbDecreaseFontSize) ||
                (sender == tsbIncreaseFontSize))
            {
                rb.SelectionFont = new Font(SelectionFont.FontFamily, SelectionFont.Size + ((sender == tsbIncreaseFontSize) ? 1.0F : -1.0F), SelectionFont.Style);
            }
            RefreshButtons(); 
        }

        /// <summary>
        /// Increases the ident of line in the RichTextBox by 10 pixels.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">EventArgs class instance.</param>
        void tsbIncreaseIndent_Click(object sender, EventArgs e)
        {
            rb.SelectionIndent += 10;
        }

        /// <summary>
        /// Decreases the ident of line in the RichTextBox by 10 pixels.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">EventArgs class instance.</param>
        void tsbDecreaseIndent_Click(object sender, EventArgs e)
        {
            rb.SelectionIndent -= 10;
        }

        /// <summary>
        /// Toggles the state of bullet-ed list in the RichTextBox selection.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">EventArgs class instance.</param>
        void tsbBulletList_Click(object sender, EventArgs e)
        {
            rb.SelectionBullet = !rb.SelectionBullet;
            tsbBulletList.Checked = rb.SelectionBullet;
        }

        /// <summary>
        /// Selects the alignment of the selected text in the RichTextBox depending on the <paramref name="sender"/> instance.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">EventArgs class instance.</param>
        void tsbAlign_Click(object sender, EventArgs e)
        {
            if (sender == tsbLeftAlign)
            {
                rb.SelectionAlignment = HorizontalAlignment.Left;
            }
            else if (sender == tsbCenter)
            {
                rb.SelectionAlignment = HorizontalAlignment.Center;
            }
            else if (sender == tsbRightAlign)
            {
                rb.SelectionAlignment = HorizontalAlignment.Right;
            }
            RefreshButtons();
        }


        /// <summary>
        /// Gets or sets the current file name filter string, which determines the choices
        /// <para/>that appear in the "Save as file type" or "Files of type" box in the dialog box.
        /// <para/>This is used in the internal OpenFileDialog for inserting images to the document.
        /// </summary>
        [Category("Misc"), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Description("A filter for inserting images to the rich text")]
        public string ImageFilter { get; set; } = "Images files|*.bmp;*.png;*.jpg;*.jpeg|Bitmaps|*.bmp|Portable Network Graphics|*.png|Jpeg|*.jpg;*.jpeg";

        /// <summary>
        /// Inserts an image (bmp, png, jpg or jpeg) to the RichTextBox.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">EventArgs class instance.</param>
        void tsbInsertImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = ImageFilter;
                if (ofd.ShowDialog(rb.FindForm()) == DialogResult.OK)
                {
                    object clipBoardSave = Clipboard.GetDataObject();
                    Clipboard.SetImage(Image.FromFile(ofd.FileName));
                    rb.Paste();
                    if (clipBoardSave != null)
                    {
                        Clipboard.SetDataObject(clipBoardSave);
                    }
                }
            }
        }

        /// <summary>
        /// Toggles the selection font style based on the <paramref name="sender"/> tag which is a font style.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">EventArgs class instance.</param>
        void tsbFontStyle_Click(object sender, EventArgs e)
        {
            ToolStripButton tsb = (ToolStripButton)sender;
            tsb.Checked = !tsb.Checked;
            rb.SelectionFont = tsb.Checked
                ? new Font(SelectionFont, SelectionFont.Style | (FontStyle) tsb.Tag)
                : new Font(SelectionFont, SelectionFont.Style & ~(FontStyle) tsb.Tag);
            RefreshButtons();
        }

        /// <summary>
        /// Changes the highlight (background) color of the text.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">ColorChangeEventArgs class instance.</param>
        void tsbHighlightDrop_ColorChanged(object sender, ColorChangeEventArgs e)
        {
            rb.SelectionBackColor = e.Color;
            tsbHighlight.HideDropDown();
            tsbHighlight.Image = IconsTextEdit.HighlightIcon(ColorButtonForeground, e.Color);
        }

        /// <summary>
        /// Changes the text color (foreground).
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">ColorChangeEventArgs class instance.</param>
        void tsbTextColorDrop_ColorChanged(object sender, ColorChangeEventArgs e)
        {
            rb.SelectionColor = e.Color;
            tsbTextColor.HideDropDown();
            tsbTextColor.Image = IconsTextEdit.TextColorIcon(ColorGlyph, e.Color);
        }

        /// <summary>
        /// Changes the text color (foreground).
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">EventArgs class instance.</param>
        void tsbTextColor_Click(object sender, EventArgs e)
        {
            rb.SelectionColor = tsbTextColorDrop.SelectedColor;
            tsbTextColor.Image = IconsTextEdit.TextColorIcon(ColorGlyph, tsbTextColorDrop.SelectedColor);
        }

        /// <summary>
        /// Changes the highlight (background) color of the text.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">EventArgs class instance.</param>
        void tsbHighlight_Click(object sender, EventArgs e)
        {
            rb.SelectionBackColor = tsbHighlightDrop.SelectedColor;
            tsbHighlight.Image = IconsTextEdit.HighlightIcon(ColorButtonForeground, tsbHighlightDrop.SelectedColor);
        }

        /// <summary>
        /// Changes the selected text font size.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">FontSizeEventArgs class instance.</param>
        void cmbFontSize_FontSizeChanged(object sender, FontSizeEventArgs e)
        {
            rb.SelectionFont = new Font(SelectionFont.FontFamily, e.FontSize, SelectionFont.Style);
            RefreshButtons();
        }

        /// <summary>
        /// Changes the selected text font family.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">FontFamilyEventArgs class instance.</param>
        void cmbFont_FontFamilyChanged(object sender, FontFamilyEventArgs e)
        {
            rb.SelectionFont = new Font(e.FontFamily, SelectionFont.Size, SelectionFont.Style);
            RefreshButtons();
        }

        #endregion

        /// <summary>
        /// The foreground color used in text edit button icon images.
        /// </summary>
        private Color colorButtonForeground = Color.Black;


        /// <summary>
        /// Gets or set the foreground color used in text edit button icon images.
        /// </summary>
        [Category("Colors"), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Description("A color used for button foreground")]
        public Color ColorButtonForeground
        {
            get => colorButtonForeground;

            set
            {
                colorButtonForeground = value;
                InitToolsStrip();
            }
        }

        /// <summary>
        /// The glyph color used in text edit button icon images.
        /// </summary>
        private Color colorGlyph = Color.Blue;

        /// <summary>
        /// Gets or set the glyph color used in text edit button icon images.
        /// </summary>
        [Category("Colors"), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Description("A color used for button glyphs")]
        public Color ColorGlyph
        {
            get => colorGlyph;

            set
            {
                colorGlyph = value;
                InitToolsStrip();
            }
        }

        /// <summary>
        /// Returns a selection property with a given name of the internal RichTextBox using reflection.
        /// <para/>To avoid null values the entire selection is looped through by selecting individual
        /// <para/>characters. Afterwards the selection is restored to its current state.
        /// </summary>
        /// <param name="typeName">A property name.</param>
        /// <returns>A selection property value with a given name of the internal RichTextBox</returns>
        private object SelectionProperty(string typeName)
        {
            PropertyInfo pi = rb.GetType().GetProperty(typeName);
            if (pi != null)
            {
                object val = pi.GetValue(rb);
                if (val == null)
                {
                    rb.SelectionChanged -= rb_SelectionChanged; // no handling on this point

                    int selStart = rb.SelectionStart;
                    int selEnd = rb.SelectionStart + rb.SelectionLength;
                    int selLen = rb.SelectionLength;

                    rb.SuspendLayout();
                    for (int i = selStart; i < selEnd; i++)
                    {
                        rb.Select(i, 1);
                        val = pi.GetValue(rb);
                        if (val != null)
                        {
                            break;
                        }
                    }
                    rb.Select(selStart, selLen);
                    rb.ResumeLayout();

                    rb.SelectionChanged += rb_SelectionChanged; // restore handling
                    return val;
                }
                else
                {
                    return val;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets or set the selected text background (highlight) color of the internal RichTextBox.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color SelectionBackColor
        {
            get
            {
                Color returnColor = Color.White;
                object propVal = SelectionProperty("SelectionBackColor");
                if (propVal != null)
                {
                    returnColor = (Color)propVal;
                }
                else
                {
                    return returnColor;
                }
                return returnColor;
            }

            set => rb.SelectionBackColor = value;
        }

        /// <summary>
        /// Gets or set the selected text foreground color of the internal RichTextBox.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color SelectionColor
        {
            get
            {
                Color returnColor = Color.Black;
                object propVal = SelectionProperty("SelectionColor");
                if (propVal != null)
                {
                    returnColor = (Color)propVal;
                }
                else
                {
                    return returnColor;
                }
                return returnColor;
            }

            set => rb.SelectionColor = value;
        }

        /// <summary>
        /// Gets or sets selected text font.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Font SelectionFont
        {
            get
            {
                Font returnFont = new Font("Arial", 8);
                object propVal = SelectionProperty("SelectionFont");
                if (propVal != null)
                {
                    returnFont = (Font)propVal;
                }
                else
                {
                    return returnFont;
                }
                return returnFont;
            }

            set => rb.SelectionFont = value;
        }

        /// <summary>
        /// Gets a value indicating if the internal RichTextBox selection has multiple fonts.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool SelectionMultiFont => rb.SelectionFont == null;

        #region ToolStripComponents

        // The components used by the internal ToolStrip
        private ToolStripButton tsbBold = new ToolStripButton();
        private ToolStripButton tsItalic = new ToolStripButton();
        private ToolStripButton tsbUnderline = new ToolStripButton();
        private ToolStripButton tsbStrikeThrough = new ToolStripButton();
        private ToolStripButton tsbSuperscript = new ToolStripButton();
        private ToolStripButton tsbSubscript = new ToolStripButton();
        private ToolStripSplitButton tsbHighlight = new ToolStripSplitButton();
        private ToolStripSplitButton tsbTextColor = new ToolStripSplitButton();

        private ToolStripColorSelect tsbHighlightDrop = new ToolStripColorSelect(true);
        private ToolStripColorSelect tsbTextColorDrop = new ToolStripColorSelect(false);

        private ToolStripButton tsbLeftAlign = new ToolStripButton();
        private ToolStripButton tsbCenter = new ToolStripButton();
        private ToolStripButton tsbRightAlign = new ToolStripButton();

        private ToolStripButton tsbBulletList = new ToolStripButton();
        private ToolStripButton tsbDecreaseIndent = new ToolStripButton();
        private ToolStripButton tsbIncreaseIndent = new ToolStripButton();
        private ToolStripButton tsbDecreaseFontSize = new ToolStripButton();
        private ToolStripButton tsbIncreaseFontSize = new ToolStripButton();
        private ToolStripButton tsbInsertImage = new ToolStripButton();

        private ToolStripFontSizeComboBox cmbFontSize = new ToolStripFontSizeComboBox();
        private ToolStripFontComboBox cmbFont = new ToolStripFontComboBox();
        private ToolStripButton tsbUndo = new ToolStripButton();
        private ToolStripButton tsbRedo = new ToolStripButton();

        #endregion

        #region MainComponents

        // The internal ToolStrip and RichTextBox
        private readonly RichTextBox rb;
        private readonly ToolStrip ts;
        #endregion

        /// <summary>
        /// A delegate for the SelectionChanged event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">SelectionChangedEventArgs class instance.</param>
        public delegate void OnSelectionChanged(object sender, SelectionChangedEventArgs e);

        /// <summary>
        /// An event that is raised when the internal RichTextBox selection is changed.
        /// </summary>
        public event OnSelectionChanged SelectionChanged;

        /// <summary>
        /// A delegate for the TextChanged event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">EventArgs class instance.</param>
        public delegate void OnTextChanged(object sender, EventArgs e);

        /// <summary>
        /// An event that is raised when the internal RichTextBox contents are changed.
        /// </summary>
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public new event OnTextChanged TextChanged;
    }

    /// <summary>
    /// A class which instance is passed with the RichTextBoxWithToolStrip.SelectionChanged event.
    /// </summary>
    public class SelectionChangedEventArgs: EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the SelectionChangedEventArgs class.
        /// </summary>
        /// <param name="line">A selection line index.</param>
        /// <param name="col">A selection start index.</param>
        /// <param name="len">A selection length.</param>
        public SelectionChangedEventArgs(int line, int col, int len)
        {
            Line = line;
            Column = col;
            Length = len;
        }

        /// <summary>
        /// A selection line index.
        /// </summary>
        public int Line { get; set; }

        /// <summary>
        /// A selection start index.
        /// </summary>
        public int Column { get; set; }

        /// <summary>
        /// A selection length.
        /// </summary>
        public int Length { get; set; } 
    }
}
