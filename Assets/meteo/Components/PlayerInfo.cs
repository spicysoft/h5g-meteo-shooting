using Unity.Entities;

namespace Meteo
{
	public struct PlayerInfo : IComponentData
	{
		public bool Initialized;
		public float Interval;
	}
}
