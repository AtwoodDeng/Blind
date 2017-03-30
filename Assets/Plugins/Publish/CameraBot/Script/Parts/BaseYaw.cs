//#define GIZMOS_DEBUG
using UnityEngine;
using Kit.Extend;

namespace CF.CameraBot.Parts
{
	public class BaseYaw : MonoBehaviour, IParts, IArc
	{
		public Preset m_Preset;
		public CameraStand m_CameraStand;
		public Transform m_InitTransform;
		
		private float _degree = 0f;
		public float Degree
		{
			get
			{
				return transform.localRotation.eulerAngles.y;
				// return _degree;
			}
			set
			{
				_degree = Mathf.Repeat(value, 360f);
				transform.localRotation = Quaternion.Euler(0f, _degree, 0f);
				if (!Application.isPlaying)
					m_InitTransform.localRotation = transform.localRotation;
			}
		}

		#region Core
		public virtual void Init(Preset preset, CameraStand cameraStand)
		{
			m_Preset = preset;
			m_CameraStand = cameraStand;
			
			m_InitTransform = new GameObject(name + "(init)").transform;
			m_InitTransform.SetParent(transform.parent); // we share same parent
			m_InitTransform.position = transform.position;
			m_InitTransform.rotation = transform.rotation;
			m_InitTransform.localScale = transform.localScale;
		}
		public virtual void ResetToInit()
		{
			transform.localPosition = m_InitTransform.localPosition;
			transform.localRotation = m_InitTransform.localRotation;
			transform.localScale = m_InitTransform.localScale;
		}
		public float SetDegreeClampAngle(float diff, float upLimit, float downLimit)
		{
			float laterDegree = Degree + diff;

			if (diff < 0f && laterDegree < -upLimit)
				diff = diff + (Mathf.Abs(laterDegree) - upLimit);
			else if (diff > 0f && laterDegree > downLimit)
				diff = diff - (laterDegree - downLimit);

			Degree += diff;
			return Degree;
		}
		#endregion
	}
}