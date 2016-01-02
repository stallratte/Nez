﻿using System;
using Microsoft.Xna.Framework;
using Nez.PhysicsShapes;


namespace Nez
{
	// TODO: should colliders have a scale property as well or is it enough to set width/height?
	public abstract class Collider
	{
		public Entity entity;
		public Shape shape;

		/// <summary>
		/// position is added to entity.position to get the final position for the collider
		/// </summary>
		protected Vector2 _localPosition;
		public Vector2 localPosition
		{
			get { return _localPosition; }
			set
			{
				if( _localPosition != value )
				{
					unregisterColliderWithPhysicsSystem();
					_localPosition = value;
					_areBoundsDirty = true;
					registerColliderWithPhysicsSystem();
				}
			}
		}

		protected Vector2 _origin;
		public Vector2 origin
		{
			get { return _origin; }
			set
			{
				if( _origin != value )
				{
					unregisterColliderWithPhysicsSystem();
					_origin = value;
					_areBoundsDirty = true;
					registerColliderWithPhysicsSystem();
				}
			}
		}

		/// <summary>
		/// helper property for setting the origin in normalized fashion (0-1 for x and y)
		/// </summary>
		/// <value>The origin normalized.</value>
		public Vector2 originNormalized
		{
			get { return new Vector2( _origin.X / bounds.Width, _origin.Y / bounds.Height ); }
			set { origin = new Vector2( value.X * bounds.Width, value.Y * bounds.Height ); }
		}

		/// <summary>
		/// represents the absolute position to this Collider. It is entity.position + localPosition - origin.
		/// </summary>
		/// <value>The absolute position.</value>
		public Vector2 absolutePosition
		{
			get { return entity.position + _localPosition - _origin; }
		}

		/// <summary>
		/// if this collider is a trigger it will not cause collisions but it will still trigger events
		/// </summary>
		public bool isTrigger;

		/// <summary>
		/// physicsLayer can be used as a filter when dealing with collisions. The Flags class has methods to assist with bitmasks.
		/// </summary>
		public int physicsLayer = 1 << 0;

		public virtual Rectangle bounds
		{
			get
			{
				if( _areBoundsDirty )
				{
					shape.recalculateBounds( this );
					_areBoundsDirty = false;
				}

				return shape.bounds;
			}
		}

		protected bool _isParentEntityAddedToScene;
		internal bool _areBoundsDirty = true;


		public Collider()
		{}


		/// <summary>
		/// Called when the parent entity is added to a scene
		/// </summary>
		public virtual void onEntityAddedToScene()
		{
			if( shape == null )
			{
				// we only deal with boxes and circles here
				Assert.isTrue( this is BoxCollider || this is CircleCollider, "Only box and circle colliders can be created automatically" );

				var renderable = entity.getComponent<RenderableComponent>();
				Debug.warnIf( renderable == null, "Collider has no shape and no RenderableComponent. Can't figure out how to size it." );
				if( renderable != null )
				{
					var renderableBounds = renderable.bounds;

					var width = renderableBounds.Width;
					var height = renderableBounds.Height;

					// circle colliders need special care with the origin
					if( this is CircleCollider )
					{
						var circle = this as CircleCollider;
						circle.shape = new Circle( width * 0.5f );
						circle.radius = width * 0.5f;

						// fetch the Renderable's center, transfer it to local coordinates and use that as the origin of our collider
						var center = renderableBounds.Center;
						var localCenter = center.ToVector2() - entity.position;
						origin = localCenter;
					}
					else
					{
						var box = this as BoxCollider;
						box.shape = new Box( width, height );
						box.width = width;
						box.height = height;
						originNormalized = renderable.originNormalized;
					}

					shape.position = entity.position;
				}
			}
			_isParentEntityAddedToScene = true;
			registerColliderWithPhysicsSystem();
		}


		/// <summary>
		/// Called when the parent entity is removed from a scene
		/// </summary>
		public virtual void onEntityRemovedFromScene()
		{
			unregisterColliderWithPhysicsSystem();
			_isParentEntityAddedToScene = false;
		}


		public virtual void onEntityPositionChanged()
		{
			_areBoundsDirty = true;
		}


		/// <summary>
		/// the parent Entity will call this at various times (when added to a scene, enabled, etc)
		/// </summary>
		public virtual void registerColliderWithPhysicsSystem()
		{
			// entity could be null if proper such as origin are changed before we are added to an Entity
			if( _isParentEntityAddedToScene )
				Physics.addCollider( this );
		}


		/// <summary>
		/// the parent Entity will call this at various times (when removed from a scene, disabled, etc)
		/// </summary>
		public virtual void unregisterColliderWithPhysicsSystem()
		{
			if( _isParentEntityAddedToScene )
				Physics.removeCollider( this, true );
		}


		/// <summary>
		/// checks to see if this Collider collides with collider. If it does, true will be returned and result will be populated
		/// with collision data
		/// </summary>
		/// <returns><c>true</c>, if with was collidesed, <c>false</c> otherwise.</returns>
		/// <param name="collider">Collider.</param>
		/// <param name="result">Result.</param>
		public bool collidesWith( Collider collider, out ShapeCollisionResult result )
		{
			return shape.collidesWithShape( collider.shape, out result );
		}


		/// <summary>
		/// checks to see if this Collider with motion applied (delta movement vector) collides with collider. If it does, true will be
		/// returned and result will be populated.
		/// with collision data
		/// </summary>
		/// <returns><c>true</c>, if with was collidesed, <c>false</c> otherwise.</returns>
		/// <param name="collider">Collider.</param>
		/// <param name="motion">Motion.</param>
		/// <param name="result">Result.</param>
		public bool collidesWith( Collider collider, Vector2 motion, out ShapeCollisionResult result )
		{
			// alter the shapes position so that it is in the place it would be after movement so we can check for overlaps
			var oldPosition = shape.position;
			shape.position = absolutePosition + motion;

			var didCollide = shape.collidesWithShape( collider.shape, out result );
			
			// return the shapes position to where it was before the check
			shape.position = oldPosition;

			return didCollide;
		}


		public virtual void debugRender( Graphics graphics )
		{
			graphics.spriteBatch.drawHollowRect( bounds, Color.IndianRed );
		}

	}
}

