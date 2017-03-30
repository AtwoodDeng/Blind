#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Kit.Extend;

namespace CF.CameraBot
{
    [CustomPropertyDrawer(typeof(VirtualPoint))]
    public class VirtualPointDrawer : PropertyDrawer
    {
        private static Texture _arrowLeft, _arrowRight, _arrowUp, _arrowDown, _mouse;
        private static Texture ArrowLeft { get { if (_arrowLeft == null) _arrowLeft = Resources.Load("arrow_left") as Texture; return _arrowLeft; } }
        private static Texture ArrowRight { get { if (_arrowRight == null) _arrowRight = Resources.Load("arrow_right") as Texture; return _arrowRight; } }
        private static Texture ArrowUp { get { if (_arrowUp == null) _arrowUp = Resources.Load("arrow_up") as Texture; return _arrowUp; } }
        private static Texture ArrowDown { get { if (_arrowDown == null) _arrowDown = Resources.Load("arrow_down") as Texture; return _arrowDown; } }
        private static Texture Mouse { get { if (_mouse == null) _mouse = Resources.Load("mouse") as Texture; return _mouse; } }
        private GUIContent ArrowLeftIcon = new GUIContent(ArrowLeft);
        private GUIContent ArrowRightIcon = new GUIContent(ArrowRight);
        private GUIContent ArrowUpIcon = new GUIContent(ArrowUp);
        private GUIContent ArrowDownIcon = new GUIContent(ArrowDown);
        private GUIContent MouseIcon = new GUIContent(Mouse);

		readonly float
			lineH = EditorGUIUtility.singleLineHeight,
			lineS = EditorGUIUtility.standardVerticalSpacing;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			Vector2 orgIconSize = EditorGUIUtility.GetIconSize();
			EditorGUI.BeginProperty(position, label, property);

			Rect line = position.Clone(height: lineH);
			Rect cell = EditorGUI.PrefixLabel(line, label);
			EditorGUI.PropertyField(cell, property.FindPropertyRelative("m_Coordinates"), GUIContent.none);
			
			line = line.GetRectBottom(height: 33f).Modify(y: lineS);
			Rect[]
				cols = line.SplitHorizontal(.5f),
				lc = cols[0].SplitHorizontal(.3f, 30f, 30f),
				rc = cols[1].SplitHorizontal(.3f, 30f, 30f);

			EditorGUIUtility.SetIconSize(Vector2.one * 16f);
			EditorGUI.LabelField(lc[0], MouseIcon);
			EditorGUI.LabelField(lc[0].Modify(y: 10f, width:30f, height:20f), ArrowLeftIcon);

			EditorGUIUtility.SetIconSize(Vector2.one * 24f);
			bool IsRight = property.FindPropertyRelative("m_OppositePolar").boolValue;
			if (GUI.Button(lc[1], ((IsRight) ? ArrowRightIcon : ArrowLeftIcon)))
			{
				property.FindPropertyRelative("m_OppositePolar").boolValue = !IsRight;
			}

			EditorGUIUtility.SetIconSize(Vector2.one * 16f);
			EditorGUI.LabelField(rc[0], MouseIcon);
			EditorGUI.LabelField(rc[0].Modify(y: 10f, width: 30f, height: 20f), ArrowUpIcon);

			EditorGUIUtility.SetIconSize(Vector2.one * 24f);
			bool IsDown = property.FindPropertyRelative("m_OppositeElevation").boolValue;
			if (GUI.Button(rc[1], ((IsDown) ? ArrowDownIcon : ArrowUpIcon)))
			{
				property.FindPropertyRelative("m_OppositeElevation").boolValue = !IsDown;
			}

			EditorGUIUtility.SetIconSize(orgIconSize);
			EditorGUI.EndProperty();
        }

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return 55f;
		}

		public static float GetStaticHeight(SerializedProperty property)
		{
			return 55f;
		}
	}
}
#endif