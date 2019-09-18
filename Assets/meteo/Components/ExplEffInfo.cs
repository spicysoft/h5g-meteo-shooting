using Unity.Entities;
using Unity.Mathematics;

namespace Meteo
{
	public struct ExplEffInfo : IComponentData
	{
		public bool IsActive;
		public bool Initialized;
		public float Timer;
	}
}
