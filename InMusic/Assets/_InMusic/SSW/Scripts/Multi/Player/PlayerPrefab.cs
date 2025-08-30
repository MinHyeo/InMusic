using System;
using UnityEngine;

public class PlayerPrefab : MonoBehaviour
{
    void Awake()
    {
        // Ensure this GameObject is not destroyed on scene load
        DontDestroyOnLoad(gameObject);
        
        // Log the creation of the PlayerPrefab instance
        Debug.Log($"[PlayerPrefab] Instance created: {gameObject.name}");
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
}
