using UnityEngine;
using System.Collections;

namespace CF.CameraBot
{
    [RequireComponent(typeof(IDevice))]
    [DisallowMultipleComponent]
    public class RTSChaseTargetInterceptor : ChaseTargetInterceptor, IControlSTR
    {
        public enum FollowMethod
        {
            None = 0,           // Manually call MoveTo(Transform/Vector3);
            RelativeToAvatar,   // keep focus on Avatar.
        }
        public FollowMethod CameraMoveMethod;
        private Transform watchingCamera { get { return CameraBot.ControlPosition; } }

        public void Trigger(ControlType type)
        {
            if(type==ControlType.STR)
            {
                // use mouse for rotate
            }
        }

        public string Horizontal = "Horizontal";
        public string Vertical = "Vertical";
        public bool flipX = true;
        public bool flipY = false;
        void FixedUpdate()
        {
            Vector2 rotate = new Vector2(
                Input.acceleration.x * ((flipX) ? 1f : -1f),
                Input.acceleration.y * ((flipY) ? 1f : -1f));
            if (rotate.sqrMagnitude > 1f)
                rotate.Normalize();

            Vector2 move = new Vector2(
                ((Horizontal.Length > 0) ? Input.GetAxis(Horizontal) : 0f),
                ((Vertical.Length > 0) ? Input.GetAxis(Vertical) : 0f)
                );


            CameraBot.UpdatePosition(move.x, move.y, rotate.y, rotate.x, 0f);
        }

        void Update()
        {
            if (CameraMoveMethod == FollowMethod.None)
                return;
            else if(CameraMoveMethod == FollowMethod.RelativeToAvatar)
            {
                MoveTo(ChaseTargetBak);
            }
        }

        #region Movement
        public MoveMethod MoveMethod;
        [Range(0.0001f, 30f)]
        public float PositionSpeed = 4f; // 30 fps, it's almost snap to position.
        public void MoveTo(Transform trans)
        {
            MoveTo(trans.position);
        }
        public void MoveTo(Vector3 position)
        {
            if (MoveMethod.Equals(MoveMethod.Snap))
                ChaseTarget.transform.position = position;
            else if(MoveMethod.Equals(MoveMethod.Lerp))
                ChaseTarget.transform.position = Vector3.Lerp(ChaseTarget.transform.position, position, Time.deltaTime * PositionSpeed);
            else
                ChaseTarget.transform.position = Vector3.Slerp(ChaseTarget.transform.position, position, Time.deltaTime * PositionSpeed);
        }
        #endregion
    }
}