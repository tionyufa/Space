using Controllers;
using Planets;
using UI;
using UnityEngine.Video;

namespace Infrastructure.Mediators
{
    public class GameMediator : IMediator
    {
        private readonly DataPlanets _dataPlanets;
        private readonly ControllerCamera _controllerCamera;
        private readonly UIDescription _uiDescription;
        private readonly VideoPlayer _videoPlayer;

        public GameMediator(DataPlanets data,ControllerCamera cameraController, UIDescription uiDescription,VideoPlayer videoPlayer)
        {
            _dataPlanets = data;
            _controllerCamera = cameraController;
            _uiDescription = uiDescription;
            _videoPlayer = videoPlayer;
            
            Subscription();
            Init();
        }

        private void OnClickToPlanet(PlanetsName namePlanet)
        {
            for (int i = 0; i < _dataPlanets.data.Count; i++)
            {
                var data = _dataPlanets.data[i];
                if (namePlanet == data.name)
                {
                    _uiDescription.Construct(data);
                    _videoPlayer.clip = data.clip;
                }
            }
        }

        private void Init() => _uiDescription.Construct(_dataPlanets.data[2]);

        private void Subscription() => _controllerCamera.OnClickToPlanet += OnClickToPlanet;

        void IMediator.UnSubscription() => _controllerCamera.OnClickToPlanet -= OnClickToPlanet;

        void IMediator.Subscription() => Subscription();
    }
}