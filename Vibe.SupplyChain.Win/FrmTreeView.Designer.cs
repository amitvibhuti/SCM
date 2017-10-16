namespace Vibe.SupplyChain.Win
{
    partial class FrmTreeView
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmTreeView));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.treeViewRoot = new System.Windows.Forms.TreeView();
            this.lstEvent = new System.Windows.Forms.ListBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.pnlEntity = new System.Windows.Forms.Panel();
            this.pnlEntityAuto = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnExportToPDF = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.lblNavigation = new System.Windows.Forms.Label();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.pnlChart = new System.Windows.Forms.Panel();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(97, 26);
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(96, 22);
            this.addToolStripMenuItem.Text = "Add";
            // 
            // treeViewRoot
            // 
            this.treeViewRoot.ContextMenuStrip = this.contextMenuStrip1;
            this.treeViewRoot.Dock = System.Windows.Forms.DockStyle.Left;
            this.treeViewRoot.Location = new System.Drawing.Point(0, 0);
            this.treeViewRoot.Name = "treeViewRoot";
            this.treeViewRoot.Size = new System.Drawing.Size(300, 425);
            this.treeViewRoot.TabIndex = 5;
            this.treeViewRoot.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewRoot_AfterSelect);
            this.treeViewRoot.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeViewRoot_NodeMouseClick);
            // 
            // lstEvent
            // 
            this.lstEvent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstEvent.FormattingEnabled = true;
            this.lstEvent.Location = new System.Drawing.Point(0, 0);
            this.lstEvent.Name = "lstEvent";
            this.lstEvent.Size = new System.Drawing.Size(614, 67);
            this.lstEvent.TabIndex = 8;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(300, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.lstEvent);
            this.splitContainer1.Size = new System.Drawing.Size(614, 425);
            this.splitContainer1.SplitterDistance = 354;
            this.splitContainer1.TabIndex = 9;
            // 
            // splitContainer2
            // 
            this.splitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.IsSplitterFixed = true;
            this.splitContainer2.Location = new System.Drawing.Point(0, 28);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.splitContainer3);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.pnlEntityAuto);
            this.splitContainer2.Size = new System.Drawing.Size(614, 326);
            this.splitContainer2.SplitterDistance = 322;
            this.splitContainer2.SplitterWidth = 2;
            this.splitContainer2.TabIndex = 7;
            // 
            // pnlEntity
            // 
            this.pnlEntity.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlEntity.Location = new System.Drawing.Point(0, 0);
            this.pnlEntity.Name = "pnlEntity";
            this.pnlEntity.Size = new System.Drawing.Size(320, 162);
            this.pnlEntity.TabIndex = 6;
            // 
            // pnlEntityAuto
            // 
            this.pnlEntityAuto.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlEntityAuto.Location = new System.Drawing.Point(0, 0);
            this.pnlEntityAuto.Name = "pnlEntityAuto";
            this.pnlEntityAuto.Size = new System.Drawing.Size(288, 324);
            this.pnlEntityAuto.TabIndex = 7;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.btnExportToPDF);
            this.panel1.Controls.Add(this.lblNavigation);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(614, 28);
            this.panel1.TabIndex = 0;
            // 
            // btnExportToPDF
            // 
            this.btnExportToPDF.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExportToPDF.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnExportToPDF.ImageKey = "pdf.png";
            this.btnExportToPDF.ImageList = this.imageList1;
            this.btnExportToPDF.Location = new System.Drawing.Point(589, 2);
            this.btnExportToPDF.Name = "btnExportToPDF";
            this.btnExportToPDF.Size = new System.Drawing.Size(20, 23);
            this.btnExportToPDF.TabIndex = 1;
            this.btnExportToPDF.UseVisualStyleBackColor = true;
            this.btnExportToPDF.Visible = false;
            this.btnExportToPDF.Click += new System.EventHandler(this.btnExportToPDF_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "pdf.png");
            this.imageList1.Images.SetKeyName(1, "fail.png");
            this.imageList1.Images.SetKeyName(2, "success.png");
            // 
            // lblNavigation
            // 
            this.lblNavigation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lblNavigation.AutoSize = true;
            this.lblNavigation.Location = new System.Drawing.Point(6, 8);
            this.lblNavigation.Name = "lblNavigation";
            this.lblNavigation.Size = new System.Drawing.Size(69, 13);
            this.lblNavigation.TabIndex = 0;
            this.lblNavigation.Text = "Supply Chain";
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.pnlEntity);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.pnlChart);
            this.splitContainer3.Size = new System.Drawing.Size(320, 324);
            this.splitContainer3.SplitterDistance = 162;
            this.splitContainer3.TabIndex = 7;
            // 
            // pnlChart
            // 
            this.pnlChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlChart.Location = new System.Drawing.Point(0, 0);
            this.pnlChart.Name = "pnlChart";
            this.pnlChart.Size = new System.Drawing.Size(320, 158);
            this.pnlChart.TabIndex = 0;
            // 
            // FrmTreeView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(914, 425);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.treeViewRoot);
            this.Name = "FrmTreeView";
            this.Text = "Supply Chain Manager";
            this.contextMenuStrip1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.TreeView treeViewRoot;
        private System.Windows.Forms.ListBox lstEvent;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel pnlEntity;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblNavigation;
        private System.Windows.Forms.Button btnExportToPDF;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Panel pnlEntityAuto;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.Panel pnlChart;
    }
}

