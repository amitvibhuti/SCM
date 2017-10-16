namespace Vibe.SupplyChain.Win
{
    partial class KPIForm
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
            this.tblKPIs = new System.Windows.Forms.TableLayoutPanel();
            this.SuspendLayout();
            // 
            // tblKPIs
            // 
            this.tblKPIs.AutoSize = true;
            this.tblKPIs.ColumnCount = 1;
            this.tblKPIs.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblKPIs.Dock = System.Windows.Forms.DockStyle.Left;
            this.tblKPIs.Location = new System.Drawing.Point(0, 0);
            this.tblKPIs.Name = "tblKPIs";
            this.tblKPIs.RowCount = 1;
            this.tblKPIs.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblKPIs.Size = new System.Drawing.Size(0, 68);
            this.tblKPIs.TabIndex = 0;
            // 
            // KPIForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(668, 68);
            this.Controls.Add(this.tblKPIs);
            this.Name = "KPIForm";
            this.Text = "KPIForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tblKPIs;
    }
}