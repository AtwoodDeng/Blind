using UnityEngine;
namespace CF.CameraBot
{
    [DisallowMultipleComponent]
    public class PCKeyInput : CameraBotSlave, IDevicePC, IDeviceMobile
    {
        public string KeyHorizontal = "Horizontal";
        public string KeyVertical = "Vertical";
        public string MouseHorizontal = "Mouse X";
        public string MouseVertical = "Mouse Y";
        public string MouseZoom = "Mouse ScrollWheel";

        private bool isMobile = false;
        [Tooltip("Use mobile acclerater as rotation control")]
        public bool accelerater = false;
        void FixedUpdate()
        {
            CameraBot.UpdatePosition(
                GetDirection.x,
                GetDirection.y,
                GetRotate.x,
                GetRotate.y,
                GetZoom);
        }


        #region device interface
        public void Trigger(DeviceType type)
        {
            isMobile = (type == DeviceType.Mobile);
        }
        #endregion

        #region getter setter
        private Vector2 GetDirection
        {
            get
            {
                return new Vector2(
                    ((KeyHorizontal.Length > 0) ? Input.GetAxis(KeyHorizontal) : 0f),
                    ((KeyVertical.Length > 0) ? Input.GetAxis(KeyVertical) : 0f)
                    );
            }
        }
        private Vector2 GetRotate
        {
            get
            {
                return (this.isMobile && accelerater) ?
                    new Vector2(
                        Input.acceleration.x,
                        Input.acceleration.y) :
                    new Vector2(
                        ((MouseHorizontal.Length > 0) ? Input.GetAxis(MouseHorizontal) : 0f),
                        ((MouseVertical.Length > 0) ? Input.GetAxis(MouseVertical) : 0f));
            }
        }

        private float GetZoom
        {
            get
            {
                return (this.isMobile) ?
                    GetMobilePinch :
                    ((MouseZoom.Length > 0) ? Input.GetAxis(MouseZoom) : 0f);
            }
        }

        private Vector2 GetDragDirection(int index)
        {
            if (Input.touchCount == 0 && Input.touchCount < index)
                return Vector2.zero;
            Touch touch = Input.GetTouch(index);
            return (touch.position - touch.deltaPosition).normalized;
        }

        private float GetMobilePinch
        {
            get
            {
                if(Input.touchCount >= 2)
                {
                    Touch
                        t0 = Input.GetTouch(0),
                        t1 = Input.GetTouch(1);
                    Vector2
                        t0Prev = t0.position - t0.deltaPosition,
                        t1Prev = t1.position - t1.deltaPosition;
                    float
                        prevTouchDeltaMag = (t0Prev - t1Prev).magnitude,
                        touchDeltaMag = (t0.position - t1.position).magnitude;
                    return prevTouchDeltaMag - touchDeltaMag;
                }
                return 0f;
            }
        }
        #endregion
    }
}

