using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SerialPortListener.Serial;
using System.IO;

namespace SerialPortListener
{
    public partial class MainForm : Form
    {
        SerialPortManager _spManager;
        public MainForm()
        {
            InitializeComponent();

            UserInitialization();
        }

      
        private void UserInitialization()
        {
            _spManager = new SerialPortManager();
            SerialSettings mySerialSettings = _spManager.CurrentSerialSettings;
            serialSettingsBindingSource.DataSource = mySerialSettings;
            portNameComboBox.DataSource = mySerialSettings.PortNameCollection;
            baudRateComboBox.DataSource = mySerialSettings.BaudRateCollection;
            dataBitsComboBox.DataSource = mySerialSettings.DataBitsCollection;
            parityComboBox.DataSource = Enum.GetValues(typeof(System.IO.Ports.Parity));
            stopBitsComboBox.DataSource = Enum.GetValues(typeof(System.IO.Ports.StopBits));

            _spManager.NewSerialDataRecieved += new EventHandler<SerialDataEventArgs>(_spManager_NewSerialDataRecieved);
            this.FormClosing += new FormClosingEventHandler(MainForm_FormClosing);
        }

        
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _spManager.Dispose();   
        }

        void _spManager_NewSerialDataRecieved(object sender, SerialDataEventArgs e)
        {
            if (this.InvokeRequired)
            {
                // Using this.Invoke causes deadlock when closing serial port, and BeginInvoke is good practice anyway.
                this.BeginInvoke(new EventHandler<SerialDataEventArgs>(_spManager_NewSerialDataRecieved), new object[] { sender, e });
                return;
            }

            int maxTextLength = 1000; // maximum text length in text box
            if (tbData.TextLength > maxTextLength)
                tbData.Text = tbData.Text.Remove(0, tbData.TextLength - maxTextLength);

            // This application is connected to a GPS sending ASCCI characters, so data is converted to text
            string str = Encoding.ASCII.GetString(e.Data);
            tbData.AppendText(str);
            tbData.ScrollToCaret();

        }

        // Handles the "Start Listening"-buttom click event
        private void btnStart_Click(object sender, EventArgs e)
        {
            // _spManager.StartListening();
            // Lấy các thông số cài đặt serial hiện tại
            SerialSettings currentSettings = _spManager.CurrentSerialSettings;

            // Kiểm tra xem các thông số đã chọn có khớp với các thông số của serial port không
            if (currentSettings.PortName == portNameComboBox.SelectedItem.ToString() &&
                currentSettings.BaudRate == int.Parse(baudRateComboBox.SelectedItem.ToString()) &&
                currentSettings.DataBits == int.Parse(dataBitsComboBox.SelectedItem.ToString()) &&
                currentSettings.Parity == (System.IO.Ports.Parity)parityComboBox.SelectedItem &&
                currentSettings.StopBits == (System.IO.Ports.StopBits)stopBitsComboBox.SelectedItem)
            {
                // Nếu các thông số khớp, bắt đầu lắng nghe trên cổng serial
                _spManager.StartListening();
            }
            else
            {
                // Nếu các thông số không khớp, hiển thị thông báo cho người dùng
                MessageBox.Show("Các thiết lập không khớp. Vui lòng chọn lại các thông số.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        // Handles the "Stop Listening"-buttom click event
        private void btnStop_Click(object sender, EventArgs e)
        {
            _spManager.StopListening();
        }

        private void portNameComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void portNameLabel_Click(object sender, EventArgs e)
        {

        }
    }
}
