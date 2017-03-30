using UnityEngine;
using System;
using System.Collections;
using Kit.Extend;

namespace CF.CameraBot
{
	public partial class CameraBot : MonoBehaviour
	{
        #region Variable
        private IEnumerator m_WaitForZoomReboundActive;
        private IEnumerator m_StartZoomRebound;
        #endregion

        #region Core
        private void UpdateZoomSectionData(Preset preset, float ZoomSectionRef)
        {
            if (preset.m_Zoom.m_Distance.EqualSimilar(0f))
                return;

			preset.m_Zoom.m_Speed = Mathf.Clamp(preset.m_Zoom.m_Speed, 0.001f, 1f);
            if (preset == activePreset ||
                preset.m_Method.m_UpdateAngle == UpdateAngleMethod.UpdateAlway)
            {
				preset.Cache.m_ZoomSectionDelta = Mathf.Clamp(preset.Cache.m_ZoomSectionDelta + ZoomSectionRef, preset.m_Zoom.m_ForwardLimit, preset.m_Zoom.m_BackwardLimit);
            }
        }
        #endregion

        #region Rebound
        private void PlayerTriggerZoomChange()
        {
            if (m_WaitForZoomReboundActive != null)
                StopCoroutine(m_WaitForZoomReboundActive);
            if (m_StartZoomRebound != null)
                StopCoroutine(m_StartZoomRebound);
            
            m_WaitForZoomReboundActive = null;
            m_StartZoomRebound = null;

            Preset preset = activePreset;

            if (preset.m_Zoom.m_Rebound)
            {
				if(preset.m_Zoom.m_WaitForClampAngle || !preset.m_Zoom.m_ReboundDelay.EqualSimilar(0f))
				{
					m_WaitForZoomReboundActive = WaitForZoomReboundActive(preset);
					StartCoroutine(m_WaitForZoomReboundActive);
				}
                else
                {
                    m_StartZoomRebound = StartZoomRebound(preset);
                    StartCoroutine(m_StartZoomRebound);
                }
            }
        }

        private IEnumerator WaitForZoomReboundActive(Preset preset)
        {
            // user may use Time.scale = 0f;
            // yield return new WaitForSeconds(1f);
            System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
            long limit = Convert.ToInt64(Mathf.Abs(preset.m_Zoom.m_ReboundDelay) * 1000); // milliseconds
            stopWatch.Start();

            if(preset.m_Zoom.m_WaitForClampAngle && preset.m_ClampAngle.m_Rebound)
            {
				while (stopWatch.ElapsedMilliseconds < limit ||
					preset.Cache.m_ReboundYaw.m_State < eReboundState.Rebounding ||
					preset.Cache.m_ReboundPitch.m_State < eReboundState.Rebounding)
					yield return 1;
            }
            else
            {
                while (stopWatch.ElapsedMilliseconds < limit)
                    yield return 1;
            }
            stopWatch.Stop();

            m_StartZoomRebound = StartZoomRebound(preset);
            StartCoroutine(m_StartZoomRebound);
        }

        private IEnumerator StartZoomRebound(Preset preset)
        {
			// if player didn't move during count down, start rebounding.
			float time = 0f;
			float startDelta = preset.Cache.m_ZoomSectionDelta;
			if (preset.m_Zoom.m_ReboundPeriod <= 0f)
			{
				preset.Instance.ZoomRange = preset.Cache.m_ZoomSectionDelta = preset.m_Zoom.m_ReboundCurve.Evaluate(1f);
				yield break;
			}
			else
			{
				while (time <= preset.m_Zoom.m_ReboundPeriod)
				{
					preset.Instance.ZoomRange = preset.Cache.m_ZoomSectionDelta = Mathf.Lerp(startDelta, 0f, preset.m_Zoom.m_ReboundCurve.Evaluate(time / preset.m_Zoom.m_ReboundPeriod));
					time += Time.deltaTime;
					yield return 1;
				}
			}
			yield break;
        }
        #endregion
    }
}
