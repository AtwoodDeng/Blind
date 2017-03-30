namespace CF.CameraBot
{
    public enum DeviceType
    {
        PC = 0,
        Mobile
    }

    public interface IDevice
    {
        void Trigger(DeviceType type);
    }

    public interface IDevicePC : IDevice{}
    public interface IDeviceMobile : IDevice{}
}
