using UnityEngine;

namespace Kit.Extend
{
    public static class ColorExtend
    {
        #region basic
        public static Color Clone(this Color self)
        {
            return new Color(self.r, self.g, self.b, self.a);
        }
        public static Color SetAlpha(this Color self, float value)
        {
            self.a = value;
            return self;
        }

        public static bool EqualSimilar(this Color self, Color target)
        {
            return
                self.r.EqualSimilar(target.r) &&
                self.g.EqualSimilar(target.g) &&
                self.b.EqualSimilar(target.b) &&
                self.a.EqualSimilar(target.a);
        }

        public static bool NealyEqual(this Color self, Color target, float epsilon)
        {
            return
                self.r.NealyEqual(target.r, epsilon) &&
                self.g.NealyEqual(target.g, epsilon) &&
                self.b.NealyEqual(target.b, epsilon) &&
                self.a.NealyEqual(target.a, epsilon);
        }
        public static Color TryParse(string RGBANumbers)
        {
            // clear up
            string[] param = RGBANumbers.Trim().Split(',');
            if (param == null || param.Length == 0)
                return Color.black;

            int pt = 0;
            int count = 0;
            bool Is255 = false;
            float[] rgba = new float[4]{ 0f,0f,0f,1f };
            
            while(param.Length > pt && count <= 4)
            {
                float tmp;
                if(float.TryParse(param[pt], out tmp))
                {
                    rgba[count] = tmp;
                    count++;
                    if (tmp > 1f) Is255 = true;
                }
                pt++;
            }

            // hotfix for 255
            if (Is255)
            {
                for (int i = 0; i < 3; i++) { rgba[i] /= 255f; }
                rgba[3] = Mathf.Clamp(rgba[3], 0f, 1f);
            }
            return new Color(rgba[0], rgba[1], rgba[2], rgba[3]);
        }
        public static Color Random(this Color color)
        {
            return color.RandomRange(Color.clear, Color.white);
        }
        public static Color RandomRange(this Color color, Color min, Color max)
        {
            color.r = UnityEngine.Random.Range(min.r, max.r);
            color.g = UnityEngine.Random.Range(min.g, max.g);
            color.b = UnityEngine.Random.Range(min.b, max.b);
            color.a = UnityEngine.Random.Range(min.a, max.a);
            return color;
        }
        #endregion
    }
}