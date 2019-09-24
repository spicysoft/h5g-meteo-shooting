using Unity.Entities;
using Unity.Mathematics;

namespace Meteo
{
	public struct PlayerInfo : IComponentData
	{
		public bool Initialized;
		public float Interval;
		public float3 PrePos;
		public float Zang;
	}
}
