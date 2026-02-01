using UnityEngine;
using UnityEngine.InputSystem;

public class MainMenuManager : MonoBehaviour
{
    [Header("Click Button Sound")]
    public AudioClip ClickedButtonSound;

    [Range(0.0f, 1.0f)] public float ClickedButtonSoundVolume = 1.0f;
    [Range(0.0f, 0.15f)] public float ClickedButtonSoundPitchVariance = 0.15f;

    [Header("Controller Connection Prompt")]
    public GameObject ConnectControllersPrompt;

    public void OnPlayButtonClicked()
    {
        Registry.CoreGameInfrastructureObject.Play_SFX_ExtendedOneShot(
            ClickedButtonSound,
            ClickedButtonSoundVolume * Registry.MasterVolume * Registry.SFXVolume,
            0.0f,
            Random.Range(1.0f - ClickedButtonSoundPitchVariance, 1.0f + ClickedButtonSoundPitchVariance));

        Registry.CoreGameInfrastructureObject.ChangeMenu(Constants.LEVEL_SELECTION_MENU);
    }

    public void OnCreditsButtonClicked()
    {
        Registry.CoreGameInfrastructureObject.Play_SFX_ExtendedOneShot(
            ClickedButtonSound,
            ClickedButtonSoundVolume * Registry.MasterVolume * Registry.SFXVolume,
            0.0f,
            Random.Range(1.0f - ClickedButtonSoundPitchVariance, 1.0f + ClickedButtonSoundPitchVariance));

        Registry.CoreGameInfrastructureObject.ChangeMenu(Constants.CREDITS_MENU);
    }

    public void OnSettingsButtonClicked()
    {
        Registry.CoreGameInfrastructureObject.Play_SFX_ExtendedOneShot(
            ClickedButtonSound,
            ClickedButtonSoundVolume * Registry.MasterVolume * Registry.SFXVolume,
            0.0f,
            Random.Range(1.0f - ClickedButtonSoundPitchVariance, 1.0f + ClickedButtonSoundPitchVariance));

        Registry.CoreGameInfrastructureObject.ChangeMenu(Constants.SETTINGS_MENU);
    }

    public void OnQuitButtonPressed()
    {
        Registry.CoreGameInfrastructureObject.Quit();
    }

    public void Start()
    {
        ConnectControllersPrompt.SetActive(Gamepad.all.Count < 2);
    }

    public void Update()
    {
        ConnectControllersPrompt.SetActive(Gamepad.all.Count < 2);
    }
}