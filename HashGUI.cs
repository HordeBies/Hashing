﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hashing
{
    public partial class HashGUI : Form
    {
        private Hash hash;
        private DataTable DTable = new DataTable();
        private int l;
        public HashGUI(int initKey,double initLoadingFactor, int initialBucketDepthLimit)
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
            hash = new Hash(l,initLoadingFactor,initialBucketDepthLimit);

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

            int width = 0;

            dataGridView1.DataSource = DTable;
            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
                col.ReadOnly = true;
                width += col.GetPreferredWidth(DataGridViewAutoSizeColumnMode.AllCells, true);
            }
            if(dataGridView1.Width+24 > this.Width)
                dataGridView1.Width = this.Width-24;
            else
                dataGridView1.Width = width+3+25;
            
            
            dataGridView1.Refresh();
            FillTable();
            label2.Text = "Current Hash Key: " +l.ToString();
            label6.Text = "Loading Factor: " + "0/"+hash.LoadingFactor.ToString();
            dataGridView1.Refresh();
            dataGridView1.ClearSelection();

        }

        private void FillTable()
        {
            int i = 0;
            foreach (LinkedList<HashValue> hashIndex in hash.HashTable)
            {
                if(hashIndex.Count > 0)
                {
                    int j = 0;
                    foreach (HashValue hasVal in hashIndex)
                    {
                        while(DTable.Rows.Count < j+1)
                        {
                            DataRow r = DTable.NewRow();
                            DTable.Rows.Add(r);
                            
                        }
                        if (hasVal.Occurrence > 1)
                        {
                            DTable.Rows[j][i] = hasVal.Value.ToString()+"("+hasVal.Occurrence.ToString()+")";
                        }
                        else
                        {
                            DTable.Rows[j][i] = hasVal.Value;
                        }
                        
                        j++;
                    }
                }
                i++;
            }
            ReSizeTable();
            label7.Text = "Bucket Depth: " + hash.CurrentBucketDepth.ToString() + "/" + hash.BucketDepthLimit.ToString();
            dataGridView1.ClearSelection();
        }

        private void ReSizeTable()
        {
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
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private int getPrevPrime(int currPrime)
        {
            int prevPrime = currPrime;
            bool isPrime;
            do
            {
                prevPrime--;
                isPrime = true;
                for (int i = 2; i < prevPrime; i++)
                {
                    if (prevPrime % i == 0)
                    {
                        isPrime = false;
                        break;
                    }
                }
            } while (!isPrime && prevPrime >2);

            return prevPrime;
        }
        private void reHash()
        {
            int lastPrime = l;
            Hash.ReHash(ref hash);
            l = hash.KeyLength;
            label2.Text = "Current Hash Key: " + l.ToString();

            for (int lp= lastPrime; lp < l; lp++)
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
            while ((double)hash.DataLength/hash.KeyLength >= hash.LoadingFactor || hash.CurrentBucketDepth > hash.BucketDepthLimit)
            {
                DTable.Clear();
                reHash();
            }
            
            FillTable();
            label6.Text = "Current Loading Factor: " + (Math.Truncate(((double)hash.DataLength / hash.KeyLength)*100)/100).ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string txt = textBox1.Text;
            textBox1.Clear();
            int val = 0;
            if (int.TryParse(txt,out val))
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
            if(e.KeyChar == (char)13)
            {
                e.Handled= true;
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
            this.Height= control.Size.Height;
            this.Width= control.Size.Width;
            ReSizeTable();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

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

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1.ClearSelection();
        }
    }
    
}
