using UnityEngine;

public class MovingPlanet : MonoBehaviour
{
    [SerializeField]
    private Transform _center;
    [SerializeField]
    private float _speed = 1;
    [SerializeField]
    private float _radius = 5f;
    private float _angel = 0f;
    [SerializeField]
    private int _segments = 100;
    [SerializeField]
    private LineRenderer _line;

    private void Start()
    {
        DrawLine();

        _line.loop = true;
    }

    private void DrawLine()
    {
        if (!_line)
            return;

        _line.positionCount = _segments + 1;
        _line.useWorldSpace = false;

        var angleStep = 360f / _segments;

        for (int i = 0; i <= _segments; i++)
        {
            var angle = Mathf.Deg2Rad * angleStep * i;
            var x = Mathf.Cos(angle) * _radius;
            var z = Mathf.Sin(angle) * _radius;
            var vector = new Vector3( x, _center.position.y,  z);
            _line.SetPosition(i, vector);

        }
    }

    private void Update()
    {
        if (!_line)
            return;
        
        _angel += _speed * Time.deltaTime;

        var x = Mathf.Cos(_angel) * _radius;
        var z = Mathf.Sin(_angel) * _radius;

        var vector = new Vector3(_center.position.x + x, _center.position.y, _center.position.z + z);
        transform.position = vector;

        DrawLine();
    }
}
