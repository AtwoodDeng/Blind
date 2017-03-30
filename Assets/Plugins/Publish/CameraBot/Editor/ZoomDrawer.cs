#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Kit.Extend;

namespace CF.CameraBot
{
    [CustomPropertyDrawer(typeof(Zoom))]
    public class ZoomDrawer : PropertyDrawer
    {
        private static Texture _zoom;
        private static Texture Zoom { get { if (_zoom == null) _zoom = Resources.Load("zoom") as Texture; return _zoom; } }

        private GUIContent ZoomSectionLabel = new GUIContent("Zoom", Zoom, "Camera zoom in/out control.");
        private GUIContent ZoomDistanceLabel = new GUIContent("Distance (+/-)", "Distance to allow zoom in/out");
        private GUIContent ZoomForwardLabel = new GUIContent("Forward","Toward chase target");
        private GUIContent ZoomBackwardLabel = new GUIContent("Backward","Backward from chase target.");
        private GUIContent ZoomSpeedLabel = new GUIContent("Speed", "Speed to zoom in/out");

        private GUIContent ReboundLabel = new GUIContent("Enable Rebound", "Allow Zoom distance rebound to its initial position.");
        private GUIContent ReboundDelayLabel = new GUIContent("Rebound Delay", "Delay second after player stop input.");
        private GUIContent ReboundPeriodLabel = new GUIContent("Rebound Period", "A fixed constant time to drag back camera to initial position.");
		private GUIContent ReboundCurveLabel = new GUIContent("Rebound Curve", "Animation curve for rebound speed.");
		private GUIContent ReboundWaitClampAngleLabel = new GUIContent("Sync with angle", "Zoom rebound will delay until clamp angle rebound start.");

		readonly float
			lineH = EditorGUIUtility.singleLineHeight,
			lineS = EditorGUIUtility.standardVerticalSpacing;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			Vector2 orgIconSize = EditorGUIUtility.GetIconSize();
			EditorGUIUtility.SetIconSize(Vector2.one * 32f);

			Rect line = position.Clone(height: lineH + 16f);
			property.isExpanded = EditorGUI.Foldout(line, property.isExpanded, ZoomSectionLabel, true);
			EditorGUIUtility.SetIconSize(orgIconSize);

			if (property.isExpanded)
            {
                float forward = property.FindPropertyRelative("m_ForwardLimit").floatValue;
                float backward = property.FindPropertyRelative("m_BackwardLimit").floatValue;
				line = line.GetRectBottom(height: lineH * 2f).Modify(y: lineS);
                if (property.FindPropertyRelative("m_Distance").floatValue < 0.1f)
                {
                    EditorGUI.HelpBox(line, "To enable zoom method, input vaild [Distance]", MessageType.Warning);
                }
                else if (forward < 0.1f && backward < 0.1f)
                {
                    EditorGUI.HelpBox(line, "Drag slider bar or input [Forward] & [Backward] to config zoom range.", MessageType.Warning);
                }
                else
                {
                    EditorGUI.HelpBox(line, "You may adjust [Speed] to override the camera zoom speed.", MessageType.Info);
                }

                // Distance
				line = line.GetRectBottom(height: lineH).Modify(y: lineS);
				SerializedProperty distanceProp = property.FindPropertyRelative("m_Distance");
				EditorGUI.BeginChangeCheck();
				EditorGUI.PropertyField(line, distanceProp, ZoomDistanceLabel);
                if(EditorGUI.EndChangeCheck())
					distanceProp.floatValue = Mathf.Abs(distanceProp.floatValue);
				
				// Speed
				line = line.GetRectBottom().Modify(y: lineS);
				SerializedProperty speedProp = property.FindPropertyRelative("m_Speed");
				EditorGUI.BeginChangeCheck();
				EditorGUI.PropertyField(line, speedProp, ZoomSpeedLabel);
                if (EditorGUI.EndChangeCheck() && speedProp.floatValue <= 0f)
					speedProp.floatValue = 1f;

				line = line.GetRectBottom().Modify(y: lineS);
				SerializedProperty
					forwardLimitProp = property.FindPropertyRelative("m_ForwardLimit"),
					backwardLimitProp = property.FindPropertyRelative("m_BackwardLimit");
				float max = distanceProp.floatValue;
				float min = -max;
				EditorGUI.BeginChangeCheck();
				EditorGUI.MinMaxSlider(line, ref forward, ref backward, min, max);
                if (EditorGUI.EndChangeCheck() && forward <= 0 && backward >= 0)
				{
					forwardLimitProp.floatValue = Mathf.Clamp(forward, min, 0f); ;
					backwardLimitProp.floatValue = Mathf.Clamp(backward, 0f, max);
                }

				line = line.GetRectBottom().Modify(y: lineS);
				Rect cell = line.Clone(width: line.width / 4f);
				EditorGUI.BeginChangeCheck();
                EditorGUI.PrefixLabel(cell, ZoomForwardLabel);
				cell = cell.GetRectRight();
                float forward2 = EditorGUI.FloatField(cell, GUIContent.none, forwardLimitProp.floatValue);
				cell = cell.GetRectRight();
				EditorGUI.PrefixLabel(cell, ZoomBackwardLabel);
				cell = cell.GetRectRight();
				float backward2 = EditorGUI.FloatField(cell, GUIContent.none, backwardLimitProp.floatValue);
                if (EditorGUI.EndChangeCheck())
                {
                    if (forward2 <= 0 && forward2 >= min)
						forwardLimitProp.floatValue = Mathf.Clamp(forward2, min, 0f);
                    if (backward2 >= 0)
						backwardLimitProp.floatValue = Mathf.Clamp(backward2, 0f, max);
                }

				SerializedProperty reboundProp = property.FindPropertyRelative("m_Rebound");
                if (reboundProp.boolValue)
                {
					line = line.GetRectBottom(height: lineH * 2f).Modify(y: lineS + 10f);
                    EditorGUI.HelpBox(line,
                        string.Format("Enable \"Rebound\" method, the camera will trying to reset its position to the initial direction."),
                        MessageType.Info);
                }

				line = line.GetRectBottom(height: lineH).Modify(y: lineS);
				EditorGUI.PropertyField(line, reboundProp, ReboundLabel);
                if (reboundProp.boolValue)
                {
					line = line.GetRectBottom(height: lineH).Modify(y: lineS);
					EditorGUI.PropertyField(line, property.FindPropertyRelative("m_ReboundDelay"), ReboundDelayLabel);
					line = line.GetRectBottom(height: lineH).Modify(y: lineS);
					EditorGUI.PropertyField(line, property.FindPropertyRelative("m_ReboundPeriod"), ReboundPeriodLabel);
					line = line.GetRectBottom(height: lineH).Modify(y: lineS);
					AnimationCurve curve = EditorGUI.CurveField(line, ReboundCurveLabel, property.FindPropertyRelative("m_ReboundCurve").animationCurveValue);
					curve.Clamp(0f, -1f, 1f, 1f);
					property.FindPropertyRelative("m_ReboundCurve").animationCurveValue = curve;
					if (!curve.MatchStartEndKeysValues(0f,1f))
					{
						line = line.GetRectBottom(height: 40f).Modify(y: lineS);
						if (GUI.Button(line, "Fix curve values into 0f..1f"))
						{
							property.FindPropertyRelative("m_ReboundCurve").animationCurveValue = curve.FixStartEndKeysValues(0f, 1f);
						}
					}
					line = line.GetRectBottom(height: lineH).Modify(y: lineS);
					EditorGUI.PropertyField(line, property.FindPropertyRelative("m_WaitForClampAngle"), ReboundWaitClampAngleLabel);
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
				rst += 130f;
				if (property.FindPropertyRelative("m_Rebound").boolValue)
					rst += 120f;
				if (!property.FindPropertyRelative("m_ReboundCurve").animationCurveValue.MatchStartEndKeysValues(0f, 1f))
					rst += 40f;
			}
			return rst;
		}
	}
}
#endif