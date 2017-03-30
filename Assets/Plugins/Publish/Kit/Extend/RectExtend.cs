using UnityEngine;

namespace Kit.Extend
{
	public static class RectExtend
	{
		public static Rect GetRectLeft(this Rect rect, float width = -1f, float height = -1f)
		{
			width = (width > 0f) ? width : rect.width;
			height = (height > 0f) ? height : rect.height;
			return new Rect(
				rect.x - rect.width + (rect.width - width),
				rect.y,
				width,
				height);
		}

		public static Rect GetRectRight(this Rect rect, float width = -1f, float height = -1f)
		{
			return new Rect(
				rect.x + rect.width,
				rect.y,
				((width > 0f) ? width : rect.width),
				((height > 0f) ? height : rect.height));
		}

		public static Rect GetRectTop(this Rect rect, float width = -1f, float height = -1f)
		{
			width = (width > 0f) ? width : rect.width;
			height = (height > 0f) ? height : rect.height;
			return new Rect(
				rect.x,
				rect.y - rect.height + (rect.height - height),
				width,
				height);
		}

		public static Rect GetRectBottom(this Rect rect, float width = -1f, float height = -1f)
		{
			return new Rect(
				rect.x,
				rect.y + rect.height,
				((width > 0f) ? width : rect.width),
				((height > 0f) ? height : rect.height));
		}

		public static Rect Clone(this Rect rect, float x = float.NaN, float y = float.NaN, float width = float.NaN, float height = float.NaN)
		{
			return new Rect(
				(float.IsNaN(x) ? rect.x : x),
				(float.IsNaN(y) ? rect.y : y),
				(float.IsNaN(width) ? rect.width : width),
				(float.IsNaN(height) ? rect.height : height)
				);
		}

		public static Rect Modify(this Rect rect, float x = float.NaN, float y = float.NaN, float width = float.NaN, float height = float.NaN)
		{
			return new Rect(
				(float.IsNaN(x) ? rect.x : rect.x + x),
				(float.IsNaN(y) ? rect.y : rect.y + y),
				(float.IsNaN(width) ? rect.width : rect.width + width),
				(float.IsNaN(height) ? rect.height : rect.height + height)
				);
		}

		public static Rect[] SplitHorizontal(this Rect rect, float percentage = .5f,
			float leftMin = 0f, float leftMax = float.PositiveInfinity)
		{
			if (percentage > 1f) percentage = 1f;
			float lWidth = Mathf.Clamp(Mathf.Abs(rect.width) * percentage, leftMin, leftMax);
			return new Rect[2]
			{
				rect.Clone(width: lWidth),
				rect.Clone(x: rect.x + lWidth, width: rect.width - lWidth)
			};
		}

		public static Rect[] SplitVertical(this Rect rect, float percentage = .5f,
			float topMin = 0f, float topMax = float.PositiveInfinity)
		{
			if (percentage > 1f) percentage = 1f;
			float lheight = Mathf.Clamp(Mathf.Abs(rect.height) * percentage, topMin, topMax);
			return new Rect[2]
			{
				rect.Clone(height: lheight),
				rect.Clone(y: rect.y + lheight, height: rect.height - lheight)
			};
		}
	}
}