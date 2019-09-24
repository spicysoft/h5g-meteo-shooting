using Unity.Entities;
using Unity.Mathematics;

namespace Meteo
{
	public struct InitMeteoInfo : IComponentData
	{
		public bool Initialized;
		public bool InitSplit;       // 分裂情報セットするか.
		public float3 SplitPos;
		public float2 SplitDir;
	}
}
