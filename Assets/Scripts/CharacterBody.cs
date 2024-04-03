using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CharacterBody : MonoBehaviour
{
    [SerializeField] private float _maxFloorDistance = .1f;
    [SerializeField] private float _brakeMultiplier = 1;
    [SerializeField] private bool _enableLog = true;
    [SerializeField] private LayerMask _floorMask;

    private Rigidbody _rigidbody;
    private MovementRequest _currentMovement = MovementRequest.InvalidRequest;
    private bool _isBrakeRequested = false;
    private readonly List<ImpulseRequest> _impulseRequests = new();

    [SerializeField] private Vector3 _floorCheckOffset = new Vector3(0, 0.001f, 0);

    public bool IsFalling { private set; get; }

    private void Reset()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnValidate()
    {
        _rigidbody ??= GetComponent<Rigidbody>();
    }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (_isBrakeRequested)
            Break();

        ManageMovement();
        ManageImpulseRequests();
    }

    public void SetMovement(MovementRequest movementRequest)
    {
        _currentMovement = movementRequest;
    }

    public void RequestBrake()
    {
        _isBrakeRequested = true;
    }

    public void RequestImpulse(ImpulseRequest request)
    {
        _impulseRequests.Add(request);
    }

    private void Break()
    {
        _rigidbody.AddForce(-_rigidbody.velocity * _brakeMultiplier, ForceMode.Impulse);
        _isBrakeRequested = false;
        if (_enableLog)
            Debug.Log($"{name}: Brake processed.");
    }

    private void ManageMovement()
    {
        var velocity = _rigidbody.velocity;
        velocity.y = 0;
        IsFalling = !Physics.Raycast(transform.position + _floorCheckOffset,
                                    -transform.up,
                                    out var hit,
                                    _maxFloorDistance,
                                    _floorMask);
        if (!_currentMovement.IsValid()
            || velocity.magnitude >= _currentMovement.GoalSpeed)
            return;
        var accelerationVector = _currentMovement.GetAccelerationVector();
        if (!IsFalling)
        {
            accelerationVector = Vector3.ProjectOnPlane(accelerationVector, hit.normal);
            Debug.DrawRay(transform.position, accelerationVector, Color.cyan);
        }
        Debug.DrawRay(transform.position, accelerationVector, Color.red);
        _rigidbody.AddForce(accelerationVector, ForceMode.Force);
    }

    private void ManageImpulseRequests()
    {
        foreach (var request in _impulseRequests)
        {
            _rigidbody.AddForce(request.GetForceVector(), ForceMode.Impulse);
        }
        _impulseRequests.Clear();
    }
}
