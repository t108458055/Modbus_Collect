using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EasyModbus;
namespace ModBusSlave
{
    public partial class Form1 : Form
    {
        ModbusServer modbusServer;
        public Form1()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (btnStart.Text == "START")
            {
                modbusServer = new ModbusServer();
                modbusServer.Listen();
                lblStatus.Text = "Status : Started";
                btnStart.Text = "STOP";

            }
            else if(btnStart.Text == "STOP")
            {
                modbusServer = null;
                lblStatus.Text = "Status : Stop";
                btnStart.Text = "START";
            }

        }

        private void btnSetVal_Click(object sender, EventArgs e)
        {
            int iAdr = int.Parse(txtRegAdr.Text);
            //short iVal = short.Parse(txtRegVal.Text);
            if (cboRegType.Text == "Holding Register")
            {
                short iVal = short.Parse(txtRegVal.Text);
                ModbusServer.HoldingRegisters regs = modbusServer.holdingRegisters;
                regs[iAdr] = iVal;
            }
            else if(cboRegType.Text == "Input Register")
            {
                short iVal = short.Parse(txtRegVal.Text);
                ModbusServer.InputRegisters regs = modbusServer.inputRegisters;
                regs[iAdr] = iVal;
            }
            else if (cboRegType.Text == "Digital Input")
            {
                bool iVal = false;
                if (txtRegAdr.Text == "1" || txtRegVal.Text.ToLower() == "true")
                {
                    iVal = true;
                }
                ModbusServer.DiscreteInputs regs = modbusServer.discreteInputs;
                regs[iAdr] = iVal ;
            }
            else if (cboRegType.Text == "Coil Output")
            {
                bool iVal = false;
                if (txtRegAdr.Text == "1" || txtRegVal.Text.ToLower() == "true")
                {
                    iVal = true;
                }
                ModbusServer.Coils regs = modbusServer.coils;
                    regs[iAdr] = iVal;
            }
        }
    }
}
