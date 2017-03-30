using UnityEngine;
using System.Collections.Generic;
using Kit.Extend;
using CF.CameraBot.Parts;

namespace CF.CameraBot
{
	public class CameraStand : MonoBehaviour, IGizmo
	{

		#region Variable
		readonly float YawFix = 180f;
		[SerializeField]
		private Preset m_Preset;

		// New System dataset
		public SpaceOffset m_ChaseTargetOffsetHandler, m_CameraOffsetHandler;
		public ArcYaw m_YawHandler;
		public ArcPitch m_PitchHandler;
		public OrbitDistance m_OrbitDistance, m_OrbitLookAtDistance, m_ZoomHandler;
		public Transform m_LookAtPivot;
		public Transform m_CameraOrbitPivot;
		public CameraPivot m_CameraPivot;
		public BaseYaw m_YawLookAtHandler;
		public BasePitch m_PitchLookAtHandler;
		#endregion

		#region Getter / Setter
		public Vector3 ChaseTargetOffset
		{
			get { return m_ChaseTargetOffsetHandler.Offset; }
			set { m_ChaseTargetOffsetHandler.Offset = value; }
		}
		public Vector3 CameraOffset
		{
			get { return m_CameraOffsetHandler.Offset; }
			set { m_CameraOffsetHandler.Offset = value; }
		}
		public float YawDegree
		{
			get { return Mathf.Repeat(m_YawHandler.Degree - YawFix, 360f); }
			set { m_YawHandler.Degree = Mathf.Repeat(value + YawFix, 360f); }
		}
		public float YawDegreeSignedDiff
		{
			get
			{
				return m_YawHandler.transform.forward.AngleBetweenDirectionSigned(m_YawHandler.m_InitTransform.forward, m_YawHandler.transform.up);
			}
		}
		public float YawLookAtDegree
		{
			get { return Mathf.Repeat(m_YawLookAtHandler.Degree, 360f); }
			set { m_YawLookAtHandler.Degree = Mathf.Repeat(value, 360f); }
		}
		public float PitchDegree
		{
			get { return m_PitchHandler.Degree; }
			set { m_PitchHandler.Degree = value; }
		}
		public float PitchDegreeSignedDiff
		{
			get
			{
				return m_PitchHandler.transform.forward.AngleBetweenDirectionSigned(m_PitchHandler.m_InitTransform.forward, m_PitchHandler.transform.right);
			}
		}
		public float PitchLookAtDegree
		{
			get { return m_PitchLookAtHandler.Degree; }
			set { m_PitchLookAtHandler.Degree = value; }
		}
		public float OrbitDistance
		{
			get { return m_OrbitDistance.Slider; }
			set { m_OrbitDistance.Slider = value; }
		}
		public float ZoomRange
		{
			get { return m_ZoomHandler.Slider; }
			set { m_ZoomHandler.Slider = value; }
		}
		public float OrbitLookAtDistance
		{
			get { return m_OrbitLookAtDistance.Slider; }
			set { m_OrbitLookAtDistance.Slider = value; }
		}
		public CameraPivot CameraPivot
		{
			get { return m_CameraPivot; }
		}

		public Transform GetCameraPivot()
		{
			return m_CameraPivot.transform;
		}
		public Transform GetCameraLookAt()
		{
			if (!m_Preset.m_VirtualPosition.m_EnableLookTarget)
				return m_ChaseTargetOffsetHandler.transform;
			else
				return m_LookAtPivot;
		}
		public Transform GetChaseTargetOffset()
		{
			return m_ChaseTargetOffsetHandler.transform;
		}
		public Transform GetCameraOffset()
		{
			return m_CameraOffsetHandler.transform;
		}

		public Quaternion GetCameraRotation()
		{
			return Quaternion.LookRotation(CameraPivot.transform.position.Direction(GetCameraLookAt().position), GetCameraUpward());
		}

		public Vector3 GetCameraUpward()
		{
			Vector3 upward;
			switch (m_Preset.m_ClampAngle.m_UpwardReferenceMethod)
			{
				default:
				case UpwardReferenceMethod.World: upward = Vector3.up; break;
				case UpwardReferenceMethod.Custom: upward = (m_Preset.m_ClampAngle.m_UpReference == null) ? Vector3.up : m_Preset.m_ClampAngle.m_UpReference.up; break;
				case UpwardReferenceMethod.Local: upward = Vector3.Lerp(Vector3.Project(CameraPivot.transform.up, transform.up), CameraPivot.transform.up, .5f); break;
				case UpwardReferenceMethod.RealLocal: upward = CameraPivot.transform.up; break;
			}
			return upward;
		}
		#endregion

		#region API
		public void DrawGizmos()
		{
			m_ChaseTargetOffsetHandler.DrawGizmos();
			m_YawHandler.DrawGizmos();
			m_PitchHandler.DrawGizmos();
			m_CameraOffsetHandler.DrawGizmos();
			m_CameraPivot.DrawGizmos();

			// Label
			string info = string.Format("{0}\nPolar = {1:F1}\nElevation = {2:F1}\nRadius = {3:F3}",
				m_Preset.name,
				m_Preset.m_VirtualPosition.m_Camera.m_Coordinates.Yaw,
				m_Preset.m_VirtualPosition.m_Camera.m_Coordinates.Pitch,
				m_Preset.m_VirtualPosition.m_Camera.m_Coordinates.radius);

			GizmosExtend.DrawLabel(m_CameraOrbitPivot.transform.position + (Vector3.down * 0.2f), info, GUI.skin.textArea);
			GizmosExtend.DrawLine(m_Preset.Instance.CameraPivot.transform.position, m_Preset.Instance.GetCameraLookAt().position, m_Preset.m_DebugColor);
			GizmosExtend.DrawLocalCube(m_Preset.Instance.m_CameraOffsetHandler.transform.parent, Vector3.one * .1f, m_Preset.m_DebugColor);
			GizmosExtend.DrawLocalCube(m_Preset.Instance.CameraPivot.transform, Vector3.one * .15f, m_Preset.m_DebugColor);
			if (m_Preset.m_VirtualPosition.m_EnableLookTarget)
			{
				GizmosExtend.DrawSphere(m_Preset.Instance.GetCameraLookAt().position, .04f, m_Preset.m_DebugColor);
			}
		}

		internal void Init(Preset preset)
		{
			m_Preset = preset;

			// TODO: May be we shouldn't re-parent to preset.
			transform.SetParent(preset.transform, false);
			GameObject tmp;
			
			// ChaseOffset
			tmp = AddChild("ChaseTargetOffset", transform);
			m_ChaseTargetOffsetHandler = tmp.AddComponent<SpaceOffset>();
			m_ChaseTargetOffsetHandler.transform.localPosition = preset.m_VirtualPosition.m_TargetOffset;
			m_ChaseTargetOffsetHandler.Init(preset, this, Color.white.SetAlpha(.4f));
			
			// Yaw
			tmp = AddChild("Yaw", m_ChaseTargetOffsetHandler.transform);
			m_YawHandler = tmp.AddComponent<ArcYaw>();
			m_YawHandler.Init(preset, this, Color.red.SetAlpha(.05f), Color.red.SetAlpha(.5f));
			m_YawHandler.Degree = preset.m_VirtualPosition.m_Camera.m_Coordinates.Yaw;
			
			// Pitch
			tmp = AddChild("Pitch", m_YawHandler.transform);
			m_PitchHandler = tmp.AddComponent<ArcPitch>();
			m_PitchHandler.Init(preset, this, Color.blue.SetAlpha(.05f), Color.blue.SetAlpha(.5f));
			m_PitchHandler.Degree = preset.m_VirtualPosition.m_Camera.m_Coordinates.Pitch;

			// Distance
			tmp = AddChild("OrbitDistance", m_PitchHandler.transform);
			m_OrbitDistance = tmp.AddComponent<OrbitDistance>();
			m_OrbitDistance.Init(preset, this);
			m_OrbitDistance.Slider = preset.m_VirtualPosition.m_Camera.m_Coordinates.radius;

			// Zoom
			tmp = AddChild("ZoomRange", m_OrbitDistance.transform);
			m_ZoomHandler = tmp.AddComponent<OrbitDistance>();
			m_ZoomHandler.Init(preset, this);
			m_ZoomHandler.Slider = 0f;

			// Camera Offset
			tmp = AddChild("CameraOffset", m_ZoomHandler.transform);
			m_CameraOffsetHandler = tmp.AddComponent<SpaceOffset>();
			m_CameraOffsetHandler.transform.localPosition = preset.m_VirtualPosition.m_CameraOffset;
			m_CameraOffsetHandler.transform.rotation = Quaternion.LookRotation(-m_CameraOffsetHandler.transform.forward, m_CameraOffsetHandler.transform.up);
			m_CameraOffsetHandler.Init(preset, this, Color.white.SetAlpha(.4f));

			// Idea orbit position
			tmp = AddChild("CameraOrbitPivot", m_CameraOffsetHandler.transform);
			m_CameraOrbitPivot = tmp.transform;

			// final position
			tmp = AddChild("CameraPivot", m_CameraOrbitPivot);
			m_CameraPivot = tmp.AddComponent<CameraPivot>();
			m_CameraPivot.Init(preset, this);
			
			// Virtual look at point
			// Yaw
			tmp = AddChild("YawLookAt", m_ChaseTargetOffsetHandler.transform);
			m_YawLookAtHandler = tmp.AddComponent<BaseYaw>();
			m_YawLookAtHandler.Init(preset, this);
			m_YawLookAtHandler.Degree = preset.m_VirtualPosition.m_LookTarget.m_Coordinates.Yaw;

			// Pitch
			tmp = AddChild("PitchLookAt", m_YawLookAtHandler.transform);
			m_PitchLookAtHandler = tmp.AddComponent<BasePitch>();
			m_PitchLookAtHandler.Init(preset, this);
			m_PitchLookAtHandler.Degree = preset.m_VirtualPosition.m_LookTarget.m_Coordinates.Pitch;

			// Distance
			tmp = AddChild("OrbitDistance", m_PitchLookAtHandler.transform);
			m_OrbitLookAtDistance = tmp.AddComponent<OrbitDistance>();
			m_OrbitLookAtDistance.Init(preset, this);
			m_OrbitLookAtDistance.Slider = preset.m_VirtualPosition.m_LookTarget.m_Coordinates.radius;

			// Point
			tmp = AddChild("LookAtPivot", m_OrbitLookAtDistance.transform);
			m_LookAtPivot = tmp.transform;
		}

		public void ResetToInit()
		{
			m_ChaseTargetOffsetHandler.ResetToInit();
			m_CameraOffsetHandler.ResetToInit();
			m_YawHandler.ResetToInit();
			m_PitchHandler.ResetToInit();
			m_OrbitDistance.ResetToInit();
			m_OrbitLookAtDistance.ResetToInit();
			m_YawLookAtHandler.ResetToInit();
			m_PitchLookAtHandler.ResetToInit();
		}
		#endregion

		#region Helper
		private GameObject AddChild(string naming, Transform parent, Transform reference = null)
		{
			GameObject rst = new GameObject(naming);
			rst.transform.SetParent(parent, false);
			return rst;
		}
		#endregion
	}
}