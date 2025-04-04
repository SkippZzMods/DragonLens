﻿using DragonLens.Configs;
using DragonLens.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace DragonLens.Content.GUI.FieldEditors
{
	internal abstract class EntityEditor<T> : FieldEditor<T> where T : Entity
	{
		public bool selecting = false;

		public override bool Editing => selecting;

		public EntityEditor(string name, Action<T> onValueChanged, Func<T> listenForUpdates = null, T initialValue = null, string description = "") : base(120, name, onValueChanged, listenForUpdates, initialValue, description)
		{
			SelectorButton<T> button = new(this);
			button.Left.Set(80, 0);
			button.Top.Set(32, 0);
			Append(button);
		}

		/// <summary>
		/// Called every frame to attempt to find the entity to select if the player is in select mode. Return null if nothing can be found, and an entity if applicable. Automatically checks for clicks and select mode.
		/// </summary>
		/// <returns></returns>
		public abstract T OnSelect();

		public override void SafeUpdate(GameTime gameTime)
		{
			if (Main.mouseRight)
				selecting = false;

			if (selecting && Main.mouseLeft)
			{
				T selected = OnSelect();

				if (selected != null)
				{
					onValueChanged(selected);
					value = selected;
					selecting = false;
				}
			}
		}

		public override void SafeDraw(SpriteBatch sprite)
		{
			Vector2 pos = GetDimensions().Position();
			var preview = new Rectangle((int)pos.X + 12, (int)pos.Y + 32, 60, 60);

			GUIHelper.DrawBox(sprite, preview, ModContent.GetInstance<GUIConfig>().buttonColor);

			if (value != null && value.active)
			{
				preview.Inflate(-4, -4);
				var source = new Rectangle((int)value.Center.X - 60, (int)value.Center.Y - 60, 120, 120);
				source.Offset((-Main.screenPosition).ToPoint());
				sprite.Draw(Main.screenTarget, preview, source, Color.White);
			}

			Utils.DrawBorderString(sprite, $"Live {typeof(T).Name} Reaction", pos + new Vector2(8, 94), Color.White, 0.7f);

			base.SafeDraw(sprite);
		}
	}

	internal class SelectorButton<T> : UIElement where T : Entity
	{
		public EntityEditor<T> parent;

		public SelectorButton(EntityEditor<T> parent)
		{
			this.parent = parent;
			Width.Set(60, 0);
			Height.Set(60, 0);
		}

		public override void Click(UIMouseEvent evt)
		{
			parent.selecting = true;
			Main.NewText("Click on an entity in the world to select it. Right click to cancel.");
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			GUIHelper.DrawBox(spriteBatch, GetDimensions().ToRectangle(), ModContent.GetInstance<GUIConfig>().buttonColor);

			Texture2D tex = ModContent.Request<Texture2D>("DragonLens/Assets/GUI/Picker").Value;
			spriteBatch.Draw(tex, GetDimensions().Center(), null, Color.White, 0, tex.Size() / 2, 1, 0, 0);
		}
	}
}
