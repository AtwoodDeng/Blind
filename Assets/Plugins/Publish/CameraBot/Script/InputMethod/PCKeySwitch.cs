using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using Kit.Extend;

namespace CF.CameraBot
{
	[ExecuteInEditMode]
    public class PCKeySwitch : CameraBotSlave, IDevicePC
    {
        void OnEnable()
        {
            if (ModelText != null)
                ModelText.text = string.Empty;
			if (!Application.isPlaying)
				SyncCameraBot();
        }

        void Update()
        {
			if(Application.isPlaying)
			{
				DisplayCameraName();
				TriggerSwitchCamera();
			}
			else
			{
				SyncCameraBot();
			}
        }

        #region device interface
        public void Trigger(DeviceType type)
        {
            this.enabled = (type == DeviceType.PC);
        }
        #endregion

        #region Change Game Method
        /// <summary>[Optional] To display the current Camera preset name</summary>
        [Tooltip("[Optional] To display the current Camera preset name")]
        public Text ModelText;

        /// <summary>CameraBot preset name list</summary>
        [Tooltip("CameraBot preset name list")]
        public List<BindKey> bindKeyList = new List<BindKey>();

        /// <summary>Synchronizes the camera bot's preset config into PCKeySwitch</summary>
        [ContextMenu("Sync CameraBot Name List")]
        public void SyncCameraBot()
        {
            if (CameraBot == null)
                return;

			try
			{
				if (bindKeyList.Count > 0)
					bindKeyList.RemoveAll(x => !CameraBot.PresetList.Exists(y => y.name == x.CameraName));
				Dictionary<string, BindKey> tmp = bindKeyList.ToDictionary(x => x.CameraName);
				bindKeyList.Clear();
				CameraBot.PresetList.ForEach(x =>
				{
					if (!bindKeyList.Exists(y => x.name == y.CameraName))
						bindKeyList.Add(new BindKey()
						{
							CameraName = x.name,
							KeyCode = (tmp.ContainsKey(x.name) ? tmp[x.name].KeyCode : KeyCode.None)
						});
				});
				tmp.Clear();
			}
			catch
			{
				bindKeyList.Clear();
				return;
			}
		}

        private void TriggerSwitchCamera()
        {
            foreach (BindKey bindKey in bindKeyList)
            {
                if (Input.GetKeyUp(bindKey.KeyCode))
                {
                    CameraBot.SwitchCamera(bindKey.CameraName);
                    break;
                }
            }
        }

        private void DisplayCameraName()
        {
            if (!ReferenceEquals(ModelText,null) &&
                ModelText.text != CameraBot.PresetList[CameraBot.Selected].name)
            {
                try
                {
                    ModelText.text = string.Format("{0}", CameraBot.PresetList[CameraBot.Selected].name);
                    // ModelText.color = CameraBot.Preset[CameraBot.Selected].DebugColor.SetAlpha(1f);
                }
                catch
                {
                    ModelText.text = "Error";
                    ModelText.color = Color.red;
                }
            }
        }
        #endregion
    }

    [Serializable]
    public class BindKey
    {
        public string CameraName = "";
        public KeyCode KeyCode = KeyCode.None;
    }
}