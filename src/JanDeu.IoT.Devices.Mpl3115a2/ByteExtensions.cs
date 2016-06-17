using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JanDeu.IoT.Devices.Mpl3115a2
{
  static class ByteExtensions
  {
    public static byte SetBit(this byte aByte, int pos, bool value)
    {
      if (value)
      {
        //left-shift 1, then bitwise OR
        return (byte)(aByte | (1 << pos));
      }
      else
      {
        //left-shift 1, then take complement, then bitwise AND
        return (byte)(aByte & ~(1 << pos));
      }
    }
  }
}
