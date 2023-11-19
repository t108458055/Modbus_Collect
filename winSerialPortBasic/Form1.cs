using System;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;

namespace winSerialPortBasic
{
    public partial class Form1 : Form
    {
        //Step 1 先建立 SerailPort 類別的執行個體並將它指派給comport變數。
        private SerialPort comport;
        // 這變數是為了確認是否資料正在傳送中
        private bool sending;
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            sending = false;
        }
        /**
           這篇文的主要大概就是說明
       (1) SerialPort.Open 方法
       (2) SerialPort.IsOpen 屬性
       (3) SerialPort.Write 方法 (Byte[], Int32, Int32)
       (4) SerialPort.Close 方法

          ref: https://dotblogs.com.tw/billchung/2012/01/12/65445
         */
        private void button1_Click(object sender, EventArgs e)
        {
            comport = new SerialPort("COM4", 9600, Parity.None, 8, StopBits.One);



            // Step 2 建立一個Byte型別的陣列，我們的例子很簡單就送一個Byte，值也固定為1；這邊要注意的是 Visual Basic 和 C# 宣告陣列大小的用法不同，Visual Basic 宣告的是上限，而 C# 宣告的是個數。
            byte[] buffer = new byte[1];
            buffer[0] = 1;
            // Step 3 使用SerialPort.IsOpen屬性來判斷序列埠是否有被開啟，如果沒有的話則使用SerialPort.Open方法來開啟序列埠
            if (!comport.IsOpen)
            {
                comport.Open();
                /**
                 *         private SerialPort comport1;
                 *         private SerialPort comport2;
                  (3-1) SerialPort.IsOpen屬性其實只有指示特定執行個體中IsOpen屬性值，它和實際的序列埠沒有關係，
                它只是當這個執行個體執行SerialPort.Open方法執行成功會將這個值改變為True，
                而當SerialPort.Close方法執行成功時會改變為Fasle，以下列的程式碼來示範，
                我使用了兩個變數個別指向兩個SerialPort執行個體，而指定的序列埠都是COM4，
                雖然comport1已經將COM4開啟了，但comport2.IsOpen的屬性依然是False，
                也就是說這個屬性並不是真的去偵測該序列埠是否真的被開啟，
                而只是一個內部的變數記錄而已。
                    comport1 = new SerialPort("COM4", 9600, Parity.None, 8, StopBits.One);
                    comport2 = new SerialPort("COM4", 9600, Parity.None, 8, StopBits.One);
                    Open(comport1);
                    Open(comport2);

                 */
            }
            //   Step 4 當SerialPort已經開啟以後就使用SerialPort.Write 方法 (Byte[], Int32, Int32)將前面的Byte陣列送出去，這個方法的參數1是你所要傳送的資料、參數二是你要從這個陣列的哪個位置開始送，參數三則是你要送的資料有多長。comport.Write(buffer, 0, buffer.Length) 這個寫法就是代表要送的資料存在buffer這個陣列中；從陣列的第一個元素 (它的Index是0)；整個buffer我都要送，所以直接用buffer.Length，要多長就多長這樣。
            //Write(要傳送的資料, 從這個陣列的哪個位置開始送, 要送的資料有多長);
            comport.Write(buffer, 0, buffer.Length);
            //    Step 5 - 發送結束後呼叫SerialPort.Close方法將序列埠關閉。
            comport.Close();
        }
        // 建立一個簡單的純發送程式(多緒型範例)
        private void button2_Click(object sender, EventArgs e)
        {
            /**
             Click事件委派函式中做了一個檢查sending值的動作，
            這個動作在確保不會在SerialPort正在傳送資料的時候，又同時再度呼叫Write，
            其實還可以更機車的在呼叫SerilPort.Write之前先讓Button1的Enabled屬性變成False，
            在Finally區塊再恢復成True，不過這牽涉到跨執行緒UI委派的問題，
            我打算以後再來寫這一部份。
            然候產生一個執行緒並使用IsBackground屬性將其設為背景執行緒，
            在這個例子中是否是背景執行緒倒是沒這麼重要，
            但如果今天執行緒中跑的是無窮迴圈的時候，IsBackground這件事就相當重要了
            ，因為背景執行緒會因為呼叫端程式的關閉而結束
            ，但如果你有一個無窮迴圈的前景執行緒，
            當你的主程式關了之後它還是繼續一直在跑。
             */
            comport = CreateComport(comport);
            if (!sending && OpenComport(comport))
            {
                Thread t = new Thread(SendData);
                t.IsBackground = true;
                t.Start(comport as Object);
            }
        }
        // 檢查當comport變數沒有指向任何一個SerialPort執行個體時則產生一個執行個體。
        /// <summary>
        /// 創建新的序列埠執行個體物件
        /// </summary>
        private SerialPort CreateComport(SerialPort serialPort) 
        {
            if (serialPort == null)
            {
                serialPort = new SerialPort("COM4", 9600, Parity.None, 8, StopBits.One);
            }
            return serialPort;
        }
        // 開啟序列埠
        /// <summary>
        /// 開啟序列埠
        /// </summary>
        private bool OpenComport(SerialPort serialPort) 
        {
            try
            {
                if ((serialPort != null) && (!serialPort.IsOpen))
                {
                    serialPort.Open();
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("出問題啦:{0}", ex.ToString()));
                return false;
            }
        }
        /// <summary>
        /// 關閉序列埠
        /// </summary>
        private bool CloseComport(SerialPort serialPort)
        {
            try
            {
                if ((serialPort != null) && (!serialPort.IsOpen) && (!sending))
                {
                    serialPort.Close();
                    MessageBox.Show("序列埠已關閉");
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("出問題啦:{0}", ex.ToString()));
                return false;
            }
        }
        // 傳送資料
        private void SendData(Object obj) 
        {
            // 是先建立一個Byte陣列
            Byte[] buffer = new Byte[1024];
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = (Byte)(i % 256);
            }
            sending = true;
            try
            {
                // 呼叫Write方法傳送出去
                (obj as SerialPort).Write(buffer, 0, buffer.Length);
            }
            catch (Exception ex)
            {
                CloseComport((obj as SerialPort));
                MessageBox.Show(String.Format("出問題啦:{0}", ex.ToString()));
            }
            finally 
            {
                sending = false;
            }
        }

        private void Open(SerialPort comport)
        {
            if (!comport.IsOpen)
            {
                comport.Open();
            }
        }

    
    }
}
