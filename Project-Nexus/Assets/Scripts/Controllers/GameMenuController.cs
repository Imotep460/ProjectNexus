using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class GameMenuController : MonoBehaviour
{
    [Header("Components:")]
    public TextMeshProUGUI volumeText;
    public AudioMixer masterVolume;
    public TMP_Dropdown resolutionSettings;
    public TextMeshProUGUI mouseSenseText;
    public static float MouseSense;
    private GameObject gameMenu;

    [Header("AvailableResolutions:")]
    Resolution[] resolutions;

    private void Start()
    {
        gameMenu = transform.GetChild(0).gameObject;
        gameMenu.SetActive(false);

        // Get the available resolutions.
        resolutions = Screen.resolutions;
        // Clear the dropdown manu.
        resolutionSettings.ClearOptions();
        int currentResolutionIndex = 0;
        // Convert the resolutions array into a List:
        List<string> resolutionOptions = new List<string>();
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            resolutionOptions.Add(option);

            // Find the resolution option that matches the Screens current Resolution.
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        // Fill out the resolition dropdown menu:
        resolutionSettings.AddOptions(resolutionOptions);
        // Display the current resolution.
        resolutionSettings.value = currentResolutionIndex;
        resolutionSettings.RefreshShownValue();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            gameMenu.SetActive(!gameMenu.activeSelf);

            if (gameMenu.activeSelf == true)
            {
                Cursor.lockState = CursorLockMode.None;
            }

            if (gameMenu.activeSelf == false)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

    /// <summary>
    /// Update the masterVolume for the game.
    /// Also updates the text in the Menu to show the volume percentage.
    /// </summary>
    /// <param name="newVolume">New Volume value.</param>
    public void ChangeVolume(float newVolume)
    {
        volumeText.text = "Volume: " + (int)(newVolume + 80);
        masterVolume.SetFloat("MasterVolume", newVolume);
    }

    /// <summary>
    /// Changes the games graphics quality.
    /// </summary>
    /// <param name="qualityIndex">The new graphics wuality index.</param>
    public void GameQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    /// <summary>
    /// Change the game between FullScreen and not FullScreen status.
    /// </summary>
    /// <param name="isFullScreen">References a bool toggle, true == game is FullScreen, false == game is Windowed.</param>
    public void FullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }

    public void SetGameResolution(int resolutionIndex)
    {
        Screen.SetResolution(resolutions[resolutionIndex].width, resolutions[resolutionIndex].height, Screen.fullScreen);
    }

    public void ChangeMouseSense(float newMouseSense)
    {
        MouseSense = newMouseSense;
        mouseSenseText.text = "Mouse Sense: " + Math.Round(MouseSense, 2);
    }

    public void ResumeGame()
    {
        // Disable the mouse:
        gameMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
