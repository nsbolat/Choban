using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class pause : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public WorldTime.WorldTime worldTimeScript; // WorldTime scriptine referans
    private bool isPaused = false;
    public Button btn1, btn2, btn3;

    private void Start()
    {
        pauseMenuUI.SetActive(false);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        worldTimeScript.ResumeTime(); // Zaman� devam ettir
        isPaused = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        worldTimeScript.PauseTime(); // Zaman� durdur
        isPaused = true;
    }

    public void mandon()
    {
        SceneManager.LoadScene(0);
    }

    public void rst()
    {
        SceneManager.LoadScene(2);
    }
    public void cik()
    {
        Application.Quit();
    }

}
