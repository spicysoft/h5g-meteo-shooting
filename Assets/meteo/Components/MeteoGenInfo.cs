using Unity.Entities;

namespace Meteo
{
	public struct MeteoGenInfo : IComponentData
	{
		public bool Initialized;
		public bool ReqSplit;       // 分裂リクエスト.
		public float Timer;
		public float TotalTimer;
		public int GeneratedCnt;	// 生成した隕石の数.
		public int MeteoNum;        // 今いる隕石の数.
		public int MeteoMax;
	}
}
