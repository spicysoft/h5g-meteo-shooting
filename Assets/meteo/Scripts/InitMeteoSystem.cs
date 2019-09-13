using Unity.Entities;
using Unity.Mathematics;
using Unity.Tiny.Core2D;


namespace Meteo
{
	public class InitMeteoSystem : ComponentSystem
	{
		Random _random;

		protected override void OnCreate()
		{
			_random = new Random();
			_random.InitState(123);
		}

		protected override void OnUpdate()
		{
			float3 playerPos = float3.zero;

			// プレイヤーの位置.
			Entities.ForEach( ( Entity entity, ref PlayerInfo player, ref Translation trans, ref Rotation rot ) => {
				playerPos = trans.Value;
			} );


			Entities.ForEach( ( Entity entity, ref MeteoInfo meteo, ref Translation trans, ref NonUniformScale scl, ref Sprite2DRendererOptions opt ) => {
				if( !meteo.IsActive )
					return;
				if( !meteo.Initialized ) {
					meteo.Initialized = true;

					// レベル.
					meteo.Level = _random.NextInt( 4 ); // 0 ~ 4.
					switch( meteo.Level ) {
					case 0:
						meteo.Life = 3;
						meteo.Radius = 80;
						break;
					case 1:
						meteo.Life = 6;
						meteo.Radius = 120;
						break;
					case 2:
						meteo.Life = 10;
						meteo.Radius = 200;
						break;
					case 3:
						meteo.Life = 15;
						meteo.Radius = 300;
						break;
					}

					// ポジション.
					float innerRange = 200f;
					float range = GameMngrSystem.BorderUp - 100f - innerRange;
					float x = _random.NextFloat( -range, range ) + innerRange;
					float y = _random.NextFloat( -range, range ) + innerRange;
					float3 pos = new float3( x, y, 0 );
					// プレイヤーと重ならないように.
					if( isHitPlayer( pos, meteo.Radius, playerPos) ) {
						pos.x *= -1f;
						pos.y *= -1f;
					}
					trans.Value = pos;

					// スピード.
					meteo.BaseSpeed = _random.NextFloat( 50f, 500f );
					// 移動ベクトル.
					float2 dir = _random.NextFloat2();
					meteo.MoveDir = math.normalize( dir );

					opt.size.x = meteo.Radius;
					opt.size.y = meteo.Radius;
				}
			} );
		}

		// プレイヤーと重なっているか?
		bool isHitPlayer( float3 pos, float r, float3 pcPos )
		{
			float distsq = math.distancesq( pos, pcPos );
			float limsq = ( r + PlayerSystem.PlayerR ) * ( r + PlayerSystem.PlayerR );

			if( distsq <= limsq )
				return true;

			return false;
		}
	}
}
