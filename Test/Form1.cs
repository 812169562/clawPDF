using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string hexString = "B9ABCBBEB9A4BBE1B9D8D3DABFAAD5B932303234C4EAC8FDB0CBB8BEC5AEBDDACFB5C1D0BBEEB6AFA1B0D1C5C8A4C9FABBEEA3ACBFAAB0F6D1B0D6E9A1B1BBEEB6AFB5C4CDA8D6AA2E646F63"; // 对应于"Hello"
            byte[] byteArray = HexStringToByteArray(hexString);
            var aa = Encoding.UTF8.GetString(byteArray);
            Console.WriteLine(Encoding.UTF8.GetString(byteArray)); // 输出: Hello

            byte[] byteArray2 = new byte[] { 0x48, 0x65, 0x6c, 0x6c, 0x6f }; // 对应于"Hello"
            string hexString2 = ByteArrayToHexString(byteArray2);
            Console.WriteLine(hexString2); // 输出: 48656c6c6f
        }

        public static string ByteArrayToHexString(byte[] ba)
        {
            var hex = new StringBuilder(ba.Length * 2);
            foreach (var b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
        public static byte[] HexStringToByteArray(string hex)
        {
            if (hex.Length % 2 != 0)
                throw new ArgumentException(nameof(hex));

            var numberOfCharacters = hex.Length;
            var returnArray = new byte[numberOfCharacters / 2];

            for (var i = 0; i < numberOfCharacters; i += 2)
                returnArray[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);

            return returnArray;
        }
    }
}
