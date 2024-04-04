using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterView : MonoBehaviour
{
    [SerializeField] private CharacterBody _characterBody;

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    private void Awake()
    {
        if (!_characterBody)
        {
            Debug.LogError($"{name}: Character Body is null\nCheck and add component.\nDisabling component.");
            enabled = false;
            return;
        }
    }
}
