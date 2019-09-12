using Unity.Entities;
using Unity.Mathematics;
using Unity.Tiny.Core;
using Unity.Tiny.Core2D;
using Unity.Tiny.Scenes;

namespace Meteo
{
	public class MeteoSystem : ComponentSystem
	{
		protected override void OnUpdate()
		{
			var deltaTime = World.TinyEnvironment().frameDeltaTime;
			float3 playerPos = float3.zero;

			// プレイヤーのポジション.
			Entities.ForEach( ( Entity entity, ref PlayerInfo player, ref Translation trans ) => {
				playerPos = trans.Value;
			} );

			bool reqHitEff = false;
			//float3 hitEffPos = float3.zero;

			Entities.ForEach( ( Entity entity, ref MeteoInfo meteo, ref Translation trans, ref NonUniformScale scl ) => {
				if( !meteo.IsActive )
					return;
				if( !meteo.Initialized ) {
					meteo.Life = 13;
					meteo.Initialized = true;
					return;
				}

				if( meteo.IsHit ) {
					meteo.Life -= 1;
					meteo.IsHit = false;

					if( meteo.Life == 0 ) {
						meteo.IsActive = false;
						scl.Value.x = 0;
					}
					else {
						meteo.ReqHitEff = true;
						reqHitEff = true;
						//hitEffPos = meteo.HitPos;
					}
					return;
				}

				var pos = trans.Value;
				var spd = meteo.MoveDir * meteo.BaseSpeed * deltaTime;
				pos.x += spd.x;
				pos.y += spd.y;
				trans.Value = pos;

				// プレイヤーとの距離の２乗.
				meteo.DistSq = math.distancesq( trans.Value, playerPos );

				//Debug.LogFormatAlways( "pos {0} {1}", pos.x, pos.y );
			} );

			if( reqHitEff ) {
				var env = World.TinyEnvironment();
				SceneReference bulletBase = env.GetConfigData<GameConfig>().PrefabHitEff;
				SceneService.LoadSceneAsync( bulletBase );
			}
		}
	}
}
