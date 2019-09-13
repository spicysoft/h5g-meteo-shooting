using Unity.Entities;
using Unity.Mathematics;
using Unity.Tiny.Core;
using Unity.Tiny.Core2D;
using Unity.Tiny.Debugging;

namespace Meteo
{
	public class BulletSystem : ComponentSystem
	{
		protected override void OnUpdate()
		{
			var deltaTime = World.TinyEnvironment().frameDeltaTime;

			//Entity delEnt = Entity.Null;

			Entities.ForEach( ( Entity entity, ref BulletInfo bullet, ref Translation trans, ref NonUniformScale scl ) => {
				if( !bullet.IsActive )
					return;
				if( !bullet.Initialized ) {
					return;
				}

				// 移動.
				var pos1 = trans.Value;
				bullet.PrePos = pos1;	// 前フレームのポジションとっておく.
				var spd = bullet.MoveDir * bullet.BaseSpeed * deltaTime;
				var pos2 = pos1;
				pos2.x += spd.x;
				pos2.y += spd.y;
				trans.Value = pos2;

				// 暫定的に時間で.
				bullet.Timer += deltaTime;
				if( bullet.Timer > 0.5f ) {
					bullet.IsActive = false;
					scl.Value.x = 0;    // スケール0にして消す.
					return;
				}


				bool isHit = false;
				Entities.ForEach( ( Entity meteoEntity, ref MeteoInfo meteo, ref Translation meteoTrans ) => {
					if( !meteo.IsActive )
						return;
					if( !meteo.Initialized )
						return;

					// 隕石との当たり判定.
					var center = meteoTrans.Value;
					var r = meteo.Radius;
					if( checkColli( pos1, pos2, center, r ) ) {
						// 当たった.
						meteo.IsHit = true;
						meteo.HitPos = pos2;
						isHit = true;
					}
				} );

				if( isHit ) {
					bullet.IsActive = false;
					scl.Value.x = 0;    // スケール0にして消す.
				}

				//Debug.LogFormatAlways( "pos {0} {1}", pos.x, pos.y );
			} );
		}


		bool checkColli( float3 vSt, float3 vEd, float3 center, float r )
		{
			// A:線分の始点、B:線分の終点、P:円の中心、X:PからABに下ろした垂線との交点.
			float2 vAB = new float2( vEd.x - vSt.x, vEd.y - vSt.y );
			float2 vAP = new float2( center.x - vSt.x, center.y - vSt.y );
			float2 vBP = new float2( center.x - vEd.x, center.y - vEd.y );

			float2 vABnorm = math.normalize( vAB );
			// AXの距離.
			float lenAX = inner2d( vAP, vABnorm );

			// 線分ABとPの最短距離
			float shortestDistance;
			if( lenAX < 0 ) {
				// AXが負なら APが円の中心までの最短距離
				shortestDistance = math.length( vAP );
			}
			else if( lenAX > math.length(vAB) ) {
				// AXがABよりも長い場合は、BPが円の中心までの最短距離
				shortestDistance = math.length( vBP );
			}
			else {
				// XがAB上にあるので、AXが最短距離
				// 単位ベクトルABとベクトルAPの外積で求める
				shortestDistance = math.abs( cross2d( vAP, vABnorm ) );
			}

			if( shortestDistance <= r ) {
				return true;
			}
			return false;
		}

		// 外積.
		float cross2d( float2 v1, float2 v2 )
		{
			return v1.x * v2.y - v1.y * v2.x;
		}

		// 内積.
		float inner2d( float2 v1, float2 v2 )
		{
			return v1.x * v2.x + v1.y * v2.y;
		}
	}
}
