using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RandomAction : MonoBehaviour
{
    public float acceleration = 10f;
    public float maxSpeed = 5f;
    public float rotationSpeed = 100f;

    private Rigidbody rb;
    private float currentSpeed = 0f;
    private readonly bool[] action = {false, false, false, false};

    
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void FixedUpdate()
    {
        GetAction();
        UpdatePosition(action);
    }

    void GetAction() {
        action[0] = Random.Range(0, 2) != 0;
        action[1] = Random.Range(0, 2) != 0;
        action[2] = Random.Range(0, 2) != 0;
        action[3] = Random.Range(0, 2) != 0;
    }

    void UpdatePosition(bool[] action) {
        // [accel, decel, left, right]

        float moveInput = 0;
        
        float turnInput = 0;

        if (action[0]) {
            moveInput += 1;
        }
        if (action[1]) {
            moveInput -= 1;
        }
        if (action[2]) {
            turnInput -= 1;
        }
        if (action[3]) {
            turnInput += 1;
        }


        // Turn first
        rb.angularVelocity = Vector3.zero;
        Quaternion turnOffset = Quaternion.Euler(0f, turnInput * rotationSpeed * Time.fixedDeltaTime, 0f);
        rb.MoveRotation(rb.rotation * turnOffset);

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
