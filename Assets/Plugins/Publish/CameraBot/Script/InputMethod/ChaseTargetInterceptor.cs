using UnityEngine;
using System.Collections;

namespace CF.CameraBot
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CameraBot))]
    public class ChaseTargetInterceptor : CameraBotSlave
    {
        public Transform ChaseTargetBak;
        public Transform ChaseTarget { set; get; }

        private void OnEnable()
        {
            InterceptorTarget();
        }

        private void InterceptorTarget()
        {
            if (!ReferenceEquals(CameraBot.ChaseTarget, null))
            {
                ChaseTargetBak = CameraBot.ChaseTarget;
                CameraBot.ChaseTarget = CreateIllusionTarget(CameraBot.ChaseTarget);
            }
        }

        private Transform CreateIllusionTarget(Transform obj)
        {
            ChaseTarget = (new GameObject()).transform;
            ChaseTarget.name = "ChaseTargetInterceptor";
            ChaseTarget.position = obj.transform.position;
            return ChaseTarget;
        }
    }
}