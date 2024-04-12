using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.HID;

[RequireComponent(typeof(Rigidbody))]
public class CharacterBody : MonoBehaviour
{
    [SerializeField] private float _maxFloorDistance = .1f;
    [SerializeField] private float _brakeMultiplier = 1;
    [SerializeField] private bool _enableLog = true;
    [SerializeField] private LayerMask _floorMask;
    [Header("Angles settings")]
    [SerializeField] private float _maxAngleToWalk = 45;
    [Tooltip("This value is for the tolerance with the maximum angle.")]
    [SerializeField] private float _angleTreshold = 5;
    private float _actualAngle;

    private Rigidbody _rigidbody;
    private MovementRequest _currentMovement = MovementRequest.InvalidRequest;
    private bool _isBrakeRequested = false;
    private readonly List<ImpulseRequest> _impulseRequests = new();

    [SerializeField] private Vector3 _floorCheckOffset = new Vector3(0, 0.001f, 0);

    public bool isFalling { private set; get; }

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

    private void Update()
    {
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out var hit, 5, _floorMask))
        {
            Vector3 vectorTo = hit.point - transform.position;
            Vector3 vectorFrom = hit.normal;

            _actualAngle = Vector3.Angle(vectorFrom, -vectorTo);
        }
        
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
        if (_rigidbody.velocity.magnitude > 0.01)
        {
            _isBrakeRequested = true;
        }
        else
        {
            _isBrakeRequested = false;
        }
       
        if (_enableLog)
            Debug.Log($"{name}: Brake processed.");
    }

    private void ManageMovement()
    {
        var velocity = _rigidbody.velocity;
        velocity.y = 0;
        isFalling = !Physics.Raycast(transform.position - _floorCheckOffset,
                                    transform.TransformDirection(Vector3.down),
                                    out var hit,
                                    _maxFloorDistance,
                                    _floorMask);

        if (!_currentMovement.IsValid()
            || velocity.magnitude >= _currentMovement.GoalSpeed || _actualAngle > _maxAngleToWalk - _angleTreshold)
            return;
        var accelerationVector = _currentMovement.GetAccelerationVector();

        if (!isFalling)
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

    [ContextMenu("DrawNormal")]
    private void DrawNormal()
    {
        
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out var hit, 5, _floorMask))
        {
            Debug.Log("si");
            Debug.DrawLine(transform.position,hit.point,Color.red,10f);
            Debug.DrawLine(hit.point,hit.point + hit.normal,Color.cyan, 10f);
            Vector3 pirulo = hit.point - transform.position;
            Vector3 paparulo = hit.normal;
            float angle = Vector3.Angle(paparulo, -pirulo);
            Debug.Log(angle);
        }
    }
}
