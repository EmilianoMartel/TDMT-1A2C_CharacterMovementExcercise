using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrainController : MonoBehaviour
{
    [SerializeField] private CharacterBody _characterBody;
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private JumpBehavior jumpBehaviour;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private bool enableLog = true;
    private Vector3 _desiredDirection;
    [Header("Parameters")]
    [SerializeField] private float _speed = 10;
    [SerializeField] private float _acceleration = 4;

    private void OnEnable()
    {
        _inputReader.onMovementInput += HandleMovementInput;
    }

    private void OnDisable()
    {
        _inputReader.onMovementInput -= HandleMovementInput;
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

        if (!cameraTransform && Camera.main)
            cameraTransform = Camera.main.transform;

        if (!cameraTransform)
            Debug.LogError($"{name}: {nameof(cameraTransform)} is null!");
    }

    private void HandleMovementInput(Vector2 input)
    {
        if (_desiredDirection.magnitude > Mathf.Epsilon
                && input.magnitude < Mathf.Epsilon)
        {
            if (enableLog)
            {
                Debug.Log($"{nameof(_desiredDirection)} magnitude: {_desiredDirection.magnitude}\t{nameof(input)} magnitude: {input.magnitude}");
            }
            _characterBody.RequestBrake();
        }

        _desiredDirection = new Vector3(input.x, 0, input.y);
        if (cameraTransform)
        {
            _desiredDirection = cameraTransform.TransformDirection(_desiredDirection);
            _desiredDirection.y = 0;
        }
        _characterBody.SetMovement(new MovementRequest(_desiredDirection, _speed, _acceleration));
    }

    private void HandleJumpInput()
    {
        jumpBehaviour.TryJump();
    }
}
