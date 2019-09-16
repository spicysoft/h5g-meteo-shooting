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
			Entities.ForEach( ( ref PlayerInfo player, ref Translation trans ) => {
				playerPos = trans.Value;
			} );


			int meteoNum = 0;
			bool reqHitEff = false;
			//float3 hitEffPos = float3.zero;

			Entities.ForEach( ( ref MeteoInfo meteo, ref Translation trans, ref NonUniformScale scl ) => {
				if( !meteo.IsActive || !meteo.Initialized )
					return;

				++meteoNum;

				if( meteo.IsHit ) {
					meteo.Life -= 1;
					meteo.IsHit = false;

					if( meteo.Life == 0 ) {
						// 消す.
						meteo.IsActive = false;
						scl.Value.x = 0;
					}
					else {
						meteo.ReqHitEff = true;
						reqHitEff = true;
						meteo.IsStop = true;
						meteo.Timer = 0;
					}
					return;
				}

				if( meteo.IsStop ) {
					meteo.Timer += World.TinyEnvironment().frameDeltaTime;
					if( meteo.Timer > 0.1f ) {
						meteo.IsStop = false;
					}
					return;
				}

				// 他の隕石とのあたり.
				float2 newDir = float2.zero;
				if( HitCheck( ref meteo, trans.Value, out newDir ) ) {
					meteo.MoveDir = newDir;
				}


				// 移動.
				var newPos = trans.Value;
				var spd = meteo.MoveDir * meteo.BaseSpeed * deltaTime;
				newPos.x += spd.x;
				newPos.y += spd.y;
				//trans.Value = newPos;

				if( newPos.y > GameMngrSystem.BorderUp ) {
					if( meteo.MoveDir.y > 0 )
						meteo.MoveDir.y *= -1f;
				}
				else if( newPos.y < GameMngrSystem.BorderLow ) {
					if( meteo.MoveDir.y < 0 )
						meteo.MoveDir.y *= -1f;
				}
				if( newPos.x > GameMngrSystem.BorderRight ) {
					if( meteo.MoveDir.x > 0 )
						meteo.MoveDir.x *= -1;
				}
				else if( newPos.x < GameMngrSystem.BorderLeft ) {
					if( meteo.MoveDir.x < 0 )
						meteo.MoveDir.x *= -1;
				}

				trans.Value = newPos;

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


			// 今の隕石の個数更新.
			Entities.ForEach( ( ref MeteoGenInfo gen ) => {
				gen.MeteoNum = meteoNum;
			} );
		}

		bool HitCheck( ref MeteoInfo meteo, float3 newPos, out float2 newDir )
		{
			int myId = meteo.UniId;
			float myR = meteo.Radius;
			float3 myPos = newPos;
			float2 movDir = meteo.MoveDir;
		
			bool isHit = false;
			Entities.ForEach( ( ref MeteoInfo other, ref Translation otherTrans ) => {
				if( !other.IsActive || !other.Initialized )
					return;
				if( myId == other.UniId )
					return;

				// 距離の２乗.
				//float3 dv = myPos - otherTrans.Value;
				//float distsq = math.lengthsq( dv );
				float distsq = math.distancesq( myPos, otherTrans.Value );
				float rr = ( myR + other.Radius ) * ( myR + other.Radius );

				if( distsq < rr ) {
					isHit = true;
					float2 dir = new float2( myPos.x - otherTrans.Value.x, myPos.y - otherTrans.Value.y );
					dir = math.normalize( dir );

					movDir += dir;
					movDir = math.normalize( movDir );
				}
			} );

			if( isHit ) {
				newDir = movDir;
				return true;
			}

			newDir = float2.zero;
			return false;
		}
	}
}
