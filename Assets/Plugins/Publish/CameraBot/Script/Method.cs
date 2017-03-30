using UnityEngine;
using System;

namespace CF.CameraBot
{
    #region Enum
    public enum UpdateAngleMethod
    {
        ResetWhenActive = 0,
        UpdateWhenActive,
        UpdateAlway,
    }
    public enum MoveMethod
    {
        Snap = 0,
        Lerp,
        QuaternionLerp,
        OrbitLerp
    }
    public enum RotationMethod
    {
        Snap = 0,
        Lerp,
        QuaternionLerp,
        LerpUnclamped
    }
    #endregion
    [Serializable]
    public class Method : ICloneable
    {
        // Update Method
        public UpdateAngleMethod m_UpdateAngle = UpdateAngleMethod.ResetWhenActive;

        // Pan & rotate camera method & speed.
        public MoveMethod m_MoveMethod = MoveMethod.Lerp;
        [Range(0.0001f, 30f)]
        public float m_PositionSpeed = 4f; // 30 fps, it's almost snap to position.
        public RotationMethod m_RotationMethod = RotationMethod.Lerp;
        [Range(0.0001f, 30f)]
        public float m_RotationSpeed = 4f;

        [Range(0f, 360f)]
        public float m_ImproveAccuracy = 90f;

		public bool m_IsRelatedAngle = false;

		public object Clone()
		{
			return new Method()
			{
				m_UpdateAngle = this.m_UpdateAngle,
				m_MoveMethod = this.m_MoveMethod,
				m_PositionSpeed = this.m_PositionSpeed,
				m_RotationMethod = this.m_RotationMethod,
				m_RotationSpeed = this.m_RotationSpeed,
				m_ImproveAccuracy = this.m_ImproveAccuracy,
				m_IsRelatedAngle = this.m_IsRelatedAngle
			};
		}
	}
}
