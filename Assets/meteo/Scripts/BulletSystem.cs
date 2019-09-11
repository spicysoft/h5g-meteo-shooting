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

			//Entity delEnt = Entity.Null;

			Entities.ForEach( ( Entity entity, ref BulletInfo bullet, ref Translation trans, ref NonUniformScale scl ) => {
				if( !bullet.IsActive )
					return;
				if( !bullet.Initialized ) {
					return;
				}

				// 移動.
				var pos = trans.Value;
				bullet.PrePos = pos;	// 前フレームのポジションとっておく.
				var spd = bullet.MoveDir * bullet.BaseSpeed * deltaTime;
				pos.x += spd.x;
				pos.y += spd.y;
				trans.Value = pos;

				// 暫定的に時間で.
				bullet.Timer += deltaTime;
				if( bullet.Timer > 0.5f ) {
					bullet.IsActive = false;
					scl.Value.x = 0;	// スケール0にして消す.
					//delEnt = entity;
				}

				//Debug.LogFormatAlways( "pos {0} {1}", pos.x, pos.y );
			} );

			/*if( delEnt != Entity.Null ) {
				// DisableにするとEntityとして拾えないのでやめる.
				//EntityManager.AddComponent( delEnt, typeof( Disabled ) );
			}*/

		}
	}
}
