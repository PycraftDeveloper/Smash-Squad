using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SettingsMenuManager : MonoBehaviour
{
    [Header("Sliders")]
    public Slider MasterVolumeSlider;

    public Slider MusicVolumeSlider;
    public Slider SFXVolumeSlider;
    public Slider P1_X_CamSens;
    public Slider P1_Y_CamSens;
    public Slider P2_X_CamSens;
    public Slider P2_Y_CamSens;

    [Header("Toggles")]
    public Toggle P1_X_InvertAxis;

    public Toggle P1_Y_InvertAxis;
    public Toggle P2_X_InvertAxis;
    public Toggle P2_Y_InvertAxis;

    [Header("Slider Titles")]
    public TMP_Text MasterVolumeTitle;

    public TMP_Text MusicVolumeTitle;
    public TMP_Text SFXVolumeTitle;

    [Header("Click Button Sound")]
    public AudioClip ClickedButtonSound;

    [Range(0.0f, 1.0f)] public float ClickedButtonSoundVolume = 1.0f;
    [Range(0.0f, 0.15f)] public float ClickedButtonSoundPitchVariance = 0.15f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        MasterVolumeSlider.value = Registry.MasterVolume;
        MusicVolumeSlider.value = Registry.MusicVolume;
        SFXVolumeSlider.value = Registry.SFXVolume;

        P1_X_CamSens.value = Registry.P1_X_CamSense;
        P1_Y_CamSens.value = Registry.P1_Y_CamSense;
        P2_X_CamSens.value = Registry.P2_X_CamSense;
        P2_Y_CamSens.value = Registry.P2_Y_CamSense;

        P1_X_InvertAxis.isOn = Registry.P1_X_InvertAxis;
        P1_Y_InvertAxis.isOn = Registry.P1_Y_InvertAxis;
        P2_X_InvertAxis.isOn = Registry.P2_X_InvertAxis;
        P2_Y_InvertAxis.isOn = Registry.P2_Y_InvertAxis;
    }

    public void OnMasterVolumeChanged()
    {
        Registry.MasterVolume = MasterVolumeSlider.value;
        MasterVolumeTitle.text = "Master Volume: " + Mathf.RoundToInt(Registry.MasterVolume * 100) + "%";
    }

    public void OnMusicVolumeChanged()
    {
        Registry.MusicVolume = MusicVolumeSlider.value;
        MusicVolumeTitle.text = "Music Volume: " + Mathf.RoundToInt(Registry.MusicVolume * 100) + "%";
    }

    public void OnSFXVolumeChanged()
    {
        Registry.SFXVolume = SFXVolumeSlider.value;
        SFXVolumeTitle.text = "SFX Volume: " + Mathf.RoundToInt(Registry.SFXVolume * 100) + "%";
    }

    public void On_P1_X_CamSensChanged()
    {
        Registry.P1_X_CamSense = P1_X_CamSens.value;
    }

    public void On_P1_Y_CamSensChanged()
    {
        Registry.P1_Y_CamSense = P1_Y_CamSens.value;
    }

    public void On_P2_X_CamSensChanged()
    {
        Registry.P2_X_CamSense = P2_X_CamSens.value;
    }

    public void On_P2_Y_CamSensChanged()
    {
        Registry.P2_Y_CamSense = P2_Y_CamSens.value;
    }

    public void On_P1_X_InvertAxisChanged()
    {
        Registry.P1_X_InvertAxis = P1_X_InvertAxis.isOn;
    }

    public void On_P1_Y_InvertAxisChanged()
    {
        Registry.P1_Y_InvertAxis = P1_Y_InvertAxis.isOn;
    }

    public void On_P2_X_InvertAxisChanged()
    {
        Registry.P2_X_InvertAxis = P2_X_InvertAxis.isOn;
    }

    public void On_P2_Y_InvertAxisChanged()
    {
        Registry.P2_Y_InvertAxis = P2_Y_InvertAxis.isOn;
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