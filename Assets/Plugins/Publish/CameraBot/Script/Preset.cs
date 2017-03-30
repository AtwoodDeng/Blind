using UnityEngine;
using System;


namespace CF.CameraBot
{
    #region Calculate    
    /// <summary>Calculate and record : transform position, direction, angle and hit point on each update.</summary>
    public class RealTimeCache
    {
		public float
			m_ZoomSectionDelta = 0f,			// current zoom distance
			m_YawClampAngleDelta = 0f,			// camera moving angle
			m_PitchClampAngleDelta = 0f;		// camera moving angle
		
		internal bool
			m_OneFrameSnapRequest = false;

		public Quaternion
			m_LastFrameTargetRotation = Quaternion.identity;
		public Vector3
			m_LastFrameTargetPosition = Vector3.zero;

		public ReboundData m_ReboundYaw = new ReboundData();
		public ReboundData m_ReboundPitch = new ReboundData();
	}
    #endregion

    #region Init Reference    
    /// <summary>To record the init position and relative angle for reference position.</summary>
    public class InitReference
    {
		public VirtualPosition m_VirtualPosition;
		public Zoom m_Zoom;
		public ClampAngle m_ClampAngle;
		public Method m_Method;
	}
    #endregion

    /// <summary>Preset for each camera config.</summary>
    public class Preset : MonoBehaviour
    {
		#region editor variable
		public CameraBot m_Host;
		public Color m_DebugColor = Color.clear;
		public bool m_DisplayOnScene = true;
		public bool m_Editing { get; set; }
		#endregion

		#region runtime variable
		RealTimeCache m_Cache = null;
		internal RealTimeCache Cache
		{
			get
			{
				// prepare runtime cache
				if (m_Cache == null)
					m_Cache = new RealTimeCache();
				return m_Cache;
			}
			set { m_Cache = value; }
		}
		InitReference m_InitReference = null;
		internal InitReference InitReference
		{
			get
			{
				if (m_InitReference == null)
				{
					// backup default values
					m_InitReference = new InitReference();
					m_InitReference.m_VirtualPosition = (VirtualPosition)m_VirtualPosition.Clone();
					m_InitReference.m_Zoom = (Zoom)m_Zoom.Clone();
					m_InitReference.m_ClampAngle = (ClampAngle)m_ClampAngle.Clone();
					m_InitReference.m_Method = (Method)m_Method.Clone();
				}
				return m_InitReference;
			}
		}
		public CameraStand Instance;
		#endregion

		#region System
		public void OnValidate()
		{
#if UNITY_EDITOR
			if (m_Host != null)
			{
				// Ensure Hierarchy structure.
				if(m_Host.transform != transform.parent)
					transform.SetParent(m_Host.transform, false);

				if (ChaseTarget != null)
					transform.position = ChaseTarget.position;
				if (ChaseTargetRotation != null)
					transform.rotation = ChaseTargetRotation.rotation;

				if (Instance == null)
					Init();
				// apply value to instance.
				Instance.ChaseTargetOffset = m_VirtualPosition.m_TargetOffset;
				Instance.CameraOffset = m_VirtualPosition.m_CameraOffset;
				Instance.YawDegree = m_VirtualPosition.m_Camera.m_Coordinates.Yaw;
				Instance.PitchDegree = m_VirtualPosition.m_Camera.m_Coordinates.Pitch;
				Instance.OrbitDistance = m_VirtualPosition.m_Camera.m_Coordinates.radius;
				Instance.YawLookAtDegree = m_VirtualPosition.m_LookTarget.m_Coordinates.Yaw;
				Instance.PitchLookAtDegree = m_VirtualPosition.m_LookTarget.m_Coordinates.Pitch;
				Instance.OrbitLookAtDistance = m_VirtualPosition.m_LookTarget.m_Coordinates.radius;
				m_PositionOverrider.Update(this);
			}
#endif
		}

		public void Init()
		{
#if UNITY_EDITOR
			if (Application.isPlaying)
				return;
			Instance = new GameObject("ChaseTarget").AddComponent<CameraStand>();
			Instance.Init(this);
			HierarchyHidden();
#endif
		}

		void Awake()
		{
			if (m_Host == null)
				throw new NullReferenceException("CameraBot can not be found on :" + this.name);

			// Init Coordinates
			m_VirtualPosition.m_Camera.m_Coordinates.SetUnlimit();
			m_VirtualPosition.m_LookTarget.m_Coordinates.SetUnlimit();
		}

		void OnDrawGizmos()
		{
#if UNITY_EDITOR
			if (m_DisplayOnScene && m_Host != null && UnityEditor.Selection.activeGameObject == m_Host.gameObject)
				Instance.DrawGizmos();
#endif
		}
		
		void OnDrawGizmosSelected()
		{
#if UNITY_EDITOR
			foreach (GameObject go in UnityEditor.Selection.gameObjects)
			{
				if (go == this.gameObject)
				{
					Instance.DrawGizmos();
					break;
				}
			}
#endif
		}
		#endregion

		#region Internal API
		internal bool OneFrameSnapRequest { get { return Cache.m_OneFrameSnapRequest; } set { Cache.m_OneFrameSnapRequest = value; } }
		
		/// <summary>Called by CameraBot editor</summary>
		public void EditorUpdate()
		{
			if (Application.isPlaying)
				return;

			if(ChaseTarget != null)
				transform.position = ChaseTarget.position;
			if (ChaseTargetRotation != null)
				transform.rotation = ChaseTargetRotation.rotation;
		}
		#endregion

		#region API
		public Transform ChaseTarget { get { return m_Host.ChaseTarget; } }
		public Transform ChaseTargetRotation { get { return (m_ClampAngle.m_ForwardReference != null) ? m_ClampAngle.m_ForwardReference : (m_Host.TargetForward != null) ? m_Host.TargetForward : ChaseTarget; } }
		public Transform TargetForward { get { return m_Host.TargetForward; } }
		public Transform ControlPosition { get { return m_Host.ControlPosition; } }
		public Transform ControlRotation { get { return m_Host.ControlRotation; } }

		public void ResetToInitStage()
		{
			Instance.ResetToInit();

			m_VirtualPosition = (VirtualPosition)InitReference.m_VirtualPosition.Clone();
			m_Zoom = (Zoom)InitReference.m_Zoom.Clone();
			m_ClampAngle = (ClampAngle)InitReference.m_ClampAngle.Clone();
			m_Method = (Method)InitReference.m_Method.Clone();
		}
		#endregion

		#region ContentMenu
		[ContextMenu("Hidden transform structure in Hierarchy")]
		void HierarchyHidden()
		{
			if (Instance != null)
				Instance.transform.hideFlags = HideFlags.HideInHierarchy;
		}
		[ContextMenu("Display transform structure in Hierarchy")]
		void HierarchyDisplay()
		{
			if (Instance != null)
				Instance.transform.hideFlags = HideFlags.None;
		}
		#endregion

		#region Dataset
		public VirtualPosition m_VirtualPosition = new VirtualPosition();
		public Zoom m_Zoom = new Zoom()
		{
			m_Distance = 1f,
			m_ForwardLimit = -.5f,
			m_BackwardLimit = .5f,
			m_Speed = 1f,
			m_Rebound = true,
			m_ReboundDelay = 1f,
			m_ReboundPeriod = 1.5f,
			m_ReboundCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f)
		};
		public ClampAngle m_ClampAngle = new ClampAngle()
		{
			m_UpwardReferenceMethod = UpwardReferenceMethod.World,
			m_PolarLeftRange = 180f,
			m_PolarRightRange = 180f,
			m_ElevationDownRange = 50f,
			m_ElevationUpRange = 50f,
			m_Rebound = true,
			m_ReboundDelay = 1f,
			m_ReboundPeriod = 1.5f,
			m_ReboundCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f)
		};
		public Method m_Method = new Method()
		{
			m_MoveMethod = MoveMethod.OrbitLerp,
			m_PositionSpeed = 8f,
			m_RotationMethod = RotationMethod.LerpUnclamped,
			m_RotationSpeed = 8f,
			m_ImproveAccuracy = 90f,
			m_IsRelatedAngle = false
		};
		public PositionOverrider m_PositionOverrider = new PositionOverrider();
		#endregion
	}
}
