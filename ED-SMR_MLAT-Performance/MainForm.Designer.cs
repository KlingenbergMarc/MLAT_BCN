namespace ED_SMR_MLAT_Performance
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle15 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle16 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle17 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle18 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle19 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle20 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle21 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle22 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle23 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle24 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle25 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle26 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle27 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle28 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panelContenedor = new System.Windows.Forms.Panel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.summaryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.discardedVehiclesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panelContenedorResultados = new System.Windows.Forms.Panel();
            this.SaveScatterButton = new System.Windows.Forms.Button();
            this.DGPSresultsButton = new System.Windows.Forms.Button();
            this.ButtonIzquierdaSummary = new System.Windows.Forms.Button();
            this.ButtonDerechaSummay = new System.Windows.Forms.Button();
            this.buttonSaveAllSummary = new System.Windows.Forms.Button();
            this.ParametersGrid = new System.Windows.Forms.DataGridView();
            this.LabelTituloSummary = new System.Windows.Forms.Label();
            this.buttonSaveSummary = new System.Windows.Forms.Button();
            this.recuadroSummary = new System.Windows.Forms.PictureBox();
            this.SummaryGrid = new System.Windows.Forms.DataGridView();
            this.panelContenedorMapa = new System.Windows.Forms.Panel();
            this.buttonHideBackground = new System.Windows.Forms.Button();
            this.ButtonMLAT_DGPS = new System.Windows.Forms.Button();
            this.ButtonDGPS = new System.Windows.Forms.Button();
            this.ButtonZoom = new System.Windows.Forms.Button();
            this.buttonViewSegmentation = new System.Windows.Forms.Button();
            this.labelCursor = new System.Windows.Forms.Label();
            this.ButtonSaveMap = new System.Windows.Forms.Button();
            this.recuadroMapa = new System.Windows.Forms.PictureBox();
            this.panelMap = new System.Windows.Forms.Panel();
            this.panelContenedorDescartados = new System.Windows.Forms.Panel();
            this.ViewWholeButton = new System.Windows.Forms.Button();
            this.AddRemoveDiscardButton = new System.Windows.Forms.Button();
            this.tablaGridNoDescartados = new System.Windows.Forms.DataGridView();
            this.tablaGridDescartados = new System.Windows.Forms.DataGridView();
            this.panelMapDescartados = new System.Windows.Forms.Panel();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.saveFileDialogExcel = new System.Windows.Forms.SaveFileDialog();
            this.saveFileDialogAll = new System.Windows.Forms.SaveFileDialog();
            this.panelContenedor.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.panelContenedorResultados.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ParametersGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.recuadroSummary)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SummaryGrid)).BeginInit();
            this.panelContenedorMapa.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.recuadroMapa)).BeginInit();
            this.panelContenedorDescartados.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tablaGridNoDescartados)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tablaGridDescartados)).BeginInit();
            this.SuspendLayout();
            // 
            // panelContenedor
            // 
            this.panelContenedor.BackColor = System.Drawing.SystemColors.Window;
            this.panelContenedor.Controls.Add(this.menuStrip1);
            this.panelContenedor.Controls.Add(this.panelContenedorResultados);
            this.panelContenedor.Controls.Add(this.panelContenedorMapa);
            this.panelContenedor.Controls.Add(this.panelContenedorDescartados);
            this.panelContenedor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContenedor.Location = new System.Drawing.Point(0, 0);
            this.panelContenedor.Name = "panelContenedor";
            this.panelContenedor.Size = new System.Drawing.Size(1350, 666);
            this.panelContenedor.TabIndex = 0;
            this.panelContenedor.MouseClick += new System.Windows.Forms.MouseEventHandler(this.panelContenedorResultados_MouseClick);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.White;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1350, 24);
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadFileToolStripMenuItem,
            this.saveAllToolStripMenuItem,
            this.closeToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // loadFileToolStripMenuItem
            // 
            this.loadFileToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("loadFileToolStripMenuItem.Image")));
            this.loadFileToolStripMenuItem.Name = "loadFileToolStripMenuItem";
            this.loadFileToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.loadFileToolStripMenuItem.Text = "Load New File";
            this.loadFileToolStripMenuItem.Click += new System.EventHandler(this.loadFileToolStripMenuItem_Click);
            // 
            // saveAllToolStripMenuItem
            // 
            this.saveAllToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveAllToolStripMenuItem.Image")));
            this.saveAllToolStripMenuItem.Name = "saveAllToolStripMenuItem";
            this.saveAllToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.saveAllToolStripMenuItem.Text = "Save All";
            this.saveAllToolStripMenuItem.Click += new System.EventHandler(this.saveAllToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("closeToolStripMenuItem.Image")));
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mapToolStripMenuItem,
            this.summaryToolStripMenuItem,
            this.discardedVehiclesToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // mapToolStripMenuItem
            // 
            this.mapToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("mapToolStripMenuItem.Image")));
            this.mapToolStripMenuItem.Name = "mapToolStripMenuItem";
            this.mapToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.mapToolStripMenuItem.Text = "Map";
            this.mapToolStripMenuItem.Click += new System.EventHandler(this.mapToolStripMenuItem_Click);
            // 
            // summaryToolStripMenuItem
            // 
            this.summaryToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("summaryToolStripMenuItem.Image")));
            this.summaryToolStripMenuItem.Name = "summaryToolStripMenuItem";
            this.summaryToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.summaryToolStripMenuItem.Text = "Summary";
            this.summaryToolStripMenuItem.Click += new System.EventHandler(this.summaryToolStripMenuItem_Click);
            // 
            // discardedVehiclesToolStripMenuItem
            // 
            this.discardedVehiclesToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("discardedVehiclesToolStripMenuItem.Image")));
            this.discardedVehiclesToolStripMenuItem.Name = "discardedVehiclesToolStripMenuItem";
            this.discardedVehiclesToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.discardedVehiclesToolStripMenuItem.Text = "Discarded Vehicles";
            this.discardedVehiclesToolStripMenuItem.Click += new System.EventHandler(this.discardedVehiclesToolStripMenuItem_Click);
            // 
            // panelContenedorResultados
            // 
            this.panelContenedorResultados.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelContenedorResultados.BackColor = System.Drawing.Color.White;
            this.panelContenedorResultados.Controls.Add(this.SaveScatterButton);
            this.panelContenedorResultados.Controls.Add(this.DGPSresultsButton);
            this.panelContenedorResultados.Controls.Add(this.ButtonIzquierdaSummary);
            this.panelContenedorResultados.Controls.Add(this.ButtonDerechaSummay);
            this.panelContenedorResultados.Controls.Add(this.buttonSaveAllSummary);
            this.panelContenedorResultados.Controls.Add(this.ParametersGrid);
            this.panelContenedorResultados.Controls.Add(this.LabelTituloSummary);
            this.panelContenedorResultados.Controls.Add(this.buttonSaveSummary);
            this.panelContenedorResultados.Controls.Add(this.recuadroSummary);
            this.panelContenedorResultados.Controls.Add(this.SummaryGrid);
            this.panelContenedorResultados.Location = new System.Drawing.Point(24, 23);
            this.panelContenedorResultados.Name = "panelContenedorResultados";
            this.panelContenedorResultados.Size = new System.Drawing.Size(1326, 642);
            this.panelContenedorResultados.TabIndex = 7;
            this.panelContenedorResultados.Visible = false;
            this.panelContenedorResultados.MouseClick += new System.Windows.Forms.MouseEventHandler(this.panelContenedorResultados_MouseClick);
            // 
            // SaveScatterButton
            // 
            this.SaveScatterButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.SaveScatterButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(200)))));
            this.SaveScatterButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.SaveScatterButton.FlatAppearance.BorderSize = 0;
            this.SaveScatterButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(11)))), ((int)(((byte)(100)))), ((int)(((byte)(162)))));
            this.SaveScatterButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SaveScatterButton.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SaveScatterButton.ForeColor = System.Drawing.Color.White;
            this.SaveScatterButton.Image = ((System.Drawing.Image)(resources.GetObject("SaveScatterButton.Image")));
            this.SaveScatterButton.Location = new System.Drawing.Point(1128, 410);
            this.SaveScatterButton.Name = "SaveScatterButton";
            this.SaveScatterButton.Size = new System.Drawing.Size(150, 38);
            this.SaveScatterButton.TabIndex = 34;
            this.SaveScatterButton.Text = "Save Scatter";
            this.SaveScatterButton.UseVisualStyleBackColor = false;
            this.SaveScatterButton.Click += new System.EventHandler(this.SaveScatterButton_Click);
            // 
            // DGPSresultsButton
            // 
            this.DGPSresultsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.DGPSresultsButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(200)))));
            this.DGPSresultsButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.DGPSresultsButton.FlatAppearance.BorderSize = 0;
            this.DGPSresultsButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(11)))), ((int)(((byte)(100)))), ((int)(((byte)(162)))));
            this.DGPSresultsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.DGPSresultsButton.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DGPSresultsButton.ForeColor = System.Drawing.Color.White;
            this.DGPSresultsButton.Image = ((System.Drawing.Image)(resources.GetObject("DGPSresultsButton.Image")));
            this.DGPSresultsButton.Location = new System.Drawing.Point(1125, 328);
            this.DGPSresultsButton.Name = "DGPSresultsButton";
            this.DGPSresultsButton.Size = new System.Drawing.Size(155, 55);
            this.DGPSresultsButton.TabIndex = 33;
            this.DGPSresultsButton.Text = "View D-GPS Results";
            this.DGPSresultsButton.UseVisualStyleBackColor = false;
            this.DGPSresultsButton.Click += new System.EventHandler(this.DGPSresultsButton_Click);
            // 
            // ButtonIzquierdaSummary
            // 
            this.ButtonIzquierdaSummary.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonIzquierdaSummary.BackColor = System.Drawing.Color.White;
            this.ButtonIzquierdaSummary.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ButtonIzquierdaSummary.FlatAppearance.BorderSize = 0;
            this.ButtonIzquierdaSummary.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(244)))), ((int)(((byte)(248)))));
            this.ButtonIzquierdaSummary.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ButtonIzquierdaSummary.Font = new System.Drawing.Font("Century Gothic", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ButtonIzquierdaSummary.ForeColor = System.Drawing.SystemColors.ButtonShadow;
            this.ButtonIzquierdaSummary.Image = ((System.Drawing.Image)(resources.GetObject("ButtonIzquierdaSummary.Image")));
            this.ButtonIzquierdaSummary.Location = new System.Drawing.Point(849, 574);
            this.ButtonIzquierdaSummary.Name = "ButtonIzquierdaSummary";
            this.ButtonIzquierdaSummary.Size = new System.Drawing.Size(54, 54);
            this.ButtonIzquierdaSummary.TabIndex = 29;
            this.ButtonIzquierdaSummary.Text = "<";
            this.ButtonIzquierdaSummary.UseVisualStyleBackColor = false;
            this.ButtonIzquierdaSummary.Click += new System.EventHandler(this.ButtonIzquierdaSummary_Click);
            // 
            // ButtonDerechaSummay
            // 
            this.ButtonDerechaSummay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonDerechaSummay.BackColor = System.Drawing.Color.White;
            this.ButtonDerechaSummay.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ButtonDerechaSummay.FlatAppearance.BorderSize = 0;
            this.ButtonDerechaSummay.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(244)))), ((int)(((byte)(248)))));
            this.ButtonDerechaSummay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ButtonDerechaSummay.Font = new System.Drawing.Font("Century Gothic", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ButtonDerechaSummay.ForeColor = System.Drawing.SystemColors.ButtonShadow;
            this.ButtonDerechaSummay.Image = ((System.Drawing.Image)(resources.GetObject("ButtonDerechaSummay.Image")));
            this.ButtonDerechaSummay.Location = new System.Drawing.Point(903, 574);
            this.ButtonDerechaSummay.Name = "ButtonDerechaSummay";
            this.ButtonDerechaSummay.Size = new System.Drawing.Size(54, 54);
            this.ButtonDerechaSummay.TabIndex = 28;
            this.ButtonDerechaSummay.Text = " >";
            this.ButtonDerechaSummay.UseVisualStyleBackColor = false;
            this.ButtonDerechaSummay.Click += new System.EventHandler(this.ButtonDerechaSummay_Click);
            // 
            // buttonSaveAllSummary
            // 
            this.buttonSaveAllSummary.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSaveAllSummary.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(200)))));
            this.buttonSaveAllSummary.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonSaveAllSummary.FlatAppearance.BorderSize = 0;
            this.buttonSaveAllSummary.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(11)))), ((int)(((byte)(100)))), ((int)(((byte)(162)))));
            this.buttonSaveAllSummary.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonSaveAllSummary.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonSaveAllSummary.ForeColor = System.Drawing.Color.White;
            this.buttonSaveAllSummary.Image = ((System.Drawing.Image)(resources.GetObject("buttonSaveAllSummary.Image")));
            this.buttonSaveAllSummary.Location = new System.Drawing.Point(1128, 545);
            this.buttonSaveAllSummary.Name = "buttonSaveAllSummary";
            this.buttonSaveAllSummary.Size = new System.Drawing.Size(150, 39);
            this.buttonSaveAllSummary.TabIndex = 27;
            this.buttonSaveAllSummary.Text = "Save All";
            this.buttonSaveAllSummary.UseVisualStyleBackColor = false;
            this.buttonSaveAllSummary.Click += new System.EventHandler(this.buttonSaveAllSummary_Click);
            // 
            // ParametersGrid
            // 
            this.ParametersGrid.AllowUserToAddRows = false;
            this.ParametersGrid.AllowUserToDeleteRows = false;
            this.ParametersGrid.AllowUserToResizeColumns = false;
            this.ParametersGrid.AllowUserToResizeRows = false;
            this.ParametersGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ParametersGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.ParametersGrid.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(244)))), ((int)(((byte)(248)))));
            this.ParametersGrid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ParametersGrid.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.ParametersGrid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle15.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle15.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle15.Font = new System.Drawing.Font("Century Gothic", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle15.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle15.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle15.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle15.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.ParametersGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle15;
            this.ParametersGrid.ColumnHeadersHeight = 40;
            this.ParametersGrid.ColumnHeadersVisible = false;
            this.ParametersGrid.Cursor = System.Windows.Forms.Cursors.Hand;
            dataGridViewCellStyle16.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle16.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(244)))), ((int)(((byte)(248)))));
            dataGridViewCellStyle16.Font = new System.Drawing.Font("Century Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle16.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle16.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle16.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle16.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ParametersGrid.DefaultCellStyle = dataGridViewCellStyle16;
            this.ParametersGrid.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(228)))), ((int)(((byte)(232)))));
            this.ParametersGrid.Location = new System.Drawing.Point(1072, 143);
            this.ParametersGrid.MultiSelect = false;
            this.ParametersGrid.Name = "ParametersGrid";
            this.ParametersGrid.ReadOnly = true;
            this.ParametersGrid.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle17.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle17.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle17.Font = new System.Drawing.Font("Century Gothic", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle17.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle17.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle17.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle17.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.ParametersGrid.RowHeadersDefaultCellStyle = dataGridViewCellStyle17;
            this.ParametersGrid.RowHeadersVisible = false;
            this.ParametersGrid.RowHeadersWidth = 250;
            this.ParametersGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.ParametersGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.ParametersGrid.ShowCellErrors = false;
            this.ParametersGrid.ShowCellToolTips = false;
            this.ParametersGrid.ShowEditingIcon = false;
            this.ParametersGrid.ShowRowErrors = false;
            this.ParametersGrid.Size = new System.Drawing.Size(249, 137);
            this.ParametersGrid.TabIndex = 26;
            this.ParametersGrid.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.ParametersGrid_CellClick);
            this.ParametersGrid.SelectionChanged += new System.EventHandler(this.ParametersGrid_SelectionChanged);
            // 
            // LabelTituloSummary
            // 
            this.LabelTituloSummary.AutoSize = true;
            this.LabelTituloSummary.Font = new System.Drawing.Font("Century Gothic", 21.75F, ((System.Drawing.FontStyle)(((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic) 
                | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelTituloSummary.Location = new System.Drawing.Point(49, 65);
            this.LabelTituloSummary.Name = "LabelTituloSummary";
            this.LabelTituloSummary.Size = new System.Drawing.Size(199, 36);
            this.LabelTituloSummary.TabIndex = 25;
            this.LabelTituloSummary.Text = "Update Rate:";
            // 
            // buttonSaveSummary
            // 
            this.buttonSaveSummary.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSaveSummary.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(200)))));
            this.buttonSaveSummary.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonSaveSummary.FlatAppearance.BorderSize = 0;
            this.buttonSaveSummary.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(11)))), ((int)(((byte)(100)))), ((int)(((byte)(162)))));
            this.buttonSaveSummary.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonSaveSummary.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonSaveSummary.ForeColor = System.Drawing.Color.White;
            this.buttonSaveSummary.Image = ((System.Drawing.Image)(resources.GetObject("buttonSaveSummary.Image")));
            this.buttonSaveSummary.Location = new System.Drawing.Point(1128, 478);
            this.buttonSaveSummary.Name = "buttonSaveSummary";
            this.buttonSaveSummary.Size = new System.Drawing.Size(150, 38);
            this.buttonSaveSummary.TabIndex = 23;
            this.buttonSaveSummary.Text = "Save";
            this.buttonSaveSummary.UseVisualStyleBackColor = false;
            this.buttonSaveSummary.Click += new System.EventHandler(this.buttonSaveSummary_Click);
            // 
            // recuadroSummary
            // 
            this.recuadroSummary.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.recuadroSummary.Image = ((System.Drawing.Image)(resources.GetObject("recuadroSummary.Image")));
            this.recuadroSummary.Location = new System.Drawing.Point(1067, 1);
            this.recuadroSummary.Name = "recuadroSummary";
            this.recuadroSummary.Size = new System.Drawing.Size(259, 643);
            this.recuadroSummary.TabIndex = 22;
            this.recuadroSummary.TabStop = false;
            // 
            // SummaryGrid
            // 
            this.SummaryGrid.AllowUserToAddRows = false;
            this.SummaryGrid.AllowUserToDeleteRows = false;
            this.SummaryGrid.AllowUserToResizeColumns = false;
            this.SummaryGrid.AllowUserToResizeRows = false;
            this.SummaryGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.SummaryGrid.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllHeaders;
            this.SummaryGrid.BackgroundColor = System.Drawing.Color.White;
            this.SummaryGrid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.SummaryGrid.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.SummaryGrid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle18.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle18.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(244)))), ((int)(((byte)(248)))));
            dataGridViewCellStyle18.Font = new System.Drawing.Font("Century Gothic", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle18.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle18.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle18.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle18.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.SummaryGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle18;
            this.SummaryGrid.ColumnHeadersHeight = 40;
            this.SummaryGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle19.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle19.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle19.Font = new System.Drawing.Font("Century Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle19.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle19.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle19.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle19.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.SummaryGrid.DefaultCellStyle = dataGridViewCellStyle19;
            this.SummaryGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.SummaryGrid.EnableHeadersVisualStyles = false;
            this.SummaryGrid.GridColor = System.Drawing.Color.White;
            this.SummaryGrid.Location = new System.Drawing.Point(55, 149);
            this.SummaryGrid.Name = "SummaryGrid";
            this.SummaryGrid.ReadOnly = true;
            this.SummaryGrid.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle20.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle20.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(244)))), ((int)(((byte)(248)))));
            dataGridViewCellStyle20.Font = new System.Drawing.Font("Century Gothic", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle20.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle20.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle20.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle20.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.SummaryGrid.RowHeadersDefaultCellStyle = dataGridViewCellStyle20;
            this.SummaryGrid.RowHeadersWidth = 250;
            this.SummaryGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.SummaryGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.SummaryGrid.ShowCellErrors = false;
            this.SummaryGrid.ShowCellToolTips = false;
            this.SummaryGrid.ShowEditingIcon = false;
            this.SummaryGrid.ShowRowErrors = false;
            this.SummaryGrid.Size = new System.Drawing.Size(1003, 440);
            this.SummaryGrid.TabIndex = 0;
            this.SummaryGrid.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.SummaryGrid_CellClick);
            this.SummaryGrid.Click += new System.EventHandler(this.SummaryGrid_Click);
            // 
            // panelContenedorMapa
            // 
            this.panelContenedorMapa.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelContenedorMapa.BackColor = System.Drawing.Color.White;
            this.panelContenedorMapa.Controls.Add(this.buttonHideBackground);
            this.panelContenedorMapa.Controls.Add(this.ButtonMLAT_DGPS);
            this.panelContenedorMapa.Controls.Add(this.ButtonDGPS);
            this.panelContenedorMapa.Controls.Add(this.ButtonZoom);
            this.panelContenedorMapa.Controls.Add(this.buttonViewSegmentation);
            this.panelContenedorMapa.Controls.Add(this.labelCursor);
            this.panelContenedorMapa.Controls.Add(this.ButtonSaveMap);
            this.panelContenedorMapa.Controls.Add(this.recuadroMapa);
            this.panelContenedorMapa.Controls.Add(this.panelMap);
            this.panelContenedorMapa.Location = new System.Drawing.Point(24, 23);
            this.panelContenedorMapa.Name = "panelContenedorMapa";
            this.panelContenedorMapa.Size = new System.Drawing.Size(1326, 642);
            this.panelContenedorMapa.TabIndex = 8;
            this.panelContenedorMapa.Visible = false;
            // 
            // buttonHideBackground
            // 
            this.buttonHideBackground.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonHideBackground.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(200)))));
            this.buttonHideBackground.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonHideBackground.FlatAppearance.BorderSize = 0;
            this.buttonHideBackground.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(11)))), ((int)(((byte)(100)))), ((int)(((byte)(162)))));
            this.buttonHideBackground.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonHideBackground.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonHideBackground.ForeColor = System.Drawing.Color.White;
            this.buttonHideBackground.Image = ((System.Drawing.Image)(resources.GetObject("buttonHideBackground.Image")));
            this.buttonHideBackground.Location = new System.Drawing.Point(1126, 105);
            this.buttonHideBackground.Name = "buttonHideBackground";
            this.buttonHideBackground.Size = new System.Drawing.Size(155, 55);
            this.buttonHideBackground.TabIndex = 23;
            this.buttonHideBackground.Text = "Hide Background";
            this.buttonHideBackground.UseVisualStyleBackColor = false;
            this.buttonHideBackground.Click += new System.EventHandler(this.buttonHideBackground_Click_1);
            // 
            // ButtonMLAT_DGPS
            // 
            this.ButtonMLAT_DGPS.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonMLAT_DGPS.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(200)))));
            this.ButtonMLAT_DGPS.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ButtonMLAT_DGPS.FlatAppearance.BorderSize = 0;
            this.ButtonMLAT_DGPS.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(11)))), ((int)(((byte)(100)))), ((int)(((byte)(162)))));
            this.ButtonMLAT_DGPS.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ButtonMLAT_DGPS.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ButtonMLAT_DGPS.ForeColor = System.Drawing.Color.White;
            this.ButtonMLAT_DGPS.Image = ((System.Drawing.Image)(resources.GetObject("ButtonMLAT_DGPS.Image")));
            this.ButtonMLAT_DGPS.Location = new System.Drawing.Point(1129, 379);
            this.ButtonMLAT_DGPS.Name = "ButtonMLAT_DGPS";
            this.ButtonMLAT_DGPS.Size = new System.Drawing.Size(155, 55);
            this.ButtonMLAT_DGPS.TabIndex = 21;
            this.ButtonMLAT_DGPS.Text = "View MLAT + D-GPS";
            this.ButtonMLAT_DGPS.UseVisualStyleBackColor = false;
            this.ButtonMLAT_DGPS.Click += new System.EventHandler(this.ButtonMLAT_DGPS_Click);
            // 
            // ButtonDGPS
            // 
            this.ButtonDGPS.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonDGPS.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(200)))));
            this.ButtonDGPS.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ButtonDGPS.FlatAppearance.BorderSize = 0;
            this.ButtonDGPS.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(11)))), ((int)(((byte)(100)))), ((int)(((byte)(162)))));
            this.ButtonDGPS.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ButtonDGPS.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ButtonDGPS.ForeColor = System.Drawing.Color.White;
            this.ButtonDGPS.Image = ((System.Drawing.Image)(resources.GetObject("ButtonDGPS.Image")));
            this.ButtonDGPS.Location = new System.Drawing.Point(1129, 317);
            this.ButtonDGPS.Name = "ButtonDGPS";
            this.ButtonDGPS.Size = new System.Drawing.Size(155, 40);
            this.ButtonDGPS.TabIndex = 20;
            this.ButtonDGPS.Text = "View only D-GPS";
            this.ButtonDGPS.UseVisualStyleBackColor = false;
            this.ButtonDGPS.Click += new System.EventHandler(this.ButtonDGPS_Click);
            // 
            // ButtonZoom
            // 
            this.ButtonZoom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonZoom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(200)))));
            this.ButtonZoom.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ButtonZoom.FlatAppearance.BorderSize = 0;
            this.ButtonZoom.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(11)))), ((int)(((byte)(100)))), ((int)(((byte)(162)))));
            this.ButtonZoom.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ButtonZoom.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ButtonZoom.ForeColor = System.Drawing.Color.White;
            this.ButtonZoom.Image = ((System.Drawing.Image)(resources.GetObject("ButtonZoom.Image")));
            this.ButtonZoom.Location = new System.Drawing.Point(1129, 252);
            this.ButtonZoom.Name = "ButtonZoom";
            this.ButtonZoom.Size = new System.Drawing.Size(155, 40);
            this.ButtonZoom.TabIndex = 19;
            this.ButtonZoom.Text = "Zoom Out";
            this.ButtonZoom.UseVisualStyleBackColor = false;
            this.ButtonZoom.Click += new System.EventHandler(this.ButtonZoom_Click);
            // 
            // buttonViewSegmentation
            // 
            this.buttonViewSegmentation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonViewSegmentation.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(200)))));
            this.buttonViewSegmentation.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonViewSegmentation.FlatAppearance.BorderSize = 0;
            this.buttonViewSegmentation.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(11)))), ((int)(((byte)(100)))), ((int)(((byte)(162)))));
            this.buttonViewSegmentation.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonViewSegmentation.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonViewSegmentation.ForeColor = System.Drawing.Color.White;
            this.buttonViewSegmentation.Image = ((System.Drawing.Image)(resources.GetObject("buttonViewSegmentation.Image")));
            this.buttonViewSegmentation.Location = new System.Drawing.Point(1129, 172);
            this.buttonViewSegmentation.Name = "buttonViewSegmentation";
            this.buttonViewSegmentation.Size = new System.Drawing.Size(155, 55);
            this.buttonViewSegmentation.TabIndex = 18;
            this.buttonViewSegmentation.Text = "View Segmentation";
            this.buttonViewSegmentation.UseVisualStyleBackColor = false;
            this.buttonViewSegmentation.Click += new System.EventHandler(this.buttonViewSegmentation_Click);
            // 
            // labelCursor
            // 
            this.labelCursor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelCursor.AutoSize = true;
            this.labelCursor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(244)))), ((int)(((byte)(248)))));
            this.labelCursor.Font = new System.Drawing.Font("Century Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCursor.Location = new System.Drawing.Point(1074, 17);
            this.labelCursor.Name = "labelCursor";
            this.labelCursor.Size = new System.Drawing.Size(0, 20);
            this.labelCursor.TabIndex = 11;
            // 
            // ButtonSaveMap
            // 
            this.ButtonSaveMap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonSaveMap.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(200)))));
            this.ButtonSaveMap.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ButtonSaveMap.FlatAppearance.BorderSize = 0;
            this.ButtonSaveMap.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(11)))), ((int)(((byte)(100)))), ((int)(((byte)(162)))));
            this.ButtonSaveMap.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ButtonSaveMap.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ButtonSaveMap.ForeColor = System.Drawing.Color.White;
            this.ButtonSaveMap.Image = ((System.Drawing.Image)(resources.GetObject("ButtonSaveMap.Image")));
            this.ButtonSaveMap.Location = new System.Drawing.Point(1129, 549);
            this.ButtonSaveMap.Name = "ButtonSaveMap";
            this.ButtonSaveMap.Size = new System.Drawing.Size(155, 40);
            this.ButtonSaveMap.TabIndex = 17;
            this.ButtonSaveMap.Text = "Save";
            this.ButtonSaveMap.UseVisualStyleBackColor = false;
            this.ButtonSaveMap.Click += new System.EventHandler(this.ButtonSaveMap_Click);
            // 
            // recuadroMapa
            // 
            this.recuadroMapa.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.recuadroMapa.Image = ((System.Drawing.Image)(resources.GetObject("recuadroMapa.Image")));
            this.recuadroMapa.Location = new System.Drawing.Point(1067, 1);
            this.recuadroMapa.Name = "recuadroMapa";
            this.recuadroMapa.Size = new System.Drawing.Size(259, 643);
            this.recuadroMapa.TabIndex = 17;
            this.recuadroMapa.TabStop = false;
            // 
            // panelMap
            // 
            this.panelMap.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelMap.BackColor = System.Drawing.SystemColors.Window;
            this.panelMap.Location = new System.Drawing.Point(81, 4);
            this.panelMap.Name = "panelMap";
            this.panelMap.Size = new System.Drawing.Size(1144, 627);
            this.panelMap.TabIndex = 10;
            this.panelMap.Paint += new System.Windows.Forms.PaintEventHandler(this.panelMap_Paint);
            this.panelMap.MouseClick += new System.Windows.Forms.MouseEventHandler(this.panelMap_MouseClick);
            this.panelMap.MouseLeave += new System.EventHandler(this.panelMap_MouseLeave);
            this.panelMap.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelMap_MouseMove);
            // 
            // panelContenedorDescartados
            // 
            this.panelContenedorDescartados.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelContenedorDescartados.BackColor = System.Drawing.Color.White;
            this.panelContenedorDescartados.Controls.Add(this.ViewWholeButton);
            this.panelContenedorDescartados.Controls.Add(this.AddRemoveDiscardButton);
            this.panelContenedorDescartados.Controls.Add(this.tablaGridNoDescartados);
            this.panelContenedorDescartados.Controls.Add(this.tablaGridDescartados);
            this.panelContenedorDescartados.Controls.Add(this.panelMapDescartados);
            this.panelContenedorDescartados.Location = new System.Drawing.Point(26, 21);
            this.panelContenedorDescartados.Name = "panelContenedorDescartados";
            this.panelContenedorDescartados.Size = new System.Drawing.Size(1326, 645);
            this.panelContenedorDescartados.TabIndex = 16;
            this.panelContenedorDescartados.Visible = false;
            // 
            // ViewWholeButton
            // 
            this.ViewWholeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ViewWholeButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(200)))));
            this.ViewWholeButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ViewWholeButton.FlatAppearance.BorderSize = 0;
            this.ViewWholeButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(11)))), ((int)(((byte)(100)))), ((int)(((byte)(162)))));
            this.ViewWholeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ViewWholeButton.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ViewWholeButton.ForeColor = System.Drawing.Color.White;
            this.ViewWholeButton.Image = ((System.Drawing.Image)(resources.GetObject("ViewWholeButton.Image")));
            this.ViewWholeButton.Location = new System.Drawing.Point(937, 586);
            this.ViewWholeButton.Name = "ViewWholeButton";
            this.ViewWholeButton.Size = new System.Drawing.Size(150, 39);
            this.ViewWholeButton.TabIndex = 42;
            this.ViewWholeButton.Text = "View Whole";
            this.ViewWholeButton.UseVisualStyleBackColor = false;
            this.ViewWholeButton.Visible = false;
            this.ViewWholeButton.Click += new System.EventHandler(this.ViewWholeButton_Click);
            // 
            // AddRemoveDiscardButton
            // 
            this.AddRemoveDiscardButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.AddRemoveDiscardButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(200)))));
            this.AddRemoveDiscardButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.AddRemoveDiscardButton.FlatAppearance.BorderSize = 0;
            this.AddRemoveDiscardButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(11)))), ((int)(((byte)(100)))), ((int)(((byte)(162)))));
            this.AddRemoveDiscardButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.AddRemoveDiscardButton.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AddRemoveDiscardButton.ForeColor = System.Drawing.Color.White;
            this.AddRemoveDiscardButton.Image = ((System.Drawing.Image)(resources.GetObject("AddRemoveDiscardButton.Image")));
            this.AddRemoveDiscardButton.Location = new System.Drawing.Point(1116, 586);
            this.AddRemoveDiscardButton.Name = "AddRemoveDiscardButton";
            this.AddRemoveDiscardButton.Size = new System.Drawing.Size(150, 39);
            this.AddRemoveDiscardButton.TabIndex = 36;
            this.AddRemoveDiscardButton.Text = "Add";
            this.AddRemoveDiscardButton.UseVisualStyleBackColor = false;
            this.AddRemoveDiscardButton.Visible = false;
            this.AddRemoveDiscardButton.Click += new System.EventHandler(this.AddRemoveDiscardButton_Click);
            // 
            // tablaGridNoDescartados
            // 
            this.tablaGridNoDescartados.AllowUserToAddRows = false;
            this.tablaGridNoDescartados.AllowUserToDeleteRows = false;
            this.tablaGridNoDescartados.AllowUserToResizeColumns = false;
            this.tablaGridNoDescartados.AllowUserToResizeRows = false;
            this.tablaGridNoDescartados.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tablaGridNoDescartados.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.tablaGridNoDescartados.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.tablaGridNoDescartados.BackgroundColor = System.Drawing.Color.White;
            this.tablaGridNoDescartados.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tablaGridNoDescartados.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.tablaGridNoDescartados.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle21.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle21.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(244)))), ((int)(((byte)(248)))));
            dataGridViewCellStyle21.Font = new System.Drawing.Font("Century Gothic", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle21.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle21.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle21.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle21.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.tablaGridNoDescartados.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle21;
            this.tablaGridNoDescartados.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle22.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle22.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle22.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle22.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle22.SelectionBackColor = System.Drawing.Color.SteelBlue;
            dataGridViewCellStyle22.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle22.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.tablaGridNoDescartados.DefaultCellStyle = dataGridViewCellStyle22;
            this.tablaGridNoDescartados.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.tablaGridNoDescartados.EnableHeadersVisualStyles = false;
            this.tablaGridNoDescartados.GridColor = System.Drawing.Color.White;
            this.tablaGridNoDescartados.Location = new System.Drawing.Point(862, 44);
            this.tablaGridNoDescartados.MultiSelect = false;
            this.tablaGridNoDescartados.Name = "tablaGridNoDescartados";
            this.tablaGridNoDescartados.ReadOnly = true;
            this.tablaGridNoDescartados.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tablaGridNoDescartados.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle23.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle23.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle23.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle23.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle23.SelectionBackColor = System.Drawing.Color.SteelBlue;
            dataGridViewCellStyle23.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle23.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.tablaGridNoDescartados.RowHeadersDefaultCellStyle = dataGridViewCellStyle23;
            dataGridViewCellStyle24.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle24.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle24.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle24.SelectionBackColor = System.Drawing.Color.SteelBlue;
            dataGridViewCellStyle24.SelectionForeColor = System.Drawing.Color.White;
            this.tablaGridNoDescartados.RowsDefaultCellStyle = dataGridViewCellStyle24;
            this.tablaGridNoDescartados.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.tablaGridNoDescartados.Size = new System.Drawing.Size(459, 536);
            this.tablaGridNoDescartados.TabIndex = 41;
            this.tablaGridNoDescartados.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.tablaGridNoDescartados_CellClick);
            this.tablaGridNoDescartados.SelectionChanged += new System.EventHandler(this.tablaGridNoDescartados_SelectionChanged);
            // 
            // tablaGridDescartados
            // 
            this.tablaGridDescartados.AllowUserToAddRows = false;
            this.tablaGridDescartados.AllowUserToDeleteRows = false;
            this.tablaGridDescartados.AllowUserToResizeColumns = false;
            this.tablaGridDescartados.AllowUserToResizeRows = false;
            this.tablaGridDescartados.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.tablaGridDescartados.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.tablaGridDescartados.BackgroundColor = System.Drawing.Color.White;
            this.tablaGridDescartados.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tablaGridDescartados.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.tablaGridDescartados.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle25.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle25.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(244)))), ((int)(((byte)(248)))));
            dataGridViewCellStyle25.Font = new System.Drawing.Font("Century Gothic", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle25.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle25.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle25.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle25.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.tablaGridDescartados.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle25;
            this.tablaGridDescartados.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle26.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle26.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle26.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle26.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle26.SelectionBackColor = System.Drawing.Color.SteelBlue;
            dataGridViewCellStyle26.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle26.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.tablaGridDescartados.DefaultCellStyle = dataGridViewCellStyle26;
            this.tablaGridDescartados.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.tablaGridDescartados.EnableHeadersVisualStyles = false;
            this.tablaGridDescartados.GridColor = System.Drawing.Color.White;
            this.tablaGridDescartados.Location = new System.Drawing.Point(-2, 44);
            this.tablaGridDescartados.MultiSelect = false;
            this.tablaGridDescartados.Name = "tablaGridDescartados";
            this.tablaGridDescartados.ReadOnly = true;
            this.tablaGridDescartados.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.tablaGridDescartados.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle27.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle27.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle27.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle27.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle27.SelectionBackColor = System.Drawing.Color.SteelBlue;
            dataGridViewCellStyle27.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle27.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.tablaGridDescartados.RowHeadersDefaultCellStyle = dataGridViewCellStyle27;
            dataGridViewCellStyle28.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle28.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle28.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle28.SelectionBackColor = System.Drawing.Color.SteelBlue;
            dataGridViewCellStyle28.SelectionForeColor = System.Drawing.Color.White;
            this.tablaGridDescartados.RowsDefaultCellStyle = dataGridViewCellStyle28;
            this.tablaGridDescartados.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.tablaGridDescartados.Size = new System.Drawing.Size(156, 586);
            this.tablaGridDescartados.TabIndex = 40;
            this.tablaGridDescartados.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.tablaGridDescartados_CellClick);
            this.tablaGridDescartados.SelectionChanged += new System.EventHandler(this.tablaGridDescartados_SelectionChanged);
            // 
            // panelMapDescartados
            // 
            this.panelMapDescartados.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelMapDescartados.BackColor = System.Drawing.SystemColors.Window;
            this.panelMapDescartados.Location = new System.Drawing.Point(161, 44);
            this.panelMapDescartados.Name = "panelMapDescartados";
            this.panelMapDescartados.Size = new System.Drawing.Size(717, 589);
            this.panelMapDescartados.TabIndex = 38;
            this.panelMapDescartados.Paint += new System.Windows.Forms.PaintEventHandler(this.panelMapDescartados_Paint);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1350, 666);
            this.Controls.Add(this.panelContenedor);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.panelContenedor.ResumeLayout(false);
            this.panelContenedor.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panelContenedorResultados.ResumeLayout(false);
            this.panelContenedorResultados.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ParametersGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.recuadroSummary)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SummaryGrid)).EndInit();
            this.panelContenedorMapa.ResumeLayout(false);
            this.panelContenedorMapa.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.recuadroMapa)).EndInit();
            this.panelContenedorDescartados.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tablaGridNoDescartados)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tablaGridDescartados)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelContenedor;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.ToolStripMenuItem mapToolStripMenuItem;
        private System.Windows.Forms.Panel panelContenedorMapa;
        private System.Windows.Forms.Panel panelMap;
        private System.Windows.Forms.Panel panelContenedorResultados;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.DataGridView SummaryGrid;
        private System.Windows.Forms.ToolStripMenuItem summaryToolStripMenuItem;
        private System.Windows.Forms.Label labelCursor;
        private System.Windows.Forms.PictureBox recuadroMapa;
        private System.Windows.Forms.Button ButtonSaveMap;
        private System.Windows.Forms.Button buttonViewSegmentation;
        private System.Windows.Forms.Button buttonSaveSummary;
        private System.Windows.Forms.Label LabelTituloSummary;
        private System.Windows.Forms.DataGridView ParametersGrid;
        private System.Windows.Forms.Button buttonSaveAllSummary;
        private System.Windows.Forms.Button ButtonDerechaSummay;
        private System.Windows.Forms.Button ButtonIzquierdaSummary;
        private System.Windows.Forms.SaveFileDialog saveFileDialogExcel;
        private System.Windows.Forms.PictureBox recuadroSummary;
        private System.Windows.Forms.SaveFileDialog saveFileDialogAll;
        private System.Windows.Forms.Button ButtonZoom;
        private System.Windows.Forms.Button ButtonDGPS;
        private System.Windows.Forms.Button ButtonMLAT_DGPS;
        private System.Windows.Forms.Button buttonHideBackground;
        private System.Windows.Forms.Button DGPSresultsButton;
        private System.Windows.Forms.ToolStripMenuItem discardedVehiclesToolStripMenuItem;
        private System.Windows.Forms.Panel panelContenedorDescartados;
        private System.Windows.Forms.Panel panelMapDescartados;
        private System.Windows.Forms.DataGridView tablaGridDescartados;
        private System.Windows.Forms.DataGridView tablaGridNoDescartados;
        private System.Windows.Forms.Button AddRemoveDiscardButton;
        private System.Windows.Forms.Button ViewWholeButton;
        private System.Windows.Forms.Button SaveScatterButton;
    }
}