namespace GeneticSharp.Runner.WinApp
{
    partial class MainForm
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
            this.pcbResult = new System.Windows.Forms.PictureBox();
            this.btnRunGeneration = new System.Windows.Forms.Button();
            this.lblGeneration = new System.Windows.Forms.Label();
            this.btnRunGenerations = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pcbResult)).BeginInit();
            this.SuspendLayout();
            // 
            // pcbResult
            // 
            this.pcbResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pcbResult.Location = new System.Drawing.Point(12, 12);
            this.pcbResult.Name = "pcbResult";
            this.pcbResult.Size = new System.Drawing.Size(377, 313);
            this.pcbResult.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pcbResult.TabIndex = 0;
            this.pcbResult.TabStop = false;
            // 
            // btnRunGeneration
            // 
            this.btnRunGeneration.Location = new System.Drawing.Point(399, 271);
            this.btnRunGeneration.Name = "btnRunGeneration";
            this.btnRunGeneration.Size = new System.Drawing.Size(95, 23);
            this.btnRunGeneration.TabIndex = 1;
            this.btnRunGeneration.Text = "&Run generation";
            this.btnRunGeneration.UseVisualStyleBackColor = true;
            this.btnRunGeneration.Click += new System.EventHandler(this.btnRunGeneration_Click);
            // 
            // lblGeneration
            // 
            this.lblGeneration.AutoSize = true;
            this.lblGeneration.Location = new System.Drawing.Point(396, 13);
            this.lblGeneration.Name = "lblGeneration";
            this.lblGeneration.Size = new System.Drawing.Size(71, 13);
            this.lblGeneration.TabIndex = 2;
            this.lblGeneration.Text = "Generation: 0";
            // 
            // btnRunGenerations
            // 
            this.btnRunGenerations.Location = new System.Drawing.Point(399, 300);
            this.btnRunGenerations.Name = "btnRunGenerations";
            this.btnRunGenerations.Size = new System.Drawing.Size(95, 23);
            this.btnRunGenerations.TabIndex = 3;
            this.btnRunGenerations.Text = "Run &Generations";
            this.btnRunGenerations.UseVisualStyleBackColor = true;
            this.btnRunGenerations.Click += new System.EventHandler(this.btnRunGenerations_Click);
            // 
            // MainForm
            // 
            this.AcceptButton = this.btnRunGeneration;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(505, 335);
            this.Controls.Add(this.btnRunGenerations);
            this.Controls.Add(this.lblGeneration);
            this.Controls.Add(this.btnRunGeneration);
            this.Controls.Add(this.pcbResult);
            this.Name = "MainForm";
            this.Text = "GeneticSharp :: Runner";
            ((System.ComponentModel.ISupportInitialize)(this.pcbResult)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pcbResult;
        private System.Windows.Forms.Button btnRunGeneration;
        private System.Windows.Forms.Label lblGeneration;
        private System.Windows.Forms.Button btnRunGenerations;
    }
}

