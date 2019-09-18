using Unity.Entities;
using Unity.Mathematics;

namespace Meteo
{
	public struct HitEffInfo : IComponentData
	{
		public bool IsActive;
		public bool Initialized;
		public float Timer;
	}
}
