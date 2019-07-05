using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProxySettingsManager
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        ProxiSettingsMgmt psm = new ProxiSettingsMgmt();


        private void cmd_enable_Click(object sender, EventArgs e)
        {
            cmd_saveProxy_Click(sender, e);
            if (txt_ip.Text != "" || txt_port.Text != "")
            {
                psm.Enable(txt_ip.Text, txt_port.Text);
                RefreshActiveProxy();
            }
            else
            {
                cmd_disable_Click(sender, e);
            }
        }

        private void cmd_disable_Click(object sender, EventArgs e)
        {
            psm.Disable();
            RefreshActiveProxy();
        }


        List<myProxy> list = new List<myProxy>()
        {
            new myProxy("USA reliable","34.87.36.45","80"),
            new myProxy("reset", "", ""),
        };


        private void Form1_Load(object sender, EventArgs e)
        {

            list = LoadProxysFromSettings(list);


            var bindingList = new BindingList<myProxy>(list);
            var source = new BindingSource(bindingList, null);
            dataGridView1.DataSource = source;


            dataGridView1.Rows[0].Selected = true;

            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            RefreshActiveProxy();
        }


        [Serializable()]
        public class myProxy
        {
            public string Name { get; set; }
            public string Ip { get; set; }
            public string Port { get; set; }

            public myProxy(string Name, string Ip, string Port)
            {
                this.Name = Name;
                this.Ip = Ip;
                this.Port = Port;
            }

            public override bool Equals(object obj)
            {
                myProxy other = obj as myProxy;

                return (other != null)
                    && (Name == other.Name)
                    && (Ip == other.Ip)
                    && (Port == other.Port);
            }

            public override int GetHashCode()
            {
                int hash = 13;
                hash = (hash * 7) + Name.GetHashCode();
                hash = (hash * 7) + Ip.GetHashCode();
                hash = (hash * 7) + Port.GetHashCode();
                return hash;
            }

        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            DataGridView dgv = sender as DataGridView;
            if (dgv != null && dgv.SelectedRows.Count > 0)
            {
                txt_proxyName.Text = dgv.SelectedRows[0].Cells[0].Value.ToString();
                txt_ip.Text = dgv.SelectedRows[0].Cells[1].Value.ToString();
                txt_port.Text = dgv.SelectedRows[0].Cells[2].Value.ToString();
            }
        }

        private void cmd_saveProxy_Click(object sender, EventArgs e)
        {
            var newProxy = new myProxy(txt_proxyName.Text, txt_ip.Text, txt_port.Text);

            while (list.Contains(newProxy))
            {
                list.Remove(newProxy);
            }

            list.Insert(0, newProxy);

            var bindingList = new BindingList<myProxy>(list);
            var source = new BindingSource(bindingList, null);
            dataGridView1.DataSource = source;
        }

        private void RefreshActiveProxy()
        {
            myProxy activeProxy = psm.GetAvtiveProxy();
            if (activeProxy != null)
            {
                txt_avtiveIP.Text = activeProxy.Ip;
                txt_avtivePort.Text = activeProxy.Port;
            }
            else
            {
                txt_avtiveIP.Text = "";
                txt_avtivePort.Text = "";
            }
        }

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            cmd_enable_Click(sender, e);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(((LinkLabel)sender).Text);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveProxysToSettings(list);
        }


        List<myProxy> LoadProxysFromSettings(List<myProxy> defaultRet)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(Properties.Settings.Default.ProxyHist)))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    return (List<myProxy>)bf.Deserialize(ms);
                }
            }
            catch (Exception)
            {
            }
            return defaultRet;
        }

        void SaveProxysToSettings(List<myProxy> ToSave)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, ToSave);
                ms.Position = 0;
                byte[] buffer = new byte[(int)ms.Length];
                ms.Read(buffer, 0, buffer.Length);
                Properties.Settings.Default.ProxyHist = Convert.ToBase64String(buffer);
                Properties.Settings.Default.Save();
            }
        }
    }
}
