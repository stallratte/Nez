// based on the FNA SpriteBatch implementation by Ethan Lee: https://github.com/FNA-XNA/FNA

using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Nez.Textures;


namespace Nez
{
	public abstract class Batcher : GraphicsResource
	{
		/// <summary>
		/// Matrix to be used when creating the projection matrix
		/// </summary>
		/// <value>The transform matrix.</value>
		public abstract  Matrix TransformMatrix {get; protected set;}

		/// <summary>
		/// If true, destination positions will be rounded before being drawn.
		/// </summary>
		public bool ShouldRoundDestinations = true;
		
		protected Batcher(GraphicsDevice graphicsDevice)
		{
			Insist.IsTrue(graphicsDevice != null);
			GraphicsDevice = graphicsDevice;
		}

		/// <summary>
		/// sets if position rounding should be ignored. Useful when you are drawing primitives for debugging.
		/// </summary>
		/// <param name="shouldIgnore">If set to <c>true</c> should ignore.</param>
		public abstract void SetIgnoreRoundingDestinations(bool shouldIgnore);

		#region Public begin/end methods

		public virtual void Begin()
		{
			Begin(BlendState.AlphaBlend, Core.DefaultSamplerState, DepthStencilState.None,
				RasterizerState.CullCounterClockwise, null, Matrix.Identity, false);
		}

		public virtual void Begin(Effect effect)
		{
			Begin(BlendState.AlphaBlend, Core.DefaultSamplerState, DepthStencilState.None,
				RasterizerState.CullCounterClockwise, effect, Matrix.Identity, false);
		}

		public virtual void Begin(Material material)
		{
			Begin(material.BlendState, material.SamplerState, material.DepthStencilState,
				RasterizerState.CullCounterClockwise, material.Effect);
		}

		public virtual void Begin(Matrix transformationMatrix)
		{
			Begin(BlendState.AlphaBlend, Core.DefaultSamplerState, DepthStencilState.None,
				RasterizerState.CullCounterClockwise, null, transformationMatrix, false);
		}

		public virtual void Begin(BlendState blendState)
		{
			Begin(blendState, Core.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise,
				null, Matrix.Identity, false);
		}

		public virtual void Begin(Material material, Matrix transformationMatrix)
		{
			Begin(material.BlendState, material.SamplerState, material.DepthStencilState,
				RasterizerState.CullCounterClockwise, material.Effect, transformationMatrix, false);
		}

		public virtual void Begin(BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState,
		                  RasterizerState rasterizerState)
		{
			Begin(
				blendState,
				samplerState,
				depthStencilState,
				rasterizerState,
				null,
				Matrix.Identity,
				false
			);
		}

		public virtual void Begin(BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState,
		                  RasterizerState rasterizerState, Effect effect)
		{
			Begin(
				blendState,
				samplerState,
				depthStencilState,
				rasterizerState,
				effect,
				Matrix.Identity,
				false
			);
		}

		public virtual void Begin(BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState,
		                  RasterizerState rasterizerState,
		                  Effect effect, Matrix transformationMatrix)
		{
			Begin(
				blendState,
				samplerState,
				depthStencilState,
				rasterizerState,
				effect,
				transformationMatrix,
				false
			);
		}

		public abstract void Begin(BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState,
		                  RasterizerState rasterizerState,
		                  Effect effect, Matrix transformationMatrix, bool disableBatching);

		public abstract void End();

		#endregion

		#region Public draw methods

		public abstract void Draw(Texture2D texture, Vector2 position);

		public abstract void Draw(Texture2D texture, Vector2 position, Color color);

		public abstract void Draw(Texture2D texture, Rectangle destinationRectangle);

		public abstract void Draw(Texture2D texture, Rectangle destinationRectangle, Color color);
		
		public abstract void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color);

		public abstract void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color,
		                 SpriteEffects effects);

		public abstract void Draw(
			Texture2D texture,
			Rectangle destinationRectangle,
			Rectangle? sourceRectangle,
			Color color,
			float rotation,
			SpriteEffects effects,
			float layerDepth,
			float skewTopX, float skewBottomX, float skewLeftY, float skewRightY
		);

		public abstract void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color);

		public abstract void Draw(
			Texture2D texture,
			Vector2 position,
			Rectangle? sourceRectangle,
			Color color,
			float rotation,
			Vector2 origin,
			float scale,
			SpriteEffects effects,
			float layerDepth
		);

		public abstract void Draw(
			Sprite sprite,
			Vector2 position,
			Color color,
			float rotation,
			Vector2 origin,
			float scale,
			SpriteEffects effects,
			float layerDepth
		);

		public abstract void Draw(
			Texture2D texture,
			Vector2 position,
			Rectangle? sourceRectangle,
			Color color,
			float rotation,
			Vector2 origin,
			Vector2 scale,
			SpriteEffects effects,
			float layerDepth
		);

		public abstract void Draw(
			Sprite sprite,
			Vector2 position,
			Color color,
			float rotation,
			Vector2 origin,
			Vector2 scale,
			SpriteEffects effects,
			float layerDepth
		);

		public abstract void Draw(
			Texture2D texture,
			Vector2 position,
			Rectangle? sourceRectangle,
			Color color,
			float rotation,
			Vector2 origin,
			Vector2 scale,
			SpriteEffects effects,
			float layerDepth,
			float skewTopX, float skewBottomX, float skewLeftY, float skewRightY
		);

		public abstract void Draw(
			Texture2D texture,
			Rectangle destinationRectangle,
			Rectangle? sourceRectangle,
			Color color,
			float rotation,
			Vector2 origin,
			SpriteEffects effects,
			float layerDepth
		);

		/// <summary>
		/// direct access to setting vert positions, UVs and colors. The order of elements is top-left, top-right, bottom-left, bottom-right
		/// </summary>
		/// <returns>The raw.</returns>
		/// <param name="texture">Texture.</param>
		/// <param name="verts">Verts.</param>
		/// <param name="textureCoords">Texture coords.</param>
		/// <param name="colors">Colors.</param>
		public virtual void DrawRaw(Texture2D texture, Vector3[] verts, Vector2[] textureCoords, Color[] colors)
		{
		}

		/// <summary>
		/// direct access to setting vert positions, UVs and colors. The order of elements is top-left, top-right, bottom-left, bottom-right
		/// </summary>
		/// <returns>The raw.</returns>
		/// <param name="texture">Texture.</param>
		/// <param name="verts">Verts.</param>
		/// <param name="textureCoords">Texture coords.</param>
		/// <param name="color">Color.</param>
		public virtual void DrawRaw(Texture2D texture, Vector3[] verts, Vector2[] textureCoords, Color color)
		{
		}

		#endregion


		[Obsolete("SpriteFont is too locked down to use directly. Wrap it in a NezSpriteFont")]
		public virtual void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation,
		                       Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
		{
		}

		public abstract void FlushBatch();

		/// <summary>
		/// enables/disables scissor testing. If the RasterizerState changes it will cause a batch flush.
		/// </summary>
		/// <returns>The scissor test.</returns>
		/// <param name="shouldEnable">Should enable.</param>
		public abstract void EnableScissorTest(bool shouldEnable);
	}
}
