using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuiControl : MonoBehaviour
{

    #region ShopInterface


    public Transform shop;
    public Transform playerGui;

    public Transform deathScreen;
    public Image blackScreenImage;
    public TextMeshProUGUI youDiedText;

    public Transform winScreen;
    public Image blackWinScreenImage;
    public TextMeshProUGUI youWinText;

    public void ShopAppear()
    {
        Cursor.lockState = CursorLockMode.None;
        shop.gameObject.SetActive(true);
        playerGui.gameObject.SetActive(false);
        Time.timeScale = 0;

    }
    public void ShopDisappear()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        shop.gameObject.SetActive(false);
        playerGui.gameObject.SetActive(true);


    }

    public void OnPlayerDeath()
    {
        deathScreen.gameObject.SetActive(true);
        StartCoroutine(fadeToBlackCoRoutine());
    }

    public IEnumerator fadeToBlackCoRoutine()
    {
        bool notFaded = true;
        float fadeSpeed = 0.4f;
        Color blackScreenImageColor = blackScreenImage.color;
        Color youDiedTextColor = youDiedText.color;
        while (notFaded)
        {
            blackScreenImageColor.a += 2 * fadeSpeed * Time.deltaTime;
            blackScreenImage.color = blackScreenImageColor;

            youDiedTextColor.a += fadeSpeed * Time.deltaTime;
            youDiedText.color = youDiedTextColor;
            notFaded = (youDiedTextColor.a < 1);

            yield return null;
        }
    }

    public void OnPlayerWin()
    {
        deathScreen.gameObject.SetActive(true);
        StartCoroutine(fadeToBlackWinCoRoutine());
    }

    public IEnumerator fadeToBlackWinCoRoutine()
    {
        bool notFaded = true;
        float fadeSpeed = 0.4f;
        Color blackScreenImageColor = blackWinScreenImage.color;
        Color youDiedTextColor = youWinText.color;
        while (notFaded)
        {
            blackScreenImageColor.a += 2 * fadeSpeed * Time.deltaTime;
            blackWinScreenImage.color = blackScreenImageColor;

            youDiedTextColor.a += fadeSpeed * Time.deltaTime;
            youWinText.color = youDiedTextColor;
            notFaded = (youDiedTextColor.a < 1);

            yield return null;
        }
    }

    #endregion


}
