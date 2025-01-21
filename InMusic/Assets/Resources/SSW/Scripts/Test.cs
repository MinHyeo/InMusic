using UnityEngine;
using UnityEngine.SceneManagement;

public class Test : MonoBehaviour
{
    public void TestMethod()
    {
        Debug.Log("TestMethod");
        SceneManager.LoadScene("test_SSW");
    }
}
