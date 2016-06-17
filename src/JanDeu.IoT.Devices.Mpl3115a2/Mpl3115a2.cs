using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;

namespace JanDeu.IoT.Devices.Mpl3115a2
{
  /// <summary>
  /// Represents a class for comunnication with MPL3115A2 device using I2C bus.
  /// The MPL3115A2 is a compact piezoresistive absolute pressure sensor with an I2C interface. 
  /// </summary>
  public partial class Mpl3115a2
  {


    /// <summary>
    /// Gets the I2C Bus name that the device is connected to.
    /// </summary>
    public string I2CBusName { get; }


    /// <summary>
    /// Creates new instalce of Mpl3115a2 class
    /// </summary>
    /// <param name="i2cBusName">The name of the bus that device is connected to. For example "I2C1"</param>
    /// <returns>Mpl3115a2 or null if inicialization failed</returns>
    public static async Task<Mpl3115a2> CreateDeviceAsync(string i2cBusName)
    {
      var result = new Mpl3115a2(i2cBusName);
      var initialized = await result.InitializeAsync();
      if (!initialized)
        return null;
      return result;
    }


    Mpl3115a2(string i2cBusName)
    {
      
      I2CBusName = i2cBusName;
    }

    /// <summary>
    /// Factory given Mpl3115a2 device I2C address
    /// </summary>
    const ushort Mpl3115a2I2cAddress = 0x0060;

    /// <summary>
    /// Gets the I2CDevice instance the device is connected to.
    /// </summary>
    public I2cDevice I2C { get; private set; }


    /// <summary>
    /// Tries to find and initialize device.
    /// </summary>
    /// <returns>True if the initialization is successfull</returns>
    async Task<bool> InitializeAsync()
    {
      string advancedQuerySyntax = I2cDevice.GetDeviceSelector(I2CBusName);
      DeviceInformationCollection deviceInformationCollection = await DeviceInformation.FindAllAsync(advancedQuerySyntax);
      string deviceId = deviceInformationCollection[0].Id;
      I2cConnectionSettings mpl3115a2Connection = new I2cConnectionSettings(Mpl3115a2I2cAddress);
      mpl3115a2Connection.BusSpeed = I2cBusSpeed.FastMode;
      mpl3115a2Connection.SharingMode = I2cSharingMode.Shared;
      this.I2C = await I2cDevice.FromIdAsync(deviceId, mpl3115a2Connection);
      if (I2C == null)
        return false;
      bool ok = I2C.ReadByte(Register.WHO_AM_I) == 0xc4;
      if (!ok)
        return false;
      I2C.WriteByte(Register.PT_DATA_CFG, 0x07);
      SetMode(Mode.Barometer);
      return true;
    }

    /// <summary>
    /// Reads the current temperature registry.
    /// </summary>
    /// <returns>Current temperature in Celsius Degrees</returns>
    public double ReadTemperature()
    {
      TakeMeassure(StatusByte.TDR);
      var data = I2C.ReadBytes(Register.OUT_T_MSB, 2);
      return ByteConverter.Q12dot4(data[0], data[1]);
    }

    /// <summary>
    /// Converts pressure to altitude
    /// </summary>
    /// <param name="pressure">Measured pressure</param>
    /// <returns>Altitude in meters</returns>
    public double GetAltitude(double pressure)
    {
      double altitude = 44330.77 * (1 - Math.Pow(pressure / 101326, 0.1902632));
      return altitude;
    }

    /// <summary>
    /// Reads the current preassure.
    /// </summary>
    /// <returns>Current pressure in Pascals</returns>
    public double ReadPressure()
    {
      TakeMeassure(waitForStatus: StatusByte.PDR);
      var data = I2C.ReadBytes(Register.OUP_P_MSB, 3);

      return ByteConverter.Q18dot2(data[0], data[1], data[2]);

    }


    /// <summary>
    /// Reads the current Pressure/Temperature and altitude in one operation.
    /// </summary>
    /// <returns>Returns <see cref="Mpl3115a2Data"/></returns>
    public Mpl3115a2Data Read()
    {
      TakeMeassure(StatusByte.TDR | StatusByte.PDR);
      byte[] data = I2C.ReadBytes(Register.OUP_P_MSB, 5);
      var result = new Mpl3115a2Data()
      {
        ReadingDate = DateTimeOffset.Now,
        Pressure = ByteConverter.Q18dot2(data[0], data[1], data[2]),
        Temperature = ByteConverter.Q12dot4(data[3], data[4])
      };
      result.Altitude = GetAltitude(result.Pressure);
      return result;
    }


    /// <summary>
    /// Reads the status registry and waiting 100ms to the given value. 
    /// </summary>
    /// <param name="status">Status to wait for</param>
    /// <exception cref="System.TimeoutException">If the status registry doesn't contains the status after 100ms</exception>
    void WaitForStatus(StatusByte status)
    {
      int counter = 0;
      while ((I2C.ReadByte(Register.STATUS) & (1 << 1)) == 0)
      {
        if (++counter > 100)
          throw new TimeoutException($"Operation timeouted while waiting for {status} status");
        Task.WaitAll(Task.Delay(1));

      }
    }


    /// <summary>
    /// Toggle the OST bit causing the sensor to immediately take another reading
    /// </summary>
    void TakeMeassure(StatusByte waitForStatus)
    {
      byte ctrlReg1 = I2C.ReadByte(Register.CTRL_REG1);
      ctrlReg1 = ctrlReg1.SetBit(1, false);//Clear OST bit
      ctrlReg1 = I2C.ReadByte(Register.CTRL_REG1);
      ctrlReg1 = ctrlReg1.SetBit(1, true); //Set OST byte
      I2C.WriteByte(Register.CTRL_REG1, ctrlReg1);

      int counter = 0;
      while ((I2C.ReadByte(Register.STATUS) & ((byte)waitForStatus)) == 0)
      {
        if (++counter > 100)
          throw new TimeoutException($"Operation timeouted while waiting for {waitForStatus} status");
        Task.WaitAll(Task.Delay(1));

      }

    }

    /// <summary>
    /// Sets the power bit of device
    /// </summary>
    /// <param name="power">StandBy or Active</param>
    public void SetPower(Power power)
    {
      byte ctrReg1 = I2C.ReadByte(Register.CTRL_REG1);
      switch (power)
      {
        case Power.StandBy:
          ctrReg1 = ctrReg1.SetBit(0, false);
          break;
        case Power.Active:
          ctrReg1 = ctrReg1.SetBit(0, true);
          break;
        default:
          throw new InvalidOperationException("Invalid device power " + power);
      }

      I2C.WriteByte(Register.CTRL_REG1, ctrReg1);
    }

    void SetMode(Mode mode)
    {
      byte ctrReg1 = I2C.ReadByte(Register.CTRL_REG1);
      switch (mode)
      {
        case Mode.Barometer:
          ctrReg1 = ctrReg1.SetBit(7, false);
          break;
        case Mode.Altimeter:
          ctrReg1 = ctrReg1.SetBit(7, true);
          break;
        default:
          throw new InvalidOperationException("Invalid device mode " + mode);
      }

      I2C.WriteByte(Register.CTRL_REG1, ctrReg1);
    }

  }
}
