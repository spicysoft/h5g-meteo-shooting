using Unity.Entities;
using Unity.Mathematics;
using Unity.Tiny.Core;
using Unity.Tiny.Core2D;
using Unity.Tiny.Debugging;
using Unity.Tiny.Scenes;
using Unity.Tiny.Text;

namespace Meteo
{
	public class MeteoSystem : ComponentSystem
	{
		protected override void OnUpdate()
		{
			bool IsPause = false;
			Entities.ForEach( ( ref GameMngr mngr ) => {
				if( mngr.IsPause ) {
					IsPause = true;
				}
			} );
			if( IsPause )
				return;

			var deltaTime = World.TinyEnvironment().frameDeltaTime;
			float3 playerPos = float3.zero;

			// プレイヤーのポジション.
			Entities.ForEach( ( ref PlayerInfo player, ref Translation trans ) => {
				playerPos = trans.Value;
			} );


			int meteoNum = 0;
			bool reqHitEff = false;
			bool reqSplit = false;
			bool reqExpl = false;
			int score = 0;
			//float3 hitEffPos = float3.zero;

			Entities.ForEach( ( ref MeteoInfo meteo, ref Translation trans, ref Rotation rot, ref NonUniformScale scl, ref Sprite2DRendererOptions opt ) => {
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
						// 爆発エフェクト.
						meteo.ReqExpl = true;
						reqExpl = true;
						score += 100;
					}
					else {
						// 分裂.
						if( meteo.Level == 3 ) {
							if( meteo.Life == 10 ) {
								// 分裂後小さくなる.
								meteo.Level = 2;
								meteo.Radius = 140f;
								meteo.ReqSplit = true;
								opt.size.x = meteo.Radius * 2f;
								opt.size.y = meteo.Radius * 2f;
								reqSplit = true;
								// 爆発エフェクト.
								meteo.ReqExpl = true;
								reqExpl = true;
							}
						}

						meteo.ReqHitEff = true;
						reqHitEff = true;
						meteo.IsStop = true;
						meteo.Timer = 0;
					}
					return;
				}

				// 回転.
				quaternion now = rot.Value;
				quaternion zrot = quaternion.RotateZ( meteo.ZrotSpd * deltaTime );
				rot.Value = math.mul( now, zrot );


				if( meteo.IsStop ) {
					meteo.Timer += World.TinyEnvironment().frameDeltaTime;
					if( meteo.Timer > 0.05f ) {
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

				if( newPos.y > GameMngrSystem.BorderUp - meteo.Radius * 0.5f ) {
					if( meteo.MoveDir.y > 0 )
						meteo.MoveDir.y *= -1f;
				}
				else if( newPos.y < GameMngrSystem.BorderLow + meteo.Radius * 0.5f ) {
					if( meteo.MoveDir.y < 0 )
						meteo.MoveDir.y *= -1f;
				}
				if( newPos.x > GameMngrSystem.BorderRight - meteo.Radius * 0.5f ) {
					if( meteo.MoveDir.x > 0 )
						meteo.MoveDir.x *= -1;
				}
				else if( newPos.x < GameMngrSystem.BorderLeft + meteo.Radius * 0.5f ) {
					if( meteo.MoveDir.x < 0 )
						meteo.MoveDir.x *= -1;
				}

				trans.Value = newPos;

				// プレイヤーとの距離の２乗.
				float distsq = math.distancesq( trans.Value, playerPos );
				meteo.DistSq = distsq;

				float rr = (PlayerSystem.PlayerR + meteo.Radius) * ( PlayerSystem.PlayerR + meteo.Radius );

				if( distsq < rr ) {
#if true
					//Debug.LogFormatAlways("pl hit {0} {1}", distsq, meteo.Radius);
					// ゲームオーバー.
					Entities.ForEach( ( ref GameMngr mngr ) => {
						mngr.IsPause = true;
						mngr.ReqGameOver = true;
					} );
#endif
				}

				//Debug.LogFormatAlways( "pos {0} {1}", pos.x, pos.y );
			} );

			if( reqHitEff ) {
				bool recycled = false;
				Entities.ForEach( ( Entity entity, ref HitEffInfo info ) => {
					if( !recycled ) {
						if( !info.IsActive ) {
							info.IsActive = true;
							info.Initialized = false;
							recycled = true;
						}
					}
				} );

				if( !recycled ) {
					var env = World.TinyEnvironment();
					SceneReference meteoBase = env.GetConfigData<GameConfig>().PrefabHitEff;
					SceneService.LoadSceneAsync( meteoBase );
				}
			}

			if( reqExpl ) {
				bool recycled = false;
				Entities.ForEach( ( Entity entity, ref ExplEffInfo info ) => {
					if( !recycled ) {
						if( !info.IsActive ) {
							info.IsActive = true;
							info.Initialized = false;
							recycled = true;
						}
					}
				} );

				if( !recycled ) {
					var env = World.TinyEnvironment();
					SceneReference meteoBase = env.GetConfigData<GameConfig>().PrefabExplEff;
					SceneService.LoadSceneAsync( meteoBase );
				}
			}

			// スコア.
			if( score > 0 ) {
				Entities.ForEach( ( ref GameMngr mngr ) => {
					mngr.Score += score;
					score = mngr.Score;
				} );
				// スコア表示.
				Entities.WithAll<TextScoreTag>().ForEach( ( Entity entity ) => {
					EntityManager.SetBufferFromString<TextString>( entity, score.ToString() );
				} );
			}


			Entities.ForEach( ( ref MeteoGenInfo gen ) => {
				// 今の隕石の個数更新.
				gen.MeteoNum = meteoNum;

				// 分裂リクエスト.
				if( reqSplit ) {
					gen.ReqSplit = true;
				}
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
