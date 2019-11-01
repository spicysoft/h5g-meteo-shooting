using Unity.Entities;
using Unity.Tiny.Core;
using Unity.Tiny.Core2D;
using Unity.Tiny.UILayout;

namespace Meteo
{
	public class CanvasFitSystem : ComponentSystem
	{
		protected override void OnUpdate()
		{
			Entities.ForEach( ( ref CanvasTag info, ref UICanvas canvas ) => {
				if( !info.Initialized ) {
					// ディスプレイ情報.
					var displayInfo = World.TinyEnvironment().GetConfigData<DisplayInfo>();
					float frameW = displayInfo.frameWidth;
					float frameH = (float)displayInfo.frameHeight;

					// キャンバス情報.
					float matchval = 1f;
					if( frameH >= frameW )
						matchval = 0;

					canvas.matchWidthOrHeight = matchval;

					info.Initialized = true;
				}
			} );
		}
	}
}
