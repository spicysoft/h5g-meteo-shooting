using Unity.Entities;
using Unity.Tiny.Core;
using Unity.Tiny.Debugging;
using Unity.Tiny.Scenes;
using Unity.Tiny.Text;
using Unity.Tiny.UIControls;

namespace Meteo
{
	// リトライボタン.
	public class BtnRetrySystem : ComponentSystem
	{
		protected override void OnUpdate()
		{
			bool btnOn = false;
			Entities.WithAll<BtnRetryTag>().ForEach( ( Entity entity, ref PointerInteraction pointerInteraction ) => {
				if( pointerInteraction.clicked ) {
					//Debug.LogAlways("btn ret click");
					btnOn = true;
				}
			} );


			if( btnOn ) {
				var env = World.TinyEnvironment();
				SceneService.UnloadAllSceneInstances( env.GetConfigData<GameConfig>().ResultScn );

				Entities.ForEach( ( ref GameMngr mngr ) => {
					mngr.IsPause = false;
					mngr.Mode = GameMngrSystem.MdGame;
					mngr.ModeTimer = 0;
					mngr.GameTimer = 0;
					mngr.Score = 0;
				} );

				// スコア表示.
				Entities.WithAll<TextScoreTag>().ForEach( ( Entity entity ) => {
					EntityManager.SetBufferFromString<TextString>( entity, "0" );
				} );

				// プレイヤー.
				Entities.ForEach( ( Entity entity, ref PlayerInfo player ) => {
					player.Initialized = false;
				} );

				SceneService.UnloadAllSceneInstances( env.GetConfigData<GameConfig>().PrefabMeteo );

			}
		}
	}
}
