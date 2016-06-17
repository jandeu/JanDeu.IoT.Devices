using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.I2c;

namespace JanDeu.IoT.Devices.Mpl3115a2
{
  static class I2cDeviceExtensions
  {
    public static void WriteRead(this I2cDevice i2c, byte write, byte[] readBuffer)
    {
      i2c.WriteRead(new byte[] { write }, readBuffer);
    }

    public static void WriteByte(this I2cDevice i2c, byte address, byte regValue)
    {
      i2c.Write(new byte[] { address, regValue });
    }

    public static byte ReadByte(this I2cDevice i2c, byte address)
    {
      var readBuffer = new byte[1];
      i2c.WriteRead(new byte[] { address }, readBuffer);
      return readBuffer[0];
    }

    public static byte[] ReadBytes(this I2cDevice i2c, byte address, int numberOfBytesToRead)
    {
      var readBuffer = new byte[numberOfBytesToRead];
      i2c.WriteRead(new byte[] { address }, readBuffer);
      return readBuffer;
    }
  }
}
