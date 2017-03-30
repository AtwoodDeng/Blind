using UnityEngine;
using System;
using System.Collections;

namespace CF.CameraBot
{
    #region Enum
    public enum UpdateFrequency
    {
        Low = 0,
        Medium,
        High
    }
    #endregion

    /// <summary>Advance Setting for CameraBot</summary>
    [Serializable]
    public class AdvanceSetting
    {
        /// <summary>Update frequency of camera bot, higher will be more smooth, but result in more system resource taken.</summary>
        public UpdateFrequency UpdateFrequency = UpdateFrequency.Medium;
    }
}
