using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez.Textures;

namespace Nez.Aseprite
{
	/// <summary>
	/// Represents a single frame in an Aseprite file.  Frames are composed of cels.
	/// </summary>
	public sealed class AsepriteFrame
	{
		/// <summary>
		/// The collection of cel elements that make up this frame.  Order of cels are from the bottom most layer to the
		/// top most layer within the frame.
		/// </summary>
		public readonly List<AsepriteCel> Cels;

		/// <summary>
		/// The name of this frame.  This name is autogenerated based on the name of the Aseprite file.
		/// </summary>
		public readonly string Name;

		/// <summary>
		/// The width, in pixels, of this frame.
		/// </summary>
		public readonly int Width;

		/// <summary>
		/// The height, in pixels, of this frame.
		/// </summary>
		public readonly int Height;

		/// <summary>
		/// The duration, in milliseconds, that this frame should be displayed when used as part of an animation.
		/// </summary>
		public readonly int Duration;

		internal AsepriteFrame(string name, int duration, List<AsepriteCel> cels, int width, int height)
		{
			Name = name;
			Duration = duration;
			Cels = cels;
			Width = width;
			Height = height;
		}

		/// <summary>
		/// Flattens this frame by blending all cel elements into a single iamge.
		/// </summary>
		/// <param name="onlyVisibleLayers">
		/// Indicates whether only cels that are on visible layers should be included when flattening this frame.
		/// </param>
		/// <param name="includeBackgroundLayer">
		/// Indicates whether the cel on the layer marked as the background layer in Aseprite should be included when
		/// flattening this frame.
		/// </param>
		/// <returns>
		/// A new array of color elements where each element represents the final pixels for this frame once flattened.
		/// Order of color element starts with the top-left most pixel and is read left-to-right from top-to-bottom.
		/// </returns>
		public Color[] FlattenFrame(bool onlyVisibleLayers = true, bool includeBackgroundLayer = false)
		{
			Color[] result = new Color[Width * Height];

			for (int c = 0; c < Cels.Count; c++)
			{
				AsepriteCel cel = Cels[c];

				//  Are we only processing cels on visible layers?
				if (onlyVisibleLayers && !cel.Layer.IsVisible) { continue; }

				//  Are we processing cels on background layers?
				if (cel.Layer.IsBackgroundLayer && !includeBackgroundLayer) { continue; }

				//	Only process image cels for now.  
				//	Note: Will look into adding tilemap cels in a future PR if it is requested enough or if someone
				//	else wants to add it in.  You can see how I do it in my MonoGame.Aseprite library for reference if
				//	needed.
			CheckCelType:
				if(cel is AsepriteLinkedCel linkedCel)
				{
					cel = linkedCel.Cel;
					goto CheckCelType;
				}

				if(cel is AsepriteImageCel imageCel)
				{
					BlendCel(backdrop: result,
							 source: imageCel.Pixels,
							 blendMode: imageCel.Layer.BlendMode,
							 celX: imageCel.Position.X,
							 celY: imageCel.Position.Y,
							 celWidth: imageCel.Width,
							 celOpacity: imageCel.Opacity,
							 layerOpacity: imageCel.Layer.Opacity);
				}
			}

			return result;
		}

		/// <summary>
		/// Translates the data in this frame into a sprite.
		/// </summary>
		/// <param name="onlyVisibleLayers">
		/// Indicates whether only cels that are on visible layers should be included when flattening this frame.
		/// </param>
		/// <param name="includeBackgroundLayer">
		/// Indicates whether the cel on the layer marked as the background layer in Aseprite should be included when
		/// flattening this frame.
		/// </param>
		/// <returns>
		/// A new instance of the <see cref="Sprite"/> class initialized by the image data in this frame.
		/// </returns>
		public Sprite ToSprite(bool onlyVisibleLayers = true, bool includeBackgroundLayer = false)
		{
			Color[] pixels = FlattenFrame(onlyVisibleLayers, includeBackgroundLayer);
			Texture2D texture = new Texture2D(Core.GraphicsDevice, Width, Height);
			texture.SetData<Color>(pixels);
			return new Sprite(texture);
		}

		private void BlendCel(Color[] backdrop, Color[] source, AsepriteBlendMode blendMode, int celX, int celY, int celWidth, int celOpacity, int layerOpacity)
		{
			for (int i = 0; i < source.Length; i++)
			{
				int x = (i % celWidth) + celX;
				int y = (i / celWidth) + celY;
				int index = y * Width + x;

				//  Sometimes a cel can have a negative x and/or y value.  This is caused by selecting an area in within
				//  Aseprite and then moving a portion of the selected pixels outside the canvas. We don't care about 
				//	these pixels, so if the index is outside the range of the array to store them in, then we'll just 
				//	ignore them.
				if (index < 0 || index >= backdrop.Length) { continue; }

				Color b = backdrop[index];
				Color s = source[i];
				byte opacity = AsepriteColorUtils.MUL_UN8(celOpacity, layerOpacity);
				backdrop[index] = AsepriteColorUtils.Blend(blendMode, b, s, opacity);
			}
		}
	}
}