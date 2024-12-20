using System;
using UnityEngine;
using UnityEngine.Video;

namespace Planets
{
    [Serializable]
    public class PlanetData
    {
        public string data;
        public bool isAvailable = true;
        public PlanetsName name;
        [TextArea]
        public string description;

        public float speed;
        public float radius;

        public Color color;

        public VideoClip clip;
        public Sprite image;
    }
}