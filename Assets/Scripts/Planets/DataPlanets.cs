using System.Collections.Generic;
using UnityEngine;

namespace Planets
{
    [CreateAssetMenu(fileName = "Data", menuName = "Data/Planet", order = 0)]
    public class DataPlanets : ScriptableObject
    {
        [SerializeField]
        private List<PlanetData> _dates;

        public List<PlanetData> data => _dates;
    }
}