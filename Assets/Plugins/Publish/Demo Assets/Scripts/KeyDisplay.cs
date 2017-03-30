using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class KeyDisplay : MonoBehaviour
{
    [Serializable]
    public class KeyBindSet
    {
        public Image Image;
        public KeyCode KeyCode;
    }

    public Color KeyUpColor;
    public Color KeyDownColor;

    public List<KeyBindSet> KeyBindSetList;

    void Start()
    {
        foreach (KeyBindSet obj in KeyBindSetList)
        {
            obj.Image.color = Color.white;
            obj.Image.CrossFadeColor(KeyUpColor, 0f, true, true);
        }
    }

	void Update ()
    {
        foreach (KeyBindSet obj in KeyBindSetList)
        {
            if (Input.GetKeyDown(obj.KeyCode))
            {
                obj.Image.CrossFadeColor(KeyDownColor, .1f, true, true);
            }
            else if (Input.GetKeyUp(obj.KeyCode))
            {
                obj.Image.CrossFadeColor(KeyUpColor, .1f, true, true);
            }
        }
	}
}
