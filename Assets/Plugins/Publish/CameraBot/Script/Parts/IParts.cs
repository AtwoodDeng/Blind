using UnityEngine;

namespace CF.CameraBot.Parts
{
	public interface IParts
	{
		void Init(Preset preset, CameraStand cameraStand);
		void ResetToInit();
	}

	public interface IGizmo
	{
		void DrawGizmos();
	}

	public interface IArc
	{
		float Degree { get; set; }
	}

	public interface IOffset
	{
		Vector3 Offset { get; set; }
	}

	public interface IDeaPosition
	{
		/// <summary>Each group can only taken a position, the one with highest weight</summary>
		int GroupNumber { get; }
		/// <summary>the recommand position will sent to <see cref="CameraPivot"/></summary>
		Vector3 ToLocalPosition { get; }
		/// <summary>To define how important of this information is.</summary>
		float Weight { get; }
		/// <summary>The information that will displayed on position overrider section.</summary>
		string ToString();
	}
}