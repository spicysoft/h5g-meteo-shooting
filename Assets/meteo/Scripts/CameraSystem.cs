using Unity.Entities;
using Unity.Mathematics;
using Unity.Tiny.Core;
using Unity.Tiny.Core2D;
using Unity.Tiny.Debugging;

namespace Meteo
{
	public class CameraSystem : ComponentSystem
	{
		protected override void OnUpdate()
		{
			float3 pcPos = float3.zero;

			Entities.ForEach( ( ref PlayerInfo player, ref Translation trans ) => {
				pcPos = trans.Value;
			} );


			Entities.ForEach( ( ref CameraInfo info, ref Camera2D camera, ref Translation trans ) => {

				if( !info.Initialized ) {

					// ディスプレイ情報.
					var displayInfo = World.TinyEnvironment().GetConfigData<DisplayInfo>();
					float frameW = displayInfo.frameWidth;
					float frameH = (float)displayInfo.frameHeight;
					float frameAsp = frameH / frameW;

					// カメラ情報.
					float rectW = camera.rect.width;
					float rectH = camera.rect.height;
					float rectAsp = rectH / rectW;

					camera.halfVerticalSize = 800f * frameAsp / rectAsp;

					info.Initialized = true;
					return;
				}

				if( pcPos.x < -400f )
					pcPos.x = -400f;
				else if( pcPos.x > 400f )
					pcPos.x = 400f;
				if( pcPos.y < -400f )
					pcPos.y = -400f;
				else if( pcPos.y > 400f )
					pcPos.y = 400f;

				pcPos.y -= 150f;		// 少し下に.
				trans.Value = pcPos;
			} );
		}
	}
}
