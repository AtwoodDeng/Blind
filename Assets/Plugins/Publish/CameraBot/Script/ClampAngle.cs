using UnityEngine;
using System;

namespace CF.CameraBot
{
    #region Enum
    public enum UpwardReferenceMethod
    {
        World = 0,
        Local,
		RealLocal,
        Custom
    }
    #endregion

    [Serializable]
    public class ClampAngle : ICloneable
    {
        // Availiable angle
        public Transform m_ForwardReference;
        public UpwardReferenceMethod m_UpwardReferenceMethod;
        public Transform m_UpReference;
        [Range(0f, 180f)]
        public float m_PolarLeftRange;
        [Range(0f, 180f)]
        public float m_PolarRightRange;
        [Range(0f, 180f)]
        public float m_ElevationUpRange;
        [Range(0f, 180f)]
        public float m_ElevationDownRange;

        public bool m_Rebound = false;
        public float m_ReboundDelay = 0f;
		[Range(0f, 30f)]
        public float m_ReboundPeriod = 10f;
		public AnimationCurve m_ReboundCurve;

		public object Clone()
		{
			return new ClampAngle()
			{
				m_ForwardReference = this.m_ForwardReference,
				m_UpwardReferenceMethod = this.m_UpwardReferenceMethod,
				m_UpReference = this.m_UpReference,
				m_PolarLeftRange = this.m_PolarLeftRange,
				m_PolarRightRange = this.m_PolarRightRange,
				m_ElevationUpRange = this.m_ElevationUpRange,
				m_ElevationDownRange = this.m_ElevationDownRange,
				m_Rebound = this.m_Rebound,
				m_ReboundDelay = this.m_ReboundDelay,
				m_ReboundPeriod = this.m_ReboundPeriod,
				m_ReboundCurve = this.m_ReboundCurve
			};
		}
	}
}
