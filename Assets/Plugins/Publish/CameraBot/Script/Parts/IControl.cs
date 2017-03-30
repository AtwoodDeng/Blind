namespace CF.CameraBot
{
    public enum ControlType
    {
        FPS = 0,
        TPS,
        STR
    }

    public interface IControl
    {
        void Trigger(ControlType type);
    }

    public interface IControlTPS : IControl{}
    public interface IControlFPS : IControl{}
    public interface IControlSTR : IControl{}
}
