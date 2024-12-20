using System;
using System.Threading.Tasks;
using Assets.Scripts.Infrastructure.AssetsLoader;
using Assets.Scripts.Infrastructure.Fabrics;
using Cinemachine;
using Controllers;
using Controllers.Input;
using Infrastructure.Factory;
using Infrastructure.Mediators;
using Infrastructure.Setting;
using Planets;
using UI;
using UnityEngine;
using UnityEngine.Video;

namespace Infrastructure
{
    public class BootStrapper : MonoBehaviour
    {
        [SerializeField] 
        private DataPlanets _baseDates;
        [SerializeField] 
        private SettingsDisplay _settingsDisplay;
        [SerializeField]
        private CinemachineVirtualCamera _camera;

        [SerializeField] 
        private UIDescription _uiDescription;

        [SerializeField]
        private UISettingDisplay _uiSettingDisplay;

        [SerializeField] private Camera _verticalCamera;
        [SerializeField] private Camera _globusCamera;
        
        private ControllerCamera _controllerCamera;
        private GameMediator _gameMediator;
        private VideoPlayer _player;

        private void Awake()
        {
            //SettingsDisplay();

            IInput input = new MobileInput();
            IAssetsLoader assetsLoader = new ResourcesLoader();
            IFactory factory = new GameFactory(assetsLoader);

            CreateParents();
            var sun = CreateSun(factory);
            CreateCamera(factory,input,sun);
            
            CreatePlanets(factory,sun);
            CreateVideoPlayer(factory);
            
            CreateMediators();
        }

        private void OnDestroy() => ((IMediator) _gameMediator).UnSubscription();
        private void CreateVideoPlayer(IFactory factory) => _player = factory.CreatePlayer();
        private void CreateMediators() => _gameMediator = new GameMediator(_baseDates,_controllerCamera,_uiDescription,_player);
        private Transform CreateSun(IFactory factory) => factory.CreatePlanets(PlanetsName.Sun).transform;


        private void CreateCamera(IFactory factory, IInput input, Transform sun)
        {
            var virtualCamera = factory.CreateCamera();
            if (virtualCamera != null)
            {
                _controllerCamera = virtualCamera.GetComponent<ControllerCamera>();
                if (_controllerCamera != null)
                    _controllerCamera.Construct(input,sun);
                
            }
        }

        private async void CreatePlanets(IFactory factory, Transform sun)
        {
            await Task.Delay(200);
            for (int i = 0; i < _baseDates.data.Count; i++)
            {
                var data = _baseDates.data[i];
                if (!data.isAvailable)
                    continue;
                var line = factory.CreateLine();
                line.transform.SetParent(_parentLines.transform);
                var namePlanet = (PlanetsName) i;
                var planet = factory.CreatePlanets(namePlanet);
                if (planet)
                {
                    planet.Construct(data, line, sun);
                    planet.transform.SetParent(_parentPlanets.transform);
                }
            }
        }

        private GameObject _parentLines;
        private GameObject _parentPlanets;
        private void CreateParents()
        {
            _parentLines = new GameObject();
            _parentPlanets = new GameObject();
            _parentPlanets.name = "ParentPlanets";
            _parentLines.name = "ParentLines";
             
        }

        private void SettingsDisplay()
        {
            for (int i = 0; i < Display.displays.Length; i++)
            {
                var display = Display.displays[i];
                display.Activate();
            }
            
            _settingsDisplay.globusDisplay = _globusCamera;
            _settingsDisplay.verticalDisplay = _verticalCamera;
            _uiSettingDisplay.Construct(_settingsDisplay);
        }
    }
}