using UnityEngine;

public class Guide_UI : MonoBehaviour
{
    public void Button(string buttonname){
        if (buttonname == "Exit") {
            Destroy(gameObject);
        }
    }
}
