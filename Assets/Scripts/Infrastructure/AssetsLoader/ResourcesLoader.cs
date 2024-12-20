using UnityEngine;

namespace Assets.Scripts.Infrastructure.AssetsLoader
{
    public class ResourcesLoader : IAssetsLoader
    {
        GameObject IAssetsLoader.LoadGameObject(string path) => Resources.Load<GameObject>(path);
    }
}