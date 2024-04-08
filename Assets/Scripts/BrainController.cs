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

    private Quaternion _characterTargetRot;
    private bool _isJumping = false;

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
    }

    private void Start()
    {
        _characterTargetRot = transform.localRotation;
    }

    private void HandleMovementInput(Vector2 input)
    {
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

    private void HandleJumpInput()
    {
        _jumpBehaviour.TryJump();
    }

    private void HandleLookInput(Vector2 look)
    {
        float yRot = look.x * _rotationSpeed;
        _characterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
        transform.localRotation = _characterTargetRot;
    }

    private void HandleIsJumping()
    {
        _isJumping = true;
    }

    private void HandleOnLand()
    {
        _isJumping = false;
    }
}
