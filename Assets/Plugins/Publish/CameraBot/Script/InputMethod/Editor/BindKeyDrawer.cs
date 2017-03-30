/*
#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;
using Kit.Editor;

namespace CF.CameraBot
{
    [CustomPropertyDrawer(typeof(BindKey))]
    public class BindKeyDrawer : PropertyDrawerExtend
    {
        public override void OnPreviewGUI(Rect position, SerializedProperty property, GUIContent label, bool preview = true)
        {
            base.OnPreviewGUI(position, property, label, preview);
            BeginProperty(position, label, property);

            PrefixLabel(NextLine(.5f), property.FindPropertyRelative("CameraName"));
            PropertyField(CurrentLine(.5f), property.FindPropertyRelative("KeyCode"));

            EndProperty();
        }
    }
}
#endif
*/