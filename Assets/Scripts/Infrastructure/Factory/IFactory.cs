using Cinemachine;
using Planets;
using UnityEngine;
using UnityEngine.Video;

namespace Assets.Scripts.Infrastructure.Fabrics
{
    interface IFactory
    {
        GameObject CreateCamera();
        MovingPlanet CreatePlanets(PlanetsName name);
        LineRenderer CreateLine();

        VideoPlayer CreatePlayer();
    }
}