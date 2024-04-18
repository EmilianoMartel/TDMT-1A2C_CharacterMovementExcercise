using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrainController : MonoBehaviour
{
    [SerializeField] private CharacterBody _characterBody;
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private JumpBehavior _jumpBehaviour;
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private bool _enableLog = true;
    private Vector3 _desiredDirection;
    [Header("Parameters")]
    [SerializeField] private float _speed = 10;
    [SerializeField] private float _acceleration = 4;
    [SerializeField] private float _rotationSpeed = 1;

    [SerializeField] private float _gravityMultiplyModifier = 1.5f;
    private Vector3 _newGravity;
    private Vector3 _basicGravity;

    private Quaternion _characterTargetRot;
    private bool _isJumping = false;
    private Vector2 _lastMovementInput;

    public Vector2 input { get { return _lastMovementInput; }  }

    private void OnEnable()
    {
        _inputReader.onMovementInput += HandleMovementInput;
        _inputReader.onJumpInput += HandleJumpInput;
        _inputReader.onLookInput += HandleLookInput;
        _jumpBehaviour.onJump += HandleIsJumping;
        _jumpBehaviour.onLand += HandleOnLand;
    }

    private void OnDisable()
    {
        _inputReader.onMovementInput -= HandleMovementInput;
        _inputReader.onJumpInput -= HandleJumpInput;
        _inputReader.onLookInput -= HandleLookInput;
        _jumpBehaviour.onJump -= HandleIsJumping;
        _jumpBehaviour.onLand -= HandleOnLand;
    }

    private void Awake()
    {
        if (!_characterBody)
        {
            Debug.LogError($"{name}: Character Body is null\nCheck and add component.\nDisabling component.");
            enabled = false;
            return;
        }
        if (!_inputReader)
        {
            Debug.LogError($"{name}: InputReader is null\nCheck and add component.\nDisabling component.");
            enabled = false;
            return;
        }

        if (!_cameraTransform && Camera.main)
            _cameraTransform = Camera.main.transform;

        if (!_cameraTransform)
            Debug.LogError($"{name}: {nameof(_cameraTransform)} is null!");

        _basicGravity = Physics.gravity;
        _newGravity = Physics.gravity;
        _newGravity.y = _newGravity.y * _gravityMultiplyModifier;
    }

    private void Start()
    {
        _characterTargetRot = transform.localRotation;
    }

    private void HandleMovementInput(Vector2 input)
    {

        _lastMovementInput = input;
        if (_desiredDirection.magnitude > Mathf.Epsilon
                && input.magnitude < Mathf.Epsilon && !_isJumping)
        {
            if (_enableLog)
            {
                Debug.Log($"{nameof(_desiredDirection)} magnitude: {_desiredDirection.magnitude}\t{nameof(input)} magnitude: {input.magnitude}");
            }
            _characterBody.RequestBrake();
        }

        _desiredDirection = new Vector3(input.x, 0, input.y);

        if (_cameraTransform)
        {
            _desiredDirection = _cameraTransform.TransformDirection(_desiredDirection);
            _desiredDirection.y = 0;
        }

        _characterBody.SetMovement(new MovementRequest(_desiredDirection, _speed, _acceleration));
    }

    private void HandleJumpInput(bool isTriggered)
    {
        _jumpBehaviour.TryJump();
    }
    
    private void HandleLookInput(Vector2 look)
    {
        float yRot = look.x * _rotationSpeed;
        _characterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
        transform.localRotation = _characterTargetRot;

        if (_lastMovementInput.magnitude > Mathf.Epsilon && _cameraTransform)
        {
            _desiredDirection = _cameraTransform.TransformDirection(new Vector3(_lastMovementInput.x, 0, _lastMovementInput.y));
            _desiredDirection.y = 0;
            _characterBody.SetMovement(new MovementRequest(_desiredDirection, _speed, _acceleration));
        }
    }

    private void HandleIsJumping()
    {
        _isJumping = true;
        Physics.gravity = _newGravity;
    }

    private void HandleOnLand()
    {
        _isJumping = false;
        Physics.gravity = _basicGravity;
    }
}