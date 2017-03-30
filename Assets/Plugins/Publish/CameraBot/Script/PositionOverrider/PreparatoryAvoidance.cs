using UnityEngine;
using Kit.Extend;

namespace CF.CameraBot.Parts
{
	/// <summary>Idea from Vector field Histogram</summary>
	[RequireComponent(typeof(Preset))]
	public class PreparatoryAvoidance : IDeaPositionBase, IDeaPosition
	{
		public bool m_AllowSpeedUpOnEdge = true;
		public float m_SafeRadiusNear = 0f;
		public float m_SafeRadiusFar = 0.2f; // safe edge of camera with other object
		public float m_RadiusSpeedFactor = 0.1f; // scale radius by moving speed
		public float m_SafeDistance = 0.2f; // safe distance when hit obstacle, get forward point.
		public bool m_BasedOnVirtualFaceTarget = false; // based on target or virtual face point

		/// <summary>Ignore nothing by default, recommand ignore chase target's layer</summary>
		public LayerMask m_IgnoreLayerMask = 1;
		private float m_Radius = 0f;

		protected override void OnValidate()
		{
			base.OnValidate();
			m_SafeRadiusFar = Mathf.Max(0, m_SafeRadiusFar);
		}

		void FixedUpdate()
		{
			PrepareReaction();
		}

		private void PrepareReaction()
		{
			Vector3 ignoreLocalSpeed = GetCameraIdeaPivot.InverseTransformDirection(preset.Instance.CameraPivot.Velocity) - preset.Instance.CameraPivot.LocalVelocity;
			float relativeSpeed = Vector3.zero.Distance(ignoreLocalSpeed) / Time.fixedDeltaTime;
			float peak = Mathf.Min(2f, relativeSpeed * m_RadiusSpeedFactor);
			m_Radius = Mathf.Lerp(m_Radius, peak, Time.fixedDeltaTime);
			float radius = m_Radius + m_SafeRadiusFar;
			float distance = GetCameraIdeaDistance;
			
			Transform
				org = m_BasedOnVirtualFaceTarget ? GetCameraLookAt : GetChaseTarget;

			Vector3
				cam = GetCameraIdeaPivot.position,
				//current = GetCameraCurrentPivot.position,
				cUp = GetCameraIdeaPivot.TransformPoint(Vector3.up * radius),
				cDown = GetCameraIdeaPivot.TransformPoint(-Vector3.up * radius),
				cLeft = GetCameraIdeaPivot.TransformPoint(-Vector3.right * radius),
				cRight = GetCameraIdeaPivot.TransformPoint(Vector3.right * radius),
				oUp = org.position.PointOnDistance(GetCameraIdeaPivot.up, m_SafeRadiusNear),
				oDown = org.position.PointOnDistance(-GetCameraIdeaPivot.up, m_SafeRadiusNear),
				oLeft = org.position.PointOnDistance(-GetCameraIdeaPivot.right, m_SafeRadiusNear),
				oRight = org.position.PointOnDistance(GetCameraIdeaPivot.right, m_SafeRadiusNear);

			float
				finalPos = PointRay(org.position, cam, distance),
				fixUp = m_SafeDistance - DirectionRay(GetCameraCurrentPivot.TransformPoint(Vector3.up * .1f), GetCameraIdeaPivot.up, m_SafeDistance),
				fixDown = m_SafeDistance - DirectionRay(GetCameraCurrentPivot.TransformPoint(-Vector3.up * .1f), -GetCameraIdeaPivot.up, m_SafeDistance),
				fixLeft = m_SafeDistance - DirectionRay(GetCameraCurrentPivot.TransformPoint(-Vector3.right * .1f), -GetCameraIdeaPivot.right, m_SafeDistance),
				fixRight = m_SafeDistance - DirectionRay(GetCameraCurrentPivot.TransformPoint(Vector3.right * .1f), GetCameraIdeaPivot.right, m_SafeDistance),
				fixH = 0f, fixV = 0f;

			// avoid contact by safe distance.
			fixV = Mathf.Lerp(-fixDown, fixUp, 0.5f); // center
			fixH = Mathf.Lerp(-fixLeft, fixRight, 0.5f); // center

			// check eye sight, predict obstacle avoidance.
			if (Mathf.Approximately(0f, finalPos))
			{
				float[] arr = new float[] {
					PointRay(oUp, cUp, distance),
					PointRay(oDown, cDown, distance),
					PointRay(oLeft, cLeft, distance),
					PointRay(oRight, cRight, distance)
				};
				finalPos = Mathf.Max(arr);
			}
			
			ToLocalPosition = new Vector3(fixH, fixV, finalPos);
		}

		private float PointRay(Vector3 startPoint, Vector3 endPoint, float distance)
		{
			Vector3 direction = startPoint.Direction(endPoint);
			return DirectionRay(startPoint, direction, distance);
		}
		private float DirectionRay(Vector3 startPoint, Vector3 direction, float distance)
		{
			Debug.DrawRay(startPoint, direction, Color.yellow);
			RaycastHit hit;
			if (Physics.Raycast(startPoint, direction, out hit, distance, ~m_IgnoreLayerMask, QueryTriggerInteraction.UseGlobal))
			{
				Debug.DrawLine(startPoint, hit.point, Color.red, 0.01f);
				Vector3 point = GetCameraIdeaPivot.InverseTransformPoint(hit.point);

				// we only care about Z-axis
				return point.z;
			}
			return 0f;
		}
	}
}