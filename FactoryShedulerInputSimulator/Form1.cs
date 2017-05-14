using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FactoryShedulerInputSimulator
{
    public partial class Form1 : Form
    {
        int lastAddress = 0;
        Dictionary<int, Device> devices = new Dictionary<int, Device>();

        public Form1()
        {
            InitializeComponent();

            setDefaultMapPoints();

            startServer();
        }

        /// <summary>
        /// Nastartuje server pro požadavky na stav zařízení
        /// </summary>
        public void startServer() {
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 6666);
            UdpClient newsock = new UdpClient(ipep);

            BackgroundWorker bw = new BackgroundWorker();

            bw.DoWork += new DoWorkEventHandler(delegate (object o, DoWorkEventArgs args)
            {
                while (true)
                {
                    readRequest(newsock);
                }
            });

            bw.RunWorkerAsync();
        }

        /// <summary>
        /// Přečte jeden UDP request
        /// </summary>
        private void readRequest(UdpClient sock)
        {
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            byte[] data = sock.Receive(ref sender);
            if (data.Length == 1)
            {
                int address = data[0];

                byte[] dataBack;
                if (devices.ContainsKey(address))
                {
                    dataBack = new byte[] { (byte)address, (byte)devices[address].type, (byte)devices[address].status };
                }
                else {
                    dataBack = new byte[] { (byte)address };
                }

                sock.Send(dataBack, dataBack.Length, sender);
            }
        }

        private void sendStatus(string type, int address, char status)
        {
            try
            {
                UdpClient udpClient = new UdpClient();
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5555);
                udpClient.Connect(endPoint);

                byte[] sendBytes = new byte[] { (byte)address, (byte)getTypeChar(type), (byte)status };

                udpClient.Send(sendBytes, sendBytes.Length);

                // Blocks until a message returns on this socket from a remote host.
                Byte[] receiveBytes = udpClient.Receive(ref endPoint);

                udpClient.Close();

                label1.Text = "";
            }
            catch (Exception e)
            {
                label1.Text = "Připojení selhalo.";
            }
        }

        

        private char getStatusChar(string status, string type) {
            switch (type)
            {
                case "Prázdné kanistry":
                    switch (status)
                    {
                        case "Volno": return 'F';
                        case "Plno": return 'T';
                        default: return 'X';
                    }
                case "Plnící stanice":
                    switch (status)
                    {
                        case "Naplněno": return 'F';
                        case "Plní": return 'T';
                        default: return 'X';
                    }
                case "Nabíjecí stanice":
                    switch (status)
                    {
                        case "Volno": return 'F';
                        case "Obsazeno": return 'T';
                        default: return 'X';
                    }
                case "Konzumní stanice":
                    switch (status)
                    {
                        case "Prázdný kontejner": return 'F';
                        case "Bez kontejneru": return 'T';
                        case "Plný kontejner": return 'K';
                        default: return 'X';
                    }
                default: return 'X';
            }
        }

        private char getTypeChar(string type)
        {
            switch (type)
            {
                case "Prázdné kanistry":
                    return 'E';
                case "Plnící stanice":
                    return 'F';
                case "Nabíjecí stanice":
                    return 'C';
                case "Konzumní stanice":
                    return 'O';
                default: return 'X';
            }
        }

        private void setDefaultMapPoints()
        {
            addPlaceWithEmptyTanks();
            addPlaceWithFullTanks();
            addChargePlace();
            addConsumerPlace();
            addConsumerPlace();
            addConsumerPlace();
            addConsumerPlace();
            addConsumerPlace();
            addConsumerPlace();
            addConsumerPlace();
        }

        private void addPlaceWithEmptyTanks()
        {
            addType("Prázdné kanistry", ++lastAddress, 'F', new object[] { "Volno", "Plno" });
        }

        private void addPlaceWithFullTanks()
        {
            addType("Plnící stanice", ++lastAddress, 'F', new object[] { "Naplněno", "Plní" });
        }

        private void addChargePlace()
        {
            addType("Nabíjecí stanice", ++lastAddress, 'F', new object[] { "Volno" , "Obsazeno" });
        }

        private void addConsumerPlace()
        {
            addType("Konzumní stanice", ++lastAddress, 'F', new object[] { "Prázdný kontejner", "Bez kontejneru", "Plný kontejner" });
        }

        private void addType(string type, int address, char status, object[] statusValues)
        {
            devices.Add(address, new Device(status, getTypeChar(type)));
            Panel panel = new Panel();
            panel.BackColor = SystemColors.ControlDark;
            panel.BorderStyle = BorderStyle.FixedSingle;
            panel.Size = new Size(211, 108);

            ComboBox comboBox = new ComboBox();
            comboBox.Items.AddRange(statusValues);
            comboBox.Location = new Point(82, 77);
            comboBox.SelectedIndex = 0;
            comboBox.SelectedValueChanged += new EventHandler(delegate (object sender, EventArgs e)
            {
                sendStatus(type, address, getStatusChar(((ComboBox)sender).Text, type));
                devices[address].status = getStatusChar(((ComboBox)sender).Text, type);
        });

            Label labelType = new Label();
            labelType.AutoSize = true;
            labelType.Location = new Point(12, 12);
            labelType.Size = new Size(40, 17);
            labelType.Text = "Typ: ";

            Label labelAddress = new Label();
            labelAddress.AutoSize = true;
            labelAddress.Location = new System.Drawing.Point(12, 43);
            labelAddress.Name = "label3";
            labelAddress.Size = new System.Drawing.Size(61, 17);
            labelAddress.TabIndex = 1;
            labelAddress.Text = "Adresa: ";

            Label labelStete = new Label();
            labelStete.AutoSize = true;
            labelStete.Location = new System.Drawing.Point(12, 77);
            labelStete.Size = new System.Drawing.Size(44, 17);
            labelStete.Text = "Stav: ";

            Label labelTypeValue = new Label();
            labelTypeValue.AutoSize = true;
            labelTypeValue.Location = new System.Drawing.Point(79, 12);
            labelTypeValue.Size = new System.Drawing.Size(46, 17);
            labelTypeValue.Text = type;

            Label labelAddressValue = new Label();
            labelAddressValue.AutoSize = true;
            labelAddressValue.Location = new System.Drawing.Point(79, 43);
            labelAddressValue.Size = new System.Drawing.Size(46, 17);
            labelAddressValue.Text = address.ToString();

            panel.Controls.Add(comboBox);
            panel.Controls.Add(labelType);
            panel.Controls.Add(labelTypeValue);
            panel.Controls.Add(labelStete);
            panel.Controls.Add(labelAddress);
            panel.Controls.Add(labelAddressValue);

            flowLayoutPanel1.Controls.Add(panel);
            flowLayoutPanel1.ResumeLayout(false);
            panel.ResumeLayout(false);
            panel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

            sendStatus(type, address, status);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            addPlaceWithEmptyTanks();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            addPlaceWithFullTanks();
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            addChargePlace();
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            addConsumerPlace();
        }
    }
}
