using UnityEngine;
using Kit.Extend;

namespace CF.CameraBot.Parts
{
	public class ArcYaw : BaseYaw, IGizmo
	{
		[SerializeField] private Color m_Color, m_Pointer;

		#region Core
		public void Init(Preset preset, CameraStand cameraStand, Color color, Color pointer)
		{
			base.Init(preset, cameraStand);
			m_Color = color;
			m_Pointer = pointer;
		}

		public void DrawGizmos()
		{
#if UNITY_EDITOR
			if (m_Preset == null)
				return;

			Color oldColor = UnityEditor.Handles.color;
			UnityEditor.Handles.color = m_Color;
			Vector3
				up = m_InitTransform.up,
				forward = m_InitTransform.forward;

			float radius = m_Preset.m_VirtualPosition.m_Camera.m_Coordinates.radius;
			UnityEditor.Handles.DrawSolidArc(transform.position, up, forward, m_Preset.m_ClampAngle.m_PolarLeftRange, radius);
			UnityEditor.Handles.DrawSolidArc(transform.position, -up, forward, m_Preset.m_ClampAngle.m_PolarRightRange, radius);
			GizmosExtend.DrawLine(m_InitTransform.position, m_InitTransform.TransformPoint(0f, 0f, radius), m_Pointer);
			UnityEditor.Handles.color = oldColor;
#endif
		}
		#endregion
	}
}