#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Kit.Extend;
using Kit.Coordinates;

namespace CF.CameraBot
{
	[CustomEditor(typeof(Preset), true)]
	public class PresetEditor : Editor
	{
		private Preset self;
		private int controlId;
		private double lastUpdateTime;
		private float delta, delta1sec, delta5sec, delta10sec;
#pragma warning disable 414
		/// <summary>Not used. <see cref="OnSceneHandleRotation(Preset, bool, out float, out float)"/></summary>
		private float diffH, diffV;
#pragma warning restore 414

		#region System
		void OnEnable()
		{
			Tools.hidden = true;
			self = (Preset)target;
			self.OnValidate();
			controlId = GUIUtility.GetControlID(FocusType.Passive);
			delta = delta1sec = delta5sec = delta10sec = 0f;
			lastUpdateTime = EditorApplication.timeSinceStartup;
			EditorApplication.update += EditorUpdate;
		}

		void OnDisable()
		{
			Tools.hidden = false;
			EditorApplication.update -= EditorUpdate;
			
			// force update addon list;
			self.m_PositionOverrider.Update(self);
		}
		void EditorUpdate()
		{
			delta = System.Convert.ToSingle(EditorApplication.timeSinceStartup - lastUpdateTime);
			lastUpdateTime = EditorApplication.timeSinceStartup;
			delta1sec = Mathf.Repeat(delta + delta1sec, 1f);
			delta5sec = Mathf.Repeat(delta / 5f + delta5sec, 1f);
			delta10sec = Mathf.Repeat(delta / 10f + delta10sec, 1f);

			// force update addon list;
			self.m_PositionOverrider.Update(self);

			SceneView.RepaintAll();
		}

		void OnSceneGUI()
		{
			OnSceneTargetForwardReference(controlId, self);
			OnSceneHandlePosition(self);
			OnSceneHandleRotation(self, false, out diffH, out diffV);
			OnSceneRotationHints(controlId, self, delta5sec);
			OnSceneZoomRange(self);
		}
		#endregion

		/// <summary>Handle developer relative postiion adjustment</summary>
		/// <param name="preset"></param>
		/// <returns></returns>
		internal static bool OnSceneHandlePosition(Preset preset)
		{
			bool changed = false;
			// Visualize handler's
			if (Tools.current == Tool.Move)
			{
				Tools.hidden = true;
				Quaternion handlerQuat = (Tools.pivotRotation == PivotRotation.Global) ? Quaternion.identity : preset.transform.rotation;

				// Camera position handler
				float
					radius = preset.m_VirtualPosition.m_Camera.m_Coordinates.radius;
				Transform
					tran = preset.Instance.m_ChaseTargetOffsetHandler.transform;
				Vector3
					org = tran.position.PointOnDistance((preset.Instance.m_PitchHandler.transform.forward * radius), radius),
					edit = Handles.PositionHandle(org, handlerQuat),
					ld1 = tran.InverseTransformPoint(org),
					ld2 = tran.InverseTransformPoint(edit);

				if (!edit.Equals(org))
				{
					Undo.RecordObject(preset, "Change Camera coordinates" + preset.GetInstanceID());
					SphericalCoordinates
						sc1 = SphericalCoordinates.RelativeToTarget(ld1, Vector3.zero),
						sc2 = SphericalCoordinates.RelativeToTarget(ld2, Vector3.zero);
					preset.Instance.YawDegree = preset.m_VirtualPosition.m_Camera.m_Coordinates.Yaw += sc1.Yaw - sc2.Yaw;
					preset.Instance.PitchDegree = preset.m_VirtualPosition.m_Camera.m_Coordinates.Pitch += sc1.Pitch - sc2.Pitch;
					preset.Instance.OrbitDistance = preset.m_VirtualPosition.m_Camera.m_Coordinates.radius = sc2.radius;
					preset.m_Editing = true;
					changed = true;
				}

				// ExtendLookAt handler
				if (preset.m_VirtualPosition.m_EnableLookTarget)
				{
					radius = preset.m_VirtualPosition.m_LookTarget.m_Coordinates.radius;
					tran = preset.Instance.m_PitchLookAtHandler.transform;
					org = tran.position.PointOnDistance((tran.forward * radius), radius);
					edit = Handles.PositionHandle(org, handlerQuat);
					ld1 = tran.InverseTransformPoint(org);
					ld2 = tran.InverseTransformPoint(edit);
					if (!edit.Equals(org))
					{
						Undo.RecordObject(preset, "Change lookAt coordinates" + preset.GetInstanceID());
						SphericalCoordinates
							sc1 = SphericalCoordinates.RelativeToTarget(ld1, Vector3.zero),
							sc2 = SphericalCoordinates.RelativeToTarget(ld2, Vector3.zero);
						preset.Instance.YawLookAtDegree = preset.m_VirtualPosition.m_LookTarget.m_Coordinates.Yaw += sc1.Yaw - sc2.Yaw;
						preset.Instance.PitchLookAtDegree = preset.m_VirtualPosition.m_LookTarget.m_Coordinates.Pitch += sc1.Pitch - sc2.Pitch;
						preset.Instance.OrbitLookAtDistance = preset.m_VirtualPosition.m_LookTarget.m_Coordinates.radius = sc2.radius;
						preset.m_Editing = true;
						changed = true;
					}
				}
			}
			return changed;
		}

		/// <summary>Handle developer relative angle adjustment</summary>
		/// <param name="preset"></param>
		/// <param name="moveAll"></param>
		/// <param name="diffH"></param>
		/// <param name="diffV"></param>
		/// <returns></returns>
		internal static bool OnSceneHandleRotation(Preset preset, bool moveAll, out float diffH, out float diffV)
		{
			bool changed = false;
			diffH = 0f;
			diffV = 0f;
			if (Tools.current == Tool.Rotate)
			{
				Quaternion edit;

				// Yaw handle
				Handles.color = (moveAll) ? Color.white : Handles.xAxisColor;
				Transform tran = preset.Instance.m_YawHandler.transform;
				EditorGUI.BeginChangeCheck();
				edit = Handles.Disc(tran.rotation, tran.position, tran.up, preset.m_VirtualPosition.m_Camera.m_Coordinates.radius + .05f, false, 0f);
				if (EditorGUI.EndChangeCheck())
				{
					changed = true;
					diffH = edit.AngleBetweenDirectionSigned(tran.rotation, Vector3.forward, tran.up);
				}

				// Pitch handle
				Handles.color = (moveAll) ? Color.white : Handles.yAxisColor;
				tran = preset.Instance.m_PitchHandler.transform;
				EditorGUI.BeginChangeCheck();
				edit = Handles.Disc(tran.rotation, tran.position, tran.right, preset.m_VirtualPosition.m_Camera.m_Coordinates.radius + .05f, false, 0f);
				if (EditorGUI.EndChangeCheck())
				{
					changed = true;
					diffV = edit.AngleBetweenDirectionSigned(tran.rotation, Vector3.forward, tran.right);
				}
			}

			if(changed)
			{
				preset.m_Editing = true;
				UpdatePresetCoordinatesValue(preset, diffH, diffV);
			}

			return changed;
		}

		/// <summary>Modify camera pivot and lookat transform, based on developer input</summary>
		/// <param name="preset"></param>
		/// <param name="diffH"></param>
		/// <param name="diffV"></param>
		internal static void UpdatePresetCoordinatesValue(Preset preset, float diffH, float diffV)
		{
			Undo.RecordObjects(new Object[] {
				preset,
				preset.Instance.m_YawHandler.transform,
				preset.Instance.m_PitchHandler.transform
			}, "Change orbit rotation" + preset.GetInstanceID());
			// Camera
			bool
				flipLP = preset.m_VirtualPosition.m_Camera.m_OppositePolar != preset.m_VirtualPosition.m_LookTarget.m_OppositePolar,
				flipLE = preset.m_VirtualPosition.m_Camera.m_OppositeElevation != preset.m_VirtualPosition.m_LookTarget.m_OppositeElevation;

			preset.m_VirtualPosition.m_Camera.m_Coordinates.Yaw -= diffH;
			preset.Instance.YawDegree = preset.m_VirtualPosition.m_Camera.m_Coordinates.Yaw;
			preset.m_VirtualPosition.m_Camera.m_Coordinates.Pitch -= diffV;
			preset.Instance.PitchDegree = preset.m_VirtualPosition.m_Camera.m_Coordinates.Pitch;

			// Look target
			preset.m_VirtualPosition.m_LookTarget.m_Coordinates.Yaw += diffH * (flipLP ? 1f : -1f);
			preset.Instance.YawLookAtDegree = preset.m_VirtualPosition.m_LookTarget.m_Coordinates.Yaw;
			preset.m_VirtualPosition.m_LookTarget.m_Coordinates.Pitch += diffV * (flipLE ? 1f : -1f);
			preset.Instance.PitchLookAtDegree = preset.m_VirtualPosition.m_LookTarget.m_Coordinates.Pitch;
		}

		internal static void OnSceneRotationHints(int id, Preset preset, float clock01)
		{
			if (Application.isPlaying || Tools.current != Tool.Rotate)
				return;

			Transform
				cam = preset.Instance.m_CameraOrbitPivot.transform,
				lookAt = preset.Instance.GetCameraLookAt();
			VirtualPosition vp = preset.m_VirtualPosition;
			Handles.color = clock01 > .5f? Color.red : Color.blue;
			if (clock01 > .5f)
				Handles.ArrowCap(id, cam.position, (vp.m_Camera.m_OppositePolar ? Quaternion.LookRotation(cam.right, cam.up) : Quaternion.LookRotation(-cam.right, cam.up)), clock01 * .5f);
			else
				Handles.ArrowCap(id, cam.position, (vp.m_Camera.m_OppositeElevation ? Quaternion.LookRotation(cam.up, -cam.forward) : Quaternion.LookRotation(-cam.up, cam.forward)), clock01 * .5f);

			if (vp.m_EnableLookTarget)
			{
				if (clock01 > .5f)
					Handles.ArrowCap(id, lookAt.position, (vp.m_LookTarget.m_OppositePolar ? Quaternion.LookRotation(lookAt.right, lookAt.up) : Quaternion.LookRotation(-lookAt.right, lookAt.up)), clock01 * .5f);
				else
					Handles.ArrowCap(id, lookAt.position, (vp.m_LookTarget.m_OppositeElevation ? Quaternion.LookRotation(lookAt.up, -lookAt.forward) : Quaternion.LookRotation(-lookAt.up, lookAt.forward)), clock01 * .5f);
			}
		}

		internal static void OnSceneZoomRange(Preset preset)
		{
			Handles.color = preset.m_DebugColor;
			if (preset != null &&
				preset.m_Zoom.m_Distance > 0f)
			{
				Transform cam = preset.Instance.m_ZoomHandler.transform.parent;
				Vector3 forwardPoint = (cam.position + (-cam.forward * -preset.m_Zoom.m_ForwardLimit));
				Vector3 backwardPoint = (cam.position + (-cam.forward * -preset.m_Zoom.m_BackwardLimit));
				Handles.DrawLine(backwardPoint, forwardPoint);
				Handles.SphereCap(0, forwardPoint, Quaternion.identity, .05f);
				Handles.SphereCap(0, backwardPoint, Quaternion.identity, .05f);
			}
		}

		internal static void OnSceneTargetForwardReference(int id, Preset preset)
		{
			Handles.SphereCap(id, preset.ChaseTarget.position, Quaternion.identity, .05f);
			Handles.ArrowCap(id, preset.ChaseTarget.position, preset.ChaseTargetRotation.rotation, 0.5f);
		}
	}
}
#endif