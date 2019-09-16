using Unity.Entities;
using Unity.Tiny.Core;
using Unity.Tiny.Scenes;

namespace Meteo
{
	public class MeteoGenSystem : ComponentSystem
	{
		protected override void OnUpdate()
		{
			bool reqGen = false;

			Entities.ForEach( ( ref MeteoGenInfo gen ) => {
				gen.Timer += World.TinyEnvironment().frameDeltaTime;
				if( gen.Timer > 2f ) {
					gen.Timer = 0;
					if( gen.MeteoNum < 10 ) {
						reqGen = true;
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
	}
}
