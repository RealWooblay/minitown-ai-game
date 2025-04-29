using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameObject menuCanvas;
    public AIClient aiClient;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        menuCanvas.SetActive(false);
    }

    public void OnMenuButtonPressed()
    {
        menuCanvas.SetActive(!menuCanvas.activeSelf);
    }
}
