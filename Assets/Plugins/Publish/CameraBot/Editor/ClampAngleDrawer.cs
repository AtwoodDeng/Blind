#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Kit.Extend;

namespace CF.CameraBot
{
    [CustomPropertyDrawer(typeof(ClampAngle))]
    public class ClampAngleDrawer : PropertyDrawer
    {
        private static Texture _angle;
        private static Texture Angle { get { if (_angle == null) _angle = Resources.Load("angle") as Texture; return _angle; } }

        private GUIContent ClampAngleLabel = new GUIContent("Clamp Angle", Angle, "Camera can only move within those angle.");
        private GUIContent ForwardReferenceLabel = new GUIContent("Forward Reference", "Override global forward reference.");
        private GUIContent RotationUpwardMethodLabel = new GUIContent("Rotation Reference", "Calculate camera upward, World = Alwary point to sky, Local = Based on camera relative upward, Custom = developer design.");
        private GUIContent CustomTransformLabel = new GUIContent("Upward Reference", "Using Custom Transform's upward as camera rotation reference.");
        private GUIContent LeftRightLabel = new GUIContent("Left/Right range");
        private GUIContent UpDownLabel = new GUIContent("Up/Down range");

        private GUIContent ReboundLabel = new GUIContent("Enable Rebound", "Drag camera back to initial position, related to target forward direction.");
        private GUIContent ReboundDelayLabel = new GUIContent("Rebound Delay", "Delay second after player stop input.");
        private GUIContent ReboundPeriodLabel = new GUIContent("Rebound Period", "A fixed constant time to drag back camera to initial position.");
		private GUIContent ReboundCurveLabel = new GUIContent("Rebound Curve", "Animation curve for rebound speed.");

		readonly float lineH = EditorGUIUtility.singleLineHeight, lineS = EditorGUIUtility.standardVerticalSpacing;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			Vector2 orgIconSize = EditorGUIUtility.GetIconSize();
			EditorGUI.BeginProperty(position, label, property);

			Rect line = position.Clone(height: lineH + 16f);
			EditorGUIUtility.SetIconSize(Vector2.one * 32f);
			property.isExpanded = EditorGUI.Foldout(line, property.isExpanded, ClampAngleLabel, true);
			EditorGUIUtility.SetIconSize(orgIconSize);

			if (property.isExpanded)
            {
                SerializedProperty prop;
				line = line.GetRectBottom(height: lineH * 2f).Modify(y: lineS);
                EditorGUI.HelpBox(line, "use [ForwardReference] to override [Target Forward], ignore this setting if using global setting.", MessageType.Info);

				line = line.GetRectBottom(height: lineH).Modify(y: lineS);
				EditorGUI.PropertyField(line, property.FindPropertyRelative("m_ForwardReference"), ForwardReferenceLabel);

                string help;
                prop = property.FindPropertyRelative("m_UpwardReferenceMethod");
                switch ((UpwardReferenceMethod)prop.enumValueIndex)
                {
                    default:
                    case UpwardReferenceMethod.World:
                        help = "World : fastest, alway point to sky, but have flip camera issue when pass though 'Gimbal Lock'";
                        break;
                    case UpwardReferenceMethod.Local:
                        help = "Local : simulate rotate orbit child object";
                        break;
					case UpwardReferenceMethod.RealLocal:
						help = "RealLocal : simulate rotate orbit child object, real local have angle!";
						break;
                    case UpwardReferenceMethod.Custom:
                        help = "Custom : assign transform's upward as a reference, you may also roll the camera by this method";
                        break;
                }
				line = line.GetRectBottom(height: lineH * 2f).Modify(y: lineS);
				EditorGUI.HelpBox(line, help, MessageType.Info);

				line = line.GetRectBottom(height: lineH).Modify(y: lineS);
				EditorGUI.PropertyField(line, prop, RotationUpwardMethodLabel);
                if (prop.enumValueIndex == (int)UpwardReferenceMethod.Custom)
                {
					line = line.GetRectBottom(height: lineH).Modify(y: lineS);
					EditorGUI.PropertyField(line, property.FindPropertyRelative("m_UpReference"), CustomTransformLabel);
                }

				line = line.GetRectBottom(height: lineH).Modify(y: lineS);
				EditorGUI.PrefixLabel(line, LeftRightLabel);
				line = line.GetRectBottom(height: lineH).Modify(y: lineS);
				EditorGUI.PropertyField(line, property.FindPropertyRelative("m_PolarLeftRange"));
				line = line.GetRectBottom(height: lineH).Modify(y: lineS);
				EditorGUI.PropertyField(line, property.FindPropertyRelative("m_PolarRightRange"));

				line = line.GetRectBottom(height: lineH).Modify(y: lineS * 2f);
				EditorGUI.PrefixLabel(line, UpDownLabel);

				line = line.GetRectBottom(height: lineH).Modify(y: lineS);
				EditorGUI.PropertyField(line, property.FindPropertyRelative("m_ElevationUpRange"));

				line = line.GetRectBottom(height: lineH).Modify(y: lineS);
				EditorGUI.PropertyField(line, property.FindPropertyRelative("m_ElevationDownRange"));

                prop = property.FindPropertyRelative("m_Rebound");
                if (prop.boolValue)
                {
					line = line.GetRectBottom(height: lineH * 2f).Modify(y: lineS * 2f);
					EditorGUI.HelpBox(line,
                        string.Format("Enable \"Rebound\" method, after changed camera direction, the camera will trying to reset its position to the initial direction."),
                        MessageType.Info);
                }

				line = line.GetRectBottom(height: lineH).Modify(y: lineS);
				EditorGUI.PropertyField(line, prop, ReboundLabel);
                if (prop.boolValue)
                {
					line = line.GetRectBottom(height: lineH).Modify(y: lineS);
					EditorGUI.PropertyField(line, property.FindPropertyRelative("m_ReboundDelay"), ReboundDelayLabel);
					line = line.GetRectBottom(height: lineH).Modify(y: lineS);
					EditorGUI.PropertyField(line, property.FindPropertyRelative("m_ReboundPeriod"), ReboundPeriodLabel);
					line = line.GetRectBottom(height: lineH).Modify(y: lineS);
					AnimationCurve curve = EditorGUI.CurveField(line, ReboundCurveLabel, property.FindPropertyRelative("m_ReboundCurve").animationCurveValue);
					curve.Clamp(0f, -1f, 1f, 1f);
					property.FindPropertyRelative("m_ReboundCurve").animationCurveValue = curve;
					if (!curve.MatchStartEndKeysValues(0f, 1f))
					{
						line = line.GetRectBottom(height: 40f).Modify(y: lineS);
						if (GUI.Button(line, "Fix curve values into 0f..1f"))
						{
							property.FindPropertyRelative("m_ReboundCurve").animationCurveValue = curve.FixStartEndKeysValues(0f, 1f);
						}
					}
				}
            }
			EditorGUI.EndProperty();
        }

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return GetStaticHeight(property);
		}

		public static float GetStaticHeight(SerializedProperty property)
		{
			float lineH = EditorGUIUtility.singleLineHeight, lineS = EditorGUIUtility.standardVerticalSpacing;
			float rst = lineH + lineS + 16f;
			if (property.isExpanded)
			{
				rst += 232f;
				rst += ((UpwardReferenceMethod)property.FindPropertyRelative("m_UpwardReferenceMethod").enumValueIndex == UpwardReferenceMethod.Custom) ? (lineH + lineS) : 0f;
				rst += (property.FindPropertyRelative("m_Rebound").boolValue) ? 90f : 0f;
				rst += (!property.FindPropertyRelative("m_ReboundCurve").animationCurveValue.MatchStartEndKeysValues(0f, 1f)) ? 40f : 0f;
			}
			return rst;
		}
	}
}
#endif