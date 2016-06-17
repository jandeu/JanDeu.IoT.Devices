using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JanDeu.IoT.Devices.Mpl3115a2
{
  public class Mpl3115a2Data
  {
    internal Mpl3115a2Data()
    {

    }

    public DateTimeOffset ReadingDate { get; set; }

    public double Temperature { get; set; }

    public double Altitude { get; set; }

    public double Pressure { get; set; }
  }
}
