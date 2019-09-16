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
			Entities.ForEach( ( ref PlayerInfo player, ref Translation trans, ref Rotation rot ) => {
				playerPos = trans.Value;
			} );

			// ジェネレート数.
			int genCnt = 0;
			Entities.ForEach( ( ref MeteoGenInfo gen ) => {
				genCnt = gen.GeneratedCnt;
			} );

			bool isInit = false;
			Entities.ForEach( ( ref MeteoInfo meteo, ref Translation trans, ref NonUniformScale scl, ref Sprite2DRendererOptions opt ) => {
				if( !meteo.IsActive )
					return;
				if( !meteo.Initialized ) {
					isInit = true;

					meteo.Initialized = true;
					meteo.IsHit = false;
					meteo.ReqHitEff = false;
					scl.Value.x = 1f;


					// ID.
					meteo.UniId = ++genCnt;

					// レベル.
					meteo.Level = _random.NextInt( 4 ); // 0 ~ 4.
					switch( meteo.Level ) {
					case 0:
						meteo.Life = 3;
						meteo.Radius = 50;
						break;
					case 1:
						meteo.Life = 6;
						meteo.Radius = 70;
						break;
					case 2:
						meteo.Life = 10;
						meteo.Radius = 100;
						break;
					case 3:
						meteo.Life = 15;
						meteo.Radius = 150;
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
					meteo.BaseSpeed = _random.NextFloat( 50f, 200f );
					// 移動ベクトル.
					float2 dir = _random.NextFloat2();
					meteo.MoveDir = math.normalize( dir );

					opt.size.x = meteo.Radius * 2f;
					opt.size.y = meteo.Radius * 2f;
				}
			} );


			if( isInit ) {
				// ジェネレート数更新.
				Entities.ForEach( ( ref MeteoGenInfo gen ) => {
					gen.GeneratedCnt = genCnt;
				} );
			}

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
