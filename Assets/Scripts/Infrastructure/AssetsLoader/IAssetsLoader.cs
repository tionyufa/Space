using UnityEngine;

namespace Assets.Scripts.Infrastructure.AssetsLoader
{
    public interface IAssetsLoader
    {
        GameObject LoadGameObject(string path);
    }
}