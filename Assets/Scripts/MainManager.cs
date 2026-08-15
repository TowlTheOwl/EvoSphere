using UnityEngine;
using UnityEngine.SceneManagement;

public class MainManager : MonoBehaviour
{
    // Start() and Update() methods deleted - we don't need them right now

    public static MainManager Instance;

    public TMPro.TMP_InputField inputHerbivores;
    public TMPro.TMP_InputField inputCarnivores;
    public int numHerbivores = 0;
    public int numCarnivores = 0;


    void Awake()
    {
        // start of new code
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        // end of new code

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void NextScene()
    {   
        numHerbivores = int.Parse(inputHerbivores.text);
        numCarnivores = int.Parse(inputCarnivores.text);
        SceneManager.LoadScene("Simulation Scene");
    }
}