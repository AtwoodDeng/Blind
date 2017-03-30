using UnityEngine;

namespace Kit.Extend
{
    public static class QuaternionExtend
    {
        /// <summary>Get the different between two Quaternion</summary>
        /// <param name="qtA"></param>
        /// <param name="qtB"></param>
        /// <returns>Different in Quaternion format</returns>
        public static Quaternion Different(this Quaternion qtA, Quaternion qtB)
        {
            return qtA * qtB.Inverse();
        }

        /// <summary>Inverse rotation</summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static Quaternion Inverse(this Quaternion self)
        {
            return Quaternion.Inverse(self);
        }

        /// <summary>Get conjugate quaternion based on current</summary>
        /// <param name="self"></param>
        /// <returns></returns>
        /// <see cref="http://stackoverflow.com/questions/22157435/difference-between-the-two-quaternions"/>
        public static Quaternion Conjugate(this Quaternion self)
        {
            return new Quaternion(-self.x, -self.y, -self.z, self.w);
        }

        /// <summary>To get the local right vector from just a rotation.</summary>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public static Vector3 GetRight(this Quaternion rotation)
        {
            return rotation * Vector3.right;
            //return new Vector3(1 - 2 * (rotation.y * rotation.y + rotation.z * rotation.z),
            //            2 * (rotation.x * rotation.y + rotation.w * rotation.z),
            //            2 * (rotation.x * rotation.z - rotation.w * rotation.y));
        }
		public static Vector3 GetLeft(this Quaternion rotation)
		{
			return rotation * Vector3.left;
		}


        /// <summary>To get the local up vector from just a rotation.</summary>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public static Vector3 GetUp(this Quaternion rotation)
        {
            return rotation * Vector3.up;
            //return new Vector3(2 * (rotation.x * rotation.y - rotation.w * rotation.z),
            //            1 - 2 * (rotation.x * rotation.x + rotation.z * rotation.z),
            //            2 * (rotation.y * rotation.z + rotation.w * rotation.x));
        }
		public static Vector3 GetDown(this Quaternion rotation)
		{
			return -rotation.GetUp();
		}

        /// <summary>To get the local forward vector from just a rotation.</summary>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public static Vector3 GetForward(this Quaternion rotation)
        {
            return rotation * Vector3.forward;
            //return new Vector3(2f * (rotation.x * rotation.z + rotation.w * rotation.y),
            //            2f * (rotation.y * rotation.x - rotation.w * rotation.x),
            //            1f - 2f * (rotation.x * rotation.x + rotation.y * rotation.y));
        }
		public static Vector3 GetBackward(this Quaternion rotation)
		{
			return -rotation.GetForward();
		}

        /// <see cref="http://sunday-lab.blogspot.hk/2008/04/get-pitch-yaw-roll-from-quaternion.html"/>
        public static float GetPitch(this Quaternion o)
        {
            return Mathf.Atan((2f * (o.y * o.z + o.w * o.x)) / (o.w * o.w - o.x * o.x - o.y * o.y + o.z * o.z));
        }
        public static float GetYaw(this Quaternion o)
        {
            return Mathf.Asin(-2f * (o.x * o.z - o.w * o.y));
        }
        public static float GetRoll(this Quaternion o)
        {
            return Mathf.Atan((2f * (o.x * o.y + o.w * o.z)) / (o.w * o.w + o.x * o.x - o.y * o.y - o.z * o.z));
        }
        /// <summary>To find a signed angle between two quaternion, based on giving axis</summary>
        /// <param name="qtA">Quaternion</param>
        /// <param name="qtB">Quaternion</param>
        /// <param name="direction">The direction of quaternion, e.g. Vector3.forward</param>
        /// <param name="normal">The normal axis, e.g. Vector3.right</param>
        /// <returns></returns>
        public static float AngleBetweenDirectionSigned(this Quaternion qtA, Quaternion qtB, Vector3 direction, Vector3 normal)
        {
            return (qtA * direction).AngleBetweenDirectionSigned(qtB * direction, normal);
        }
    }
}