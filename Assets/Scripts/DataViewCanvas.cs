using System;
using System.Runtime.ExceptionServices;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.UI;

public class DataViewCanvas : MonoBehaviour
{
    private GameObject target;
    private CameraController cam;

    // text
    public TMPro.TextMeshProUGUI generationText;
    public TMPro.TextMeshProUGUI typeText;
    public TMPro.TextMeshProUGUI numFoodText;
    public TMPro.TextMeshProUGUI numOffspringText;
    public TMPro.TextMeshProUGUI energyText;
    public TMPro.TextMeshProUGUI speedText;
    public Slider action1;
    public Slider action2;
    public Slider action3;
    public Slider action4;
    public Vector2 rayOrigin = new(0, -200);
    public UILineDrawer lineDrawer;

    private OrganismManager followTarget;
    private OrganismMovement targetMovement;
    private NeuralNetwork targetNN;
    private RaycastDetection targetRays;
    private int numRays;
    private float rayAngle;
    public float rayDist = 300f;
    private Color[] colors;
    


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        target = null;
        followTarget = null;
        targetMovement = null;
        targetNN = null;
        cam = Camera.main.GetComponent<CameraController>();
        
        colors = new Color[3]; // predator, other, prey
        colors[0] = new Color(215f/255f, 61f/255f, 67f/255f);
        colors[1] = new Color(255f/255f, 255f/255f, 255f/255f);
        colors[2] = new Color(100f/255f, 183f/255f, 16f/255f);
    }

    // Update is called once per frame
    void Update()
    {
        if (cam.followTarget != null) {
            if (target == null || cam.followTarget != target) {
                target = cam.followTarget.gameObject;
                followTarget = target.GetComponent<OrganismManager>();
                targetMovement = followTarget.movement;
                targetNN = followTarget.nn;
                targetRays = followTarget.rayDetection;
                numRays = followTarget.numRays;
                rayAngle = followTarget.rayAngle;
            }
            if (gameObject.activeSelf) {
                generationText.text = $"Generation: {followTarget.generation}";
                typeText.text = $"Type: {followTarget.gameObject.tag}";
                numFoodText.text = $"Num Food Consumed: {followTarget.numConsumed}";
                numOffspringText.text = $"Num Offsprings:{followTarget.numOffsprings}";
                energyText.text = $"Energy: {followTarget.currentEnergy}";
                speedText.text = $"Speed: {targetMovement.currentSpeed}";

                action1.value = targetNN.returnArray[0];
                action2.value = targetNN.returnArray[1];

                lineDrawer.ClearLines();
                // visualize raycast
                for (int i=0; i<numRays; i++) {
                    float angle = (-rayAngle/2 + i * rayAngle/(float)(numRays-1)) * (Mathf.PI / 180); // calculate angle and convert to radians
                    float actualRayDistance = rayDist * -(targetRays.inputs[i*2]-1);

                    lineDrawer.DrawLine(rayOrigin, new Vector2(actualRayDistance * Mathf.Sin(angle) + rayOrigin[0], actualRayDistance * Mathf.Cos(angle) + rayOrigin[1]), colors[(int)targetRays.inputs[i*2+1]+1]);
                }
            }
        }
    }
}
