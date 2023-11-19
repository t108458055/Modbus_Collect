using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadFromArduino
{
    internal class Program
    {
        static void Main(string[] args)
        {
            SerialInformation.GetPorts();
            var serialInformation = new SerialInformation();
            serialInformation.ReadFromPort();
            Console.ReadKey();
            serialInformation.Close();

        }



    }





    public class SerialInformation
    {
        public static void GetPorts()
        {
            Console.WriteLine("Serial ports available:");
            Console.WriteLine("-----------------------");
            foreach (var portName in SerialPort.GetPortNames())
            {
                Console.WriteLine(portName);
            }
        }

        private SerialPort _seiralPort = new SerialPort("COM4")  
        {
            BaudRate = 9600,
            Parity = Parity.None,
            StopBits = StopBits.One,
            DataBits = 8,
            Handshake = Handshake.None
        };
        public SerialInformation()
        {
                
        }

        public void ReadFromPort()
        {
            // Initialise the serial port on COM4.
            // obviously we would normally parameterise this, but
            // this is for demonstration purposes only.
            //var serialPort = new SerialPort("COM4")
            //{
            //    BaudRate = 9600,
            //    Parity = Parity.None,
            //    StopBits = StopBits.One,
            //    DataBits = 8,
            //    Handshake = Handshake.None
            //};

            // Subscribe to the DataReceived event.
            _seiralPort.DataReceived += SerialPortDataReceived;

            // Now open the port.
            _seiralPort.Open();
        }

        public void Close() 
        {
            _seiralPort.Close();
        }

        private void SerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var serialPort = (SerialPort)sender;

            // Read the data that's in the serial buffer.
            var serialdata = serialPort.ReadExisting();
            var sd = serialPort.ReadByte();
            var sesds = serialPort.BytesToRead;
           // Write to debug output.
        //   Debug.Write(serialdata);
        }

    }




}
