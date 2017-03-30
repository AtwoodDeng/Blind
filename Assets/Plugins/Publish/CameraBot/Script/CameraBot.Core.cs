using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Kit.Extend;

namespace CF.CameraBot
{
	/// <summary>
	/// CF Camera for Unity3D - a spherical coordinates camera handler.
	/// </summary>
	/// <see cref="http://www.clonefactor.com"/>
	public partial class CameraBot : MonoBehaviour
	{
		#region Const
		const float Circle = 360f;
		const float SemiCircle = 180f;
		const float QuarterCircle = 90f;
		#endregion

		#region system
		public void OnValidate()
		{
			if (!Application.isPlaying)
			{
				if (ChaseTarget != null && TargetForward == null)
					TargetForward = ChaseTarget;
				if (ControlPosition != null && ControlRotation == null)
					ControlRotation = ControlPosition;
				if (ControlRotation != null && ControlPosition == null)
					ControlPosition = ControlRotation;

				// Ensure preset format
				ValidElementOrder();
			}
			else
			{
				// in case developer change active camera in inspector.
				HandlePresetActiveState();
			}
		}
		void Awake()
		{
			InitRequiredObject();
			InitCameraPosition();
			InitReference();
		}
		void LateUpdate()
		{
			if (AdvanceSetting.UpdateFrequency.Equals(UpdateFrequency.Medium))
			{
				UpdateCameraStandTransformByChaseTarget();
				UpdateActiveCamera();
			}
			else if (AdvanceSetting.UpdateFrequency.Equals(UpdateFrequency.Low))
			{
				UpdateCameraStandTransformByChaseTarget();
				UpdateInstanceTransform();
				UpdateActiveCamera();
			}
		}
		void FixedUpdate()
		{
			if (AdvanceSetting.UpdateFrequency == UpdateFrequency.High)
			{
				UpdateCameraStandTransformByChaseTarget();
				UpdateInstanceTransform();
				UpdateActiveCamera();
			}
			else if (AdvanceSetting.UpdateFrequency.Equals(UpdateFrequency.Medium))
			{
				UpdateInstanceTransform();
			}
		}
		#endregion

		#region Init
		protected void InitRequiredObject()
		{
			if (ControlPosition == null)
				ControlPosition = Camera.main.transform;
			if (ControlRotation == null)
				ControlRotation = ControlPosition;
			if (ControlPosition == null)
			{
				this.enabled = false;
				throw new MissingReferenceException("CameraBot missing ControlPosition.");
			}
			if (ChaseTarget == null)
			{
				this.enabled = false;
				throw new MissingReferenceException("CameraBot missing chaseTarget.");
			}
			if (TargetForward == null)
			{
				TargetForward = ChaseTarget;
			}
			if (PresetList.Count == 0)
			{
				Debug.LogError("CameraBot require at least one camera.");
				this.enabled = false;
			}
		}
		protected void InitReference()
		{
			foreach(Preset preset in PresetList)
			{
				// Init Camera Reference;
				UpdateOrbitCoordinateData(0f, 0f, 0f, preset);
				UpdateZoomSectionData(preset, 0f);
			}
        }
        protected void ResetToInitStage(int pt)
        {
			// reset zoom section & angle when switch
			PresetList[pt].ResetToInitStage();
        }
		
		protected void InitCameraPosition()
		{
			if (ControlPosition == null || ControlRotation == null)
			{
				this.enabled = false;
				throw new MissingReferenceException("CameraBot missing ControlPosition / ControlLookAt.");
			}
			else
			{
				SwitchCamera(Selected);
				activePreset.OneFrameSnapRequest = true;
				UpdateCameraStandTransformByChaseTarget();
			}
		}
        #endregion

        #region core
        private void UpdateActiveCamera()
        {
			if (Selected >= 0 &&
				Selected < PresetList.Count
				&& activePreset != null)
			{
				UpdateActiveCameraPosition();
				UpdateActiveCameraRotation();
				ClearOneFrameRequests();
			}
			else
			{
				Selected = 0;
			}
        }
		private void UpdateActiveCameraPosition()
		{
			if (ControlPosition == null)
				return;
			
			if(activePreset.OneFrameSnapRequest)
			{
				ControlPosition.position = activePreset.Instance.CameraPivot.transform.position;
				return;
			}

			float time = (AdvanceSetting.UpdateFrequency < UpdateFrequency.High) ? Time.deltaTime : Time.fixedDeltaTime;
			time *= activePreset.m_Method.m_PositionSpeed;
			ControlPosition.position =
				(activePreset.m_Method.m_MoveMethod.Equals(MoveMethod.QuaternionLerp)) ? Vector3.Slerp(ControlPosition.position, activePreset.Instance.CameraPivot.transform.position, time) :
				(activePreset.m_Method.m_MoveMethod.Equals(MoveMethod.Lerp)) ? Vector3.Lerp(ControlPosition.position, activePreset.Instance.CameraPivot.transform.position, time) :
				(activePreset.m_Method.m_MoveMethod.Equals(MoveMethod.OrbitLerp)) ? GetOrbitLerpFramePosition(activePreset, time) : activePreset.Instance.CameraPivot.transform.position;
		}
        private void UpdateActiveCameraRotation()
		{
			if (ControlRotation == null)
				return;
			
			if(activePreset.OneFrameSnapRequest)
			{
				ControlRotation.rotation = activePreset.Instance.GetCameraRotation();
				return;
			}

			float time = (AdvanceSetting.UpdateFrequency < UpdateFrequency.High) ? Time.deltaTime : Time.fixedDeltaTime;
			time *= activePreset.m_Method.m_RotationSpeed;
			ControlRotation.rotation =
				(activePreset.m_Method.m_RotationMethod.Equals(RotationMethod.QuaternionLerp)) ? Quaternion.Slerp(ControlRotation.rotation, activePreset.Instance.GetCameraRotation(), time) :
				(activePreset.m_Method.m_RotationMethod.Equals(RotationMethod.Lerp)) ? Quaternion.Lerp(ControlRotation.rotation, activePreset.Instance.GetCameraRotation(), time) :
				(activePreset.m_Method.m_RotationMethod.Equals(RotationMethod.LerpUnclamped)) ? Quaternion.LerpUnclamped(ControlRotation.rotation, activePreset.Instance.GetCameraRotation(), time) : activePreset.Instance.GetCameraRotation();
		}
		private void ClearOneFrameRequests()
		{
			activePreset.OneFrameSnapRequest = false;
		}

		/// <summary>Sync <see cref="ChaseTarget"/> and <see cref="CameraStand"/>.</summary>
		/// <remarks>For Clamp angle</remarks>
		private void UpdateCameraStandTransformByChaseTarget()
		{
			if (Selected >= 0 &&
				Selected < PresetList.Count &&
				activePreset != null)
			{
				ChaseTargetTriggerAngleChange();


				activePreset.Cache.m_LastFrameTargetPosition = activePreset.transform.position;
				activePreset.Cache.m_LastFrameTargetRotation = activePreset.transform.rotation;
				if (ChaseTarget != null)
					activePreset.transform.position = ChaseTarget.position;

				if (activePreset.ChaseTargetRotation != null)
					activePreset.transform.rotation = activePreset.ChaseTargetRotation.rotation;

				if (!activePreset.OneFrameSnapRequest)
				{
					RelativeTargetRotation();
				}
			}
		}
        #endregion

        #region tools
		internal void HandlePresetActiveState()
		{
			// disable other, except selected one.
			foreach (Preset preset in PresetList)
			{
				if (preset != activePreset)
					preset.gameObject.SetActive(false);
			}
			activePreset.gameObject.SetActive(true);
			activePreset.m_DisplayOnScene = true;
		}
		internal void ValidElementOrder()
		{
			// update list when different
			IEnumerable<Preset> tmp = GetComponentsInChildren<Preset>(false).OrderBy(o => o.transform.GetSiblingIndex());
			if (tmp.Except(PresetList).Count() > 0 || PresetList.Except(tmp).Count() > 0)
			{
				PresetList = new List<Preset>(tmp);
			}

			// ensure hierarchy structure.
			foreach (Preset obj in PresetList)
			{
				obj.m_Host = this;
				if (obj.transform.parent != transform)
				{
					obj.transform.SetParent(transform);
				}
			}
		}

        protected Vector3 GetOrbitLerpFramePosition(Preset preset, float delta)
        {
			Vector3 deltaUp = Vector3.Lerp(ControlPosition.up, preset.Instance.CameraPivot.transform.up, delta);
			Quaternion
				startQuat = Quaternion.LookRotation(preset.ChaseTarget.position.Direction(ControlPosition.position), deltaUp),
				endQuat = Quaternion.LookRotation(preset.ChaseTarget.position.Direction(preset.Instance.CameraPivot.transform.position), deltaUp);
			Vector3 deltaDirection = Quaternion.LerpUnclamped(startQuat, endQuat, delta).GetForward();

			float
				startDistance = (preset.ChaseTarget.position - ControlPosition.position).sqrMagnitude,
				endDistance = (preset.ChaseTarget.position - preset.Instance.CameraPivot.transform.position).sqrMagnitude,
				deltaDistance = Mathf.Sqrt(Mathf.Lerp(startDistance, endDistance, delta));
			return preset.ChaseTarget.position.PointOnDistance(deltaDirection, deltaDistance);
		}
        #endregion
    }
}