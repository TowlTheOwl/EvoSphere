using System.Collections.Generic;
using System.Globalization;
using Unity.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static Dictionary<string, int> objectCount = new();
    public GameObject foodPrefab;
    public GameObject herbivorePrefab;
    public GameObject carnivorePrefab;
    public static List<(int, int, int, int)> data = new();

    public TMPro.TextMeshProUGUI herbivoreCountText;
    public TMPro.TextMeshProUGUI carnivoreCountText;
    public TMPro.TextMeshProUGUI producerCountText;
    private int seconds = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    void Start()
    {   
        // set seed to keep consitency
        // found seeds: 777
        int seed = Random.Range(0, 1000);
        // int seed = 63;
        Random.InitState(seed);
        Debug.Log($"Current Random Seed: {seed}");
        
        int numHerbivores = 60;
        int numCarnivores = 10;
        if (MainManager.Instance != null)
        {
            numHerbivores = MainManager.Instance.numHerbivores;
            numCarnivores = MainManager.Instance.numCarnivores;
        }
        
        objectCount["Herbivore"] = 0;
        objectCount["Carnivore"] = 0;
        objectCount["Producer"] = 0;

        for (int i = 0; i < numHerbivores; i++) {
            Instantiate(herbivorePrefab, new Vector3(Random.Range(-95f, 95f), 1f, Random.Range(-95f, 95f)), Quaternion.Euler(0, Random.Range(0, 360f), 0)).GetComponent<OrganismManager>().Initialize();
            objectCount["Herbivore"]++;
        }
        Debug.Log("Herbivores Generated: " + objectCount["Herbivore"]);

        for (int i = 0; i < numCarnivores; i++) {
            Instantiate(carnivorePrefab, new Vector3(Random.Range(-95f, 95f), 1f, Random.Range(-95f, 95f)), Quaternion.Euler(0, Random.Range(0, 360f), 0)).GetComponent<OrganismManager>().Initialize();
            objectCount["Carnivore"]++;
        }
        Debug.Log("Carnivores Generated: " + objectCount["Carnivore"]);

        for (int i=0; i < 100; i++) {
            SpawnFood();
        }
        InvokeRepeating("SpawnFood", 0f, 0.20f);
        InvokeRepeating("Observe", 0f, 1f);
    }

    void Update() {
        herbivoreCountText.text = $"Herbivore Count: {objectCount["Herbivore"]}";
        carnivoreCountText.text = $"Carnivore Count: {objectCount["Carnivore"]}";
        // producerCountText.text = $"Producer Count: {objectCount["Producer"]}";
        if (Input.GetKey(KeyCode.R)) {
            Reset();
        }
    }

    void SpawnFood() {
        Instantiate(foodPrefab, new Vector3(Random.Range(-95f, 95f), 1f, Random.Range(-95f, 95f)), Quaternion.identity);
        objectCount["Producer"]++;
    }

    void Observe() {
        if (objectCount["Herbivore"] == 0 || objectCount["Carnivore"] == 0) {
        // if (objectCount["Herbivore"] == 0) {
            Reset();
        }
        else {
            data.Add((seconds, objectCount["Producer"], objectCount["Herbivore"], objectCount["Carnivore"]));
            seconds += 1;
        }
    }

    void Reset() {
        Debug.Log($"Simulation over. Time lasted: {seconds} seconds");
        DeleteAllObjectsWithTag("Producer");
        DeleteAllObjectsWithTag("Herbivore");
        DeleteAllObjectsWithTag("Carnivore");
        CancelInvoke("SpawnFood");
        CancelInvoke("Observe");
        data.Clear();
        seconds = 0;
        Start();
    }

    void DeleteAllObjectsWithTag(string tag) {
        // delete producers
        GameObject[] gos = GameObject.FindGameObjectsWithTag(tag);
        foreach(GameObject go in gos) {
            Destroy(go);
        }
    }
}
