
namespace Modbus_Poll
{
    public class Enums
    {
        public string GetValues(string input)
        {
            return $"{input}SB";
        }
    }

    public  enum BaudRate
    {
       
        Baud1200 = 1200,
        Baud2400 = 2400,
        Baud4800 = 4800,
        Baud9600 = 9600,
        Baud19200 = 19200,
        Baud38400 = 38400,
        Baud57600 = 57600,
        Baud115200 = 115200,
        Baud230400 = 230400,
        Baud345600 = 345600,
        Baud460800 = 460800,
        Baud576000 = 576000,
        Baud921600 = 921600,
        Baud1382400 = 1382400,
        UnknownBaud = -1
    };

    public enum DataType
    {
        Decimal,
        Hexadecimal,
        Float,
        Reverse
    }
}
