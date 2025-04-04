﻿using DragonLens.Content.GUI;
using DragonLens.Content.Tools.Spawners;
using Terraria.ModLoader;

namespace DragonLens.Content.Filters.ItemFilters
{
	internal class DamageClassFilter : Filter
	{
		public DamageClass damageClass;

		public DamageClassFilter(DamageClass damageClass, string texture) : base(texture, damageClass.DisplayName, $"Items with {damageClass.DisplayName}", n => FilterByDamageClass(n, damageClass))
		{
			this.damageClass = damageClass;
		}

		public static bool FilterByDamageClass(BrowserButton button, DamageClass damageClass)
		{
			if (button is ItemButton)
			{
				var ib = button as ItemButton;

				if (ib.item.damage > 0 && ib.item.DamageType.CountsAsClass(damageClass))
					return false;
			}

			return true;
		}
	}
}
