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
    /// Represents the divice registry values
    /// </summary>
    public static class Register
    {
      /// <summary>
      /// Sensor Status Register
      /// </summary>
      public const byte STATUS = 0x00;

      /// <summary>
      /// Preassure Data Out MSB
      /// </summary>
      public const byte OUP_P_MSB = 0x01;
      /// <summary>
      /// Preassure Data Out CSB
      /// </summary>
      public const byte OUP_P_CSB = 0x02;
      /// <summary>
      /// Preassure Data Out LSB
      /// </summary>
      public const byte OUP_P_LSB = 0x03;

      /// <summary>
      /// Temperature Data Out MSB
      /// </summary>
      public const byte OUT_T_MSB = 0x04;
      /// <summary>
      /// /// Temperature Data Out LSB
      /// </summary>
      public const byte OUT_T_LSB = 0x05;

      /// <summary>
      /// PT Data Configuration Register
      /// </summary>
      public const byte PT_DATA_CFG = 0x13;

      /// <summary>
      /// Control register 1
      /// </summary>
      public const byte CTRL_REG1 = 0x26;

      /// <summary>
      /// Device Identification Register
      /// </summary>
      public const byte WHO_AM_I = 0x0c;
    }
  }
  
}
