namespace ED_SMR_MLAT_Performance
{
    partial class Form1
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.labelWelcome = new System.Windows.Forms.Label();
            this.labelInformativa = new System.Windows.Forms.Label();
            this.LoadFileButton = new System.Windows.Forms.Button();
            this.AvaluateButton = new System.Windows.Forms.Button();
            this.labelSubWelcome = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AddDGPSButton = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // labelWelcome
            // 
            this.labelWelcome.AutoSize = true;
            this.labelWelcome.Font = new System.Drawing.Font("Century Gothic", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelWelcome.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(44)))), ((int)(((byte)(51)))));
            this.labelWelcome.Location = new System.Drawing.Point(131, 94);
            this.labelWelcome.Name = "labelWelcome";
            this.labelWelcome.Size = new System.Drawing.Size(0, 32);
            this.labelWelcome.TabIndex = 2;
            // 
            // labelInformativa
            // 
            this.labelInformativa.AutoSize = true;
            this.labelInformativa.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelInformativa.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(44)))), ((int)(((byte)(51)))));
            this.labelInformativa.Location = new System.Drawing.Point(242, 109);
            this.labelInformativa.Name = "labelInformativa";
            this.labelInformativa.Size = new System.Drawing.Size(0, 21);
            this.labelInformativa.TabIndex = 5;
            // 
            // LoadFileButton
            // 
            this.LoadFileButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(200)))));
            this.LoadFileButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.LoadFileButton.FlatAppearance.BorderSize = 0;
            this.LoadFileButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(11)))), ((int)(((byte)(100)))), ((int)(((byte)(162)))));
            this.LoadFileButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.LoadFileButton.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LoadFileButton.ForeColor = System.Drawing.Color.White;
            this.LoadFileButton.Image = ((System.Drawing.Image)(resources.GetObject("LoadFileButton.Image")));
            this.LoadFileButton.Location = new System.Drawing.Point(264, 351);
            this.LoadFileButton.Name = "LoadFileButton";
            this.LoadFileButton.Size = new System.Drawing.Size(187, 57);
            this.LoadFileButton.TabIndex = 8;
            this.LoadFileButton.Text = "Load File";
            this.LoadFileButton.UseVisualStyleBackColor = false;
            this.LoadFileButton.Click += new System.EventHandler(this.LoadFileButton_Click);
            // 
            // AvaluateButton
            // 
            this.AvaluateButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(200)))));
            this.AvaluateButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.AvaluateButton.FlatAppearance.BorderSize = 0;
            this.AvaluateButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(11)))), ((int)(((byte)(100)))), ((int)(((byte)(162)))));
            this.AvaluateButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.AvaluateButton.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AvaluateButton.ForeColor = System.Drawing.Color.White;
            this.AvaluateButton.Image = ((System.Drawing.Image)(resources.GetObject("AvaluateButton.Image")));
            this.AvaluateButton.Location = new System.Drawing.Point(409, 352);
            this.AvaluateButton.Name = "AvaluateButton";
            this.AvaluateButton.Size = new System.Drawing.Size(187, 56);
            this.AvaluateButton.TabIndex = 9;
            this.AvaluateButton.Text = "Evaluate";
            this.AvaluateButton.UseVisualStyleBackColor = false;
            this.AvaluateButton.Visible = false;
            this.AvaluateButton.Click += new System.EventHandler(this.AvaluateButton_Click);
            // 
            // labelSubWelcome
            // 
            this.labelSubWelcome.AutoSize = true;
            this.labelSubWelcome.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSubWelcome.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(44)))), ((int)(((byte)(51)))));
            this.labelSubWelcome.Location = new System.Drawing.Point(117, 175);
            this.labelSubWelcome.Name = "labelSubWelcome";
            this.labelSubWelcome.Size = new System.Drawing.Size(0, 21);
            this.labelSubWelcome.TabIndex = 10;
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.Transparent;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.closeToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(715, 24);
            this.menuStrip1.TabIndex = 12;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadFileToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // loadFileToolStripMenuItem
            // 
            this.loadFileToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("loadFileToolStripMenuItem.Image")));
            this.loadFileToolStripMenuItem.Name = "loadFileToolStripMenuItem";
            this.loadFileToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.loadFileToolStripMenuItem.Text = "Load New File";
            this.loadFileToolStripMenuItem.Click += new System.EventHandler(this.LoadFileButton_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // AddDGPSButton
            // 
            this.AddDGPSButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.AddDGPSButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.AddDGPSButton.FlatAppearance.BorderSize = 0;
            this.AddDGPSButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.AddDGPSButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.AddDGPSButton.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AddDGPSButton.ForeColor = System.Drawing.Color.White;
            this.AddDGPSButton.Image = ((System.Drawing.Image)(resources.GetObject("AddDGPSButton.Image")));
            this.AddDGPSButton.Location = new System.Drawing.Point(145, 352);
            this.AddDGPSButton.Name = "AddDGPSButton";
            this.AddDGPSButton.Size = new System.Drawing.Size(187, 56);
            this.AddDGPSButton.TabIndex = 16;
            this.AddDGPSButton.Text = "Add D-GPS";
            this.AddDGPSButton.UseVisualStyleBackColor = false;
            this.AddDGPSButton.Visible = false;
            this.AddDGPSButton.Click += new System.EventHandler(this.AddDGPSButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(715, 473);
            this.Controls.Add(this.AddDGPSButton);
            this.Controls.Add(this.labelSubWelcome);
            this.Controls.Add(this.AvaluateButton);
            this.Controls.Add(this.LoadFileButton);
            this.Controls.Add(this.labelInformativa);
            this.Controls.Add(this.labelWelcome);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ED-MLAT Permormance Evaluator";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label labelWelcome;
        private System.Windows.Forms.Label labelInformativa;
        private System.Windows.Forms.Button LoadFileButton;
        private System.Windows.Forms.Button AvaluateButton;
        private System.Windows.Forms.Label labelSubWelcome;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.Button AddDGPSButton;
    }
}

