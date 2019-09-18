using Unity.Entities;
using Unity.Mathematics;
using Unity.Tiny.Core2D;
using Unity.Tiny.Debugging;

namespace Meteo
{
	public class InitExplEffSystem : ComponentSystem
	{
		protected override void OnUpdate()
		{
			Entities.ForEach( ( Entity entity, ref ExplEffInfo eff, ref Translation trans, ref NonUniformScale scl, ref Sprite2DSequencePlayer seq ) => {
				if( !eff.IsActive )
					return;

				if( !eff.Initialized ) {
					bool bFound = false;
					float3 explPos = float3.zero;
					//int cnt = 0;
					// 爆発する隕石探す.
					Entities.ForEach( ( ref MeteoInfo meteo, ref Translation meteoTrans ) => {
						//if( !meteo.IsActive || !meteo.Initialized )
						//	return;
						//++cnt;
						//Debug.LogFormatAlways( "meteo search {0}", cnt );
						if( meteo.ReqExpl ) {
							Debug.LogFormatAlways( "req expl {0}", meteoTrans.Value );
							bFound = true;
							explPos = meteoTrans.Value;
							meteo.ReqExpl = false;
						}
					} );

					if( bFound ) {
						trans.Value = explPos;
					}
					scl.Value.x = 1f;

					eff.Timer = 0;
					eff.Initialized = true;

					seq.time = 0;
				}
			} );
		}
	}
}
