using Unity.Entities;
using Unity.Mathematics;
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


			Entities.ForEach( ( ref Camera2D cam, ref Translation trans ) => {
				if( pcPos.x < -400f )
					pcPos.x = -400f;
				else if( pcPos.x > 400f )
					pcPos.x = 400f;
				if( pcPos.y < -400f )
					pcPos.y = -400f;
				else if( pcPos.y > 400f )
					pcPos.y = 400f;

				pcPos.y -= 100f;
				trans.Value = pcPos;
			} );
		}
	}
}
