using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Slider volumeSlider;
    private AudioSource audioSource;

    public void onSliderChange()
    {
        GameManager.masterVolume = volumeSlider.value;
        audioSource.volume = volumeSlider.value;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
        Cursor.lockState = CursorLockMode.None;
        volumeSlider.value = volumeSlider.maxValue = 1f;
    }
    public void PlayGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void QuitGame()
    {
        Debug.Log("exiting game");
        Application.Quit();
    }
}
