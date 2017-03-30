using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using Kit.Extend;

namespace CF.CameraBot.Parts
{
	[Serializable]
	public struct WeightPoint
	{
		public Vector3 m_Point;
		public float m_Weight;
	}

	public class CameraPivot : MonoBehaviour, IParts, IGizmo
	{
		#region Variables
		public Preset m_Preset;
		public CameraStand m_CameraStand;
		
		private HashSet<IDeaPosition> m_Consultant = new HashSet<IDeaPosition>();
		private List<WeightPoint> m_DestinationPoint = new List<WeightPoint>();
		private Vector3 m_LastPosition = Vector3.zero, m_LastLocalPosition = Vector3.zero;
		private Vector3 m_Velocity = Vector3.zero, m_LocalVelocity = Vector3.zero;
		public Vector3 Velocity { get { return m_Velocity; } }
		public Vector3 LocalVelocity { get { return m_LocalVelocity; } }
		public float Speed { get { return Vector3.zero.Distance(m_Velocity) / Time.fixedDeltaTime; } }
		public float LocalSpeed { get { return Vector3.zero.Distance(m_LocalVelocity) / Time.fixedDeltaTime; } }
		#endregion

		#region System
		void Awake()
		{
			m_LastLocalPosition = transform.localPosition;
			m_LastPosition = transform.position;
			m_LocalVelocity = Vector3.zero;
			m_Velocity = Vector3.zero;
		}

		void FixedUpdate()
		{
			CalculateVelocity();
			PerformIdeasPosition();
			transform.localPosition = CenterOfVectors(m_DestinationPoint);
			// Who need correction ?! we don't care.
			//if (transform.localPosition.EqualRoughly(Vector3.zero, .3f))
			//	transform.localRotation = Quaternion.identity;
			//else
			//	transform.LookAt(m_Preset.Instance.CameraLookAt.position, m_Preset.Instance.CameraUpward());
		}
		#endregion

		#region Interface
		public void Init(Preset preset, CameraStand cameraStand)
		{
			m_Preset = preset;
			m_CameraStand = cameraStand;
		}

		public void ResetToInit()
		{
			m_LocalVelocity = m_Velocity = m_LastPosition = transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
			transform.localPosition = Vector3.zero;
		}

		public void DrawGizmos()
		{
			// GizmosExtend.DrawLabel(transform.position + Vector3.up * 0.2f, "Velocity:" + m_Velocity, GUI.skin.textArea);
		}
		#endregion

		#region Core
		public void RegisterOperator(IDeaPosition who)
		{
			if (who != null)
				m_Consultant.Add(who);
		}

		public void UnregisterOperator(IDeaPosition who)
		{
			if (who != null)
				m_Consultant.Remove(who);
		}

		private void PerformIdeasPosition()
		{
			m_DestinationPoint.Clear();
			int groupPt = int.MaxValue;
			m_Consultant.OrderByDescending(o => o.GroupNumber).ThenBy(o => o.Weight);
			foreach (IDeaPosition obj in m_Consultant)
			{
				if (groupPt > obj.GroupNumber && // one for each group
					obj.Weight > 0f && // ignore zero-weight
					!obj.ToLocalPosition.EqualSimilar(Vector3.zero)) // with result
				{
					groupPt = obj.GroupNumber;
					m_DestinationPoint.Add(new WeightPoint() { m_Point = obj.ToLocalPosition, m_Weight = obj.Weight });
				}
			}
		}

		private Vector3 CenterOfVectors(List<Vector3> points)
		{
			if (points == null || points.Count == 0)
				return Vector3.zero;

			Vector3 sum = Vector3.zero;
			foreach (Vector3 point in points)
			{
				sum += point;
			}
			return sum / points.Count;
		}

		private Vector3 CenterOfVectors(List<WeightPoint> points)
		{
			if (points == null || points.Count == 0)
				return Vector3.zero;

			Vector3 sum = Vector3.zero;
			float totalWeight = 0f;
			foreach (WeightPoint point in points)
			{
				sum += point.m_Point * point.m_Weight;
				totalWeight += point.m_Weight;
			}
			return sum / totalWeight;
		}

		private void CalculateVelocity()
		{
			m_Velocity = m_LastPosition.Direction(transform.position);
			m_LastPosition = transform.position;
			m_LocalVelocity = m_LastLocalPosition.Direction(transform.localPosition);
			m_LastLocalPosition = transform.localPosition;
		}
		#endregion
	}
}