using System;
using UnityEngine;
using Kit.Coordinates;

namespace CF.CameraBot
{
	[Serializable]
	public class VirtualPosition : ICloneable
	{
		/// <summary>offset relate to target pivot position</summary>
		public Vector3 m_TargetOffset = Vector3.zero;
		/// <summary>offset relate to camera pivot position</summary>
		public Vector3 m_CameraOffset = Vector3.zero;
		public VirtualPoint m_Camera = new VirtualPoint()
		{
			m_Coordinates = new SphericalCoordinates()
			{
				radius = 3f,
				Yaw = 0f,
				Pitch = -15f
			},
			m_OppositePolar = false,
			m_OppositeElevation = true
		};

		public bool m_EnableLookTarget = false;
		public VirtualPoint m_LookTarget = new VirtualPoint()
		{
			m_Coordinates = new SphericalCoordinates()
			{
				radius = 1f,
				Yaw = 0f,
				Pitch = -15f
			},
			m_OppositePolar = false,
			m_OppositeElevation = false
		};

		public object Clone()
		{
			return new VirtualPosition()
			{
				m_Camera = (VirtualPoint) this.m_Camera.Clone(),
				m_LookTarget = (VirtualPoint) this.m_LookTarget.Clone(),
				m_EnableLookTarget = this.m_EnableLookTarget,
				m_TargetOffset = this.m_TargetOffset,
				m_CameraOffset = this.m_CameraOffset
			};
		}
    }

    [Serializable]
    public class VirtualPoint : ICloneable
    {
		public SphericalCoordinates m_Coordinates = new SphericalCoordinates() { radius = 3f, Yaw = 0f, Pitch = -15f };
        public bool m_OppositePolar = false;
        public bool m_OppositeElevation = true;

		public object Clone()
		{
			return new VirtualPoint()
			{
				m_Coordinates = this.m_Coordinates.Clone(),
				m_OppositePolar = this.m_OppositePolar,
				m_OppositeElevation = this.m_OppositeElevation
			};
		}
    }
}
