using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Selects the PowerUpType
public enum PowerUpType { None, Pushback, Rockets, Smash }

public class PowerUp : MonoBehaviour
{
    // Sets the type of the powerup 
    public PowerUpType powerUpType;

    private float rotationSpeed = 100.0f;

    void Update()
    {
        // Rotates the powerup to add visual effect
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}
