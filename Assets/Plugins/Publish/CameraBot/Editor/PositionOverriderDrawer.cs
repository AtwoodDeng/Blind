#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Kit.Extend;

namespace CF.CameraBot
{
    [CustomPropertyDrawer(typeof(PositionOverrider))]
    public class PositionOverriderDrawer : PropertyDrawer
    {
        private static Texture _addon;
        private static Texture addon { get { if (_addon == null) _addon = Resources.Load("detection") as Texture; return _addon; } }

        private GUIContent PositionOverriderLabel = new GUIContent("Position Overrider", addon, "Camera final position will based on addon(s).");
		readonly static float
			lineH = EditorGUIUtility.singleLineHeight,
			lineS = EditorGUIUtility.standardVerticalSpacing;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			Vector2 orgIconSize = EditorGUIUtility.GetIconSize();
			EditorGUI.BeginProperty(position, label, property);

			Rect line = position.Clone(height: lineH + 16f);
			EditorGUIUtility.SetIconSize(Vector2.one * 32f);
			property.isExpanded = EditorGUI.Foldout(line, property.isExpanded, PositionOverriderLabel, true);
			EditorGUIUtility.SetIconSize(orgIconSize);

			if (property.isExpanded)
            {
				SerializedProperty addonCount = property.FindPropertyRelative("m_AddonCount");
				int lineCnt = addonCount.intValue + 1;
				line = line.GetRectBottom(height: lineH * lineCnt).Modify(y: lineS);
				SerializedProperty addonList = property.FindPropertyRelative("m_AddonList");
				EditorGUI.BeginDisabledGroup(true);
				EditorGUI.TextArea(line, addonList.stringValue);
				EditorGUI.EndDisabledGroup();
            }

			EditorGUI.EndProperty();
        }

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return GetStaticHeight(property);
		}

		public static float GetStaticHeight(SerializedProperty property)
		{
			SerializedProperty addonCount = property.FindPropertyRelative("m_AddonCount");
			int lineCnt = addonCount.intValue + 1;
			float height = 34f;
			if(property.isExpanded)
				height += (lineH * lineCnt);
			return height;
		}
	}
}
#endif