#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Kit.Extend;

namespace CF.CameraBot
{
    [CustomPropertyDrawer(typeof(Method))]
    public class MethodDrawer : PropertyDrawer
    {
        private static Texture _method;
        private static Texture Method { get { if (_method == null) _method = Resources.Load("method") as Texture; return _method; } }

        private GUIContent MethodLabel = new GUIContent("Method", Method, "movement & update method.");
        private GUIContent UpdateAngleLabel = new GUIContent("Update Angle", "Update frequency for this camera.");
        private GUIContent MoveLabel = new GUIContent("Move Method", "Camera position translation method.");
        private GUIContent RotationLabel = new GUIContent("Rotation Method", "Camera rotation method.");
        private GUIContent AccuracyLabel = new GUIContent("Angle Accuracy", "Adjust the input fineness to improve accuracy. any input more that this number will linear lerp to zero");

		readonly static float
			lineH = EditorGUIUtility.singleLineHeight,
			lineS = EditorGUIUtility.standardVerticalSpacing;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			Vector2 orgIconSize = EditorGUIUtility.GetIconSize();
			EditorGUI.BeginProperty(position, label, property);

			Rect line = position.Clone(height: lineH + 16f);
			EditorGUIUtility.SetIconSize(Vector2.one * 32f);
			property.isExpanded = EditorGUI.Foldout(line, property.isExpanded, MethodLabel, true);
			EditorGUIUtility.SetIconSize(orgIconSize);

			if (property.isExpanded)
            {
				line = line.GetRectBottom(height: lineH * 2f).Modify(y: lineS);

				SerializedProperty prop;
                string help;
                prop = property.FindPropertyRelative("m_UpdateAngle");
                switch ((UpdateAngleMethod)prop.enumValueIndex)
                {
                    default:
                    case UpdateAngleMethod.ResetWhenActive:
                        help = "ResetWhenActive : every time active will reset to relative angle of chase target";
                        break;
                    case UpdateAngleMethod.UpdateWhenActive:
                        help = "UpdateWhenActive : only update when this preset config in use.\nRemind last time position.";
                        break;
                    case UpdateAngleMethod.UpdateAlway:
                        help = "UpdateAlway : keep update camera angle even deactivate.\nResource warning keep mulit preset angle relative each others.";
                        break;
                }
                EditorGUI.HelpBox(line, help, MessageType.Info);
				line = line.GetRectBottom(height: lineH).Modify(y: lineS);

				EditorGUI.PropertyField(line, prop, UpdateAngleLabel);

				line = line.GetRectBottom().Modify(y: lineS + 5f);
                prop = property.FindPropertyRelative("m_MoveMethod");
				EditorGUI.PropertyField(line, prop, MoveLabel);

				if (prop.enumValueIndex > 0)
				{
					line = line.GetRectBottom().Modify(y: lineS);
					EditorGUI.PropertyField(line, property.FindPropertyRelative("m_PositionSpeed"));
				}

				line = line.GetRectBottom().Modify(y: lineS);
				prop = property.FindPropertyRelative("m_RotationMethod");
				EditorGUI.PropertyField(line, prop, RotationLabel);

				if (prop.enumValueIndex > 0)
				{
					line = line.GetRectBottom().Modify(y: lineS);
					EditorGUI.PropertyField(line, property.FindPropertyRelative("m_RotationSpeed"));
				}

				line = line.GetRectBottom(height: lineH * 2f).Modify(y: lineS);
				prop = property.FindPropertyRelative("m_ImproveAccuracy");
                if (prop.floatValue < 10f)
                {
                    EditorGUI.HelpBox(line,
                        "When angle therhold are too small, the input will nearly no effect on camera position.",
                        MessageType.Warning);
                }
                else if (prop.floatValue > 180f)
                {
                    EditorGUI.HelpBox(line,
                        "When angle therhold are too big, the camera orbit may acting weird when moving really fast.",
                        MessageType.Warning);
                }
                else
                {
                    EditorGUI.HelpBox(line,
                        string.Format("When angle larger than ({0:F1}) the input value will ease down by scale, which mean reduce player input impact for final angle.", prop.floatValue),
                        MessageType.Info);
                }

				line = line.GetRectBottom(height: lineH).Modify(y: lineS);
				EditorGUI.PropertyField(line, prop, AccuracyLabel);
				
				line = line.GetRectBottom(height: lineH * 2f).Modify(y: lineS * 2f);
				prop = property.FindPropertyRelative("m_IsRelatedAngle");
				if(prop.boolValue)
				{
					EditorGUI.HelpBox(line, "Camera direction will always related to the chase target forward.", MessageType.Info);
				}
				else
				{
					EditorGUI.HelpBox(line, "Camera direction will only bounded by clamp angle, unless rebound force is on.", MessageType.Info);
				}
				line = line.GetRectBottom(height: lineH).Modify(y: lineS);
				EditorGUI.PropertyField(line, prop);
			}

			EditorGUI.EndProperty();
        }

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return GetStaticHeight(property);
		}

		public static float GetStaticHeight(SerializedProperty property)
		{
			float
				rst = lineH + lineS + 16f;
			if (property.isExpanded)
			{
				rst += 150f + 32f + 16f;
				rst += (property.FindPropertyRelative("m_MoveMethod").enumValueIndex > 0 ? (lineH + lineS) : 0f);
				rst += (property.FindPropertyRelative("m_RotationMethod").enumValueIndex > 0 ? (lineH + lineS) : 0f);
			}
			return rst;
			
		}
	}
}
#endif