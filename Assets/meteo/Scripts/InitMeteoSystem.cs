using Unity.Entities;
using Unity.Mathematics;
using Unity.Tiny.Core2D;
using Unity.Tiny.Debugging;

namespace Meteo
{
	[UpdateAfter(typeof(MeteoGenSystem))]
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
			Entities.ForEach( ( ref PlayerInfo player, ref Translation trans ) => {
				playerPos = trans.Value;
			} );

			// ジェネレート数
			int genCnt = 0;
			Entities.ForEach( ( ref MeteoGenInfo gen ) => {
				genCnt = gen.GeneratedCnt;
			} );

			// 分裂あるか?
			bool reqSplit = false;
			float3 splitPos = float3.zero;
			float2 splitDir = float2.zero;
			Entities.ForEach( ( ref MeteoInfo meteo, ref Translation trans ) => {
				if( !meteo.IsActive || !meteo.Initialized )
					return;

				if( meteo.ReqSplit ) {
					meteo.ReqSplit = false;
					reqSplit = true;
					splitPos = trans.Value;
					splitPos.y += 1f;
					splitDir = meteo.MoveDir;
				}
			} );

			// 分裂情報とっておく.
			if( reqSplit ) {
				Entities.ForEach( ( ref InitMeteoInfo info ) => {
					info.InitSplit = true;
					info.SplitPos = splitPos;
					info.SplitDir = splitDir;
				} );
			}

			// 改めて、分裂あるか?
			Entities.ForEach( ( ref InitMeteoInfo info ) => {
				if( info.InitSplit ) {
					reqSplit = true;
					splitPos = info.SplitPos;
					splitDir = info.SplitDir;
				}
			} );



			bool isInit = false;
			bool isSplitSet = false;
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

					if( reqSplit ) {
						isSplitSet = true;
						reqSplit = false;
						meteo.Level = 2;
						meteo.Life = 10;
						meteo.Radius = 140;
						trans.Value = splitPos;
						meteo.ZrotSpd = math.radians( _random.NextFloat( -60f, 60f ) );
						meteo.BaseSpeed = _random.NextFloat( 50f, 150f );
						meteo.MoveDir = -splitDir;
						opt.size.x = meteo.Radius * 2f;
						opt.size.y = meteo.Radius * 2f;
						return;
					}

					// レベル.
					meteo.Level = _random.NextInt( 4 ); // 0 ~ 3.
					//meteo.Level = 3;
					switch( meteo.Level ) {
					case 0:
						meteo.Life = 3;
						meteo.Radius = 80;
						break;
					case 1:
						meteo.Life = 6;
						meteo.Radius = 110;
						break;
					case 2:
						meteo.Life = 10;
						meteo.Radius = 140;
						break;
					case 3:
						meteo.Life = 15;
						meteo.Radius = 170;
						break;
					}

					// ポジション.
					float innerRange = 300f;
					float range = GameMngrSystem.BorderUp - 100f - innerRange;
					// todo 長方形対応.
					float x = _random.NextFloat( -range, range );
					if( x > 0 )
						x += innerRange;
					else if( x < 0 )
						x -= innerRange;
					float y = _random.NextFloat( -range, range );
					if( y > 0 )
						y += innerRange;
					else if( y < 0 )
						y -= innerRange;
					float3 pos = new float3( x, y, 0 );

					// プレイヤーと重ならないように.
					if( isHitPlayer( pos, meteo.Radius * 1.2f, playerPos) ) {
						pos.x *= -1f;
						pos.y *= -1f;
					}
					trans.Value = pos;

					meteo.ZrotSpd = math.radians( _random.NextFloat( -60f, 60f ) );

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

			if( isSplitSet ) {
				Entities.ForEach( ( ref InitMeteoInfo info ) => {
					if( info.InitSplit ) {
						info.InitSplit = false;
					}
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
