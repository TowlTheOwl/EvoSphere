using System;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.Callbacks;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class OrganismMovement : MonoBehaviour
{
    private float acceleration;
    private float maxSpeed;
    private float rotationSpeed;
    public float currentSpeed;
    public void Initialize(float accel, float max, float rot, float curr) {
        acceleration = accel;
        maxSpeed = max;
        rotationSpeed = rot;
        currentSpeed = curr;
    }

    public void UpdatePosition(Rigidbody rb, float[] action) {
        // Normalize inputs to be between -1 and 1
        float moveInput = Mathf.Pow((action[0] - 0.5f) * 2f, 3f); // cubic so keeps the sign
        
        float turnInput = Mathf.Pow((action[1] - 0.5f) * 2f, 3f);

        // Turn first
        rb.angularVelocity = Vector3.zero;

        rb.MoveRotation(Quaternion.Euler(0f, rb.rotation.eulerAngles.y + turnInput * rotationSpeed * Time.fixedDeltaTime, 0f));

        if (moveInput != 0) {
            currentSpeed += moveInput * acceleration;
            if (currentSpeed > maxSpeed) {
                currentSpeed = maxSpeed;
            }
        }
        if (currentSpeed < 0) {
            currentSpeed = 0f;
        }

        rb.linearVelocity = Vector3.zero;
        Vector3 move = transform.forward * currentSpeed;
        rb.MovePosition(rb.position + move * Time.fixedDeltaTime);
    }
}
