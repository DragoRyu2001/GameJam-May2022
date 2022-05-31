using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField]Slider audioVolume;
    void Start()
    {
        audioVolume.value = AudioListener.volume;
    }
    public void SetAudioVolume(Slider val)
    {
        AudioListener.volume = val.value;
    }

    //Main Menu
    public void loadLevel(int level)
    {
        if(GameManager.instance!=null)
            Destroy(GameManager.instance.gameObject);
        SceneManager.LoadScene(level);
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    public void BackToMainMenu()
    {
        if(GameManager.instance!=null)
            Destroy(GameManager.instance.gameObject);
        SceneManager.LoadScene(0);
    }
    public void GlobalVolume(float _volume)
    {
        AudioListener.volume = _volume;
    }
}
