using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 50f;
    public float zoomSpeed = 100f;
    public float minZoom = 30f;
    public float maxZoom = 190f;
    public float followSmoothTime = 0.3f;
    public float trackZoomValue = 50f;
    public Transform followTarget;
    public GameObject trackingCanvas;
    public GameObject dataViewCanvas;
    public GameObject graphViewCanvas;
    public TMPro.TextMeshProUGUI energyDisplay;
    public TMPro.TextMeshProUGUI generationDisplay;
    public Vector3 offset = new(0f, 0f, -15f);

    private Camera cam;
    private Vector3 targetZoom;
    private Vector3 velocity = Vector3.zero;
    private int followMode = 0; // 0: not following, 1: birds eye, 2: low view, 3: data view
    private Vector3 defaultPos = new Vector3(0, 190, 0);
    private Quaternion defaultRotation = Quaternion.Euler(90, 0, 0);
    private bool graphing;

    private static readonly HashSet<string> validTags = new() { "Character", "Producer", "Herbivore", "Carnivore" };

    void Start()
    {
        cam = Camera.main;
        targetZoom = cam.transform.position;
        trackingCanvas.SetActive(false);
        dataViewCanvas.SetActive(false);
        graphViewCanvas.SetActive(false);
        followTarget = null;
        graphing = false;
    }

    void Update()
    {
        HandleMovement();
        HandleZoom();
        HandleMouseClick();
        FollowTarget();
    }

    void HandleMovement()
    {
        if (followTarget != null) return; // disable manual movement while following

        float h = Input.GetKey(KeyCode.RightArrow) ? 1 : Input.GetKey(KeyCode.LeftArrow) ? -1 : 0;

        float v = Input.GetKey(KeyCode.UpArrow) ? 1 : Input.GetKey(KeyCode.DownArrow) ? -1 : 0;
        Vector3 dir = new Vector3(h, 0, v).normalized;

        if (dir.magnitude >= 0.1f)
        {
            transform.position += dir * moveSpeed * Time.deltaTime;
        }
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Vector3 zoomDirection = cam.transform.forward * scroll * zoomSpeed;
        Vector3 newPos = cam.transform.position + zoomDirection;

        float distance = Vector3.Distance(newPos, followTarget != null ? followTarget.position : Vector3.zero);
        if (distance > minZoom && distance < maxZoom)
        {
            cam.transform.position = newPos;
        }
    }

    void HandleMouseClick()
    {
        if (Input.GetMouseButtonDown(0)) // left-click
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (validTags.Contains(hit.collider.tag)) // target is a trackable character
                {
                    followTarget = hit.transform;

                    // Get the current position of the camera
                    Vector3 currentPosition = cam.transform.position;

                    // Modify only the Y value while keeping X and Z the same
                    currentPosition.y = trackZoomValue;  // Set this to the desired Y value

                    // Apply the new position to the camera
                    cam.transform.position = currentPosition;

                    followMode = 1;
                }
                else
                {
                    UnfollowTarget(); // stop following if clicked on something else
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape)) // allow manual cancel
        {
            UnfollowTarget();
        }

        if (Input.GetKeyDown(KeyCode.T)) {
            followMode = (followMode) % 3 + 1;
        }

        if (Input.GetKeyDown(KeyCode.G)) {
            UnfollowTarget();
            graphing = !graphing;
            if (graphing) {
                graphViewCanvas.SetActive(true);
            }
            else {
                graphViewCanvas.SetActive(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.Return) && followTarget != null) {
            float[][][] targetNN = followTarget.GetComponent<NeuralNetwork>().weights;
            Debug.Log("Target Neural Network:");
            for (int i = 0; i < targetNN.Length; i++) {
                Debug.Log(i);
                for (int j = 0; j < targetNN[i].Length; j++) {
                    Debug.Log($"{string.Join(", ", targetNN[i][j])}");
                }
            }
        }
    }

    void UnfollowTarget() {
        followTarget = null;
        transform.position = defaultPos;
        transform.rotation = defaultRotation;
        followMode = 0;
        dataViewCanvas.SetActive(false);
        trackingCanvas.SetActive(false);
    }

    void FollowTarget()
    {
        if ((UnityEngine.Object)followTarget != null)
        {   
            Vector3 targetPos = followTarget.position;
            targetPos.y = cam.transform.position.y;


            if (followMode == 1) {
                transform.position = targetPos;
            } 
            else if (followMode == 2) {
                transform.position = targetPos + followTarget.rotation * offset;
            }

            transform.LookAt(followTarget);
            UpdateUI();
        }
        else {
            if (followMode != 0) {
                UnfollowTarget();
            }
        }
    }

    void UpdateUI() {
        if (followMode == 1 || followMode == 2) {
            OrganismManager orgStats = followTarget.GetComponent<OrganismManager>();
            if (orgStats != null) {
                trackingCanvas.SetActive(true);
                energyDisplay.text = $"Energy: {orgStats.currentEnergy}";
                generationDisplay.text = $"Generation: {orgStats.generation}";
            }
        }
        else {
            trackingCanvas.SetActive(false);
        }
        if (followMode == 3) {
            dataViewCanvas.SetActive(true);
        } else {
            dataViewCanvas.SetActive(false);
        }
    }

}
