using Unity.Entities;
using Unity.Mathematics;
using Unity.Tiny.Core2D;
using Unity.Tiny.Debugging;

namespace Meteo
{
	[UpdateAfter( typeof( PlayerSystem ) )]
	public class BgSystem : ComponentSystem
	{
		protected override void OnUpdate()
		{
			float3 pcPos = float3.zero;
			float3 pcPrePos = float3.zero;

			Entities.ForEach( ( ref PlayerInfo player, ref Translation trans ) => {
				pcPos = trans.Value;
				pcPrePos = player.PrePos;
			} );


			Entities.ForEach( ( ref BgTag bg, ref Translation trans ) => {

				float xspd = ( pcPos.x - pcPrePos.x ) * 0.25f;
				float yspd = ( pcPos.y - pcPrePos.y ) * 0.25f;

				//Debug.LogFormatAlways("xspd {0}", xspd);

				float3 pos = trans.Value;
				pos.x += xspd;
				pos.y += yspd;
				trans.Value = pos;
			} );
		}
	}
}
