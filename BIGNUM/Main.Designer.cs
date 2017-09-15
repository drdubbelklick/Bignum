namespace BIGNUM
{
    partial class Factorization
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
            this.numberToFactorizeLabel = new System.Windows.Forms.Label();
            this.primesListBox = new System.Windows.Forms.ListBox();
            this.startButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.numberToFactorize = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.numberToFactorize)).BeginInit();
            this.SuspendLayout();
            // 
            // numberToFactorizeLabel
            // 
            this.numberToFactorizeLabel.AutoSize = true;
            this.numberToFactorizeLabel.Location = new System.Drawing.Point(11, 19);
            this.numberToFactorizeLabel.Name = "numberToFactorizeLabel";
            this.numberToFactorizeLabel.Size = new System.Drawing.Size(101, 13);
            this.numberToFactorizeLabel.TabIndex = 2;
            this.numberToFactorizeLabel.Text = "Heltal att faktorisera";
            // 
            // primesListBox
            // 
            this.primesListBox.FormattingEnabled = true;
            this.primesListBox.Location = new System.Drawing.Point(10, 119);
            this.primesListBox.Name = "primesListBox";
            this.primesListBox.Size = new System.Drawing.Size(258, 160);
            this.primesListBox.TabIndex = 3;
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(37, 81);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(75, 23);
            this.startButton.TabIndex = 4;
            this.startButton.Text = "Starta";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // stopButton
            // 
            this.stopButton.Location = new System.Drawing.Point(149, 81);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(75, 23);
            this.stopButton.TabIndex = 5;
            this.stopButton.Text = "Stopp!";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // numberToFactorize
            // 
            this.numberToFactorize.Location = new System.Drawing.Point(10, 48);
            this.numberToFactorize.Name = "numberToFactorize";
            this.numberToFactorize.Size = new System.Drawing.Size(257, 20);
            this.numberToFactorize.TabIndex = 6;
            // 
            // Factorization
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 306);
            this.Controls.Add(this.numberToFactorize);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.primesListBox);
            this.Controls.Add(this.numberToFactorizeLabel);
            this.Name = "Factorization";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Factorization";
            ((System.ComponentModel.ISupportInitialize)(this.numberToFactorize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label numberToFactorizeLabel;
        private System.Windows.Forms.ListBox primesListBox;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.NumericUpDown numberToFactorize;
    }
}

