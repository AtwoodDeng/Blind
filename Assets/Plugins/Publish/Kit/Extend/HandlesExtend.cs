#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Kit.Extend
{
	public static class HandlesExtend
	{
		public static Vector3 PositionHandle(
			Vector3 pos,
			Quaternion rotation = default(Quaternion),
			float size = 1f,
			Vector3 snap = default(Vector3),
			string controlName = "handle",
			bool freeMode = true)
		{
			if (rotation == default(Quaternion) || Tools.pivotRotation == PivotRotation.Global)
				rotation = Quaternion.identity;

			if (snap == default(Vector3))
				snap = Vector3.zero;

			size = HandleUtility.GetHandleSize(pos) * size;

			if (freeMode)
			{
				size *= 0.075f;
				GUI.SetNextControlName(controlName);
				return Handles.FreeMoveHandle(pos, rotation, size, snap, Handles.CircleCap);
			}
			else
			{
				Color oldColor = Handles.color;

				string nameX = controlName + "_x", nameY = controlName + "_y", nameZ = controlName + "_z";

				Handles.color = Handles.xAxisColor;
				GUI.SetNextControlName(nameX);
				Vector3 x = Handles.Slider(pos, rotation * Vector3.left, size, Handles.ArrowCap, snap.x);

				Handles.color = Handles.yAxisColor;
				GUI.SetNextControlName(nameY);
				Vector3 y = Handles.Slider(pos, rotation * Vector3.up, size, Handles.ArrowCap, snap.y);

				Handles.color = Handles.zAxisColor;
				GUI.SetNextControlName(nameZ);
				Vector3 z = Handles.Slider(pos, rotation * Vector3.forward, size, Handles.ArrowCap, snap.z);

				Handles.color = oldColor;

				string active = GUI.GetNameOfFocusedControl();
				return nameX == active ? x : nameY == active ? y : z;
			}
		}

		public static void DirectionCap(
			Vector3 pos,
			Quaternion rotation = default(Quaternion),
			float size = 1f)
		{
			if (rotation == default(Quaternion))
				rotation = Quaternion.identity;

			size = HandleUtility.GetHandleSize(pos) * size;

			Color oldColor = Handles.color;

			
			Handles.color = Handles.xAxisColor;
			Handles.DrawLine(pos, pos.PointOnDistance(rotation.GetRight(), size));

			Handles.color = Handles.yAxisColor;
			Handles.DrawLine(pos, pos.PointOnDistance(rotation.GetUp(), size));

			Handles.color = Handles.zAxisColor;
			Handles.DrawLine(pos, pos.PointOnDistance(rotation.GetForward(), size));

			Handles.color = oldColor;
		}
	}
}
#endif