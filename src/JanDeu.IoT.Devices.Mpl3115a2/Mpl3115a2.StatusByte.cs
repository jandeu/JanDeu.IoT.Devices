using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JanDeu.IoT.Devices.Mpl3115a2
{
  public partial class Mpl3115a2
  {
    /// <summary>
    /// Represents the device status byte
    /// </summary>
    [Flags]
    public enum StatusByte : Byte
    {
      /// <summary>
      /// Temperatire new data avaiable
      /// </summary>
      TDR = 2,
      /// <summary>
      /// Pressure/Altitude new data avaiable
      /// </summary>
      PDR = 4,
      /// <summary>
      /// Pressure/Altitude or Temperature data ready
      /// </summary>
      PTDR = 8,
      /// <summary>
      /// Temperature data override
      /// </summary>
      TOW = 32,
      /// <summary>
      /// Pressure/Altitude data override
      /// </summary>
      POW = 64,
      /// <summary>
      /// Pressure/Altitude or Temperature data override
      /// </summary>
      PTOW = 128
    }
  }

 
}
