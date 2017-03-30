using UnityEngine;
using Kit.Extend;
using System;

namespace CF.CameraBot.Parts
{
	[RequireComponent(typeof(Preset))]
	public class IDeaPositionBase : MonoBehaviour, IDeaPosition
	{
		#region interface
		
		[SerializeField]
		private Preset m_Preset;
		protected Preset preset { get { return m_Preset; } }

		[Header("Position overrider")]
		[SerializeField]
		private int m_GroupNumber;
		public int GroupNumber { get { return m_GroupNumber; } }

		[SerializeField]
		private Vector3 m_LocalPosition;
		public Vector3 ToLocalPosition
		{
			get { return m_LocalPosition; }
			protected set { m_LocalPosition = value; }
		}

		[SerializeField]
		[Range(0f,1f)]
		private float m_Weight = .5f;
		public float Weight { get { return m_Weight; } }
		#endregion

		#region common variables
		/// <summary>The clone position & rotation related to chase target</summary>
		/// <remarks>Non-real chase target, it's preset's instance</remarks>
		protected Transform GetChaseTarget { get { return preset.Instance.transform; } }
		/// <summary>The offset applied on chase target</summary>
		protected Transform GetChaseTargetOffset { get { return preset.Instance.GetChaseTargetOffset(); } }
		/// <summary>The camera pivot point, position & rotation</summary>
		protected Transform GetCameraCurrentPivot { get { return preset.Instance.GetCameraPivot(); } }
		/// <summary>The camera pivot point, without modify</summary>
		protected Transform GetCameraIdeaPivot { get { return preset.Instance.GetCameraOffset(); } }
		/// <summary>The location, should focus by camera</summary>
		protected Transform GetCameraLookAt { get { return preset.Instance.GetCameraLookAt(); } }
		/// <summary>The idea Camera direction</summary>
		protected Vector3 GetCameraIdeaDirection { get { return GetCameraLookAt.position.Direction(GetCameraIdeaPivot.position); } }
		/// <summary>The idea Camera distance</summary>
		protected float GetCameraIdeaDistance { get { return GetCameraLookAt.position.Distance(GetCameraIdeaPivot.position); } }
		#endregion

		#region helper
		protected virtual void OnValidate()
		{
			if (m_Preset == null)
			{
				m_Preset = GetComponent<Preset>();
			}
			
			// for debug purpose, not allow to change.
			m_LocalPosition = Vector3.zero;
		}

		protected virtual void OnEnable()
		{
			m_Preset.Instance.CameraPivot.RegisterOperator(this);
		}

		protected virtual void OnDisable()
		{
			m_Preset.Instance.CameraPivot.UnregisterOperator(this);
		}

		public override string ToString()
		{
			return string.Format("[G:{5}][W:{0:F2}] {1} :: Pos({2:F2},{3:F2},{4:F2})", Weight, GetType().Name, ToLocalPosition.x, ToLocalPosition.y, ToLocalPosition.z, GroupNumber);
		}
		#endregion
	}
}