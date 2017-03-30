using System;

namespace Kit.Extend
{
    public static class FloatExtend
    {
        public static bool EqualSimilar(this float self, float target)
        {
            return UnityEngine.Mathf.Approximately(self, target);
        }
        public static bool NealyEqual(this float self, float target, float epsilon = float.Epsilon)
        {
            // http://floating-point-gui.de/errors/comparison/
            float absA = Math.Abs(self);
            float absB = Math.Abs(target);
            float diff = Math.Abs(self - target);
            
            if (self == target) { // shortcut, handles infinities
                return true;
            }
            else if (self == 0f || target == 0f || diff < float.Epsilon )
            {
                // a or b is zero or both are extremely close to it
                // relative error is less meaningful here
                return diff < (epsilon * float.Epsilon);
            } else
            {
                // use relative error
                return diff / Math.Min((absA + absB), float.MaxValue) < epsilon;
            }
        }

        public static bool EqualRoughly(this float self, float target, float threshold = float.Epsilon)
        {
            return Math.Abs(self - target) < threshold;
        }
		/// <summary>Get Number after scale.</summary>
		/// <param name="self"></param>
		/// <param name="fromMin"></param>
		/// <param name="fromMax"></param>
		/// <param name="toMin"></param>
		/// <param name="toMax"></param>
		/// <returns></returns>
		/// <see cref="http://stackoverflow.com/questions/11121012/how-to-scale-down-the-values-so-they-could-fit-inside-the-min-and-max-values"/>
		public static float Scale(this float self, float fromMin, float fromMax, float toMin, float toMax)
		{
			return toMin + ((toMax - toMin) / (fromMax - fromMin)) * (self - fromMin);
		}
    }
}