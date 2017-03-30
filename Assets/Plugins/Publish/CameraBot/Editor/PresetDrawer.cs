#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Kit.Extend;
using Kit.Inspector;

namespace CF.CameraBot
{
	[CustomPropertyDrawer(typeof(Preset))]
	public class PresetDrawer : PropertyDrawer
	{
		readonly static float
			lineH = EditorGUIUtility.singleLineHeight,
			lineS = EditorGUIUtility.standardVerticalSpacing;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (property.serializedObject.targetObject is CameraBot)
			{
				OnCameraBotGUI(position, property, label);
			}
			else
			{
				EditorGUI.BeginProperty(position, label, property);
				EditorGUI.PropertyField(position, property);
				EditorGUI.EndProperty();
			}
		}
		private void OnCameraBotGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			Preset preset = PropertyExtend.GetCurrent(property) as Preset;
			if (preset == null && property.serializedObject.targetObject is CameraBot)
			{
				CameraBot self = property.serializedObject.targetObject as CameraBot;
				self.OnValidate();
				return;
			}
			SerializedObject presetObj = new SerializedObject(preset);
			presetObj.Update();

			EditorGUI.BeginProperty(position, label, property);

			Rect line = position.Clone(height: lineH);
			Rect[] cells = line.SplitHorizontal(.1f, 50, 70);

			// First line
			if (GUI.Button(cells[0], (preset.m_DisplayOnScene ? "Show" : "Hide"))) { preset.m_DisplayOnScene = !preset.m_DisplayOnScene; }

			cells = cells[1].Modify(x: 20f, width: -20f).SplitHorizontal(.85f);
			property.isExpanded = EditorGUI.Foldout(cells[0].Modify(), property.isExpanded, ((!property.isExpanded) ? preset.gameObject.name : ""));
			preset.m_DebugColor = EditorGUI.ColorField(cells[1], GUIContent.none, preset.m_DebugColor);

			if (property.isExpanded)
			{
				line = line.GetRectBottom(height: lineH).Modify(y: lineS);
				preset.gameObject.name = EditorGUI.TextField(line, preset.gameObject.name);

				SerializedProperty virtualPositionProp = presetObj.FindProperty("m_VirtualPosition");
				line = line.GetRectBottom(height: VirtualPositionDrawer.GetStaticHeight(virtualPositionProp)).Modify(y: lineS);
				EditorGUI.PropertyField(line, virtualPositionProp, true);

				SerializedProperty zoomProp = presetObj.FindProperty("m_Zoom");
				line = line.GetRectBottom(height: ZoomDrawer.GetStaticHeight(zoomProp)).Modify(y: lineS);
				EditorGUI.PropertyField(line, zoomProp, true);

				SerializedProperty clampAngleProp = presetObj.FindProperty("m_ClampAngle");
				line = line.GetRectBottom(height: ClampAngleDrawer.GetStaticHeight(clampAngleProp)).Modify(y: lineS);
				EditorGUI.PropertyField(line, clampAngleProp, true);

				SerializedProperty methodProp = presetObj.FindProperty("m_Method");
				line = line.GetRectBottom(height: MethodDrawer.GetStaticHeight(methodProp)).Modify(y: lineS);
				EditorGUI.PropertyField(line, methodProp, true);

				SerializedProperty positionOverriderProp = presetObj.FindProperty("m_PositionOverrider");
				line = line.GetRectBottom(height: PositionOverriderDrawer.GetStaticHeight(positionOverriderProp)).Modify(y: lineS);
				EditorGUI.PropertyField(line, positionOverriderProp, true);
			}


			presetObj.ApplyModifiedProperties();
			EditorGUI.EndProperty();
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if (property.serializedObject.targetObject is CameraBot)
			{
				return GetPresetPropertyHeight(property);
			}
			else
			{
				return lineH + lineS;
			}
		}
		public float GetPresetPropertyHeight(SerializedProperty property)
		{ 
			Preset preset = PropertyExtend.GetCurrent(property) as Preset;
			if (preset == null)
				return 0f;
			SerializedObject presetObj = new SerializedObject(preset);
			float rst = lineH + lineS;
			
			if (property.isExpanded)
			{
				rst += 16f;
				rst += VirtualPositionDrawer.GetStaticHeight(presetObj.FindProperty("m_VirtualPosition"));
				rst += ZoomDrawer.GetStaticHeight(presetObj.FindProperty("m_Zoom"));
				rst += ClampAngleDrawer.GetStaticHeight(presetObj.FindProperty("m_ClampAngle"));
				rst += MethodDrawer.GetStaticHeight(presetObj.FindProperty("m_Method"));
				rst += PositionOverriderDrawer.GetStaticHeight(presetObj.FindProperty("m_PositionOverrider"));
				rst += lineH;
			}
			return rst;
		}

		public static float GetStaticHeight(SerializedProperty property)
		{
			float rst = lineH + lineS;
			if (property.isExpanded)
			{
				rst += VirtualPositionDrawer.GetStaticHeight(property.FindPropertyRelative("m_VirtualPosition"));
				rst += ZoomDrawer.GetStaticHeight(property.FindPropertyRelative("m_Zoom"));
				rst += ClampAngleDrawer.GetStaticHeight(property.FindPropertyRelative("m_ClampAngle"));
				rst += MethodDrawer.GetStaticHeight(property.FindPropertyRelative("m_Method"));
				rst += PositionOverriderDrawer.GetStaticHeight(property.FindPropertyRelative("m_PositionOverrider"));
				rst += lineH;
			}
			return rst;
		}
	}
}
#endif