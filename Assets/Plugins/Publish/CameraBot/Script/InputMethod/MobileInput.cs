using UnityEngine;
using System.Collections.Generic;

namespace CF.CameraBot
{
    public class MobileInput : CameraBotSlave
    {
	    void Update ()
        {
            int nbTouches = Input.touchCount;

            if (nbTouches > 0)
            {
                for (int i = 0; i < nbTouches; i++)
                {
                    Touch touch = Input.GetTouch(i);

                    TouchPhase phase = touch.phase;

                    switch (phase)
                    {
                        case TouchPhase.Began:
                            print("New touch detected at position " + touch.position + " , index " + touch.fingerId);
                            break;
                        case TouchPhase.Moved:
                            print("Touch index " + touch.fingerId + " has moved by " + touch.deltaPosition);
                            break;
                        case TouchPhase.Stationary:
                            print("Touch index " + touch.fingerId + " is stationary at position " + touch.position);
                            break;
                        case TouchPhase.Ended:
                            print("Touch index " + touch.fingerId + " ended at position " + touch.position);
                            break;
                        case TouchPhase.Canceled:
                            print("Touch index " + touch.fingerId + " cancelled");
                            break;
                    }
                }
            }
        }

    }
}

