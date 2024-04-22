using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpBehavior : MonoBehaviour
{
    [SerializeField] private CharacterBody _body;
    [Header("Jump parameters")]
    [SerializeField] private JumpData _jumpData;
    private int _currentJumpQty = 0;
    [SerializeField] private float _floorAngle = 30;

    [Header("View parameterrs")]
    [SerializeField] private float _waitForAnimation = 0.5f;
    [SerializeField] private bool _enableLog = true;

    private float _actualJumpForce = 0f;
    private bool _isIncrementedtJumpForce = false;

    public event Action onJump = delegate { };
    public event Action onLand = delegate { };

    private void Reset()
    {
        _body = GetComponent<CharacterBody>();
    }

    private void TryJump()
    {
        if (_currentJumpQty >= _jumpData.maxJumpQty)
            return;

        onJump.Invoke();
        StartCoroutine(Jump());
    }


    private void Update()
    {
        IncrementJumpForce();
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

    private IEnumerator Jump()
    {
        yield return new WaitForSeconds(_waitForAnimation);
        if (_enableLog)
            Debug.Log($"{name}: jumped!");
        _currentJumpQty++;
        _body.RequestImpulse(new ImpulseRequest(Vector3.up, _actualJumpForce));
        _actualJumpForce = 0;
        _isIncrementedtJumpForce = false;
    }

    public void HandleJump()
    {
        _actualJumpForce = _jumpData.minJumpForce;
        _isIncrementedtJumpForce = true;
    }

    public void HandleFinishJump()
    {
        TryJump();
    }

    private void IncrementJumpForce()
    {
        if (!_isIncrementedtJumpForce)
            return;

        _actualJumpForce += Time.deltaTime ;

        if (_actualJumpForce >= _jumpData.maxJumForce)
            TryJump();
    }
}