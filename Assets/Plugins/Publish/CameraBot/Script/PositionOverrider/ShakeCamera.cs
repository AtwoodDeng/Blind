using UnityEngine;
using System.Collections;

namespace CF.CameraBot.Parts
{
	/// <summary>Shake</summary>
	/// <see cref="https://gist.github.com/bitbutter/210eb42b004d84cebb4e"/>
	[RequireComponent(typeof(Preset))]
	public class ShakeCamera : IDeaPositionBase, IDeaPosition
	{
		public float m_Magnitude = 0.1f;
		public Vector3 m_DirectionForce = Vector3.zero;
		public AnimationCurve m_ForceDuration = AnimationCurve.EaseInOut(0f, 1f, .5f, 0f);
		public bool m_Trigger = false;

		private IEnumerator m_Coroutine;

		#region System
		protected override void OnValidate()
		{
			base.OnValidate();
			m_Magnitude = Mathf.Max(0f, m_Magnitude);
		}

		protected void Awake()
		{
			m_Trigger = false;
		}

		void Update()
		{
			if (m_Trigger)
			{
				m_Trigger = false;
				if (m_Coroutine != null)
					StopCoroutine(m_Coroutine);
				m_Coroutine = Shake();
				StartCoroutine(m_Coroutine);
			}
		}
		#endregion

		#region Core
		public void Trigger()
		{
			m_Trigger = true;
		}
		
		public void Trigger(float magnitude, Vector3 directionForce, AnimationCurve forceDuration)
		{
			m_Magnitude = magnitude;
			m_DirectionForce = directionForce;
			m_ForceDuration = forceDuration;
			Trigger();
		}

		IEnumerator Shake()
		{
			float elapsed = 0f;
			if (m_ForceDuration.length == 0)
				yield break;
			float time = m_ForceDuration[m_ForceDuration.length - 1].time;
			while (elapsed < time)
			{
				elapsed += Time.deltaTime;
				float
					percentComplete = elapsed / time,
					// We want to reduce the shake from full power to 0 starting half way through
					damper = (1f - Mathf.Clamp(4f * percentComplete - 3f, 0f, 1f)) * m_Magnitude,
					// map noise to [-1, 1]
					x = (Random.value * 2f - 1f) * damper,
					y = (Random.value * 2f - 1f) * damper,
					z = (Random.value * 2f - 1f) * damper;

				ToLocalPosition = new Vector3(x, y, z) + GetCameraCurrentPivot.localPosition;

				yield return null;
			}

			ToLocalPosition = Vector3.zero;
		}
		#endregion
	}
}