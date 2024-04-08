using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterView : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidBody;
    [SerializeField] private Animator _animator;
    [Header("Animator Parameters")]
    [SerializeField] private string _horSpeed = "horSpeed";
    [SerializeField] private string _zSpeed = "zSpeed";


    private float x;
    private float y;

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
    }

    private void Update()
    {
        var velocity = _rigidBody.velocity;
        velocity.y = 0;
        var speed = velocity.magnitude;
        var xSpeed = velocity.x;
        x = xSpeed;
        var ySpeed = velocity.z;
        y = ySpeed;
        if (_animator)
        {
            _animator.SetFloat(_horSpeed, xSpeed);
            _animator.SetFloat(_zSpeed, ySpeed);
        }
    }
}
