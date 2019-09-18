using Unity.Entities;
using Unity.Tiny.Core;
using Unity.Tiny.Core2D;

namespace Meteo
{
	public class ExplEffSystem : ComponentSystem
	{
		protected override void OnUpdate()
		{
			var deltaTime = World.TinyEnvironment().frameDeltaTime;

			Entities.ForEach( ( Entity entity, ref ExplEffInfo eff, ref Translation trans, ref NonUniformScale scl ) => {
				if( !eff.IsActive || !eff.Initialized )
					return;

				eff.Timer += deltaTime;
				if( eff.Timer > 1.0f ) {
					eff.IsActive = false;
					scl.Value.x = 0;
				}
			} );
		}
	}
}
