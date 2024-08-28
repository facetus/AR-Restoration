using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsController : MonoBehaviour
{
    public static event Action<bool> OnPause;


    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject exitPanel;


    public void PausePanel()
    {
        settingsPanel.SetActive(true);
        OnPause?.Invoke(true);
    }

    public void ExitPanel()
    {
        exitPanel.SetActive(true);
        OnPause?.Invoke(true);
    }

    public void ResumeGame()
    {
        settingsPanel.SetActive(false);
        exitPanel.SetActive(false);
        OnPause?.Invoke(false);
    }

    public void ExitGame()
    {
        //Exit from the game;
        Destroy(transform.root.gameObject);
    }
}
