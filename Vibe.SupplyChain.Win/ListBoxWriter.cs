using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vibe.SupplyChain.Win
{
    public class ListBoxWriter : TextWriter
    {
        private ListBox list;
        private StringBuilder content = new StringBuilder();

        public ListBoxWriter(ListBox list)
        {
            this.list = list;
        }
        object _obj = new object();
        public override void WriteLine(string value)
        {
            base.WriteLine(value);
            lock(_obj){
                File.AppendAllText("C:\\logs\\eventlog.txt", "\r\n" + 
                    DateTime.Now.ToString("HH:mm:ss.fff") + 
                    "\t" +
                    Thread.CurrentThread.ManagedThreadId.ToString() +
                    "\t" +
                    value);
                UpdateListBox(value);
            }
        }
        private void UpdateListBox(string value)
        {
            if (list.InvokeRequired)
            {
                list.BeginInvoke(new Action<string>(UpdateListBox), value);
            }
            else
            {
                list.Items.Add(DateTime.Now.ToString("HH:mm:ss.fff") + "\t" + value);
                list.TopIndex = list.Items.Count - 1;
            }
        }
        public override Encoding Encoding
        {
            get { return System.Text.Encoding.UTF8; }
        }
    }
}
