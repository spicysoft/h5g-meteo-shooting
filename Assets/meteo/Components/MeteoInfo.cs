using Unity.Entities;
using Unity.Mathematics;

namespace Meteo
{
	public struct MeteoInfo : IComponentData
	{
		public bool IsActive;
		public bool Initialized;
		public bool IsHit;
		public float BaseSpeed;
		public float2 MoveDir;
		public float Radius;
		public float DistSq;		// プレイヤーとの距離の２乗.
		public int Life;
	}
}
