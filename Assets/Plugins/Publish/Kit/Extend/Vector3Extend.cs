using UnityEngine;

namespace Kit.Extend
{
	public static class Vector3Extend
    {
        #region Enum
        public static Vector3 AxisXY { get { return Vector3.Cross(Vector3.right, Vector3.up); } }
        public static Vector3 AxisXZ { get { return Vector3.Cross(Vector3.right, Vector3.forward); } }
        public static Vector3 AxisYZ { get { return Vector3.Cross(Vector3.up, Vector3.forward); } }
        #endregion

        #region cast to Vector2
        public static Vector2 CastToVector2(this Vector3 self, Vector3 normal)
        {
            if (normal == AxisXY)
                return new Vector2(self.x, self.y);
            else if (normal == AxisXZ)
                return new Vector2(self.x, self.z);
            else if (normal == AxisYZ)
                return new Vector2(self.y, self.z);
            Debug.LogWarning(string.Format("Vector3Extend : fail to CastToVector2({0},{1})", self, normal));
            return Vector2.zero;
        }
        #endregion

        #region basic
        /// <summary>Clone a new Vector based on reference</summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static Vector3 Clone(this Vector3 self)
        {
            return new Vector3(self.x, self.y, self.z);
        }
        /// <summary>Assign a new value to X-axis</summary>
        /// <param name="self"></param>
        /// <param name="value"></param>
        /// <returns>this vector3</returns>
        public static Vector3 SetX(this Vector3 self, float value)
        {
            self.x = value;
            return self;
        }
        /// <summary>Assign a new value to Y-axis</summary>
        /// <param name="self"></param>
        /// <param name="value"></param>
        /// <returns>this vector3</returns>
        public static Vector3 SetY(this Vector3 self, float value)
        {
            self.y = value;
            return self;
        }
        /// <summary>Assign a new value to Z-axis</summary>
        /// <param name="self"></param>
        /// <param name="value"></param>
        /// <returns>this vector3</returns>
        public static Vector3 SetZ(this Vector3 self, float value)
        {
            self.z = value;
            return self;
        }
		/// <summary>Clamp Vector3 by giving range</summary>
		/// <param name="self"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <returns></returns>
        public static Vector3 Clamp(this Vector3 self, Vector3 min, Vector3 max)
		{
            self.x = Mathf.Clamp(self.x, min.x, max.x);
            self.y = Mathf.Clamp(self.y, min.y, max.y);
            self.z = Mathf.Clamp(self.z, min.z, max.z);
            return self;
		}
        public static Vector3 ClampX(this Vector3 self, float min, float max)
        {
            self.x = Mathf.Clamp(self.x, min, max);
            return self;
        }
        public static Vector3 ClampY(this Vector3 self, float min, float max)
        {
            self.y = Mathf.Clamp(self.y, min, max);
            return self;
        }
        public static Vector3 ClampZ(this Vector3 self, float min, float max)
        {
            self.z = Mathf.Clamp(self.z, min, max);
            return self;
        }
        public static bool EqualSimilar(this Vector3 self, Vector3 target)
        {
            return
                self.x.EqualSimilar(target.x) &&
                self.y.EqualSimilar(target.y) &&
                self.z.EqualSimilar(target.z);
        }
        /// <summary>Compare two Vector is roughly equal to each others</summary>
        /// <param name="self">Vector3</param>
        /// <param name="target">Vector3</param>
        /// <param name="threshold">The threshold value that can ignore.</param>
        /// <returns>true/false</returns>
        public static bool EqualRoughly(this Vector3 self, Vector3 target, float threshold = float.Epsilon)
        {
            return self.x.EqualRoughly(target.x, threshold) &&
                self.y.EqualRoughly(target.y, threshold) &&
                self.z.EqualRoughly(target.z, threshold);
        }
        /// <summary>Absolute value of vector</summary>
        /// <param name="self"></param>
        /// <returns></returns>
        /// <example>Vector3(2f,-1f,-100f) = Vector3(2f,1f,100f)</example>
        public static Vector3 Abs(this Vector3 self)
        {
            return new Vector3(Mathf.Abs(self.x),Mathf.Abs(self.y),Mathf.Abs(self.z));
        }
        /// <summary>Divide current Vector by the other</summary>
        /// <param name="self"></param>
        /// <param name="denominator"></param>
        /// <returns></returns>
        /// <example>Vector3(6,4,2).Divide(new Vector3(2,2,2)) == Vector3(3,2,1)</example>
        public static Vector3 Divide(this Vector3 self, Vector3 denominator)
        {
            return new Vector3(self.x / denominator.x, self.y / denominator.y, self.z / denominator.z);
        }
        #endregion

        #region Position
        /// <summary>Transforms position from local space to world space.</summary>
        /// <param name="position"></param>
        /// <param name="localRotate"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        /// <remarks>As same as Transform.TransformPoint</remarks>
        /// <see cref="http://docs.unity3d.com/412/Documentation/ScriptReference/Transform.TransformPoint.html"/>
        /// <seealso cref="https://en.wikipedia.org/wiki/Transformation_matrix"/>
        public static Vector3 TransformPoint(this Vector3 position, Quaternion localRotate, Vector3 localScale, Vector3 offset)
        {
            Matrix4x4 matrix = new Matrix4x4();
            matrix.SetTRS(position, localRotate, localScale);
            //return position + localRotate * Vector3.Scale(offset, localScale);
            // Why the fuck in the world your document didn't write it down.
            return matrix.MultiplyPoint3x4(offset);
        }
        public static Vector3 TransformPoint(this Vector3 position, Quaternion localRotate, Vector3 offset)
        {
            return TransformPoint(position, localRotate, Vector3.one, offset);
        }
        /// <summary>Transforms position from world space to local space.</summary>
        /// <param name="position"></param>
        /// <param name="localRotate"></param>
        /// <param name="targetPosition"></param>
        /// <returns></returns>
        public static Vector3 InverseTransformPoint(this Vector3 position, Quaternion localRotate, Vector3 localScale, Vector3 targetPosition)
        {
            // http://answers.unity3d.com/questions/1124805/world-to-local-matrix-without-transform.html
            // return (localRotate.Inverse() * (targetPosition - position)).Divide(localScale);

            Matrix4x4 matrix = new Matrix4x4();
            matrix.SetTRS(position, localRotate, localScale);
            return matrix.inverse.MultiplyPoint(targetPosition);
        }
		public static Vector3 InverseTransformPoint(this Vector3 position, Quaternion localRotate, Vector3 targetPosition)
		{
			return position.InverseTransformPoint(localRotate, Vector3.one, targetPosition);
		}
		#endregion

		#region Direction
		/// <summary>Transforms Direction from local space to world space</summary>
		/// <param name="position"></param>
		/// <param name="localRotate"></param>
		/// <param name="localScale"></param>
		/// <param name="direction"></param>
		/// <returns></returns>
		public static Vector3 TransformDirection(this Vector3 position, Quaternion localRotate, Vector3 localScale, Vector3 direction)
        {
            return position.TransformVector(localRotate, Vector3.one, direction);
        }
        /// <summary>Transform Direction from world space to local space</summary>
        /// <param name="position"></param>
        /// <param name="localRotate"></param>
        /// <param name="localScale"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static Vector3 InverseTransformDirection(this Vector3 position, Quaternion localRotate, Vector3 localScale, Vector3 direction)
        {
            return position.InverseTransformVector(localRotate, Vector3.one, direction);
        }
        /// <summary>Transform vector from local space to world space</summary>
        /// <param name="position"></param>
        /// <param name="localRotate"></param>
        /// <param name="localScale"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector3 TransformVector(this Vector3 position, Quaternion localRotate, Vector3 localScale, Vector3 vector)
        {
            Matrix4x4 matrix = new Matrix4x4();
            matrix.SetTRS(position, localRotate, localScale);
            return matrix.MultiplyVector(vector);
        }
        /// <summary>Transforms vector from world space to local space</summary>
        /// <param name="position"></param>
        /// <param name="localRotate"></param>
        /// <param name="localScale"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector3 InverseTransformVector(this Vector3 position, Quaternion localRotate, Vector3 localScale, Vector3 vector)
        {
            Matrix4x4 matrix = new Matrix4x4();
            matrix.SetTRS(position, localRotate, localScale);
            return matrix.inverse.MultiplyVector(vector);
        }
        /// <summary>Direction between 2 position</summary>
        /// <param name="from">Position</param>
        /// <param name="to">Position</param>
        /// <returns>Direction Vector</returns>
        public static Vector3 Direction(this Vector3 from, Vector3 to)
        {
            return to - from;
        }
        /// <summary>Rotate X axis on current direction vector</summary>
        /// <param name="self"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Vector3 RotateX(this Vector3 self, float angle)
        {
            float sin = Mathf.Sin(angle);
            float cos = Mathf.Cos(angle);

            float ty = self.y;
            float tz = self.z;
            self.y = (cos * ty) - (sin * tz);
            self.z = (cos * tz) + (sin * ty);
            return self;
        }
        /// <summary>Rotate Y axis on current direction vector</summary>
        /// <param name="self"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Vector3 RotateY(this Vector3 self, float angle)
        {
            float sin = Mathf.Sin(angle);
            float cos = Mathf.Cos(angle);

            float tx = self.x;
            float tz = self.z;
            self.x = (cos * tx) + (sin * tz);
            self.z = (cos * tz) - (sin * tx);
            return self;
        }
        /// <summary>Rotate Z axis on current direction vector</summary>
        /// <param name="self"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Vector3 RotateZ(this Vector3 self, float angle)
        {
            float sin = Mathf.Sin(angle);
            float cos = Mathf.Cos(angle);

            float tx = self.x;
            float ty = self.y;
            self.x = (cos * tx) - (sin * ty);
            self.y = (cos * ty) + (sin * tx);
            return self;
        }
        /// <summary>Find the relative vector from giving angle & axis</summary>
        /// <param name="self"></param>
        /// <param name="angle">0~360</param>
        /// <param name="axis">Vector direction e.g. Vector.up</param>
        /// <param name="useRadians">0~360 = false, 0~1 = true</param>
        /// <returns></returns>
        public static Vector3 RotateAroundAxis(this Vector3 self, float angle, Vector3 axis, bool useRadians = false)
        {
            if (useRadians) angle *= Mathf.Rad2Deg;
            Quaternion q = Quaternion.AngleAxis(angle, axis);
            return (q * self);
        }
        #endregion

        #region Distance
        /// <summary>Distance between two position</summary>
        /// <param name="self"></param>
        /// <param name="position"></param>
        /// <returns>disatnce</returns>
        /// <remarks>Faster run time for Vector3.Distance</remarks>
        /// <see cref="http://answers.unity3d.com/questions/384932/best-way-to-find-distance.html"/>
        /// <seealso cref="http://forum.unity3d.com/threads/square-root-runs-1000-times-in-0-01ms.147661/"/>
        public static float Distance(this Vector3 self, Vector3 position)
        {
            position -= self;
            return Mathf.Sqrt( position.x * position.x + position.y * position.y + position.z * position.z);
        }
        /// <summary>Return lerp Vector3 by giving distance</summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static Vector3 LerpByDistance(Vector3 start, Vector3 end, float distance)
        {
            return distance * (end - start).normalized + start;
        }
        /// <summary>Use start position, direction and known distance to find the end point position</summary>
        /// <param name="position"></param>
        /// <param name="direction"></param>
        /// <param name="distance"></param>
        /// <returns>End point position</returns>
        public static Vector3 PointOnDistance(this Vector3 position, Vector3 direction, float distance)
        {
            return position + (direction.normalized * distance);
        }
        #endregion

        #region Angle
        /// <summary>find angle between 2 position, using itself as center</summary>
        /// <param name="center"></param>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public static float AngleBetweenPosition(this Vector3 center, Vector3 point1, Vector3 point2)
        {
            return Vector3.Angle((point1 - center).normalized, (point2 - center).normalized);
        }

        /// <summary>Determine the signed angle between two vectors, with normal as the rotation axis.</summary>
        /// <example>Vector3.AngleBetweenDirectionSigned(Vector3.forward,Vector3.right)</example>
        /// <param name="direction1">Direction vector</param>
        /// <param name="direction2">Direction vector</param>
        /// <param name="normal">normal vector e.g. AxisXZ = Vector3.Cross(Vector3.forward, Vector3.right);</param>
        /// <see cref="http://forum.unity3d.com/threads/need-vector3-angle-to-return-a-negtive-or-relative-value.51092/"/>
        /// <see cref="http://stackoverflow.com/questions/19675676/calculating-actual-angle-between-two-vectors-in-unity3d"/>
        public static float AngleBetweenDirectionSigned(this Vector3 direction1, Vector3 direction2, Vector3 normal)
        {
            return Mathf.Rad2Deg * Mathf.Atan2(Vector3.Dot(normal, Vector3.Cross(direction1, direction2)), Vector3.Dot(direction1, direction2));
            // return Vector3.Angle(direction1, direction2) * Mathf.Sign(Vector3.Dot(normal, Vector3.Cross(direction1, direction2)));
        }
        #endregion

        
	}
}
