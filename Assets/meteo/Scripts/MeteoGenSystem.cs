using Unity.Entities;
using Unity.Tiny.Core;
using Unity.Tiny.Debugging;
using Unity.Tiny.Scenes;

namespace Meteo
{
	public class MeteoGenSystem : ComponentSystem
	{
		private const float MinTime = 20f;
		private const float UpdateTime = 8f;
		private const int FirstNum = 10;
		private const int MaxNum = 40;

		protected override void OnUpdate()
		{
			bool IsPause = false;
			Entities.ForEach( ( ref GameMngr mngr ) => {
				IsPause = mngr.IsPause;
			} );
			if( IsPause )
				return;


			bool reqGen = false;

			Entities.ForEach( ( ref MeteoGenInfo gen ) => {
				if( !gen.Initialized ) {
					gen.Initialized = true;
					gen.ReqSplit = false;
					gen.Timer = 0;
					gen.TotalTimer = 0;
					gen.MeteoNum = 0;
					gen.GeneratedCnt = 0;
					gen.MeteoMax = 10;
					return;
				}

				float dt = World.TinyEnvironment().frameDeltaTime;
				gen.TotalTimer += dt;
				gen.MeteoMax = getMeteoMax( gen.TotalTimer );

				// 分裂?
				if( gen.ReqSplit ) {
					gen.ReqSplit = false;
					reqGen = true;
				}
				else {
					gen.Timer -= dt;
					if( gen.Timer < 0 ) {
						gen.Timer = 2f;
						if( gen.MeteoNum < gen.MeteoMax ) {	// 個数制限.
							reqGen = true;
						}
					}
				}
			} );

			if( reqGen ) {
				bool recycled = false;

				Entities.ForEach( ( Entity entity, ref MeteoInfo meteo ) => {
					if( !recycled ) {
						if( !meteo.IsActive ) {
							meteo.IsActive = true;
							meteo.Initialized = false;
							recycled = true;
						}
					}
				} );

				//Debug.LogFormatAlways( "bulcnt {0} recycled {1}", bulCnt, recycled );

				if( !recycled ) {
					var env = World.TinyEnvironment();
					SceneReference meteoBase = env.GetConfigData<GameConfig>().PrefabMeteo;
					SceneService.LoadSceneAsync( meteoBase );
				}
			}
		}

		int getMeteoMax( float time )
		{
			if( time < MinTime )
				return FirstNum;

			float t = time - MinTime;
			int meteoNum = FirstNum + (int)( t / UpdateTime );
			if( meteoNum > MaxNum )
				meteoNum = MaxNum;

			return meteoNum;
		}

	}
}
