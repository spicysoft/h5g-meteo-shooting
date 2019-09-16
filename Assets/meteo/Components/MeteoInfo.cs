using Unity.Entities;
using Unity.Mathematics;

namespace Meteo
{
	public struct MeteoInfo : IComponentData
	{
		public bool IsActive;
		public bool Initialized;
		public bool IsHit;
		public bool ReqHitEff;
		public bool IsStop;
		public int UniId;
		public float BaseSpeed;
		public float2 MoveDir;
		public float Radius;
		public float DistSq;        // プレイヤーとの距離の２乗.
		public int Level;
		public int Life;
		public float3 HitPos;       // 弾が当たった位置.
		public float Timer;
	}
}
