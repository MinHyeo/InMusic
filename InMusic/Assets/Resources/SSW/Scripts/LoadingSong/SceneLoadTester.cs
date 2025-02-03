using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadTester : MonoBehaviour
{
    [SerializeField] private GameObject loadingUIPrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    // void Update()
    // {
    //     if(Input.GetKeyDown(KeyCode.Space))
    //     {
    //         string currentScene = SceneManager.GetActiveScene().name;
    //         string nextScene = (currentScene == "Test1") ? "Test2" : "Test1";

    //         GameObject loadingUI = Instantiate(loadingUIPrefab);
    //         LoadingSong loadingSong = loadingUI.GetComponent<LoadingSong>();
    //         loadingSong.LoadPlay(nextScene);
    //         // LoadingSong.Instance.LoadPlay("Test2");
    //     }
    // }
}
