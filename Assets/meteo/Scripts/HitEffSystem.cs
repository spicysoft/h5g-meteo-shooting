using Unity.Entities;
using Unity.Tiny.Core;
using Unity.Tiny.Core2D;

namespace Meteo
{
	public class HitEffSystem : ComponentSystem
	{
		protected override void OnUpdate()
		{
			var deltaTime = World.TinyEnvironment().frameDeltaTime;


			Entities.ForEach( ( Entity entity, ref SimpleEffInfo eff, ref Translation trans, ref NonUniformScale scl ) => {
				if( !eff.IsActive )
					return;
				if( !eff.Initialized )
					return;

				eff.Timer += deltaTime;
				if( eff.Timer > 0.5f ) {
					eff.IsActive = false;
					scl.Value.x = 0;
				}
			} );
		}
	}
}
