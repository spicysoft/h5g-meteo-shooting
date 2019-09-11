using Unity.Entities;
using Unity.Mathematics;
using Unity.Tiny.Core;
using Unity.Tiny.Core2D;

namespace Meteo
{
	public class MeteoSystem : ComponentSystem
	{
		protected override void OnUpdate()
		{
			var deltaTime = World.TinyEnvironment().frameDeltaTime;
			float3 playerPos = float3.zero;

			Entities.ForEach( ( Entity entity, ref PlayerInfo player, ref Translation trans ) => {
				playerPos = trans.Value;
			} );


			Entities.ForEach( ( Entity entity, ref MeteoInfo meteo, ref Translation trans ) => {
				if( !meteo.IsActive )
					return;
				if( !meteo.Initialized ) {
					meteo.Initialized = true;
					return;
				}

				var pos = trans.Value;
				var spd = meteo.MoveDir * meteo.BaseSpeed * deltaTime;
				pos.x += spd.x;
				pos.y += spd.y;
				trans.Value = pos;

				//float3 dv = trans.Value - playerPos;
				//meteo.DistSq = dv.x * dv.x + dv.y * dv.y;
				meteo.DistSq = math.distancesq( trans.Value, playerPos );

				//Debug.LogFormatAlways( "pos {0} {1}", pos.x, pos.y );
			} );


		}
	}
}
