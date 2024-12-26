using System;
using Assets.Scripts.Infrastructure.AssetsLoader;
using Assets.Scripts.Infrastructure.Fabrics;
using Cinemachine;
using Planets;
using UnityEngine;
using UnityEngine.Video;
using Object = UnityEngine.Object;

namespace Infrastructure.Factory
{
    public class GameFactory : IFactory
    {
        private const string PathPlanets = "Planets/{0}";
        private const string PathLine = "LinePlanets";
        private const string PathPlayer = "VideoPlayer";
        private const string PathCamera = "MainCamera";

        private IAssetsLoader _assetsLoader;
        public GameFactory(IAssetsLoader assetsLoader) => _assetsLoader = assetsLoader;

        public GameObject CreateCamera()
        {
            var cameraObject = _assetsLoader.LoadGameObject(PathCamera);
            var gObject = Object.Instantiate(cameraObject);
            return gObject;
        }

        public MovingPlanet CreatePlanets(PlanetsName name)
        {
            var gameObject = _assetsLoader.LoadGameObject(String.Format(PathPlanets, name));
            if (gameObject)
            {
                var gObject = InstantiateObject(gameObject);
                if (gObject)
                {
                    var planet = gObject.GetComponent<MovingPlanet>();
                    if (planet)
                        return planet;
                    else
                        return gObject.AddComponent<MovingPlanet>();
                    
                }
            }
            
            return null;
        }

        public LineRenderer CreateLine()
        {
            var linePrefab = _assetsLoader.LoadGameObject(PathLine);
            if (linePrefab)
            {
                var line = InstantiateObject(linePrefab);
                if (line)
                    return line.GetComponent<LineRenderer>();
            }

            return null;
        }

        public VideoPlayer CreatePlayer()
        {
            var prefab = _assetsLoader.LoadGameObject(PathPlayer);
            if (prefab)
            {
                var obj = InstantiateObject(prefab);
                if (obj)
                    return obj.GetComponent<VideoPlayer>();
            }
            return null;
        }


        InterfaceAnimManager IFactory.CreateUI(InterfaceAnimManager animManager, Transform parent) => Object.Instantiate(animManager,parent);
        GameObject InstantiateObject(GameObject gameObject) => Object.Instantiate(gameObject);
    }
}