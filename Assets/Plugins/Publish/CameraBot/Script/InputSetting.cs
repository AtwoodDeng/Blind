using UnityEngine;
using System;
using System.Collections;

namespace CF.CameraBot
{
    /// <summary>Global input setting, control sensitive, threshold, and flip control here.</summary>
    [Serializable]
    public class InputSetting
    {
        /// <summary>The flip mouse X-axis direction</summary>
        [Tooltip("Flip mouse X-axis direction.")]
        public bool FlipMouseX = false;
        /// <summary>The flip mouse Y-axis direction</summary>
        [Tooltip("Flip mouse Y-axis direction.")]
        public bool FlipMouseY = false;
        /// <summary>The flip mouse wheel direction</summary>
        [Tooltip("Flip mouse wheel direction.")]
        public bool FlipMouseWheel = false;
        /// <summary>The wheel sensitive</summary>
        [Tooltip("Wheel sensitive")]
        public float Sensitive = 0.8f;
        /// <summary>The wheel scroll amount for each sector.</summary>
        [Tooltip("Mouse wheel speed, magnitude input value.")]
        public float WheelSpeed = 200f;
        /// <summary>The threshold for gamepad/joystick, the movement smaller than threshold will not affect update.</summary>
        [Tooltip("Hotfix for gamepad/joystick, the movement smaller than threshold will not affect update.")]
        public float Threshold = 0.1f;
    }
}
