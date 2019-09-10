using Unity.Entities;
using Unity.Mathematics;

namespace Meteo
{
	public struct BulletInfo : IComponentData
	{
		public bool IsActive;
		public bool Initialized;
		public float BaseSpeed;
		public float2 MoveDir;
	}
}
