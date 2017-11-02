namespace Vibe.SupplyChain.Win
{
    partial class FrmModelOverview
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
            this.tvModel = new System.Windows.Forms.TreeView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tvModelEntity = new System.Windows.Forms.TreeView();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tvModel
            // 
            this.tvModel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvModel.ItemHeight = 25;
            this.tvModel.Location = new System.Drawing.Point(0, 0);
            this.tvModel.Name = "tvModel";
            this.tvModel.ShowPlusMinus = false;
            this.tvModel.Size = new System.Drawing.Size(176, 262);
            this.tvModel.TabIndex = 0;
            this.tvModel.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvModel_AfterSelect);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tvModel);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tvModelEntity);
            this.splitContainer1.Size = new System.Drawing.Size(530, 262);
            this.splitContainer1.SplitterDistance = 176;
            this.splitContainer1.TabIndex = 1;
            // 
            // tvModelEntity
            // 
            this.tvModelEntity.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvModelEntity.ItemHeight = 25;
            this.tvModelEntity.Location = new System.Drawing.Point(0, 0);
            this.tvModelEntity.Name = "tvModelEntity";
            this.tvModelEntity.ShowPlusMinus = false;
            this.tvModelEntity.Size = new System.Drawing.Size(350, 262);
            this.tvModelEntity.TabIndex = 1;
            // 
            // FrmModelOverview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(530, 262);
            this.Controls.Add(this.splitContainer1);
            this.Name = "FrmModelOverview";
            this.Text = "Model Overview";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView tvModel;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView tvModelEntity;
    }
}