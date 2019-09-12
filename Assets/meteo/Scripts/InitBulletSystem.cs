using Unity.Entities;
using Unity.Mathematics;
using Unity.Tiny.Core2D;

namespace Meteo
{
	// 弾初期化.
	public class InitBulletSystem : ComponentSystem
	{
		protected override void OnUpdate()
		{
			float3 playerPos = float3.zero;
			quaternion playerRot = quaternion.identity;

			Entities.ForEach( ( Entity entity, ref PlayerInfo player, ref Translation trans, ref Rotation rot ) => {
				playerPos = trans.Value;
				playerRot = rot.Value;
			} );

			Entities.ForEach( ( Entity entity, ref BulletInfo bullet, ref Translation trans, ref Rotation rot, ref NonUniformScale scl ) => {
				if( !bullet.IsActive )
					return;

				if( !bullet.Initialized ) {
					bullet.Initialized = true;
					bullet.BaseSpeed = 1200f;

					trans.Value = playerPos;
					rot.Value = playerRot;
					float3 dir = math.rotate( rot.Value, new float3( 0, 1f, 0 ) );
					bullet.MoveDir.x = dir.x;
					bullet.MoveDir.y = dir.y;

					bullet.Timer = 0;
					scl.Value.x = 1f;
					return;
				}
			} );

		}
	}
}
