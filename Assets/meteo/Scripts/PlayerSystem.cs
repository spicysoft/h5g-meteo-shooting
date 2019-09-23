using Unity.Entities;
using Unity.Tiny.Core;
using Unity.Tiny.Core2D;
using Unity.Mathematics;
using Unity.Tiny.Scenes;
using Unity.Tiny.Debugging;

namespace Meteo
{
	public class PlayerSystem : ComponentSystem
	{
		// 半径.
		public const float PlayerR = 32f;
		public const float PlayerRsq = PlayerR * PlayerR;


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
			var moveDirection = float2.zero;
			var moveMagnitude = 0f;
			bool isInput = false;

			// ジョイスティック.
			Entities.ForEach( ( ref Joystick joystick ) => {
				if( joystick.Direction.x != 0f || joystick.Direction.y != 0f ) {
					moveMagnitude = math.min( 1f, math.distance( float2.zero, joystick.Direction ) );
					if( moveMagnitude > 0.3f ) {
						isInput = true;
						moveDirection = moveMagnitude * math.normalize( joystick.Direction );
					}
				}
			} );

			// 近い隕石.
			float minDist = -1f;
			float3 minPos = float3.zero;
			Entities.ForEach( ( ref MeteoInfo meteo, ref Translation trans ) => {
				if( !meteo.IsActive )
					return;
				if( !meteo.Initialized ) {
					return;
				}

				// todo 射程距離考慮.
				if( minDist < 0 || minDist > meteo.DistSq ) {
					minDist = meteo.DistSq;
					minPos = trans.Value;
				}
			} );


			bool reqBullet = false;
			float3 upVec = new float3(0, 0, 1f);

			Entities.ForEach( ( Entity entity, ref PlayerInfo player, ref Translation trans, ref Rotation rot ) => {
				if( !player.Initialized ) {
					player.Initialized = true;
					trans.Value = float3.zero;
					rot.Value = quaternion.identity;
					return;
				}

				// 移動.
				var position = trans.Value;
				player.PrePos = position;		// 取っておく.
				if( isInput ) {
					float baseSpd = 180f;

					position.x += moveDirection.x * baseSpd * deltaTime;
					if( position.x < GameMngrSystem.BorderLeft + PlayerR )
						position.x = GameMngrSystem.BorderLeft + PlayerR;
					else if( position.x > GameMngrSystem.BorderRight - PlayerR )
						position.x = GameMngrSystem.BorderRight - PlayerR;

					position.y += moveDirection.y * baseSpd * deltaTime;
					if( position.y < GameMngrSystem.BorderLow + PlayerR )
						position.y = GameMngrSystem.BorderLow + PlayerR;
					else if( position.y > GameMngrSystem.BorderUp - PlayerR )
						position.y = GameMngrSystem.BorderUp - PlayerR;

					trans.Value = position;
				}

				// 向き.
				if( minDist > 0 ) {
					float3 dir = minPos - position;
					//float3 dirN = math.normalize( dir );

					float za = math.atan2( dir.y, dir.x );
					za -= math.radians( 90f );
					rot.Value = quaternion.RotateZ( za );
				}
				else if( isInput ) {
					float za = math.atan2( moveDirection.y, moveDirection.x );
					za -= math.radians( 90f );
					rot.Value = quaternion.RotateZ( za );
				}


				// 弾.
				player.Interval += deltaTime;
				if( player.Interval > 0.5f ) {
					player.Interval = 0;
					reqBullet = true;
				}
			} );

			if( reqBullet ) {
				bool recycled = false;
				int bulCnt = 0;

				Entities.ForEach( ( Entity entity, ref BulletInfo bullet ) => {
					bulCnt++;
					if( !recycled ) {
						if( !bullet.IsActive ) {
							bullet.IsActive = true;
							bullet.Initialized = false;
							recycled = true;
						}
					}
				} );

				//Debug.LogFormatAlways( "bulcnt {0} recycled {1}", bulCnt, recycled );

				if( !recycled ) {
					var env = World.TinyEnvironment();
					SceneReference bulletBase = env.GetConfigData<GameConfig>().PrefabBullet;
					SceneService.LoadSceneAsync( bulletBase );
				}
			}

		}
	}
}
