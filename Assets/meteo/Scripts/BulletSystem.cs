using Unity.Entities;
using Unity.Tiny.Core;
using Unity.Tiny.Core2D;
using Unity.Tiny.Debugging;

namespace Meteo
{
	public class BulletSystem : ComponentSystem
	{
		protected override void OnUpdate()
		{
			var deltaTime = World.TinyEnvironment().frameDeltaTime;

			Entity delEnt = Entity.Null;

			Entities.ForEach( ( Entity entity, ref BulletInfo bullet, ref Translation trans ) => {
				if( !bullet.IsActive )
					return;
				if( !bullet.Initialized ) {
					return;
				}

				var pos = trans.Value;
				var spd = bullet.MoveDir * bullet.BaseSpeed * deltaTime;
				pos.x += spd.x;
				pos.y += spd.y;
				trans.Value = pos;

				if( pos.y > 300f ) {
					bullet.IsActive = false;
					//EntityManager.AddComponent( entity, typeof(Disabled) );
					delEnt = entity;
				}


				//Debug.LogFormatAlways( "pos {0} {1}", pos.x, pos.y );
			} );

			if( delEnt != Entity.Null ) {
				EntityManager.AddComponent( delEnt, typeof( Disabled ) );
			}

		}
	}
}
