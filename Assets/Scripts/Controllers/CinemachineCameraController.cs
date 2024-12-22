using System;
using System.Collections;
using Controllers.Input;
using Planets;
using UnityEngine;

public class CinemachineCameraController : MonoBehaviour
{
    public event Action<PlanetsName> OnClickToPlanet;

    [SerializeField] 
    private Transform _target;

    [SerializeField] 
    private float distance = 5f;

    [SerializeField] 
    private float rotationSpeed = 10f;

    [SerializeField] 
    private Vector2 pitchLimits = new Vector2(-30, 60);

    [SerializeField] 
    private float zoomSpeed = 2f;

    [SerializeField] 
    private float minDistance = 2f;

    [SerializeField] 
    private float maxDistance = 10f;

    [SerializeField]
    private float transitionDuration = 1.5f;
    [SerializeField]
    private float currentYaw = 0f;
    [SerializeField]
    private float currentPitch = 0f;
    private Vector2 lookInput;
    private float zoomInput;
    private IInput _input;
    private Camera _camera;
    private bool _block;

    private Transform newTarget;


    public void Construct(IInput input, Transform sun)
    {
        _input = input;
        _target = sun;

        Init();
    }

    private void Init()
    {
        _input.Init();
        _camera = GetComponentInChildren<Camera>();

        _input.OnZoom += OnZoom;
        _input.OnSwap += OnSwap;
        _input.OnClick += OnClick;

        currentPitch = 16.3f;
        currentYaw = 26.5f;
    }

    private void OnDisable()
    {
        _input.OnDisable();
    }

    private void OnClick(Vector2 position)
    {
        Ray ray = _camera.ScreenPointToRay(position);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            var onPlanet = hit.transform.GetComponent<MovingPlanet>();
            if (onPlanet)
            {
                onPlanet.StopSpeed();
                _block = true;
                SetTarget(onPlanet.transform);
                OnClickToPlanet?.Invoke(onPlanet.namePlanet);
                    
                StartCoroutine(nameof(MoveTPlanet));
            }
        }
    }

    private IEnumerator MoveTPlanet()
    {
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation; 
        Vector3 startTargetPosition = _target.position;

        Vector3 endTargetPosition = newTarget.position;
        float elapsedTime = 0f;
        distance = Vector3.Distance(startPosition, endTargetPosition);
        
        while (elapsedTime < transitionDuration || distance < 5)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / transitionDuration;
            t = Mathf.SmoothStep(0f, 1f, t);

            var interpolatedTargetPosition = Vector3.Lerp(startTargetPosition, endTargetPosition, t);
            transform.position = Vector3.Lerp(startPosition, interpolatedTargetPosition, t);

            transform.rotation = Quaternion.Slerp(startRotation, Quaternion.LookRotation(endTargetPosition - transform.position), t);
            distance = Vector3.Distance(transform.position, newTarget.position);
            
            if (distance <= 5)
            {
                _target = newTarget;
                _block = false;
                var rotation = transform.rotation;
                
                currentYaw = rotation.eulerAngles.y;
                currentPitch = rotation.eulerAngles.x;
                yield break;
            }
            yield return null;
        }

        _target = newTarget;
        _block = false;

    }

    private void SetTarget(Transform target) => newTarget = target;

    private void OnSwap(Vector2 obj) => lookInput = obj;

    private void OnZoom(int obj) => zoomInput = obj;


    private void LateUpdate()
    {
        if (_target == null || _block)
            return;

        currentYaw += lookInput.x * rotationSpeed * Time.deltaTime;
        currentPitch -= lookInput.y * rotationSpeed * Time.deltaTime;
        currentPitch = Mathf.Clamp(currentPitch, pitchLimits.x, pitchLimits.y);

        distance -= zoomInput * zoomSpeed * Time.deltaTime;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        Vector3 direction = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0);
        transform.position = _target.position + rotation * direction;
        transform.LookAt(_target);
    }
}
