using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterView : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidBody;
    [SerializeField] private Animator _animator;
    [SerializeField] private JumpBehavior _jumpBehavior;
    [Header("Animator Parameters")]
    [SerializeField] private string _xSpeed = "xSpeed";
    [SerializeField] private string _zSpeed = "zSpeed";
    [SerializeField] private string _isJumping = "isJumping";

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
        var velocity = _rigidBody.velocity;
        velocity.y = 0;
        var xSpeed = velocity.x;
        var ySpeed = velocity.z;
        
        if (_animator)
        {
            _animator.SetFloat(_xSpeed, xSpeed);
            _animator.SetFloat(_zSpeed, ySpeed);
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
