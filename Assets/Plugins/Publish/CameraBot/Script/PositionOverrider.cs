using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using CF.CameraBot.Parts;

namespace CF.CameraBot
{
	[Serializable]
    public class PositionOverrider
    {
		/// <summary>For developer view all addon in one panel.</summary>
		/// <see cref="PositionOverriderDrawer"/>
		public string m_AddonList;
		public int m_AddonCount;
		
		public void Update(Preset preset)
		{
			if (preset == null)
			{
				m_AddonList = string.Empty;
				m_AddonCount = 0;
				return;
			}
			List<Component> addons = preset.GetComponents<Component>()
				.OfType<IDeaPosition>()
				.Cast<Component>()
				.ToList();

			m_AddonCount = addons.Count;
			m_AddonList =
				"Total addon(s) : " + m_AddonCount + "\n" +
				string.Join("\n", addons
				.Cast<IDeaPosition>()
				.OrderByDescending(o => o.Weight)
				.Select(o =>
				{
					return o.ToString();
				})
				.ToArray());
		}
	}
}
