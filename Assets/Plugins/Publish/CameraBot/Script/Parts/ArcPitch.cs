using UnityEngine;
using Kit.Extend;

namespace CF.CameraBot.Parts
{
	public class ArcPitch : BasePitch, IGizmo
	{
		[SerializeField]
		Color m_Color, m_Pointer;
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

			Vector3
				right = m_InitTransform.right,
				forward = m_InitTransform.forward;

			Color oldColor = UnityEditor.Handles.color;
			UnityEditor.Handles.color = m_Color;
			float radius = m_Preset.m_VirtualPosition.m_Camera.m_Coordinates.radius;
			UnityEditor.Handles.DrawSolidArc(transform.position, -right, forward, m_Preset.m_ClampAngle.m_ElevationUpRange, radius);
			UnityEditor.Handles.DrawSolidArc(transform.position, right, forward, m_Preset.m_ClampAngle.m_ElevationDownRange, radius);
			GizmosExtend.DrawLine(m_InitTransform.position, m_InitTransform.TransformPoint(0f, 0f, radius), m_Pointer);
			UnityEditor.Handles.color = oldColor;
#endif
		}
		#endregion
	}
}