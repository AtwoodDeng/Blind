//#define DEBUG_TMP_CAMERA

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace CF.CameraBot
{
    public partial class CameraBotEditor
    {
        #region Temp Camera in Editor
        private const string TEMP_CAMERA = "_tmp_Camera";
        private Dictionary<Preset, Camera> TmpCamera = new Dictionary<Preset, Camera>();
		/// <summary>Inits the editor camera on scene.</summary>
		/// <seealso cref="http://answers.unity3d.com/questions/638778/looking-for-a-workaround-for-drawcameragame-view-c.html?sort=oldest"/>
		private void InitTmpCameraOnScene()
		{
			if (EditorApplication.isPlayingOrWillChangePlaymode)
				return;
			ClearTmpCameraOnScene();
			for (int i = 0; i < self.PresetList.Count; i++)
			{
				if (!self.PresetList[i].m_DisplayOnScene || self.PresetList[i] == null)
					continue;
				GameObject tmp = new GameObject(TEMP_CAMERA, typeof(Camera));
#if !DEBUG_TMP_CAMERA
				tmp.hideFlags = HideFlags.HideAndDontSave;
#endif
				Camera cam = tmp.GetComponent<Camera>();
				cam.transform.SetParent(self.PresetList[i].transform);
				RenderTexture renderTexture = new RenderTexture(TmpCameraWidth, TmpCameraHeight,
					24, RenderTextureFormat.Default, RenderTextureReadWrite.Default);
				cam.targetTexture = renderTexture;
				cam.Render();
				TmpCamera.Add(self.PresetList[i], cam);
			}
		}
		private void ClearTmpCameraOnScene()
		{
#if !DEBUG_TMP_CAMERA
			GameObject tmp;
			TmpCamera.Clear();
			do
			{
				tmp = GameObject.Find(TEMP_CAMERA);
				if (!ReferenceEquals(null, tmp))
				{
					Camera cam = tmp.GetComponent<Camera>();
					if (cam != null && cam.targetTexture != null)
						cam.targetTexture = null;
					DestroyImmediate(tmp);
				}
			}
			while (!ReferenceEquals(null, tmp));
#endif
        }
		#endregion

		#region Visulize Editor Only
		private Vector2 cameraScroll = Vector2.zero;
		
		private static class panel
		{
			public static readonly Rect size = new Rect(0f, 0f, 220f, 240f);
			public const float margin = 10f;
			public static float innerWidth { get { return size.width - (margin * 2f); } }
			public static float innerHeight { get { return size.height - (margin * 2f); } }
		}

		private void OnScenePresetDraw(Preset preset)
		{
			if (preset == null)
			{
				self.OnValidate();
				return;
			}
			
			// for any reason fail safe, if it failure to find tmp-camera.
			if (TmpCamera == null ||
				!TmpCamera.ContainsKey(preset) ||
				TmpCamera[preset] == null)
			{	
				InitTmpCameraOnScene();
				return;
			}

			// maintance hierarchy structure.
			if (preset.transform.parent != self.transform)
			{
				preset.transform.SetParent(self.transform, false);
			}

			PresetEditor.OnSceneTargetForwardReference(controlId, preset);
			OnSceneHandleCoordinatesTools(preset);
			PresetEditor.OnSceneRotationHints(controlId, preset, delta5sec);
			PresetEditor.OnSceneZoomRange(preset);
		}
		/// <summary>Draw a set of camera preview on left-top</summary>
		private void OnSceneDrawCameraSets()
		{
			int total = GetCameraDisplayOnScene;
			if (total <= 0)
				return;

			int col = 0, row = 0;
			GUILayout.BeginArea(panel.size);
			cameraScroll = GUI.BeginScrollView(panel.size, cameraScroll, new Rect(0f, 0f, total * panel.size.width, panel.innerHeight));
			for (int i = 0; i < self.PresetList.Count; i++)
			{
				if (self.PresetList[i] == null)
					continue;
				try
				{
					if (self.PresetList[i].m_DisplayOnScene)
					{
						if (self.PresetList[i].m_Editing)
							cameraScroll = new Vector2(col * panel.size.width, row * panel.size.height);
						OnSceneCameraPreview(self.PresetList[i], col, row, TmpCameraWidth, TmpCameraHeight);
						col++;
					}
					self.PresetList[i].m_Editing = false;
				}
				catch { }
			}
			GUI.EndScrollView();
			GUILayout.EndArea();
		}
		/// <summary>Draw single camera preview</summary>
		/// <param name="preset"></param>
		/// <param name="col"></param>
		/// <param name="row"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		private void OnSceneCameraPreview(Preset preset, int col = 0, int row = 0, float width = 200, float height = 200)
		{
			if (TmpCamera.ContainsKey(preset))
			{
				float LabelH = 20f;
				float rWidth = width + panel.margin * 2f;
				float rHeight = height + panel.margin + LabelH;
				float baseX = col * rWidth;
				float baseY = row * rHeight;
				GUI.BeginGroup(new Rect(baseX, baseY, rWidth, rHeight), GUI.skin.textArea);
				GUI.Label(new Rect(panel.margin, 0f, width, LabelH), preset.name);
				Camera cam = TmpCamera[preset];
				if (cam != null)
				{
					cam.Render();
					Rect area = new Rect(panel.margin, panel.margin * 2f, width, height);
					GUI.DrawTexture(area, cam.targetTexture);
				}
				GUI.EndGroup();
			}
		}
		/// <summary>Draw handler of camera stand. Move & Rotate.</summary>
		/// <param name="preset"></param>
		private void OnSceneHandleCoordinatesTools(Preset preset)
        {
			if (Tools.current == Tool.Move)
			{
				PresetEditor.OnSceneHandlePosition(preset);
				// Override Move tools in EditorScene
				Tools.hidden = IsAnyCameraDisplayOnScene;
			}
			else if (Tools.current == Tool.Rotate)
			{
				// Override Move tools in EditorScene
				Tools.hidden = IsAnyCameraDisplayOnScene;
				bool moveAll = Event.current.control;
				float diffH, diffV;
				if (PresetEditor.OnSceneHandleRotation(preset, moveAll, out diffH, out diffV))
				{
					if (moveAll)
					{
						foreach (Preset otherPreset in self.PresetList)
						{
							if (otherPreset.m_DisplayOnScene)
							{
								PresetEditor.UpdatePresetCoordinatesValue(otherPreset, diffH, diffV);
							}
						}
					}
				}
			}
			// Update Relative PointTo Location
			if (TmpCamera.ContainsKey(preset))
			{
				TmpCamera[preset].transform.position = preset.Instance.GetCameraPivot().position;
				Vector3 upward;
				switch(preset.m_ClampAngle.m_UpwardReferenceMethod)
				{
					default:
					case UpwardReferenceMethod.World: upward = Vector3.up; break;
					case UpwardReferenceMethod.Custom: upward = (preset.m_ClampAngle.m_UpReference == null)? Vector3.up : preset.m_ClampAngle.m_UpReference.up; break;
					case UpwardReferenceMethod.Local: upward = Vector3.Lerp(Vector3.Project(preset.Instance.m_CameraPivot.transform.up, preset.Instance.transform.up), preset.Instance.transform.up, .5f); break;
					case UpwardReferenceMethod.RealLocal: upward = preset.Instance.CameraPivot.transform.up; break;
				}
				TmpCamera[preset].transform.LookAt(preset.Instance.GetCameraLookAt(), upward);
			}
		}
		#endregion
	}
}
#endif