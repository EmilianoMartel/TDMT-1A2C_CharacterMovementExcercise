using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player Data/MovementData", fileName = "MovementData")]
public class JumpData : ScriptableObject
{
    [Header("Jump")]
    [SerializeField] private float _minJumpForce = 10;
    [SerializeField] private float _maxJumForce = 15;
    [SerializeField] private int _maxJumpQty = 1;
}