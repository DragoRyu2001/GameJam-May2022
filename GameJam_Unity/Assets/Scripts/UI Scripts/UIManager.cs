using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class UIManager : MonoBehaviour
{
    [SerializeField] Image sprintBarImage;
    [SerializeField] Image healthBarImage;
    [SerializeField] Image manaBarImage;
    [SerializeField] static bool GameIsPaused = false;
    [SerializeField] GameObject pauseMenu;

    //Main Menu
    public void loadLevel(int level)
    {
        SceneManager.LoadScene(level);
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    //Pause Menu
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
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
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    

    //Sprint Bar
    public void UpdateSprintBar(float sprintFrac)
    {
        sprintBarImage.fillAmount = Mathf.Clamp(sprintFrac , 0, 1f);
    }

    //Mana Bar
    public void UpdateManaBar(float manaFrac)
    {
        manaBarImage.fillAmount = Mathf.Clamp(manaFrac, 0, 1f);
    }

    //Health Bra
    public void SetHealthBarColor(Color healthColor)
    {
        healthBarImage.color = healthColor;
    }

    public void UpdateHealthBar(float healthBar)
    {
        healthBarImage.fillAmount = Mathf.Clamp(healthBar, 0, 1f);
        if(healthBarImage.fillAmount < 0.2f)
        {
            SetHealthBarColor(Color.red);
        }
        else if(healthBarImage.fillAmount < 0.4f)
        {
            SetHealthBarColor(Color.yellow);
        }
        else
        {
            SetHealthBarColor(Color.green);
        }
    }
}
