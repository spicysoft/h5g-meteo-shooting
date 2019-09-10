using Unity.Entities;
using Unity.Tiny.Core;
using Unity.Tiny.Core2D;
using Unity.Mathematics;
using Unity.Tiny.Scenes;

namespace Meteo
{
	public class PlayerSystem : ComponentSystem
	{
		protected override void OnUpdate()
		{
			var deltaTime = World.TinyEnvironment().frameDeltaTime;
			var moveDirection = float2.zero;
			var moveMagnitude = 0f;

			Entities.ForEach( ( Entity entity, ref Joystick joystick ) => {
				if( joystick.Direction.x != 0f || joystick.Direction.y != 0f ) {
					moveMagnitude = math.min( 1f, math.distance( float2.zero, joystick.Direction ) );
					moveDirection = moveMagnitude * math.normalize( joystick.Direction );
				}
			} );


			bool reqBullet = false;

			Entities.ForEach( ( Entity entity, ref PlayerInfo player, ref Translation trans ) => {
				if( !player.Initialized ) {
					player.Initialized = true;
					return;
				}

				var position = trans.Value;
				position.x += moveDirection.x * 100f * deltaTime;
				position.y += moveDirection.y * 100f * deltaTime;
				trans.Value = position;

				player.Interval += deltaTime;
				if( player.Interval > 5f ) {
					player.Interval = 0;
					reqBullet = true;
				}
			} );

			if( reqBullet ) {

				var env = World.TinyEnvironment();
				SceneReference bulletBase = env.GetConfigData<GameConfig>().PrefabBullet;
				SceneService.LoadSceneAsync( bulletBase );
			}


		}
	}
}
