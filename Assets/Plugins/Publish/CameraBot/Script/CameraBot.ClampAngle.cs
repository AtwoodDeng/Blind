// #define TRIGGER_LOG
using UnityEngine;
using Kit.Extend;

namespace CF.CameraBot
{
	public partial class CameraBot : MonoBehaviour
	{
		#region Core
		private void UpdateOrbitCoordinateData(float vertical, float horizontal, float amount, Preset preset)
		{
			// convert input valus into angle
			amount *= Mathf.Rad2Deg;
			preset.Cache.m_YawClampAngleDelta += horizontal * amount;
			preset.Cache.m_PitchClampAngleDelta += vertical * amount;
		}

		/// <summary>In order to remind angle within range, generate reverse input based on target rotation.</summary>
		private void RelativeTargetRotation()
		{
			// do nothing if user need a static local angle related to chase target.
			if (!activePreset.m_Method.m_IsRelatedAngle)
			{
				// try to remind lastFramePosition, find angle difference.
				Quaternion
					qt1 = activePreset.Cache.m_LastFrameTargetRotation,
					qt2 = activePreset.transform.rotation,
					qt3 = qt1.Inverse() * qt2;
				
				Vector3 diff = qt3.eulerAngles;
				diff.x = ToSemiCircleSigned(diff.x);
				diff.y = ToSemiCircleSigned(diff.y);
				diff.z = ToSemiCircleSigned(diff.z);
				diff *= (activePreset.m_VirtualPosition.m_Camera.m_OppositePolar ? -1f : 1f);
				activePreset.Cache.m_YawClampAngleDelta += diff.y;
				activePreset.Cache.m_PitchClampAngleDelta += diff.x;
			}
		}

		private void UpdateInstanceTransform()
		{
			foreach (Preset preset in PresetList)
			{

				float
					flipCP = (preset.m_VirtualPosition.m_Camera.m_OppositePolar ? -1f : 1f),
					flipCE = (preset.m_VirtualPosition.m_Camera.m_OppositeElevation ? -1f : 1f),
					flipLP = (preset.m_VirtualPosition.m_LookTarget.m_OppositePolar == preset.m_VirtualPosition.m_Camera.m_OppositePolar ? 1f : -1f),
					flipLE = (preset.m_VirtualPosition.m_LookTarget.m_OppositeElevation == preset.m_VirtualPosition.m_Camera.m_OppositeElevation ? 1f : -1f),
					yawDegree = preset.Instance.YawDegreeSignedDiff,
					pitchDegree = preset.Instance.PitchDegreeSignedDiff,
					reboundYaw = YawReboundForce(preset, yawDegree),
					reboundPitch = PitchReboundForce(preset, pitchDegree),
					finalDeltaYaw = (preset.Cache.m_YawClampAngleDelta * flipCP + reboundYaw),
					finalDeltaPitch = (preset.Cache.m_PitchClampAngleDelta * flipCE + reboundPitch);

				float
					deltaYaw = ClampYawAngle(preset, yawDegree, finalDeltaYaw),
					deltaPitch = ClampPitchAngle(preset, pitchDegree, finalDeltaPitch);
				// if (finalDeltaYaw != 0f) Debug.Log("yawDegree " + yawDegree + " delta " + finalDeltaYaw);
				// if (finalDeltaPitch != 0f) Debug.Log("pitchDegree " + pitchDegree + " delta " + finalDeltaPitch);

				preset.Cache.m_YawClampAngleDelta = preset.Cache.m_PitchClampAngleDelta = 0f;

				// camera pivot
				preset.Instance.YawDegree = preset.m_VirtualPosition.m_Camera.m_Coordinates.Yaw -= deltaYaw;
				preset.Instance.PitchDegree = preset.m_VirtualPosition.m_Camera.m_Coordinates.Pitch -= deltaPitch;

				// Look target
				preset.Instance.YawLookAtDegree = preset.m_VirtualPosition.m_LookTarget.m_Coordinates.Yaw -= deltaYaw * flipLP;
				preset.Instance.PitchLookAtDegree = preset.m_VirtualPosition.m_LookTarget.m_Coordinates.Pitch -= deltaPitch * flipLE;

				// camera distance
				preset.Instance.ZoomRange = Mathf.Lerp(preset.Instance.ZoomRange, preset.Cache.m_ZoomSectionDelta, preset.m_Zoom.m_Speed);
			}
		}

		private float YawReboundForce(Preset preset, float degree)
		{
			// Needed to Rebound
			return preset.Cache.m_ReboundYaw
				.GetReboundForce(
					degree,
					preset.m_ClampAngle.m_ReboundCurve,
					preset.m_ClampAngle.m_ReboundPeriod,
					(AdvanceSetting.UpdateFrequency == UpdateFrequency.High) ? Time.fixedDeltaTime : Time.deltaTime
				);
		}
		private float ClampYawAngle(Preset preset, float degree, float deltaYaw)
		{
			float
				laterDegree = degree + deltaYaw,
				leftLimit = preset.m_ClampAngle.m_PolarLeftRange,
				rightLimit = preset.m_ClampAngle.m_PolarRightRange;

			if ((int)leftLimit == 0 && (int)rightLimit == 0)
			{
				if (degree != 0f)
					deltaYaw = -degree; // fix runtime modify clamp angle limit
				else
					deltaYaw = 0; // can't move
			}
			else if (leftLimit + rightLimit < Circle)
			{
				if (deltaYaw < 0f && laterDegree < -leftLimit)
					deltaYaw = deltaYaw + (Mathf.Abs(laterDegree) - leftLimit);
				else if (deltaYaw > 0f && laterDegree > rightLimit)
					deltaYaw = deltaYaw - (laterDegree - rightLimit);
			}
			return deltaYaw;
		}
		private float PitchReboundForce(Preset preset, float degree)
		{
			return preset.Cache.m_ReboundPitch
				.GetReboundForce(
					degree,
					preset.m_ClampAngle.m_ReboundCurve,
					preset.m_ClampAngle.m_ReboundPeriod,
					(AdvanceSetting.UpdateFrequency == UpdateFrequency.High) ? Time.fixedDeltaTime : Time.deltaTime
				);
		}
		private float ClampPitchAngle(Preset preset, float degree, float deltaPitch)
		{
			// up is negative, down is positive.
			float
				laterDegree = degree + deltaPitch,
				upLimit = preset.m_ClampAngle.m_ElevationUpRange,
				downLimit = preset.m_ClampAngle.m_ElevationDownRange;

			if ((int)upLimit == 0 && (int)downLimit == 0)
			{
				if (degree != 0f)
					deltaPitch = -degree; // fix runtime modify clamp angle limit
				else
					deltaPitch = 0; // can't move
			}
			else if (upLimit + downLimit < Circle)
			{
				if (deltaPitch < 0f && laterDegree < -downLimit)
					deltaPitch = deltaPitch + (Mathf.Abs(laterDegree) - downLimit);
				else if (deltaPitch > 0f && laterDegree > upLimit)
					deltaPitch = deltaPitch - (laterDegree - upLimit);
			}
			
			return deltaPitch;
		}

		/// <summary>Repersent circle degree in +/- 180 degree.</summary>
		/// <remarks>left = 0 ~ -180, right = 0 ~ 180</remarks>
		/// <param name="degree"></param>
		/// <returns></returns>
		public static float ToSemiCircleSigned(float degree)
		{
			// An angle more then half circle, left + right = 180+180=360 degree.
			if (Mathf.Abs(degree) > 180f)
			{
				degree = Mathf.Repeat(180f - degree, 180f) * Mathf.Sign(-degree);
			}
			return degree;
		}
		#endregion

		#region Angle Accuracy
		/// <summary>compare current & destination angle, adjust input amount.
		/// when angle larger then threhold, the input angle radio become smaller</summary>
		/// <param name="amount"></param>
		/// <returns>the adjusted angle</returns>
		private float OverTurnAngleAccuracy(float amount)
        {
			float
				threshold = activePreset.m_Method.m_ImproveAccuracy,
                AngleDiff = activePreset.Instance.CameraOffset.AngleBetweenPosition(ControlPosition.position, activePreset.Instance.m_CameraOrbitPivot.transform.position),
                InputOverTurnHotfix = Mathf.Lerp(1f, 0.1f, ((AngleDiff > threshold) ? 1f : AngleDiff / threshold));

            return amount * InputOverTurnHotfix;
        }
		#endregion

		#region Rebound
		private void ChaseTargetTriggerAngleChange()
		{
			if (activePreset.m_ClampAngle.m_Rebound)
			{
				bool isChaseTargetTurning = Quaternion.Angle(activePreset.Cache.m_LastFrameTargetRotation, activePreset.transform.rotation) > InputSetting.Threshold;
				bool isChaseTargetMoving = !activePreset.Cache.m_LastFrameTargetPosition.EqualSimilar(activePreset.transform.position);
				if (isChaseTargetTurning && isChaseTargetMoving)
				{
					// only trigger this when player not moving camera angle, but moving chase target.
					PlayerTriggerAngleChange();
				}
			}
		}

		private void PlayerTriggerAngleChange()
        {
			if (activePreset.m_ClampAngle.m_Rebound)
			{
				activePreset.Cache.m_ReboundYaw.WaitForRebound(activePreset.m_ClampAngle.m_ReboundDelay);
				activePreset.Cache.m_ReboundPitch.WaitForRebound(activePreset.m_ClampAngle.m_ReboundDelay);
			}
        }
        #endregion
    }
}
