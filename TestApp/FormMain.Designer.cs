namespace TestApp
{
    partial class FormMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.richTextBoxWithToolStrip1 = new VPKSoft.RichTextEdit.RichTextBoxWithToolStrip();
            this.SuspendLayout();
            // 
            // richTextBoxWithToolStrip1
            // 
            this.richTextBoxWithToolStrip1.AcceptsTab = true;
            this.richTextBoxWithToolStrip1.AutomaticColorText = "Automatic";
            this.richTextBoxWithToolStrip1.ColorButtonForeground = System.Drawing.Color.Black;
            this.richTextBoxWithToolStrip1.ColorGlyph = System.Drawing.Color.Blue;
            this.richTextBoxWithToolStrip1.ImageFilter = "Images files|*.bmp;*.png;*.jpg;*.jpeg|Bitmaps|*.bmp|Portable Network Graphics|*.p" +
    "ng|Jpeg|*.jpg;*.jpeg";
            this.richTextBoxWithToolStrip1.Location = new System.Drawing.Point(3, 12);
            this.richTextBoxWithToolStrip1.MoreColorsText = "More colors...";
            this.richTextBoxWithToolStrip1.Name = "richTextBoxWithToolStrip1";
            this.richTextBoxWithToolStrip1.Size = new System.Drawing.Size(785, 426);
            this.richTextBoxWithToolStrip1.TabIndex = 0;
            this.richTextBoxWithToolStrip1.WordWrap = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.richTextBoxWithToolStrip1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private VPKSoft.RichTextEdit.RichTextBoxWithToolStrip richTextBoxWithToolStrip1;
    }
}

