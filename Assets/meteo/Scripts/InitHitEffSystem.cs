using Unity.Entities;
using Unity.Mathematics;
using Unity.Tiny.Core2D;

namespace Meteo
{
	public class InitHitEffSystem : ComponentSystem
	{
		protected override void OnUpdate()
		{


			Entities.ForEach( ( Entity entity, ref SimpleEffInfo eff, ref Translation trans, ref NonUniformScale scl ) => {
				if( !eff.IsActive )
					return;

				if( !eff.Initialized ) {
					bool bFound = false;
					float3 hitPos = float3.zero;
					Entities.ForEach( ( ref MeteoInfo meteo ) => {
						if( !meteo.IsActive || !meteo.Initialized )
							return;
						if( meteo.ReqHitEff ) {
							bFound = true;
							hitPos = meteo.HitPos;
							meteo.ReqHitEff = false;
						}
					} );

					if( bFound ) {
						trans.Value = hitPos;
					}

					eff.Timer = 0;
					eff.Initialized = true;
				}
			} );
		}
	}
}
