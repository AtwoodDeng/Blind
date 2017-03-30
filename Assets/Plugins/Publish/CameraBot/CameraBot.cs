using UnityEngine;
using System.Collections.Generic;

namespace CF.CameraBot
{
    /// <summary>CF Camera for Unity3D - a spherical coordinates camera handler.</summary>
    /// <see cref="http://www.clonefactor.com"/>
    public partial class CameraBot : MonoBehaviour
    {
        #region variable
        /// <summary>Target gameobject want to chase</summary>
        public Transform ChaseTarget;
        /// <summary>Override chase target forward direction by this transform forward.</summary>
		public Transform TargetForward;

        /// <summary>Translate object to chase target's relative position.</summary>
        public Transform ControlPosition;
        /// <summary>Rotate gameObject to facing chase target.</summary>
        public Transform ControlRotation;
        /// <summary>Input handler parameter.</summary>
        public InputSetting InputSetting;
        /// <summary>Advance Setting</summary>
        public AdvanceSetting AdvanceSetting;
		/// <summary>Currect Selected handler for camera.</summary>
		/// <remarks>Assign and change this at run time can also affect SwitchCamera method.</remarks>
		/// <see cref="SwitchCamera()"/>
		public int Selected = 0;
        /// <summary>A list for developer to setup the camera detail.</summary>
        public List<Preset> PresetList = new List<Preset>();
		public Preset activePreset { get { return PresetList[Selected]; } }
		#endregion

        #region API
        /// <summary>In order to control camera you need to giving the following values.</summary>
        /// <param name="keyHorizontal">keyboard horizontal value, rotate orbit yaw angle.</param>
        /// <param name="keyVertical">keyboard vertical value, rotate orbit pitch angle.</param>
        /// <param name="mouseHorizontal">mouse horizontal value, rotate orbit yaw angle.</param>
        /// <param name="mouseVertical">mouse vertical value, rotate orbit pitch angle.</param>
        /// <param name="mouseWheel">mouse wheel value, for camera zooming usage.</param>
        public void UpdatePosition(float keyHorizontal, float keyVertical, float mouseHorizontal, float mouseVertical, float mouseWheel)
        {
            mouseHorizontal = InputSetting.FlipMouseX ? -mouseHorizontal : mouseHorizontal;
            mouseVertical = InputSetting.FlipMouseY ? -mouseVertical : mouseVertical;
            mouseWheel = InputSetting.FlipMouseWheel ? -mouseWheel : mouseWheel;
            
            // we only care the angle.
            // combine keyboard & mouse value into one value,
            float
				horizontal = keyHorizontal * keyHorizontal > mouseHorizontal * mouseHorizontal ? keyHorizontal : mouseHorizontal,
				vertical = keyVertical * keyVertical > mouseVertical * mouseVertical ? keyVertical : mouseVertical,
				wheel = ((Mathf.Abs(mouseWheel) > float.MinValue) ? mouseWheel * Time.deltaTime * InputSetting.WheelSpeed : 0f),
				amount = OverTurnAngleAccuracy(InputSetting.Sensitive * Time.fixedDeltaTime);

			bool
				updateAngle = (horizontal * horizontal > InputSetting.Threshold || vertical * vertical > InputSetting.Threshold),
				updateZoom = !Mathf.Approximately(wheel, 0f);

			// apply change on active camera coordinate.
			if (updateAngle)
			{
				PlayerTriggerAngleChange();
				UpdateOrbitCoordinateData(mouseVertical, horizontal, amount, activePreset);
			}
			if (updateZoom)
			{
				PlayerTriggerZoomChange();
				UpdateZoomSectionData(activePreset, wheel);
			}
			
			// update Spherical Coordinates in memory.
			for (int i = 0; i < PresetList.Count; i++)
            {
				if (i == Selected)
					continue; // skip, handled before.

				if (updateAngle)
					UpdateOrbitCoordinateData(mouseVertical, horizontal, amount, PresetList[i]);

				if (updateZoom)
					UpdateZoomSectionData(PresetList[i], wheel);
            }
        }
        /// <summary>Select Camera preset by giving name.</summary>
        /// <param name="cameraLabel">the camera name on preset list.</param>
        public void SwitchCamera(string cameraLabel)
        {
            for (int i = 0; i < PresetList.Count; i++)
            {
                if (PresetList[i].name.Trim().ToLower().Equals(cameraLabel.Trim().ToLower()))
                {
                    SwitchCamera(i);
                    return;
                }
            }
        }
        /// <summary>Select Camera preset by giving index.</summary>
        /// <param name="id">the camera index on the list.</param>
        public void SwitchCamera(int id)
        {
            if (id == Selected)
                return;
            Selected = Mathf.Clamp(id, 0, PresetList.Count);
			HandlePresetActiveState();

			if (activePreset.m_Method.m_UpdateAngle == UpdateAngleMethod.ResetWhenActive)
            {
                ResetToInitStage(Selected);
            }
        }

        /// <summary>Gets current chase target.</summary>
        /// <param name="targetReference">The target reference.</param>
        /// <param name="forwardReference">The forward reference.</param>
        public void GetChaseTarget(out Transform targetReference, out Transform forwardReference)
        {
            forwardReference = (TargetForward == null) ? ChaseTarget : TargetForward;
            targetReference = ChaseTarget;
        }
        /// <summary>Sets chase target and it's forward reference.</summary>
        /// <param name="newTarget">The new target.</param>
        /// <param name="newForward">The new forward reference, default is newTarget itself.</param>
        public void SetChaseTarget(Transform newTarget, Transform newForward = null)
        {
            TargetForward = (newForward == null) ? newTarget : newForward;
            ChaseTarget = newTarget;
        }
        /// <summary>Gets camera position & rotation transform reference.</summary>
        /// <param name="positionReference">The position reference.</param>
        /// <param name="rotationReference">The rotation reference.</param>
        public void GetCamera(out Transform positionReference, out Transform rotationReference)
        {
            positionReference = ControlPosition;
            rotationReference = ControlRotation;
        }
        /// <summary>Sets the camera transform position pivot point and rotation transform object.</summary>
        /// <param name="positionReference">The position reference.</param>
        /// <param name="rotationReference">The rotation reference.</param>
        public void SetCamera(Transform positionReference, Transform rotationReference = null)
        {
            ControlPosition = positionReference;
            ControlRotation = (rotationReference == null) ? positionReference : rotationReference;
        }
        #endregion
	}
}