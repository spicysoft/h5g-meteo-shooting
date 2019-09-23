using Unity.Entities;
using Unity.Tiny.Scenes;

namespace Meteo
{
	public struct GameConfig : IComponentData
	{
		public SceneReference PrefabBullet;
		public SceneReference PrefabHitEff;
		public SceneReference PrefabExplEff;
		public SceneReference PrefabMeteo;

		public SceneReference TitleScn;
		public SceneReference MainScn;
		public SceneReference GameOverScn;
		public SceneReference ResultScn;
	}
}
