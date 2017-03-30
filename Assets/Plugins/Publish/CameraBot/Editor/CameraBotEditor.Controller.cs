#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Kit.Extend;

namespace CF.CameraBot
{
    public partial class CameraBotEditor
    {
        const string Visible = "CameraBot-Visible-Controller";
        protected void DrawControllerSet()
        {
            EditorGUILayout.HelpBox("Default Controller to camera\nYou can write your own!", MessageType.Info);
            EditorPrefs.SetBool(Visible, EditorGUILayout.Toggle("Controller", EditorPrefs.GetBool(Visible)));
            if (EditorPrefs.GetBool(Visible))
            {
                EditorGUILayout.Separator();
                ApplyMouseSetting();

                EditorGUILayout.Separator();
                ApplyKeyboardSetting();
            }
        }

        private void ApplyKeyboardSetting()
        {
            EditorGUILayout.HelpBox("PC: FPS, TPS, Keyboard", MessageType.Info);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("PC Key Input"))
            {
                self.GetOrAddComponent<PCKeyInput>();
                EditorUtility.SetDirty(self);
            }

            if (GUILayout.Button("PC Key Switch"))
            {
                PCKeySwitch obj = self.GetOrAddComponent<PCKeySwitch>();
                obj.SyncCameraBot();
                EditorUtility.SetDirty(self);
            }
            EditorGUILayout.EndHorizontal();
        }
        private void ApplyMouseSetting()
        {
            EditorGUILayout.HelpBox("PC: Mouse Lock and hidden within screen", MessageType.Info);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Cursor Lock"))
            {
                self.GetOrAddComponent<CursorLock>();
                EditorUtility.SetDirty(self);
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
#endif