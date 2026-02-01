using UnityEngine;
using UnityEngine.InputSystem;

public class CreditsMenuManager : MonoBehaviour
{
    [Header("Click Button Sound")]
    public AudioClip ClickedButtonSound;

    [Range(0.0f, 1.0f)] public float ClickedButtonSoundVolume = 1.0f;
    [Range(0.0f, 0.15f)] public float ClickedButtonSoundPitchVariance = 0.15f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
    }

    private void Update()
    {
        if (Gamepad.current != null && Gamepad.current.buttonEast.wasPressedThisFrame)
        {
            Registry.CoreGameInfrastructureObject.ChangeMenu(Constants.MAIN_MENU);
        }
    }

    public void OnBackButtonPressed()
    {
        Registry.CoreGameInfrastructureObject.Play_SFX_ExtendedOneShot(
            ClickedButtonSound,
            ClickedButtonSoundVolume * Registry.MasterVolume * Registry.SFXVolume,
            0.0f,
            Random.Range(1.0f - ClickedButtonSoundPitchVariance, 1.0f + ClickedButtonSoundPitchVariance));

        Registry.CoreGameInfrastructureObject.ChangeMenu(Constants.MAIN_MENU);
    }
}