namespace Vibe.SupplyChain.Win
{
    partial class EntityListGraph
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.entityChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.entityChart)).BeginInit();
            this.SuspendLayout();
            // 
            // entityChart
            // 
            chartArea1.Name = "ChartArea1";
            this.entityChart.ChartAreas.Add(chartArea1);
            this.entityChart.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.Name = "Legend1";
            this.entityChart.Legends.Add(legend1);
            this.entityChart.Location = new System.Drawing.Point(0, 0);
            this.entityChart.Name = "entityChart";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.entityChart.Series.Add(series1);
            this.entityChart.Size = new System.Drawing.Size(347, 243);
            this.entityChart.TabIndex = 0;
            // 
            // EntityListGraph
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.entityChart);
            this.Name = "EntityListGraph";
            this.Size = new System.Drawing.Size(347, 243);
            ((System.ComponentModel.ISupportInitialize)(this.entityChart)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart entityChart;
    }
}
