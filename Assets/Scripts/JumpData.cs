using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player Data/JumpData", fileName = "JumpData")]
public class JumpData : ScriptableObject
{
    [Header("Jump")]
    [SerializeField] private float _minJumpForce = 10;
    [SerializeField] private float _maxJumForce = 15;
    [SerializeField] private int _maxJumpQty = 1;

    public float minJumpForce { get { return _minJumpForce; } }
    public float maxJumForce { get { return _maxJumForce; } }
    public int maxJumpQty { get { return _maxJumpQty; } }
}