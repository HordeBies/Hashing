using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Hashing
{
    public partial class HashGUI : Form
    {
        private Hash hash;
        private DataTable DTable = new DataTable();
        private int l;
        public HashGUI(int initKey, double initLoadingFactor, int initialBucketDepthLimit)
        {
            l = initKey;
            InitializeComponent();

            /*
            var child = new Form();
            child.TopLevel = true;
            child.Location = new Point(10, 5);
            child.Size = new Size(100, 100);
            child.BackColor = Color.Yellow;
            child.FormBorderStyle = FormBorderStyle.None;
            child.Visible = true;
            this.Controls.Add(child);*//*
            var child = new HashVisualization();            
            child.Location = new Point(0,0);
            child.Visible = true;
            child.updateHashTable(20);
            this.Controls.Add(child);*/
            hash = new Hash(initKey, initLoadingFactor, initialBucketDepthLimit);

            dataGridView1.RowHeadersVisible = false;
            dataGridView1.ReadOnly = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.LightYellow;
            dataGridView1.EnableHeadersVisualStyles = false;



            for (int i = 0; i < l; i++)
            {
                DataColumn col = new DataColumn();
                col.ColumnName = i.ToString();
                DTable.Columns.Add(col);
            }
            /*
            for (int i = 0; i < 10; i++)
            {
                DataRow r = DTable.NewRow();
                r.BeginEdit();
                int j = 0;
                foreach (DataColumn c in DTable.Columns)
                {
                    r[c.ColumnName] = i * 10 + j;
                    j++;
                }
                r.EndEdit();
                DTable.Rows.Add(r);
            }*/

            dataGridView1.DataSource = DTable;
            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
                col.ReadOnly = true;
            }

            FillTable();
            label2.Text = "Current Hash Key: " + l.ToString();
            label6.Text = "Loading Factor: " + "0/" + hash.LoadingFactor.ToString();
            dataGridView1.Refresh();
            dataGridView1.ClearSelection();
            ReSizeTable();
        }

        private void FillTable()
        {
            int i = 0;
            foreach (LinkedList<HashValue> hashIndex in hash.HashTable)
            {
                if (hashIndex.Count > 0)
                {
                    int j = 0;
                    foreach (HashValue hasVal in hashIndex)
                    {
                        while (DTable.Rows.Count < j + 1)
                        {
                            DataRow r = DTable.NewRow();
                            DTable.Rows.Add(r);

                        }
                        if (hasVal.Occurrence > 1)
                        {
                            DTable.Rows[j][i] = hasVal.ToString() + "(" + hasVal.Occurrence.ToString() + ")";
                        }
                        else
                        {
                            DTable.Rows[j][i] = hasVal.ToString();
                        }

                        j++;
                    }
                }
                i++;
            }
            label7.Text = "Bucket Depth: " + hash.CurrentBucketDepth.ToString() + "/" + hash.BucketDepthLimit.ToString();
            label6.Text = "Current Loading Factor: " + (Math.Truncate(((double)hash.DataLength / hash.KeyLength) * 100) / 100).ToString()+"/"+(Math.Truncate(hash.LoadingFactor*100)/100).ToString();
            dataGridView1.ClearSelection();
        }

        private void ReSizeTable()
        {
            /* //this part dynamically sizes the table depending on boundaries but costs so many resources therefore not active yet.
            int width = 0;
            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                width += col.GetPreferredWidth(DataGridViewAutoSizeColumnMode.AllCells, true);
            }

            int newHeight = dataGridView1.RowCount * 22 + 40;
            while (newHeight + dataGridView1.Location.Y > this.Height - 40)
            {
                newHeight -= 22;
            }
            dataGridView1.Height = newHeight;

            if (width + 24 + 3 + 25 > this.Width)
                dataGridView1.Width = this.Width - 40;
            else
                dataGridView1.Width = width + 3 + 25;
            */
            dataGridView1.Width = this.Width - 40;
            dataGridView1.Height = this.Height - 40 - dataGridView1.Location.Y;
        }

        private void reHash()
        {
            int lastPrime = l;
            Hash.ReHash(ref hash);
            l = hash.KeyLength;
            label2.Text = "Current Hash Key: " + l.ToString();

            for (int lp = lastPrime; lp < l; lp++)
            {
                DataColumn col = new DataColumn();
                col.ColumnName = lp.ToString();
                DTable.Columns.Add(col);
                dataGridView1.Columns[lp].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGridView1.Columns[lp].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns[lp].ReadOnly = true;
            }
        }

        private void AddHash(int val)
        {
            hash.Add(val, 1);
            while ((double)hash.DataLength / hash.KeyLength >= hash.LoadingFactor || hash.CurrentBucketDepth > hash.BucketDepthLimit)
            {
                DTable.Clear();
                reHash();
            }

            FillTable();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string txt = textBox1.Text;
            textBox1.Clear();
            int val = 0;
            if (int.TryParse(txt, out val))
            {
                AddHash(val);
            }
        }

        private void HashGUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.init.Location = this.Location;
            Program.init.Show();

        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                e.Handled = true;
                string txt = textBox1.Text;
                textBox1.Clear();
                int val = 0;
                if (int.TryParse(txt, out val))
                {
                    AddHash(val);
                }
            }
        }

        private void HashGUI_Resize(object sender, EventArgs e)
        {
            Control control = (Control)sender;
            this.Height = control.Size.Height;
            this.Width = control.Size.Width;
            ReSizeTable();
        }

        private void RemoveHash(int val)
        {
            hash.Remove(val);
            DTable.Clear();
            FillTable();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string txt = textBox2.Text;
            textBox2.Clear();
            int val = 0;
            if (int.TryParse(txt, out val))
            {
                RemoveHash(val);
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                e.Handled = true;
                string txt = textBox2.Text;
                textBox2.Clear();
                int val = 0;
                if (int.TryParse(txt, out val))
                {
                    RemoveHash(val);
                }
            }
        }
    }

}
