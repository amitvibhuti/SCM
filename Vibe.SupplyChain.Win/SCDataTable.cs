using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.Linq;

namespace Vibe.SupplyChain.Win
{
    public class SCDataTable
    {
        public List<SCDataColumn> Columns { get; set; }
        public List<SCDataRow> Rows { get; set; }
        public SCDataTable() { Columns = new List<SCDataColumn>(); Rows = new List<SCDataRow>(); }
        public List<SCDataColumn> SumColumns { get {
                return this.Columns.Where(c => c.IsSumColumn).ToList();
            }
        }
        public SCDataRow FooterRow { get; set; }
        public int AddRow(Entity ent)
        {
            List<ExpandoObject> rowdata = new List<ExpandoObject>();
            dynamic exp = new ExpandoObject();
            foreach (SCDataColumn col in this.Columns)
            {
                EntityObjectAttribute eoattr = Entity.GetEntityAttribute(col.EntityType, col.PropertyName);
                string prefix = "";
                ParsedEntityValue pev = ent.ParseEntityNavigation(col.MapText);
                object boundValue = pev.Value; 
                string displaytext = boundValue.ToString();
                if (!String.IsNullOrEmpty(eoattr.DisplayPrefix))
                {
                    prefix = ent.ParseEntityNavigation(eoattr.DisplayPrefix).Value.ToString();
                }
                if (!String.IsNullOrEmpty(eoattr.SelectOptions) && !String.IsNullOrEmpty(eoattr.SelectText))
                {
                    int val = (int)ent.GetValue(col.PropertyName); //pInfo.GetValue(ent, null);
                    EntityProperty ep = ent.Properties.Find(a => a.Name == col.PropertyName);
                    displaytext = ep.SelectOptions.ToList().Find(np => np.ID == val).Name;
                }
                exp = new ExpandoObject();
                exp.DisplayValue = prefix + " " + displaytext;
                exp.BoundValue = boundValue;
                rowdata.Add(exp);
            }
            SCDataRow row = new SCDataRow(ent);
            int i = 0;
            foreach (dynamic data in rowdata)
            {
                row.Cells.Add(new SCDataCell()
                {
                    Name = Columns[i].Name,
                    DisplayValue = data.DisplayValue,
                    BoundValue = data.BoundValue
                });
                i++;
            }
            Rows.Add(row);
            foreach (SCDataColumn col in this.Columns)
            {
                ParsedEntityValue pev = ent.ParseEntityNavigation(col.MapText);
                EntityObjectAttribute eoattr = Entity.GetEntityAttribute(pev.Entity.GetType(), pev.PropertyName);
                if (eoattr.SuccessColor != BGColor.None)
                {
                    row.GetCell(col.Name).BackColor = pev.Entity.EvaluateCriteriaColor(pev.PropertyName);
                }
            }
            return Rows.Count - 1;
        }
        public int AddFooterRow()
        {
            if (this.Rows.Count == 0)
                return -1;
            List<ExpandoObject> rowdata = new List<ExpandoObject>();
            string displaytext = "";
            object boundvalue = null;
            bool showfooterrow = false;

            dynamic exp = new ExpandoObject();
            foreach (SCDataColumn col in this.Columns)
            {
                EntityObjectAttribute eoattr = Entity.GetEntityAttribute(col.EntityType, col.PropertyName);

                if (eoattr.Accessibility != Accessibility.NoView)
                {
                    if (col.IsSumColumn)
                    {
                        double d1 = 0;
                        if (!col.Cells.TrueForAll(c => double.TryParse(c.BoundValue.ToString(), out d1)))
                            throw new Exception("Data type mismatch. " + col.Name + " can not be used for sum column.");
                        boundvalue = col.Cells.Sum(c => (double)c.BoundValue);
                        string prefix = "";
                        if (!String.IsNullOrEmpty(eoattr.DisplayPrefix))
                        {
                            prefix = Entity.ParseEntityNavigation(this.Rows[0].Entity, eoattr.DisplayPrefix).Value.ToString();
                        }
                        displaytext = prefix + " " + boundvalue;
                        showfooterrow = true;
                    }
                    else 
                    {
                        displaytext = "-";
                        boundvalue = "";
                    }

                    exp = new ExpandoObject();
                    exp.DisplayValue = displaytext;
                    exp.BoundValue = boundvalue;
                    rowdata.Add(exp);
                }
            }
            if (showfooterrow)
            {
                FooterRow = new SCDataRow(null);
                int i = 0;
                foreach (dynamic data in rowdata)
                {
                    FooterRow.Cells.Add(new SCDataCell() { Name = Columns[i].Name, DisplayValue = data.DisplayValue, BoundValue = data.BoundValue });
                    i++;
                }
            }
            return Rows.Count;
        }
        public int AddColumn(SCDataColumn col, EntityObjectAttribute eoattr)
        {
            col.Table = this;
            if (eoattr.CalculatedValue == CalculatedValue.Image)
                col.DataType = ColumnDataType.Image;
            if (eoattr.Width != 0)
            {
                col.Width = eoattr.Width;
            }
            this.Columns.Add(col);
            return Columns.IndexOf(col);
        }
        
    }
    public enum ColumnDataType { Text, Image }
    public class SCDataColumn
    {
        public SCDataTable Table { get; set; }
        public ColumnDataType DataType { get; set; }
        public List<SCDataCell> Cells { get {
                return Table.Rows.Select(r => r.Cells.Find(c => c.Name == this.Name)).ToList();
            }
        }
        public string Name { get; set; }
        public string PropertyName { get; set; }
        public string HeaderText { get; set; }
        public string MapText { get; set; }
        public int Width { get; set; }
        public SCDataColumn(Type eType)
        {
            EntityType = eType;
        }
        public Type EntityType { get; set; }
        public bool IsSumColumn { get; set; }
    }
    public class SCDataRow
    {
        public object[] RowData { get { return Cells.Select(c => c.DisplayValue).ToArray(); } }
        public List<SCDataCell> Cells { get; set; }
        public Entity Entity { get; set; }
        public SCDataRow(Entity ent)
        {
            Cells = new List<SCDataCell>();
            Entity = ent;
        }
        public SCDataCell GetCell(string name)
        {
            return Cells.Find(c => c.Name == name);
        }
    }
    public class SCDataCell
    {
        public string Name { get; set; }
        public Color BackColor { get; set; }
        public string DisplayValue { get; set; }
        public object BoundValue { get; set; }
    }
    public class SCDataMap
    {
        public string HeaderText { get; set; }
        public Entity Entity { get; set; }
        public string MapText { get; set; }
    }
}
