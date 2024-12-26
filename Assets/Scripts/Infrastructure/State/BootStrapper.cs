using System.Threading.Tasks;
using Assets.Scripts.Infrastructure.AssetsLoader;
using Assets.Scripts.Infrastructure.Fabrics;
using Cinemachine;
using Controllers;
using Controllers.Input;
using Infrastructure.Factory;
using Infrastructure.Mediators;
using Infrastructure.Setting;
using Infrastructure.State;
using Infrastructure.State.States;
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
        private UIDescription _uiDescription;

        [SerializeField]
        private UISettingDisplay _uiSettingDisplay;

        [SerializeField] private Camera _verticalCamera;
        [SerializeField] private Camera _globusCamera;
        [SerializeField] private InterfaceAnimManager _animManager;
        
        private CinemachineCameraController _controllerCamera;
        private GameMediator _gameMediator;
        private VideoPlayer _player;

        private void Awake()
        {
            IInput input = new MobileInput();
            
            SettingsDisplay((MobileInput) input);
            StartState();

            IAssetsLoader assetsLoader = new ResourcesLoader();
            IFactory factory = new GameFactory(assetsLoader);

            CreateParents();
            var sun = CreateSun(factory);
            var ui = CreateUI(factory,out UIDescription description);
            CreateCamera(factory,input,sun,ui);

            CreatePlanets(factory,sun);
            CreateVideoPlayer(factory);

            CreateMediators(description);
        }

        private void StartState()
        {
            var state = new GameStateMachine();
            state.Registration(new GameLoopState(), StateTriggers.GameLoop);
            state.Registration(new ViewPlanetState(), StateTriggers.ViewPlanet);
            state.Registration(new BlockState(), StateTriggers.Block);
        }


        private void OnDestroy() => ((IMediator) _gameMediator).UnSubscription();
        private void CreateVideoPlayer(IFactory factory) => _player = factory.CreatePlayer();
        private void CreateMediators(UIDescription uiDescription)
        {
            UIDescription[] descriptions = {uiDescription, _uiDescription};
            _gameMediator = new GameMediator(_baseDates, _controllerCamera, descriptions, _player);
        }

        private Transform CreateSun(IFactory factory) => factory.CreatePlanets(PlanetsName.Sun).transform;

        private InterfaceAnimManager CreateUI(IFactory factory, out UIDescription uiDescription)
        {
            uiDescription = null;
            var ui = factory.CreateUI(_animManager, _uiSettingDisplay.transform);
            if (ui)
                uiDescription = ui.GetComponent<UIDescription>();
            return ui;
        }

        private void CreateCamera(IFactory factory, IInput input, Transform sun,
            InterfaceAnimManager interfaceAnimManager)
        {
            var virtualCamera = factory.CreateCamera();
            if (virtualCamera != null)
            {
                _controllerCamera = virtualCamera.GetComponent<CinemachineCameraController>();
                if (_controllerCamera != null)
                    _controllerCamera.Construct(input,sun,_settingsDisplay,interfaceAnimManager);
                
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

        private void SettingsDisplay(MobileInput input)
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