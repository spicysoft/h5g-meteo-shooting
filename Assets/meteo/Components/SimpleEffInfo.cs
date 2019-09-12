using Unity.Entities;
using Unity.Mathematics;

namespace Meteo
{
	public struct SimpleEffInfo : IComponentData
	{
		public bool IsActive;
		public bool Initialized;
		public float Timer;
	}
}
