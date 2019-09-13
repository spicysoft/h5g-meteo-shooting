using Unity.Entities;
using Unity.Mathematics;
using Unity.Tiny.Core;
using Unity.Tiny.Core2D;
using Unity.Tiny.Debugging;
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
//					meteo.Life = 13;
//					meteo.Radius = 100;
//					meteo.Initialized = true;
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

				if( pos.y > GameMngrSystem.BorderUp ) {
					if( meteo.MoveDir.y > 0 )
						meteo.MoveDir.y *= -1f;
				}
				else if( pos.y < GameMngrSystem.BorderLow ) {
					if( meteo.MoveDir.y < 0 )
						meteo.MoveDir.y *= -1f;
				}
				if( pos.x > GameMngrSystem.BorderRight ) {
					if( meteo.MoveDir.x > 0 )
						meteo.MoveDir.x *= -1;
				}
				else if( pos.x < GameMngrSystem.BorderLeft ) {
					if( meteo.MoveDir.x < 0 )
						meteo.MoveDir.x *= -1;
				}

				// プレイヤーとの距離の２乗.
				float distsq = math.distancesq( trans.Value, playerPos );
				meteo.DistSq = distsq;

				float rr = (PlayerSystem.PlayerR + meteo.Radius) * ( PlayerSystem.PlayerR + meteo.Radius );

				if( distsq < rr ) {
					//Debug.LogFormatAlways("pl hit {0} {1}", distsq, meteo.RadiusSq);
				}

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
