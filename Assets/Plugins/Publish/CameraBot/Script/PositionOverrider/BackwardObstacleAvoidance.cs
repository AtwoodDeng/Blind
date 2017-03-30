using UnityEngine;
using Kit.Extend;
namespace CF.CameraBot.Parts
{
	[RequireComponent(typeof(Preset))]
	public class BackwardObstacleAvoidance : IDeaPositionBase, IDeaPosition
	{
		public float m_SafeDistance = 0.2f;
		
		/// <summary>Ignore nothing by default, recommand ignore chase target's layer</summary>
		public LayerMask m_IgnoreLayerMask = 1;

		void FixedUpdate()
		{
			BackwardRaycast();
		}

		private void BackwardRaycast()
		{
			RaycastHit hit;
			if (Physics.Raycast(
					GetCameraLookAt.position,
					GetCameraIdeaDirection,
					out hit,
					GetCameraIdeaDistance,
					~m_IgnoreLayerMask,
					QueryTriggerInteraction.UseGlobal))
			{
				ToLocalPosition = GetCameraIdeaPivot.InverseTransformPoint(hit.point.PointOnDistance(-GetCameraIdeaDirection, m_SafeDistance));
				Debug.DrawLine(GetCameraLookAt.position, hit.point, Color.red, 0.01f);
			}
			else
			{
				ToLocalPosition = Vector3.zero;
			}
		}
	}
}

