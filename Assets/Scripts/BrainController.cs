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
    [SerializeField] private MovementData _moventenData;

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
        _inputReader.finishJumpInput += HandleFinishJump;
        _jumpBehaviour.onJump += HandleIsJumping;
        _jumpBehaviour.onLand += HandleOnLand;
        
    }

    private void OnDisable()
    {
        _inputReader.onMovementInput -= HandleMovementInput;
        _inputReader.onJumpInput -= HandleJumpInput;
        _inputReader.onLookInput -= HandleLookInput;
        _inputReader.finishJumpInput -= HandleFinishJump;
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

        BrakeRequestLogic();

        _desiredDirection = SetDesiredDirection(input);

        _characterBody.SetMovement(new MovementRequest(_desiredDirection, _moventenData.speed, _moventenData.acceletarion));
    }

    private void HandleJumpInput(bool isTriggered)
    {
        _jumpBehaviour.HandleJump();
    }

    private void HandleFinishJump()
    {
        _jumpBehaviour.HandleFinishJump();
    }
    
    private void HandleLookInput(Vector2 look)
    {
        float yRot = look.x * _moventenData.rotationSpeed;
        _characterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
        transform.localRotation = _characterTargetRot;

        if (_lastMovementInput.magnitude > Mathf.Epsilon && _cameraTransform)
        {
            _desiredDirection = _cameraTransform.TransformDirection(new Vector3(_lastMovementInput.x, 0, _lastMovementInput.y));
            _desiredDirection.y = 0;
            _characterBody.SetMovement(new MovementRequest(_desiredDirection, _moventenData.speed, _moventenData.acceletarion));
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

    private void BrakeRequestLogic()
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
    }

    private Vector3 SetDesiredDirection(Vector2 input)
    {
        Vector3 tempDesiredDirection = new Vector3(input.x, 0, input.y);

        if (_cameraTransform)
        {
            tempDesiredDirection = _cameraTransform.TransformDirection(tempDesiredDirection);
            tempDesiredDirection.y = 0;
        }

        return tempDesiredDirection;
    }
}