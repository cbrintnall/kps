using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject Skull;
    public Camera SkullCamera;

    public float SkullLookSpeed = 2.0f;

    public void Play()
    {
        SceneManager.LoadScene("ObsidianLibrary");
    }

    public void Quit() => Application.Quit();

    void Update()
    {
        var t = SkullCamera.ScreenToWorldPoint(Input.mousePosition);

        Skull.transform.forward = Vector3.Lerp(
            Skull.transform.forward,
            t,
            Time.deltaTime * SkullLookSpeed
        );
    }
}
