using UnityEngine;

namespace CF.CameraBot.Parts
{
	public class OrbitDistance : MonoBehaviour, IParts
	{
		public Preset m_Preset;
		public CameraStand m_CameraStand;
		public Transform m_InitTransform;

		private float _slider = 0f;
		public float Slider
		{
			get
			{
				return _slider;
			}
			set
			{
				_slider = value;
				transform.localPosition = new Vector3(0f, 0f, value);
				if (!Application.isPlaying)
					m_InitTransform.localPosition = transform.localPosition;
			}
		}
		
		void Awake()
		{
			// Get current distance from local.z
			_slider = transform.localPosition.z;
		}

		public void Init(Preset preset, CameraStand cameraStand)
		{
			m_Preset = preset;
			m_CameraStand = cameraStand;
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