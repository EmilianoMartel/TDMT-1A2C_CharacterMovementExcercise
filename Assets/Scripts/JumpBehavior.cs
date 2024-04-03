using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpBehavior : MonoBehaviour
{
    [SerializeField] private CharacterBody _body;
    [SerializeField] private float _jumpForce = 10;
    [SerializeField] private int _maxJumpQty = 1;
    private int _currentJumpQty = 0;
    [SerializeField] private float _floorAngle = 30;
    [SerializeField] private bool _enableLog = true;

    public event Action onJump = delegate { };
    public event Action onLand = delegate { };
    
    private void Reset()
    {
        _body = GetComponent<CharacterBody>();
    }

    public bool TryJump()
    {
        if (_currentJumpQty >= _maxJumpQty)
        {
            return false;
        }

        if (_enableLog)
            Debug.Log($"{name}: jumped!");
        _currentJumpQty++;
        _body.RequestImpulse(new ImpulseRequest(Vector3.up, _jumpForce));
        onJump.Invoke();
        return true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        var contact = collision.contacts[0];
        var contactAngle = Vector3.Angle(contact.normal, Vector3.up);
        if (contactAngle <= _floorAngle)
        {
            _currentJumpQty = 0;
            if (_enableLog)
                Debug.Log($"{name}: jump count reset!");
            onLand.Invoke();
        }

        if (_enableLog)
            Debug.Log($"{name}: Collided with normal angle: {contactAngle}");
    }
}
