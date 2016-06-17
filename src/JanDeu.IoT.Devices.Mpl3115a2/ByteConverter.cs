using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JanDeu.IoT.Devices.Mpl3115a2
{
  class ByteConverter
  {
    public static double Q18dot2(byte msb, byte csb, byte lsb)
    {
      int imsb = msb << 16;
      int icsb = csb << 8;
      int tmpRes = imsb | icsb | lsb;
      double r = tmpRes / 64.0;

      long wholePressure = (long)msb << 16 | (long)csb << 8 | lsb;// Pressure comes back as a left shifted 20 bit number
      wholePressure >>= 6;//Pressure is an 18 bit number with 2 bits of decimal. Get rid of decimal portion.
      lsb &= 48; //Bits 5/4 represent the fractional component
      lsb >>= 4; //Get it right aligned
      double pressure_decimal = lsb / 4.0; //Turn it into fraction
      double pressure = wholePressure + pressure_decimal;
      return pressure;
    }

    public static double Q12dot4(byte msb, byte lsb)
    {
      double fraction = (lsb >> 4) / 16.0;

      double temperature = msb + fraction;

      return temperature;
    }

  }
}
