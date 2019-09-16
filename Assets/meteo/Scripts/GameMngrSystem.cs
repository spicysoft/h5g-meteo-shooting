using Unity.Entities;
using Unity.Tiny.Debugging;
using Unity.Collections;
using Unity.Tiny.Core;
using Unity.Tiny.Input;
using Unity.Tiny.Scenes;
using Unity.Tiny.Text;

namespace Meteo
{
	public class GameMngrSystem : ComponentSystem
	{
		// 境界.
		public const float BorderUp = 600f;
		public const float BorderLow = -600f;
		public const float BorderLeft = -600f;
		public const float BorderRight = 600f;



		public const float GameTimeLimit = 190f;        // ゲーム時間.
		public const int MdTitle = 0;
		public const int MdGame = 1;
		public const int MdGameOver = 2;
		public const int MdResult = 3;


		protected override void OnUpdate()
		{
			int score = 0;
			bool reqGameOver = false;
			bool reqResult = false;

			Entities.ForEach( ( ref GameMngr mngr ) => {

				float dt = World.TinyEnvironment().frameDeltaTime;
#if false
				switch( mngr.Mode ) {
				case MdTitle:
					mngr.Mode = MdGame;
					break;
				case MdGame:
					if( mngr.ReqGameOver ) {
						mngr.ReqGameOver = false;
						reqGameOver = true;
						mngr.Mode = MdGameOver;
						mngr.ModeTimer = 0;
					}
					break;
				case MdGameOver:
					mngr.ModeTimer += dt;
					if( mngr.ModeTimer > 1.5f ) {
						mngr.Mode = MdResult;
						reqResult = true;
					}
					break;

				}
#endif

				if( mngr.IsPause ) {
					//isPause = true;
					return;
				}


				score = mngr.Score;

				// タイマー.
				mngr.GameTimer += dt;
#if false
				timer = mngr.GameTimer;
				if( timer >= GameTimeLimit ) {
					mngr.IsPause = true;
				}
#endif
			} );


#if false
			// タイマー表示.
			if( !isPause ) {
				Entities.WithAll<TextTimerTag>().ForEach( ( Entity entity ) => {
					int t = (int)( GameTimeLimit - timer );
					EntityManager.SetBufferFromString<TextString>( entity, t.ToString() );
				} );
			}

			if( reqResult ) {
				// ゲームオーバーシーンアンロード.
				SceneReference panelBase = new SceneReference();
				panelBase = World.TinyEnvironment().GetConfigData<GameConfig>().GameOverScn;
				SceneService.UnloadAllSceneInstances( panelBase );
				// リザルト表示.
				//SceneReference panelBase = new SceneReference();
				panelBase = World.TinyEnvironment().GetConfigData<GameConfig>().ResultScn;
				SceneService.LoadSceneAsync( panelBase );
			}
			else if( reqGameOver ) {
				// ゲームオーバー表示.
				SceneReference panelBase = new SceneReference();
				panelBase = World.TinyEnvironment().GetConfigData<GameConfig>().GameOverScn;
				SceneService.LoadSceneAsync( panelBase );

				// ブロック削除.
				var env = World.TinyEnvironment();
				SceneService.UnloadAllSceneInstances( env.GetConfigData<GameConfig>().PrefabBlock );
				SceneService.UnloadAllSceneInstances( env.GetConfigData<GameConfig>().PrefabBlockStay );
				SceneService.UnloadAllSceneInstances( env.GetConfigData<GameConfig>().PrefabStar );

			}
#endif

		}

	}
}
