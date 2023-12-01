namespace MerryTest.testitem
{
    partial class MessageBoxs
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
            this.Message_label = new System.Windows.Forms.Label();
            this.True_button = new System.Windows.Forms.Button();
            this.False_button = new System.Windows.Forms.Button();
            this.BarCode_textBox = new System.Windows.Forms.TextBox();
            this.text_SN = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Message_label
            // 
            this.Message_label.BackColor = System.Drawing.Color.Transparent;
            this.Message_label.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Message_label.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Message_label.Location = new System.Drawing.Point(0, 15);
            this.Message_label.Name = "Message_label";
            this.Message_label.Size = new System.Drawing.Size(493, 162);
            this.Message_label.TabIndex = 5;
            this.Message_label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // True_button
            // 
            this.True_button.Location = new System.Drawing.Point(85, 208);
            this.True_button.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.True_button.Name = "True_button";
            this.True_button.Size = new System.Drawing.Size(89, 42);
            this.True_button.TabIndex = 3;
            this.True_button.Text = "确认 (Y)";
            this.True_button.UseVisualStyleBackColor = true;
            this.True_button.Visible = false;
            this.True_button.Click += new System.EventHandler(this.button1_Click);
            // 
            // False_button
            // 
            this.False_button.Location = new System.Drawing.Point(321, 208);
            this.False_button.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.False_button.Name = "False_button";
            this.False_button.Size = new System.Drawing.Size(89, 42);
            this.False_button.TabIndex = 4;
            this.False_button.Text = "取消 (N)";
            this.False_button.UseVisualStyleBackColor = true;
            this.False_button.Visible = false;
            this.False_button.Click += new System.EventHandler(this.button2_Click);
            // 
            // BarCode_textBox
            // 
            this.BarCode_textBox.Font = new System.Drawing.Font("新細明體", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.BarCode_textBox.Location = new System.Drawing.Point(17, 133);
            this.BarCode_textBox.Margin = new System.Windows.Forms.Padding(4);
            this.BarCode_textBox.Name = "BarCode_textBox";
            this.BarCode_textBox.Size = new System.Drawing.Size(480, 51);
            this.BarCode_textBox.TabIndex = 6;
            this.BarCode_textBox.Visible = false;
            this.BarCode_textBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
            // 
            // text_SN
            // 
            this.text_SN.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.text_SN.Location = new System.Drawing.Point(118, 146);
            this.text_SN.Name = "text_SN";
            this.text_SN.Size = new System.Drawing.Size(344, 31);
            this.text_SN.TabIndex = 7;
            this.text_SN.Visible = false;
            this.text_SN.KeyDown += new System.Windows.Forms.KeyEventHandler(this.text_SN_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("新細明體", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.Location = new System.Drawing.Point(13, 153);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 19);
            this.label1.TabIndex = 8;
            this.label1.Text = "不良SN：";
            this.label1.Visible = false;
            // 
            // MessageBoxs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 264);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.text_SN);
            this.Controls.Add(this.BarCode_textBox);
            this.Controls.Add(this.Message_label);
            this.Controls.Add(this.True_button);
            this.Controls.Add(this.False_button);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "MessageBoxs";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MessageBox";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.messageBox_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MessageBoxs_KeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        internal System.Windows.Forms.TextBox BarCode_textBox;
        internal System.Windows.Forms.Button True_button;
        internal System.Windows.Forms.Button False_button;
        internal System.Windows.Forms.Label Message_label;
        internal System.Windows.Forms.TextBox text_SN;
        internal System.Windows.Forms.Label label1;
    }
}