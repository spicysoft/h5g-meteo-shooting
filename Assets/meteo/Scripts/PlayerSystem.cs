using Unity.Entities;
using Unity.Tiny.Core;
using Unity.Tiny.Core2D;

namespace Meteo
{
	public class PlayerSystem : ComponentSystem
	{
		protected override void OnUpdate()
		{

			Entities.ForEach( ( Entity entity, ref PlayerInfo player, ref Translation trans ) => {
				if( !player.Initialized ) {
					player.Initialized = true;
					return;
				}

				trans.Value.y += 10f * World.TinyEnvironment().frameDeltaTime;


			} );

		}
	}
}
