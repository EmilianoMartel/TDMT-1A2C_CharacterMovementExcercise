using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterView : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidBody;
    [SerializeField] private Animator _animator;
    [SerializeField] private JumpBehavior _jumpBehavior;
    [SerializeField] private CharacterBody _characterBody;
    [SerializeField] private BrainController _brainController;
    [Header("Animator Parameters")]
    [SerializeField] private string _xSpeed = "xSpeed";
    [SerializeField] private string _zSpeed = "zSpeed";
    [SerializeField] private string _isJumping = "isJumping";
    [SerializeField] private string _isFalling = "isFalling";

    private void OnEnable()
    {
        _jumpBehavior.onJump += HandleJump;
        _jumpBehavior.onLand += HandleOnLand;
    }

    private void OnDisable()
    {
        _jumpBehavior.onJump -= HandleJump;
        _jumpBehavior.onLand -= HandleOnLand;
    }

    private void Awake()
    {
        if (!_rigidBody)
        {
            Debug.LogError($"{name}: RigidBody is null\nCheck and add component.\nDisabling component.");
            enabled = false;
            return;
        }
        if (!_animator)
        {
            Debug.LogError($"{name}: AnimatorController is null\nCheck and add component.\nDisabling component.");
            enabled = false;
            return;
        }
        if (!_jumpBehavior)
        {
            Debug.LogError($"{name}: JumpBehavior is null\nCheck and add component.\nDisabling component.");
            enabled = false;
            return;
        }
    }

    private void Update()
    {
        var input = _brainController.input;
        var velocity = _rigidBody.velocity.magnitude;
        var xSpeed = input.x * _rigidBody.velocity.magnitude;
        var ySpeed = input.y * _rigidBody.velocity.magnitude;
        
        if (_animator)
        {
            _animator.SetFloat(_xSpeed, xSpeed);
            _animator.SetFloat(_zSpeed, ySpeed);
            _animator.SetBool(_isFalling, _characterBody.isFalling);
        }
    }

    private void HandleJump()
    {
        _animator.SetBool(_isJumping,true);
    }

    private void HandleOnLand()
    {
        _animator.SetBool(_isJumping, false);
    }
}
