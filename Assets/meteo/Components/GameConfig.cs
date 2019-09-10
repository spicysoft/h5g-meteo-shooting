using Unity.Entities;
using Unity.Tiny.Scenes;

namespace Meteo
{
	public struct GameConfig : IComponentData
	{
		public SceneReference PrefabBullet;

		//public SceneReference TitleScn;
		//public SceneReference GameOverScn;
		//public SceneReference ResultScn;
	}
}
