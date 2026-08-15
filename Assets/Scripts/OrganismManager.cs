using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class OrganismManager : MonoBehaviour
{   
    public float defaultEnergy = 10f;
    public float maxEnergy = 15f;
    public float energyGain = 5f;
    public float currentEnergy;   
    public GameObject childPrefab;
    public string preyTag = "";
    public string predatorTag = "";
    public float rayDistance = 10f;  // Max distance for the raycast
    public LineRenderer lineRenderer;
    public int numRays = 5;
    public float rayAngle = 45f;
    public float acceleration = 10f;
    public float maxSpeed = 5f;
    public float rotationSpeed = 100f;
    private float currentSpeed = 0f;
    public NeuralNetwork nn;
    public int generation;
    public string wallTag = "Wall";

    private Rigidbody rb;
    public RaycastDetection rayDetection;
    public OrganismMovement movement;
    private bool isDead = false;
    public int numOffsprings;
    public int numConsumed;

    public float energyDrainMultiplier = 10f;
    private int inputArrayLength;

    public OrganismManager Initialize()
    {   
        generation = 1;
        numOffsprings = 0;
        currentEnergy = defaultEnergy;
        

        nn = GetComponent<NeuralNetwork>();
        nn.Initialize(numRays);

        globalInit();
        return this;
    }

    private void globalInit() {
        numConsumed = 0;
        rb = gameObject.GetComponent<Rigidbody>();
        InvokeRepeating("DrainEnergy", 0f, 1f);

        inputArrayLength = numRays * 2 + 1;

        rayDetection = GetComponent<RaycastDetection>();
        rayDetection.Initialize(rayDistance, numRays, rayAngle, preyTag, predatorTag);

        movement = GetComponent<OrganismMovement>();
        movement.Initialize(acceleration, maxSpeed, rotationSpeed, currentSpeed);
    }

    void Inherit(OrganismManager parent) {
        generation = parent.generation+1;
        currentEnergy = defaultEnergy;

        nn = GetComponent<NeuralNetwork>();
        nn.Inherit(parent.nn);
        globalInit();
    }

    public void SetEnergy(int energy) {
        currentEnergy = energy;
    }

    void FixedUpdate() {
        // detect
        rayDetection.DetectObjects();

        rayDetection.inputs[inputArrayLength-1] = movement.currentSpeed/maxSpeed;
        // get action
        nn.Forward(rayDetection.inputs);

        // move
        movement.UpdatePosition(rb, nn.returnArray);
    }

    void DrainEnergy() {
        currentEnergy -= (Math.Abs(movement.currentSpeed)*energyDrainMultiplier)+1;
        if (currentEnergy <= 0) {
            if (!isDead)
            {   
                isDead = true;
                GameManager.objectCount[gameObject.tag]--;
                Destroy(gameObject);
            }
        }
    }

    void Reproduce()
    {
        Instantiate(childPrefab, gameObject.transform.position, Quaternion.Euler(0, Random.Range(0, 360f), 0)).GetComponent<OrganismManager>().Inherit(this);
        GameManager.objectCount[gameObject.tag]++;
        numOffsprings++;
        
        currentEnergy = defaultEnergy;
    }
    
    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == preyTag) {
            if (preyTag == "Producer") {
                Destroy(collision.gameObject);
                GameManager.objectCount[preyTag]--;
            }
            else {
                OrganismManager preyManager = collision.gameObject.GetComponent<OrganismManager>();
                if (!preyManager.isDead)
                {
                    GameManager.objectCount[preyTag]--;
                    preyManager.isDead = true; // mark as dead to prevent multiple collisions
                    Destroy(collision.gameObject);
                }
            }
            currentEnergy += energyGain;
            numConsumed++;
            if (currentEnergy >= maxEnergy) {
                Reproduce();
            }
        }
    }
}
