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
	/// <summary>
	/// A UI element for changing the value of 'something'. 
	/// </summary>
	internal abstract class FieldEditor<T> : UIElement
	{
		/// <summary>
		/// The name that gets displated above the panel to the user
		/// </summary>
		public readonly string name;

		/// <summary>
		/// The info sown when hovering over this panel
		/// </summary>
		public readonly string description;

		/// <summary>
		/// The current value this editor believes the field its tied to to have. This wont update in real time so be careful
		/// </summary>
		public T value;

		/// <summary>
		/// The callback that should happen when this editor thinks the value its tracking has changed. You'll likely need to cast the object parameter to the correct type.
		/// </summary>
		protected readonly Action<T> onValueChanged;

		/// <summary>
		/// This function, called every frame while the editor is not being used, is used to update the editor's value to the current value of the tracked value.
		/// </summary>
		protected readonly Func<T> listenForUpdate;

		/// <summary>
		/// If this editor is currently being used to change a value, and thus shouldn't listen for update
		/// </summary>
		public virtual bool Editing => false;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="height">Height of the panel</param>
		/// <param name="name">The name that gets displated above the panel to the user</param>
		/// <param name="onValueChanged">The callback that should happen when this editor thinks the value its tracking has changed. You'll likely need to cast the object parameter to the correct type.</param>
		/// <param name="initialValue">A hint for what the initial value of the field tracked by this editor is</param>
		public FieldEditor(int height, string name, Action<T> onValueChanged, Func<T> listenForUpdate = null, T initialValue = default, string description = "")
		{
			Width.Set(150, 0);
			Height.Set(height, 0);
			this.name = name;
			this.onValueChanged = onValueChanged;
			this.listenForUpdate = listenForUpdate;
			value = initialValue;
			this.description = description;
		}

		/// <summary>
		/// Defines what should happen when a new value is recieved from the value update listener. Note that value has not yet been updated when this is called, so you can compare to the old value.
		/// </summary>
		/// <param name="newValue">The new value that was recieved</param>
		public virtual void OnRecieveNewValue(T newValue) { }

		public sealed override void Update(GameTime gameTime)
		{
			if (!Editing && listenForUpdate != null)
			{
				T newValue = listenForUpdate();
				OnRecieveNewValue(newValue);
				value = newValue;
			}

			base.Update(gameTime);
			SafeUpdate(gameTime);
		}

		public virtual void SafeUpdate(GameTime gameTime) { }

		public sealed override void Draw(SpriteBatch spriteBatch)
		{
			GUIHelper.DrawBox(spriteBatch, GetDimensions().ToRectangle(), ModContent.GetInstance<GUIConfig>().backgroundColor);

			Texture2D back = ModContent.Request<Texture2D>("DragonLens/Assets/GUI/Gradient").Value;
			var backTarget = GetDimensions().ToRectangle();
			backTarget.Height = 24;
			backTarget.Offset(new Point(4, 4));
			spriteBatch.Draw(back, backTarget, Color.Black * 0.5f);

			Utils.DrawBorderString(spriteBatch, name, GetDimensions().Position() + new Vector2(8, 4), Color.White, 0.7f);
			Utils.DrawBorderString(spriteBatch, typeof(T).Name, GetDimensions().Position() + new Vector2(8, 18), Color.Gray, 0.65f);

			base.Draw(spriteBatch);

			SafeDraw(spriteBatch);

			if (IsMouseHovering)
			{
				Tooltip.SetName(name);
				Tooltip.SetTooltip(description);
			}
		}

		public virtual void SafeDraw(SpriteBatch sprite) { }
	}
}
