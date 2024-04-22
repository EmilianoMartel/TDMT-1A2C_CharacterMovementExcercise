using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player Data/MovementData", fileName = "MovementData")]
public class MovementData : ScriptableObject
{
    [Header("Movement")]
    [SerializeField] private float _speed = 10;
    [SerializeField] private float _acceleration = 4;
    [SerializeField] private float _rotationSpeed = 1;

    public float speed { get { return _speed; } }
    public float acceletarion { get { return _acceleration; } }
    public float rotationSpeed { get { return _rotationSpeed;} }
}
