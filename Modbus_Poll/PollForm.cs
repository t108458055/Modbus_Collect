using System;
using System.IO.Ports;
using System.Timers;
using System.Windows.Forms;

namespace Modbus_Poll
{
    public partial class PollForm : Form
    {
        //DATA Field
        Modbus _mb = new Modbus();
        System.Timers.Timer _timer = new System.Timers.Timer();
        string dataType;
        bool isPolling = false;
        int pollCount;
        // GUI Delegate Declarations Step 2.1
        public delegate void GUIDelegate(string paramStr);
        public delegate void GUIClear();
        public delegate void GUIStatus(string paramStr);

        public PollForm()
        {
            InitializeComponent();
            LoadListboxes();
            _timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
        }

        // Step 2.2 Delegate Functions
        public void DoGUIClear()
        {
            if (this.InvokeRequired)
            {
                GUIClear delegateMethod = new GUIClear(this.DoGUIClear);
                this.Invoke(delegateMethod);
            }
            else
            {
                this.lstRegisterValues.Items.Clear();
            }
        }
        public void DoGUIStatus(string paramStr)
        {
            if (this.InvokeRequired)
            {
                GUIStatus delegateMethod = new GUIStatus(this.DoGUIStatus);
                this.Invoke(delegateMethod, new object[] { paramStr });
            }
            else
            {
                this.lblStatus.Text = paramStr;
            }
      
        }
        public void DoGUIUpdate(string paramStr)
        {
            //if (this.InvokeRequired)
            //{
            //    GUIDelegate delegateMethod = new GUIDelegate(this.DoGUIUpdate);
            //    this.Invoke(delegateMethod, new object[] { paramStr });
            //}
            //else
            //{
            //    this.lstRegisterValues.Items.Add(paramStr);
            //}
            if (lstRegisterValues.InvokeRequired)
            {
                Invoke((Action)(() => lstRegisterValues.Items.Add(paramStr)));
            }
            else
            {
                lstRegisterValues.Items.Add(paramStr);
            }
        }

        //Step 2
        /// <summary>
        /// Timer Elapsed Event Handler
        /// </summary>
        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            PollFunc();
        }

        // Step 1
        /// <summary>
        /// 載入
        /// </summary>
        private void LoadListboxes()
        {
            //Three to load - ports, baudrates, datetype.  Also set default textbox values:
            //1) Available Ports:
            //////string[] ports = SerialPort.GetPortNames();
            //////if (ports.Length != 0)
            //////{
            //////    foreach (string port in ports)
            //////    {
            //////        lstPorts.Items.Add(port);
            //////    }
            //////    lstPorts.SelectedIndex = 0;
            //////}
            var ports = SerialPort.GetPortNames();
            if (ports.Length != 0)
            {
                lstPorts.Items.AddRange(ports);
                lstPorts.SelectedIndex = 0;
            }
          
            //2) Baudrates:
            foreach (BaudRate baudrate in Enum.GetValues(typeof(BaudRate)))
            {
                lstBaudrate.Items.Add((int)baudrate);
            }
            lstBaudrate.SelectedIndex = 1;

            //3) Datatype:
          //   string[] dataTypes = { "Decimal", "Hexadecimal", "Float", "Reverse" };
          //  BaudRate.
            foreach (DataType dataType in Enum.GetValues(typeof(DataType)))
            {
                lstDataType.Items.Add(dataType);
            }

            lstDataType.SelectedIndex = 0;

            //Textbox defaults:
            txtRegisterQty.Text = "20";
            txtSampleRate.Text = "1000";
            txtSlaveID.Text = "1";
            txtStartAddr.Text = "0";
        }
        //  Start and Stop Procedures Step 3.1
        /// <summary>
        /// 開啟
        /// </summary>
        private void StartPoll()
        {
            pollCount = 0;
            if (lstPorts.SelectedItem == null || lstBaudrate.SelectedItem == null)
            {
                MessageBox.Show("空!!");
                return;
            }
            //Open COM port using provided settings:
            if (_mb.IsOpen(lstPorts.SelectedItem.ToString(), Convert.ToInt32(lstBaudrate.SelectedItem.ToString()),
                8, Parity.None, StopBits.One))
            {
                //Disable double starts:
                btnStart.Enabled = false;
                dataType = lstDataType.SelectedItem.ToString();

                //Set polling flag:
                isPolling = true;

                //Start timer using provided values:
                _timer.AutoReset = true;
                //if (!string.IsNullOrEmpty(txtSampleRate.Text))
                //{
                //    _timer.Interval = Convert.ToDouble(txtSampleRate.Text);
                //}
                if (double.TryParse(txtSampleRate.Text, out double sampleRate))
                {
                    _timer.Interval = sampleRate;
                }
                else
                {
                    _timer.Interval = 1000;
                }
                _timer.Start();
            }

            lblStatus.Text = _mb.modbusStatusStr;
        }
        /// <summary>
        /// 關閉
        /// </summary>
        private void StopPoll()
        {
            //Stop timer and close COM port:
            isPolling = false;
            _timer.Stop();
            _mb.IsClose();

            btnStart.Enabled = true;

            lblStatus.Text = _mb.modbusStatusStr;
        }
        /// <summary>
        ///  按鈕事件-開始
        /// </summary>
        private void btnStart_Click(object sender, EventArgs e)
        {
            StartPoll();
        }
        /// <summary>
        ///  重新掃COM
        /// </summary>
        private void btnReScan_Click(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();
            if (ports.Length != 0)
            {
                foreach (string port in ports)
                {
                    lstPorts.Items.Add(port);
                }
                lstPorts.SelectedIndex = 0;
            }
            MessageBox.Show("已重新掃");
        }
        /// <summary>
        /// 按鈕事件-停止
        /// </summary>
        private void btnStop_Click(object sender, EventArgs e)
        {
            StopPoll();
        }
        // Step 3 Read   Poll Function  Start  then  Stop ...
        private void PollFunc()
        {
            //Update GUI:
            DoGUIClear();
            pollCount++;
            DoGUIStatus("Poll count: " + pollCount.ToString());
            //Create array to accept read values:
            short[] values = new short[Convert.ToInt32(txtRegisterQty.Text)];
            ushort pollStart;
            ushort pollLength;

            if (!string.IsNullOrEmpty(txtStartAddr.Text))
            {
                pollStart = Convert.ToUInt16(txtStartAddr.Text);
            }
            else
            {
                pollStart = 0;
            }
            if (!string.IsNullOrEmpty(txtRegisterQty.Text))
            {
                pollLength = Convert.ToUInt16(txtRegisterQty.Text);
            }
            else
            {
                pollLength = 20;
            }

            //Read registers and display data in desired format:
            try
            {
                while (!_mb.SendFunc3(Convert.ToByte(txtSlaveID.Text), pollStart, pollLength, ref values)) ;
            }
            catch (Exception err)
            {
                DoGUIStatus("ModBus讀取錯誤(Error in modbus read) " + err.Message);
            }

            string itemString;
            switch (dataType)
            {
                case "Decimal":
                    for (int i = 0; i < pollLength; i++)
                    {
                        itemString = "[" + Convert.ToString(pollStart + i + 40001) + "] , MB[" +
                            Convert.ToString(pollStart + i) + "] = " + values[i].ToString();
                        DoGUIUpdate(itemString);
                    }
                    break;
                case "Hexadecimal":
                    for (int i = 0; i < pollLength; i++)
                    {
                        itemString = "[" + Convert.ToString(pollStart + i + 40001) + "] , MB[" +
                            Convert.ToString(pollStart + i) + "] = " + values[i].ToString("X");
                        DoGUIUpdate(itemString);
                    }
                    break;
                case "Float":
                    for (int i = 0; i < (pollLength / 2); i++)
                    {
                        int intValue = (int)values[2 * i];
                        intValue <<= 16;
                        intValue += (int)values[2 * i + 1];
                        itemString = "[" + Convert.ToString(pollStart + 2 * i + 40001) + "] , MB[" +
                            Convert.ToString(pollStart + 2 * i) + "] = " +
                            (BitConverter.ToSingle(BitConverter.GetBytes(intValue), 0)).ToString();
                        DoGUIUpdate(itemString);
                    }
                    break;
                case "Reverse":
                    for (int i = 0; i < (pollLength / 2); i++)
                    {
                        int intValue = (int)values[2 * i + 1];
                        intValue <<= 16;
                        intValue += (int)values[2 * i];
                        itemString = "[" + Convert.ToString(pollStart + 2 * i + 40001) + "] , MB[" +
                            Convert.ToString(pollStart + 2 * i) + "] = " +
                            (BitConverter.ToSingle(BitConverter.GetBytes(intValue), 0)).ToString();
                        DoGUIUpdate(itemString);
                    }
                    break;
            }
        }

        // Step 4. Write Function
        /// <summary>
        ///  寫入函式
        /// </summary>
        private void WriteFunc()
        {
            //StopPoll();
           
            if (!string.IsNullOrEmpty(txtWriteRegister.Text) && !string.IsNullOrEmpty(txtWriteValue.Text)
                && !string.IsNullOrEmpty(txtSlaveID.Text))
            {
                byte address = Convert.ToByte(txtSlaveID.Text);
                ushort start = Convert.ToUInt16(txtWriteRegister.Text);
                short[] value = new short[1];
                value[0] = Convert.ToInt16(txtWriteValue.Text);

                try
                {
                    while (!_mb.SendFunc16(address, start, (ushort)1, value)) ;
                }
                catch (Exception err)
                {
                    DoGUIStatus("寫入錯誤(Error in write function) " + err.Message);
                }
                DoGUIStatus(_mb.modbusStatusStr);
            }
            else
                DoGUIStatus("在嘗試寫入之前輸入所有欄位(Enter all fields before attempting a write)");

            //StartPoll();
        }
        /// <summary>
        ///  按鈕事件-寫入
        /// </summary>
        private void btnWrite_Click(object sender, EventArgs e)
        {
            WriteFunc();
        }
        //  Data Type Event Handler
        private void lstDataType_SelectedIndexChanged(object sender, EventArgs e)
        {
            //restart the data poll if datatype is changed during the process:
            if (isPolling)
            {
                StopPoll();
                dataType = lstDataType.SelectedItem.ToString();
                StartPoll();
            }

        }
    }

}
