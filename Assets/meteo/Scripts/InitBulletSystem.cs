using Unity.Entities;
using Unity.Mathematics;
using Unity.Tiny.Core2D;

namespace Meteo
{

	public class InitBulletSystem : ComponentSystem
	{
		protected override void OnUpdate()
		{
			float3 playerPos = float3.zero;

			Entities.ForEach( ( Entity entity, ref PlayerInfo player, ref Translation trans ) => {
				playerPos = trans.Value;
			} );

			Entities.ForEach( ( Entity entity, ref BulletInfo bullet, ref Translation trans ) => {
				if( !bullet.IsActive )
					return;

				if( !bullet.Initialized ) {
					bullet.Initialized = true;
					bullet.BaseSpeed = 500f;
					bullet.MoveDir.x = 0;
					bullet.MoveDir.y = 1f;
					trans.Value = playerPos;
					return;
				}
			} );

		}
	}
}
