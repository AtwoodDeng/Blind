using UnityEngine;
using System.Collections;

namespace CF.CameraBot
{
    public class CursorLock : MonoBehaviour
    {
#if UNITY_WEBPLAYER || UNITY_STANDALONE_WIN || UNITY_EDITOR || UNITY_WEBGL
        public bool ToggleCursorMode = true;
        public KeyCode ToggleCursorKey = KeyCode.Escape;
        public bool AutoTakenCursorMode = true;

        private bool _ownCursor = false;

        private void Awake()
        {
            useGUILayout = false;
        }
        private void Update()
        {
            HandleCursor();
        }

        private void HandleCursor()
        {
            if (AutoTakenCursorMode && Input.anyKey && !_ownCursor)
            {
                _ownCursor = true;
            } 
            if (ToggleCursorMode)
            {
                if (Input.GetKeyUp(ToggleCursorKey))
                    _ownCursor = !_ownCursor;
            }
            
            SetHidden(_ownCursor);
        }
        public void SetHidden(bool hide)
        {
            if (hide)
                CursorHideLock();
            else
                CursorShowUnlock();
        }
#endif

#if UNITY_2_6 || UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6
        private void CursorHideLock()
        {
            if (!(!Screen.showCursor && Screen.lockCursor))
            {
                _ownCursor = true;
                Screen.showCursor = false;
                Screen.lockCursor = true;
            }
        }
        private void CursorShowUnlock()
        {
            if (!(Screen.showCursor && !Screen.lockCursor))
            {
                _ownCursor = false;
                Screen.showCursor = true;
                Screen.lockCursor = false;
            }
        }
#else
        private void CursorHideLock()
        {
            _ownCursor = (!Cursor.visible && Cursor.lockState == CursorLockMode.Locked);
            if (_ownCursor) return;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        private void CursorShowUnlock()
        {
            _ownCursor = !(Cursor.visible && Cursor.lockState == CursorLockMode.None);
            if (!_ownCursor) return;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
#endif
    }
}
