#if UNITY_EDITOR
using UnityEngine;
using System.Linq;
using UnityEditor;
using Kit.Extend;

namespace CF.CameraBot
{
	[DisallowMultipleComponent]
    [CustomEditor(typeof(CameraBot), true)]
    public partial class CameraBotEditor : Editor
    {
        #region Const
        const float Circle = 360f;
        const float SemiCircle = 180f;
        const float QuarterCircle = 90f;
        const int TmpCameraWidth = 200;
        const int TmpCameraHeight = 200;
		#endregion

		#region internal variable
		private CameraBot self;
		internal int controlId;
		private double lastUpdateTime, last5Sec;
		private float delta, delta1sec, delta5sec, delta10sec;
		#endregion

		#region getter setter
		private int GetCameraDisplayOnScene { get { return self.PresetList.Count(x => x.m_DisplayOnScene); } }
		private bool IsAnyCameraDisplayOnScene { get { return GetCameraDisplayOnScene > 0; } }
		#endregion

		#region system
		void OnEnable()
		{
			Tools.hidden = true;
			controlId = GUIUtility.GetControlID(FocusType.Passive);
			self = (CameraBot)target;
			InitReorderableList();
			InitTmpCameraOnScene();
			delta = delta1sec = delta5sec = delta10sec = 0f;
			lastUpdateTime = EditorApplication.timeSinceStartup;
			EditorApplication.update += EditorUpdate;
		}
		void OnDisable()
		{
			Tools.hidden = false;
			EditorApplication.update -= EditorUpdate;
			DestoryReorderableList();
			ClearTmpCameraOnScene();
		}
		void EditorUpdate()
		{
			delta = System.Convert.ToSingle(EditorApplication.timeSinceStartup - lastUpdateTime);
			lastUpdateTime = EditorApplication.timeSinceStartup;
			delta1sec = Mathf.Repeat(delta + delta1sec, 1f);
			delta5sec = Mathf.Repeat(delta / 5f + delta5sec, 1f);
			delta10sec = Mathf.Repeat(delta / 10f + delta10sec, 1f);
			// Debug.LogFormat("Update {0:F2} {1:F2} {2:F2}", delta, delta1sec, delta10sec);
			last5Sec += delta5sec;
			if(last5Sec >= 1f)
			{
				last5Sec = 0f;
				foreach(Preset preset in self.PresetList)
				{
					preset.EditorUpdate();
				}
			}

			SceneView.RepaintAll();
		}
		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.PropertyField(serializedObject.FindProperty("ChaseTarget"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("TargetForward"));
			EditorGUILayout.Separator();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("ControlPosition"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("ControlRotation"));
			EditorGUILayout.Separator();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("InputSetting"), true);
			EditorGUILayout.Separator();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("AdvanceSetting"), true);

			EditorGUILayout.Separator();
			if (self != null && self.PresetList.Count > 0 && self.Selected < self.PresetList.Count && self.PresetList[self.Selected] != null)
				EditorGUILayout.HelpBox("Camera : " + self.PresetList[self.Selected].name, MessageType.Info);
			else
			{
				EditorGUILayout.HelpBox("Camera : None", MessageType.Warning);
				self.Selected = 0;
			}
			EditorGUI.BeginChangeCheck();
			self.Selected = EditorGUILayout.IntSlider("Default Camera", self.Selected, 0, self.PresetList.Count - 1);
			if(EditorGUI.EndChangeCheck())
			{
				self.OnValidate();
			}
			EditorGUILayout.Separator();

			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("All"))
			{
				foreach (Preset obj in self.PresetList)
					obj.m_DisplayOnScene = true;
				EditorUtility.SetDirty(self);
			}
			if (GUILayout.Button("Toggle All"))
			{
				foreach (Preset obj in self.PresetList)
					obj.m_DisplayOnScene = !obj.m_DisplayOnScene;
				EditorUtility.SetDirty(self);
			}
			EditorGUILayout.EndHorizontal();

			DrawCameraPresetList();

			DrawControllerSet();

			if (GUI.changed)
				serializedObject.ApplyModifiedProperties();
		}
		void OnSceneGUI()
		{
			if (ReferenceEquals(self, null) ||
				ReferenceEquals(self.ChaseTarget, null) ||
				self.PresetList.Count == 0)
				return;
			if (EditorApplication.isPlayingOrWillChangePlaymode || !self.enabled)
				OnSceneGUIPlayMode();
			else
				OnSceneGUIEditorMode();
		}
		#endregion

		#region Developing scene view
		private void OnSceneGUIPlayMode()
		{
			Preset preset = self.PresetList[self.Selected];
			if (preset == null)
				return;

			PresetEditor.OnSceneZoomRange(preset);
		}
		private void OnSceneGUIEditorMode()
		{
			for (int i = 0; i < self.PresetList.Count; i++)
			{
				if (self.PresetList[i] != null &&
					self.PresetList[i].m_DisplayOnScene &&
					self.ChaseTarget != null &&
					self.ControlPosition != null)
				{
					OnScenePresetDraw(self.PresetList[i]);
				}
			}
			OnSceneDrawCameraSets();
		}
		#endregion
	}
}
#endif