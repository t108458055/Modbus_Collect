using System;
using System.IO.Ports;

namespace Modbus_Poll
{
    internal class Modbus :IDisposable
    {
        //DATA Field
        private SerialPort _sp =  new SerialPort();
        /// <summary>
        /// 狀態顯示
        /// </summary>
        public string  modbusStatusStr;
        // Constructor / Deconstructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Modbus()
        {                
        }
        /// <summary>
        ///  Deconstructor
        /// </summary>
        ~Modbus() 
        {
        }
        /// <summary>
        /// Stage 1 Open
        /// </summary>
        public bool IsOpen(string portName, int baudRate, int dataBits,
            Parity parity, StopBits stopBits) 
        {
            /**
             Serial Port 系列(4) 基本篇--基本屬性 | .Net 海角點部落 - 點部落
                https://dotblogs.com.tw/billchung/2012/01/11/65270
             */
            // Ensure port isn't already opened
            if (!_sp.IsOpen)
            {
                // Assign desired setting s to the serial port:
                _sp.PortName = portName; // Serial PortName　序列埠
                _sp.BaudRate = baudRate; // Serial BaudRate　　鮑率
                _sp.DataBits = dataBits;     // 資料長度 5 ~ 8 bit
                _sp.Parity = parity;             //Serial  Parity  同位元檢查　
                _sp.StopBits = stopBits;      //  資料停止位元
                // These timeouts are default and  cannot be editted through the class at this point:
                _sp.ReadTimeout = 1000;  // 10sec
                _sp.WriteTimeout = 1000;
                try
                {
                    _sp.Open();            
                }
                catch (Exception ex)
                {
                    modbusStatusStr = $"錯誤Error，開啟{portName}事件產生：{ex.Message}";
                    return false;
                }
                modbusStatusStr = $"{portName}開啟成功(Open successfully.)";
                return true;
            }
            else
            {
                modbusStatusStr = $"{portName}已經開啟(already Opened.)";
                return false;
            }
        }
        /// <summary>
        /// Stage 2 Close
        /// </summary>
        public bool IsClose() 
        {
            //Ensure port is opened before attempting to close:
            if (_sp.IsOpen)
            {
                try
                {
                    _sp.Close();
                }
                catch (Exception ex)
                {
                    modbusStatusStr = $"錯誤Error，關閉{_sp.PortName}事件產生：{ex.Message}";              
                    return false;
                }
                modbusStatusStr = $"關閉{_sp.PortName}成功(closed successfully.)";
                return true;
            }
            else
            {
                modbusStatusStr = $"錯誤Error ! {_sp.PortName}未開啟(is not open.)";                
                return false;
            }
        }
        //
        public void Dispose()
        {
            // 實現 IDisposable 確保資源被正確釋放
           _sp.Dispose();
        }
        /// <summary>
        /// CRC Computation
        /// </summary>
        private void GetCRC(byte[] message, ref byte[] CRC)
        {
            //Function expects a modbus message of any length as well as a 2 byte CRC array in which to 
            //return the CRC values:

            ushort CRCFull = 0xFFFF;
            byte CRCHigh = 0xFF, CRCLow = 0xFF;
            char CRCLSB;

            for (int i = 0; i < (message.Length) - 2; i++)
            {
                CRCFull = (ushort)(CRCFull ^ message[i]);

                for (int j = 0; j < 8; j++)
                {
                    CRCLSB = (char)(CRCFull & 0x0001);
                    CRCFull = (ushort)((CRCFull >> 1) & 0x7FFF);

                    if (CRCLSB == 1)
                    {
                        CRCFull = (ushort)(CRCFull ^ 0xA001);
                    }
                }
            }
            CRC[1] = CRCHigh = (byte)((CRCFull >> 8) & 0xFF);
            CRC[0] = CRCLow = (byte)(CRCFull & 0xFF);
        }
        // Stage 5 ChckCRC
        /// <summary>
        /// CRC Computation
        /// </summary>
        private void CalculateModbusCRC(byte[] msg, ref byte[] CRC)
        {
            // Modbus CRC calculation
            try
            {
                ushort CRCFull = 0xFFFF;
                const ushort CRCPolynomial = 0xA001;

                for (int i = 0; i < msg.Length - 2; i++)
                {
                    CRCFull ^= msg[i];

                    for (int j = 0; j < 8; j++)
                    {
                        if ((CRCFull & 0x0001) == 1)
                        {
                            CRCFull = (ushort)((CRCFull >> 1) ^ CRCPolynomial);
                        }
                        else
                        {
                            CRCFull = (ushort)(CRCFull >> 1);
                        }
                    }
                }

                CRC[0] = (byte)(CRCFull & 0xFF);      // Low byte
                CRC[1] = (byte)(CRCFull >> 8);        // High byte

            }
            catch (Exception ex)
            {
                modbusStatusStr = $"錯誤Error：{ex.Message}";
            }
        }
        /// <summary>
        ///  Build Message
        /// </summary>
        private void BuildMessage(byte address, byte type, ushort start, ushort registers, ref byte[] message)
        {
            //Array to receive CRC bytes:
            byte[] CRC = new byte[2];

            message[0] = address;
            message[1] = type;
            message[2] = (byte)(start >> 8);
            message[3] = (byte)start;
            message[4] = (byte)(registers >> 8);
            message[5] = (byte)registers;

            // GetCRC(message, ref CRC);
            CalculateModbusCRC(message, ref CRC);
            message[message.Length - 2] = CRC[0];
            message[message.Length - 1] = CRC[1];
        }
        /// <summary>
        /// Check Response
        /// </summary>
        private bool CheckResponse(byte[] response)
        {
            //Perform a basic CRC check:
            byte[] CRC = new byte[2];
            GetCRC(response, ref CRC);
            if (CRC[0] == response[response.Length - 2] && CRC[1] == response[response.Length - 1])
                return true;
            else
                return false;
        }
        /// <summary>
        /// Get Response
        /// </summary>
        private void GetResponse(ref byte[] response)
        {
            //There is a bug in .Net 2.0 DataReceived Event that prevents people from using this
            //event as an interrupt to handle data (it doesn't fire all of the time).  Therefore
            //we have to use the ReadByte command for a fixed length as it's been shown to be reliable.
            try
            {
                for (int i = 0; i < response.Length; i++)
                {
                    response[i] = (byte)(_sp.ReadByte());
                }
            }
            catch (Exception ex)
            {
                modbusStatusStr = $"錯誤Error：{ex.Message}";
            }
        }
        /// <summary>
        ///  Stage 4 Write
        /// Function 16 - Write Multiple Registers
        /// </summary>
        public bool SendFunc16(byte address, ushort start, ushort registers, short[] values)
        {
            //Ensure port is open:
            if (_sp.IsOpen)
            {
                //Clear in/out buffers:
                _sp.DiscardOutBuffer();
                _sp.DiscardInBuffer();
                //Message is 1 addr + 1 fcn + 2 start + 2 reg + 1 count + 2 * reg vals + 2 CRC
                byte[] message = new byte[9 + 2 * registers];
                //Function 16 response is fixed at 8 bytes
                byte[] response = new byte[8];

                //Add bytecount to message:
                message[6] = (byte)(registers * 2);
                //Put write values into message prior to sending:
                for (int i = 0; i < registers; i++)
                {
                    message[7 + 2 * i] = (byte)(values[i] >> 8);
                    message[8 + 2 * i] = (byte)(values[i]);
                }
                //Build outgoing message:
                BuildMessage(address, (byte)16, start, registers, ref message);

                //Send Modbus message to Serial Port:
                try
                {
                    _sp.Write(message, 0, message.Length);
                    GetResponse(ref response);
                }
                catch (Exception ex)
                {
                    modbusStatusStr = $"錯誤Error，寫入之事件產生(Error in write event)：{ex.Message}";
                    return false;
                }
                //Evaluate message:
                if (CheckResponse(response))
                {
                    modbusStatusStr = $"寫入成功!(Write successful!)";
                    return true;
                }
                else
                {
                    modbusStatusStr = $"CRC錯誤Error!!(CRC error!)";
                    return false;
                }
            }
            else
            {
                modbusStatusStr = $"序列埠未開啟!(Serial port not open.)";
                return false;
            }
        }
        /// <summary>
        /// Stage 3 Read
        /// Function 3 - Read Registers
        /// </summary>
        public bool SendFunc3(byte address, ushort start, ushort registers, ref short[] values)
        {
            //Ensure port is open:
            if (_sp.IsOpen)
            {
                //Clear in/out buffers:
                _sp.DiscardOutBuffer();
                _sp.DiscardInBuffer();
                //Function 3 request is always 8 bytes:
                byte[] message = new byte[8];
                //Function 3 response buffer:
                byte[] response = new byte[5 + 2 * registers];
                //Build outgoing modbus message:
                BuildMessage(address, (byte)3, start, registers, ref message);
                //Send modbus message to Serial Port:
                try
                {
                    _sp.Write(message, 0, message.Length);
                    GetResponse(ref response);
                }
                catch (Exception ex)
                {
                    modbusStatusStr = $"錯誤Error，讀取之事件產生(Error in read event:)：{ex.Message}";
                    return false;
                }
                //Evaluate message:
                if (CheckResponse(response))
                {
                    //Return requested register values:
                    for (int i = 0; i < (response.Length - 5) / 2; i++)
                    {
                        values[i] = response[2 * i + 3];
                        values[i] <<= 8;
                        values[i] += response[2 * i + 4];
                    }
                    modbusStatusStr = $"讀取成功!(Read successful!)";
                    return true;
                }
                else
                {
                    modbusStatusStr = $"CRC錯誤(CRC error)";
                    return false;
                }
            }
            else
            {
                modbusStatusStr = $"序列埠未開啟!(Serial port not open.)";
                return false;
            }
        }
        class SP_ConnectSet
        {
            public string portName { get; }
            public int baudRate { get; }
            public int dataBits { get; }
            public Parity parity { get; }
            public StopBits stopBits { get; }
            public SP_ConnectSet(string portName, int baudRate, int dataBits, 
                Parity parity, StopBits stopBits) 
            {
                this.portName = portName;
                this.baudRate = baudRate;
                this.dataBits = dataBits;
                this.parity = parity;
                this.stopBits = stopBits;
            }
        }
    }
}
