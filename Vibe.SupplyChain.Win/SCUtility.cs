using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vibe.SupplyChain;
using Vibe.SupplyChain.Data;
using Vibe.SupplyChain.Win;

namespace Vibe.SupplyChain.Win
{
    public static class SCUtility
    {
        public static SCDataTable ToDataTable(this IEnumerable<dynamic> items)
        {
            SCDataTable dt = new SCDataTable();

            var data = items.ToArray();
            if (data.Count() == 0) return dt;
            Entity ent = ((IDictionary<string, object>)data[0]).Values.Cast<SCDataMap>().First().Entity;
            //Header
            foreach (var key in ((IDictionary<string, object>)data[0]).Keys)
            {
                SCDataMap dMap = (SCDataMap)((IDictionary<string, object>)data[0])[key];
                ParsedEntityValue pev = ent.ParseEntityNavigation(dMap.MapText);
                if (pev == null)
                    continue;
                EntityObjectAttribute eoattr = pev.Entity.GetEntityAttribute(pev.PropertyName);
                SCDataColumn col = new SCDataColumn(pev.Entity.GetType())
                {
                    Name = dMap.MapText,//pev.PropertyName, #101
                    PropertyName = pev.PropertyName,
                    HeaderText = dMap.HeaderText,
                    MapText = dMap.MapText,
                    IsSumColumn = pev.IsSumColumn
                };
                dt.AddColumn(col, eoattr);
            }
            //Body
            foreach (var d in data)
            {
                ent = ((IDictionary<string, object>)d).Values.Cast<SCDataMap>().First().Entity;
                dt.AddRow(ent);
            }
            dt.AddFooterRow();
            return dt;
        }
        public static SCDataTable ToDataTable(this EntityList elist)
        {
            SCDataTable dt = new SCDataTable();
            //Header
            elist.ForEachColumn((attr, pInfo, eoattr) => {
                if (eoattr != null 
                    && eoattr.Accessibility != Accessibility.NoView
                    && eoattr.CalculatedValue != CalculatedValue.Image)
                {
                    if (elist.CanEdit || attr != "ID")
                    {
                        SCDataColumn col = new SCDataColumn(elist.ItemType)
                        {
                            Name = attr,
                            PropertyName = attr,
                            MapText = attr,
                            HeaderText = String.IsNullOrEmpty(eoattr.DisplayLabel) ? attr : eoattr.DisplayLabel,
                            IsSumColumn = eoattr.FooterMode == FooterMode.Sum
                        };
                        dt.AddColumn(col, eoattr);
                    }
                }
            });
            // Body
            foreach (Entity ent in elist)
            {
                dt.AddRow(ent);
            }
            // Footer
            dt.AddFooterRow();
            return dt;
        }
        public static List<TreeNode> GetAllNodes(this TreeView _self)
        {
            List<TreeNode> result = new List<TreeNode>();
            foreach (TreeNode child in _self.Nodes)
            {
                result.AddRange(child.GetAllNodes());
            }
            return result;
        }
        public static List<TreeNode> GetAllNodes(this TreeNode _self)
        {
            List<TreeNode> result = new List<TreeNode>();
            result.Add(_self);
            foreach (TreeNode child in _self.Nodes)
            {
                result.AddRange(child.GetAllNodes());
            }
            return result;
        }
        public static void ExportToPDF(DataManager manager, DataGridView dgv, string fileName, string headerText)
        {
            ExportToPDF(manager, dgv, fileName, headerText, null);
        }
        public static void ExportToPDF(DataManager manager, DataGridView dgv, string fileName, string headerText, System.Drawing.Image imgChart)
        {
            Document doc = new Document(iTextSharp.text.PageSize.LETTER, 10, 10, 42, 35);
            string datepart = DateTime.Now.ToString().Replace(" ", "").Replace("/", "").Replace(":", "");
            string filepath = manager.Data.Root.ExportLocation + "/Export" + fileName + datepart + ".pdf";
            PdfWriter wri = PdfWriter.GetInstance(doc, new FileStream(filepath, FileMode.Create));

            doc.Open();

            Paragraph paragraph = new Paragraph(manager.Data.Root.Name);
            var titleFont = FontFactory.GetFont("Courier", 24, BaseColor.BLACK);
            titleFont.Size = 20;
            titleFont.Color = BaseColor.BLUE;
            paragraph.Font = titleFont;
            paragraph.Alignment = 1;
            doc.Add(paragraph);

            paragraph = new Paragraph(headerText);
            paragraph.Alignment = 1;
            doc.Add(paragraph);

            paragraph = new Paragraph("______________________________________________________________________");
            paragraph.Alignment = 1;
            paragraph.SpacingAfter = 10;
            doc.Add(paragraph);

            paragraph = new Paragraph("Datasheet");
            paragraph.Alignment = 0;
            paragraph.IndentationLeft = 40;
            paragraph.SpacingBefore = 10;
            paragraph.SpacingAfter = 10;
            doc.Add(paragraph);

            PdfPTable t1 = new PdfPTable(dgv.Columns.Count);
            float[] widths = Enumerable.Range(1, dgv.Columns.Count).Select(i => 25f).ToArray();
            t1.SetWidths(widths);

            DataTable dt = (DataTable)dgv.DataSource;

            foreach (DataGridViewColumn col in dgv.Columns)
            {
                PdfPCell c = new PdfPCell(new Phrase(col.HeaderText));
                c.BackgroundColor = BaseColor.LIGHT_GRAY;
                t1.AddCell(c);
            }


            foreach (DataGridViewRow rows in dgv.Rows)
            {
                foreach (DataGridViewColumn col in dgv.Columns)
                {
                    string val = "";
                    PdfPCell c = new PdfPCell();
                    if (dgv.Rows[rows.Index].Cells[col.Name].Value == null)
                    {
                        c = new PdfPCell(new Phrase(val));
                    }
                    else if (dgv.Rows[rows.Index].Cells[col.Name].Value.GetType() == typeof(System.Drawing.Bitmap))
                    {
                        iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance((System.Drawing.Bitmap)dgv.Rows[rows.Index].Cells[col.Name].Value, BaseColor.WHITE);
                        c = new PdfPCell(jpg);
                        c.Padding = 5;
                    }
                    else
                    {
                        val = dgv.Rows[rows.Index].Cells[col.Name].Value.ToString();
                        c = new PdfPCell(new Phrase(val));
                    }
                    //c.Width = dgv.Columns[col.Name].Width;
                    c.BackgroundColor = new BaseColor(dgv.Rows[rows.Index].Cells[col.Name].Style.BackColor);
                    c.VerticalAlignment = (int)dgv.Rows[rows.Index].Cells[col.Name].Style.Alignment;
                    t1.AddCell(c);
                }
            }
            doc.Add(t1);

            if (imgChart != null)
            {
                paragraph = new Paragraph("Chart");
                paragraph.Alignment = 0;
                paragraph.IndentationLeft = 40;
                paragraph.SpacingBefore = 30;
                doc.Add(paragraph);

                iTextSharp.text.Image chart = iTextSharp.text.Image.GetInstance(imgChart, BaseColor.WHITE);
                chart.ScaleToFit(doc.PageSize);
                chart.WidthPercentage = 100;
                doc.Add(chart);
            }
            paragraph = new Paragraph("Signature");
            paragraph.SpacingBefore = 50;
            paragraph.Alignment = 2;
            paragraph.IndentationRight = 40;
            doc.Add(paragraph);

            paragraph = new Paragraph("Stamp/Date");
            paragraph.Alignment = 2;
            paragraph.SpacingBefore = 20;
            paragraph.IndentationRight = 40;
            doc.Add(paragraph);

            doc.Close();
            System.Diagnostics.Process.Start(filepath);
        }
        public static void BindData(this DataGridView dgv, SCDataTable dt)
        {
            dgv.DataError += (sender, e) =>
            {
                if (e.Exception.GetType() == typeof(ConstraintException))
                {
                    dgv.Rows[e.RowIndex].ErrorText = "Error";
                    dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = "Error";
                    e.ThrowException = false;
                }
            };
            dgv.Dock = DockStyle.Fill;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle() { BackColor = Color.SteelBlue, ForeColor = Color.White };

            // Header
            DataGridViewColumn col = new DataGridViewTextBoxColumn();
            col = new DataGridViewImageColumn() { ImageLayout = DataGridViewImageCellLayout.Zoom };
            col.Name = "#";
            col.Width = 22;
            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgv.Columns.Add(col);
            foreach (SCDataColumn column in dt.Columns)
            {
                col = new DataGridViewTextBoxColumn();
                if (column.DataType == ColumnDataType.Image)
                    col = new DataGridViewImageColumn() { ImageLayout = DataGridViewImageCellLayout.Zoom };
                col.DataPropertyName = column.PropertyName;
                col.Name = column.Name;
                col.HeaderText = column.HeaderText;
                if (column.Width != 0)
                {
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    col.Width = column.Width;
                }
                dgv.Columns.Add(col);
            }
            // Body
            foreach (SCDataRow row in dt.Rows)
            {
                int index = dgv.Rows.Add(row.RowData);
                dgv.Rows[index].Cells["#"].Value = GetConstraintImage(!row.Entity.HasConstraintViolation ? ConstraintImage.Success : ConstraintImage.Fail);

                foreach (SCDataCell cell in row.Cells)
                {
                    dgv.Rows[index].Cells[cell.Name].Style.BackColor = cell.BackColor;
                    dgv.Rows[index].Cells[cell.Name].Value = cell.DisplayValue;
                };
            }
            // Footer
            if (dt.FooterRow != null)
            {
                int i = dgv.Rows.Add(dt.FooterRow.RowData);
                dgv.Rows[i].DefaultCellStyle = new DataGridViewCellStyle() { BackColor = Color.SteelBlue, ForeColor = Color.White };
                dgv.Rows[i].Cells["#"].Value = GetConstraintImage(ConstraintImage.None);

                foreach (SCDataCell cell in dt.FooterRow.Cells)
                {
                    dgv.Rows[i].Cells[cell.Name].Value = cell.DisplayValue;
                };
            }
        }
        public static System.Drawing.Image GetConstraintImage(ConstraintImage color)
        {
            switch (color)
            {
                case ConstraintImage.Fail:
                    return Resource.fail;
                case ConstraintImage.Success:
                    return Resource.success;
                default:
                case ConstraintImage.None:
                    return Resource.none;
            }
        }
    }
}
