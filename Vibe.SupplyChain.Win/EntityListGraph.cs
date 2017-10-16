using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vibe.SupplyChain.Data;
using System.Windows.Forms.DataVisualization.Charting;

namespace Vibe.SupplyChain.Win
{
    public partial class EntityListGraph : UserControl
    {
        public Chart Chart { get { return entityChart; } }
        DataManager _manager;
        public EntityListGraph(DataManager manager, EntityList elist)
        {
            InitializeComponent();
            _manager = manager == null ? new DataManager() : manager;
            string chartText = elist.EntityObjectAttribute.ShowChart;
            string[] seriesList = chartText.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (seriesList.Length == 0)
                return;
            //entityChart.Line
            entityChart.DataSource = elist;
            entityChart.Series.Clear();
            foreach (string series in seriesList)
            {
                string[] seriesXY = series.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (seriesXY.Length != 4)
                    return;
                Series s1 = entityChart.Series.Add(seriesXY[1]);
                switch(seriesXY[0].ToLower())
                {
                    case "area":
                        s1.ChartType = SeriesChartType.Area;
                        break;
                    case "bubble":
                        s1.ChartType = SeriesChartType.Bubble;
                        break;
                }
                s1.XValueMember = seriesXY[2];
                s1.YValueMembers = seriesXY[3];
            }
            entityChart.DataBind();
        }
    }
}
