using UnityEngine;
using Kit.Extend;

namespace CF.CameraBot.Parts
{
	public class SpaceOffset : MonoBehaviour, IParts, IGizmo, IOffset
	{
		public Preset m_Preset;
		public CameraStand m_CameraStand;
		public Color m_Color;
		private float m_MaxAlpha = 0f;
		public Transform m_InitTransform;

		private Vector3 _Offset = Vector3.zero;
		public Vector3 Offset
		{
			get
			{
				return _Offset;
			}
			set
			{
				_Offset = transform.localPosition = value;
				if (!Application.isPlaying)
					m_InitTransform.localPosition = transform.localPosition;
			}
		}

		public void DrawGizmos()
		{
			if(!transform.position.EqualSimilar(transform.parent.position))
			{
				GizmosExtend.DrawLine(transform.position, transform.parent.position, m_Color.SetAlpha(Mathf.PingPong(Time.time, m_MaxAlpha)));
			}
		}

		public void Init(Preset preset, CameraStand cameraStand)
		{
			Init(preset, cameraStand, m_Preset.m_DebugColor);
		}

		public void Init(Preset preset, CameraStand cameraStand, Color color)
		{
			m_Preset = preset;

			m_CameraStand = cameraStand;
			m_Color = color;
			m_MaxAlpha = m_Color.a;

			m_InitTransform = new GameObject(name + "(init)").transform;
			m_InitTransform.SetParent(transform.parent); // we share same parent
			m_InitTransform.position = transform.position;
			m_InitTransform.rotation = transform.rotation;
			m_InitTransform.localScale = transform.localScale;
		}

		public void ResetToInit()
		{
			transform.localPosition = m_InitTransform.localPosition;
			transform.localRotation = m_InitTransform.localRotation;
			transform.localScale = m_InitTransform.localScale;
		}
	}
}