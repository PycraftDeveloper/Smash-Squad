using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    public TMP_Text PlayerOneText;
    public TMP_Text PlayerTwoText;

    public Image PlayerOneIcon;
    public Image PlayerTwoIcon;

    public Sprite PausedSprite;
    public Sprite PlaySprite;

    [Header("Click Button Sound")]
    public AudioClip ClickedButtonSound;

    [Range(0.0f, 1.0f)] public float ClickedButtonSoundVolume = 1.0f;
    [Range(0.0f, 0.15f)] public float ClickedButtonSoundPitchVariance = 0.15f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        if (Registry.PlayerOnePaused)
        {
            PlayerOneIcon.sprite = PausedSprite;
        }
        else
        {
            PlayerOneIcon.sprite = PlaySprite;
        }

        if (Registry.PlayerTwoPaused)
        {
            PlayerTwoIcon.sprite = PausedSprite;
        }
        else
        {
            PlayerTwoIcon.sprite = PlaySprite;
        }

        if (Registry.PlayerOnePaused && !Registry.PlayerTwoPaused)
        {
            PlayerOneText.text = "Player One\n\nGame Paused\n\nPress B to resume";
            PlayerTwoText.text = "Player Two\n\nPlayer One paused\nthe game!";
        }
        else if (Registry.PlayerTwoPaused && !Registry.PlayerOnePaused)
        {
            PlayerOneText.text = "Player One\n\nPlayer Two paused\nthe game!";
            PlayerTwoText.text = "Player Two\n\nGame Paused\n\nPress B to resume";
        }
        else
        {
            PlayerOneText.text = "Player One\n\nGame Paused\n\nPress B to resume";
            PlayerTwoText.text = "Player Two\n\nGame Paused\n\nPress B to resume";
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (Registry.PlayerOneInput.startButton.IsPressed() && !Registry.PlayerOnePaused)
        {
            Registry.CoreGameInfrastructureObject.Play_SFX_ExtendedOneShot(
                ClickedButtonSound,
                ClickedButtonSoundVolume * Registry.MasterVolume * Registry.SFXVolume,
                0.0f,
                Random.Range(1.0f - ClickedButtonSoundPitchVariance, 1.0f + ClickedButtonSoundPitchVariance));

            Registry.PlayerOnePaused = true;
        }
        if (Registry.PlayerTwoInput.startButton.IsPressed() && !Registry.PlayerTwoPaused)
        {
            Registry.CoreGameInfrastructureObject.Play_SFX_ExtendedOneShot(
                ClickedButtonSound,
                ClickedButtonSoundVolume * Registry.MasterVolume * Registry.SFXVolume,
                0.0f,
                Random.Range(1.0f - ClickedButtonSoundPitchVariance, 1.0f + ClickedButtonSoundPitchVariance));

            Registry.PlayerTwoPaused = true;
        }

        if (Registry.PlayerOneInput.bButton.IsPressed() && Registry.PlayerOnePaused)
        {
            Registry.CoreGameInfrastructureObject.Play_SFX_ExtendedOneShot(
                ClickedButtonSound,
                ClickedButtonSoundVolume * Registry.MasterVolume * Registry.SFXVolume,
                0.0f,
                Random.Range(1.0f - ClickedButtonSoundPitchVariance, 1.0f + ClickedButtonSoundPitchVariance));

            Registry.PlayerOnePaused = false;
        }
        if (Registry.PlayerTwoInput.bButton.IsPressed() && Registry.PlayerTwoPaused)
        {
            Registry.CoreGameInfrastructureObject.Play_SFX_ExtendedOneShot(
                ClickedButtonSound,
                ClickedButtonSoundVolume * Registry.MasterVolume * Registry.SFXVolume,
                0.0f,
                Random.Range(1.0f - ClickedButtonSoundPitchVariance, 1.0f + ClickedButtonSoundPitchVariance));

            Registry.PlayerTwoPaused = false;
        }

        if (!Registry.PlayerOnePaused && !Registry.PlayerTwoPaused)
        {
            Registry.GamePaused = false;
            Registry.CoreGameInfrastructureObject.CloseMenu();
        }

        if (Registry.PlayerOnePaused && !Registry.PlayerTwoPaused)
        {
            PlayerOneText.text = "Player One\n\nGame Paused\n\nPress B to resume";
            PlayerTwoText.text = "Player Two\n\nPlayer One paused\nthe game!";
        }
        else if (Registry.PlayerTwoPaused && !Registry.PlayerOnePaused)
        {
            PlayerOneText.text = "Player One\n\nPlayer Two paused\nthe game!";
            PlayerTwoText.text = "Player Two\n\nGame Paused\n\nPress B to resume";
        }
        else
        {
            PlayerOneText.text = "Player One\n\nGame Paused\n\nPress B to resume";
            PlayerTwoText.text = "Player Two\n\nGame Paused\n\nPress B to resume";
        }
    }
}