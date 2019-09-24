using Unity.Entities;

namespace Meteo
{
	public struct BgTag : IComponentData
	{
		public bool Initialized;	// 空の構造体ではだめだったので追加.
	}
}
