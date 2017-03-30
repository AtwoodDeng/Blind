using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Kit.Extend;

public class MouseDisplay : MonoBehaviour
{
    public Image ImageUp;
    public Image ImageDown;
    public Image ImageLeft;
    public Image ImageRight;
    public Image ImageWheel;
    public float Threshold = 0.1f;

    float On = 1f;
    float Off = .3f;

    float mouseHorizontal;
    float mouseVertical;
    float mouseScrollWheel;
    void OnEnable()
    {
        ImageUp.color = Color.white.SetAlpha(Off);
        ImageDown.color = Color.white.SetAlpha(Off);
        ImageLeft.color = Color.white.SetAlpha(Off);
        ImageRight.color = Color.white.SetAlpha(Off);
        ImageWheel.color = ImageWheel.color.SetAlpha(Off);
        StartCoroutine(MouseScroll());
    }
    void OnDisable()
    {
        StopAllCoroutines();
    }
    void Update ()
    {
        mouseHorizontal = Input.GetAxis("Mouse X");
        mouseVertical = Input.GetAxis("Mouse Y");
        mouseScrollWheel = Input.GetAxis("Mouse ScrollWheel");

        // Left/Right
        if (Mathf.Abs(mouseHorizontal) > Threshold)
        {
            ImageRight.color = (mouseHorizontal > 0f) ?
                Color.white.SetAlpha(On) :
                Color.white.SetAlpha(Off);
            ImageLeft.color = (mouseHorizontal < 0f) ?
                Color.white.SetAlpha(On) :
                Color.white.SetAlpha(Off);
        }
        else
        {
            ImageRight.color = Color.white.SetAlpha(Off);
            ImageLeft.color = Color.white.SetAlpha(Off);
        }

        // Up/Down
        if (Mathf.Abs(mouseVertical) > Threshold)
        {
            ImageUp.color = (mouseVertical > 0f) ?
                Color.white.SetAlpha(On) :
                Color.white.SetAlpha(Off);
            ImageDown.color = (mouseVertical < 0f) ?
                Color.white.SetAlpha(On) :
                Color.white.SetAlpha(Off);
        }
        else
        {
            ImageUp.color = Color.white.SetAlpha(Off);
            ImageDown.color = Color.white.SetAlpha(Off);
        }

	}

    // Wheel
    private IEnumerator MouseScroll()
    {
        float ScrollDegree = 0f;
        float currDegree = ScrollDegree = 0f;
        while(true)
        {
            if(Mathf.Abs(mouseScrollWheel) > 0f)
            {
                ImageWheel.color = ImageWheel.color.SetAlpha(On);
                ScrollDegree = (mouseScrollWheel > 0f) ? 10f : -10f;
                currDegree = mouseScrollWheel = 0f;
                while (!currDegree.NealyEqual(ScrollDegree, .1f))
                {
                    currDegree = Mathf.Lerp(currDegree, ScrollDegree, 0.1f);
                    ImageWheel.rectTransform.Rotate(Vector3.forward, currDegree);
                    yield return new WaitForEndOfFrame();
                }
            }
            ImageWheel.color = ImageWheel.color.SetAlpha(Off);
            yield return new WaitForEndOfFrame();
        }
    }
}
