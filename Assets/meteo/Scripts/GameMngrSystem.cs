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
		public const float BorderUp = 800f;
		public const float BorderLow = -800f;
		public const float BorderLeft = -800f;
		public const float BorderRight = 800f;



		public const float GameTimeLimit = 190f;        // ゲーム時間.
		//public const int MdTitle = 0;
		public const int MdGame = 0;
		public const int MdGameOver = 1;
		public const int MdResult = 2;


		protected override void OnUpdate()
		{
			int score = 0;
			bool reqGameOver = false;
			bool reqResult = false;

			Entities.ForEach( ( ref GameMngr mngr ) => {

				float dt = World.TinyEnvironment().frameDeltaTime;

				switch( mngr.Mode ) {
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


				if( mngr.IsPause ) {
					//isPause = true;
					return;
				}

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
#endif
			if( reqResult ) {
#if true
				// ゲームオーバーシーンアンロード.
				SceneReference gameoverScn = World.TinyEnvironment().GetConfigData<GameConfig>().GameOverScn;
				SceneService.UnloadAllSceneInstances( gameoverScn );
				// リザルト表示.
				SceneReference resultScn = World.TinyEnvironment().GetConfigData<GameConfig>().ResultScn;
				SceneService.LoadSceneAsync( resultScn );
#endif
			}
			else if( reqGameOver ) {
				// ゲームオーバー表示.
				SceneReference panelBase = new SceneReference();
				panelBase = World.TinyEnvironment().GetConfigData<GameConfig>().GameOverScn;
				SceneService.LoadSceneAsync( panelBase );
			}


		}

	}
}
