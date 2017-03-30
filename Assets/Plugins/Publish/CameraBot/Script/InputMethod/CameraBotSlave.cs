using UnityEngine;
namespace CF.CameraBot
{
    /// <summary>The component depend on CameraBot</summary>
    [RequireComponent(typeof(CameraBot))]
    public class CameraBotSlave : MonoBehaviour
    {
        private CameraBot _CameraBot = null;
        public CameraBot CameraBot
        {
            get
            {
                if(ReferenceEquals(_CameraBot,null))
                    _CameraBot = GetComponent<CameraBot>();
                return _CameraBot;
            }
        }
	}
}
