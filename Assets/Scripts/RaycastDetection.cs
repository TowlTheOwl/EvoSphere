using System;
using Unity.VisualScripting;
using UnityEngine;

public class RaycastDetection : MonoBehaviour
{
    public float[] inputs;
    public LineRenderer lineRenderer;
    public LayerMask raycastMask;


    private Camera mainCamera;
    private Transform followTarget;
    private string preyTag;
    private string predatorTag;
    private float rayDistance;  // Max distance for the raycast
    private int numRays;
    private float rayAngle;
    
    void Awake() {
        mainCamera = Camera.main;
        followTarget = mainCamera.GetComponent<CameraController>().followTarget;
        if (gameObject.tag == "Carnivore") {
            raycastMask = ~(1 << LayerMask.NameToLayer("Producer"));
        }
        else {
            raycastMask = ~0; // collide with everything
        }
    }

    void Update()
    {
        followTarget = mainCamera.GetComponent<CameraController>().followTarget;
    }

    public void Initialize(float rayDist, int numRays, float rayAngle, string preyType, string predatorType) {
        rayDistance = rayDist;
        this.numRays = numRays;
        this.rayAngle = rayAngle;
        inputs = new float[numRays * 2 + 1];

        preyTag = preyType;
        predatorTag = predatorType;

        // Ensure you have a LineRenderer attached
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        // Assign the default material for LineRenderer
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));


        lineRenderer.startWidth = .1f;
        lineRenderer.endWidth = .1f;
    }

    private float EncodeObjectType(GameObject obj) {
        if (obj.CompareTag("Wall"))
            return 0f;
        else if (preyTag.Length > 0 && obj.CompareTag(preyTag))
            return 1f;
        else if (predatorTag.Length > 0 && obj.CompareTag(predatorTag))
            return -1f;
        else
            return 0f;
    }
    public void DetectObjects()
    {   
        if (followTarget != null && followTarget.gameObject == gameObject) {
            lineRenderer.positionCount = numRays * 2;
        }
        else {
            lineRenderer.positionCount = 0;
        }
        for (int i=0; i<numRays; i++) {
            float angle = -rayAngle/2 + i * rayAngle/(float)(numRays-1);
        
            Vector3 direction = Quaternion.Euler(0, angle, 0) * transform.forward;

            int lineIndex = i*2;

            Ray ray = new(transform.position, direction);
            if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, raycastMask))
            {   
                if (hit.collider.gameObject != gameObject)
                {
                    inputs[lineIndex] = 1.0f - (hit.distance/rayDistance);
                    inputs[lineIndex + 1] = EncodeObjectType(hit.collider.gameObject);
                    
                    if (lineRenderer.positionCount != 0) {
                        lineRenderer.SetPosition(lineIndex, transform.position);
                        lineRenderer.SetPosition(lineIndex + 1, hit.point);
                    }
                }
                else {
                    inputs[lineIndex] = 0f;
                    inputs[lineIndex + 1] = -1f;
                    if (lineRenderer.positionCount != 0) {
                        lineRenderer.SetPosition(lineIndex, transform.position);
                        lineRenderer.SetPosition(lineIndex + 1, transform.position + direction * rayDistance);
                    }
                }
            }
            else {
                inputs[lineIndex] = 0f;
                inputs[lineIndex + 1] = 0f;

                if (lineRenderer.positionCount != 0) {
                    lineRenderer.SetPosition(lineIndex, transform.position);
                    lineRenderer.SetPosition(lineIndex + 1, transform.position + direction * rayDistance);
                }
            }
        }
    }
}
