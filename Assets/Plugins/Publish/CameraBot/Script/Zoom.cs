using System;
using UnityEngine;

namespace CF.CameraBot
{
    [Serializable]
    public class Zoom : ICloneable
    {
        // Zoom
        public float m_Distance = 0f;
        public float m_ForwardLimit = 0f;
        public float m_BackwardLimit = 0f;
        public float m_Speed = 1f;

        public bool m_Rebound = false;
        public float m_ReboundDelay = 0f;
        [Range(0f, 30f)] public float m_ReboundPeriod = 10f;
		public AnimationCurve m_ReboundCurve;
        public bool m_WaitForClampAngle = false;

		public object Clone()
		{
			return new Zoom()
			{
				m_Distance = this.m_Distance,
				m_ForwardLimit = this.m_ForwardLimit,
				m_BackwardLimit = this.m_BackwardLimit,
				m_Speed = this.m_Speed,
				m_Rebound = this.m_Rebound,
				m_ReboundDelay = this.m_ReboundDelay,
				m_ReboundPeriod = this.m_ReboundPeriod,
				m_ReboundCurve = this.m_ReboundCurve,
				m_WaitForClampAngle = this.m_WaitForClampAngle
			};
		}
	}
}
