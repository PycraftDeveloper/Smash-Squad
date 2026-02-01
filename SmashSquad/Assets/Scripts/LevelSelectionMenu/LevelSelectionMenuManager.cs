using UnityEngine;
using UnityEngine.InputSystem;

public class LevelSelectionMenuManager : MonoBehaviour
{
    [Header("Click Button Sound")]
    public AudioClip ClickedButtonSound;

    [Range(0.0f, 1.0f)] public float ClickedButtonSoundVolume = 1.0f;
    [Range(0.0f, 0.15f)] public float ClickedButtonSoundPitchVariance = 0.15f;

    public void OnCandylandButtonPressed()
    {
        Registry.CoreGameInfrastructureObject.Play_SFX_ExtendedOneShot(
            ClickedButtonSound,
            ClickedButtonSoundVolume * Registry.MasterVolume * Registry.SFXVolume,
            0.0f,
            Random.Range(1.0f - ClickedButtonSoundPitchVariance, 1.0f + ClickedButtonSoundPitchVariance));

        if (Registry.MapLoaded == Constants.CANDYLAND_MAP)
        {
            DontDestroyOnLoad(Registry.Map);
        }
        else
        {
            Registry.CoreGameInfrastructureObject.LoadMap(Constants.CANDYLAND_MAP);
        }
        Registry.CoreGameInfrastructureObject.ChangeMenu(Constants.CLICK_TO_JOIN_MENU);
    }

    public void OnTheForbiddenButtonPressed()
    {
        Registry.CoreGameInfrastructureObject.Play_SFX_ExtendedOneShot(
            ClickedButtonSound,
            ClickedButtonSoundVolume * Registry.MasterVolume * Registry.SFXVolume,
            0.0f,
            Random.Range(1.0f - ClickedButtonSoundPitchVariance, 1.0f + ClickedButtonSoundPitchVariance));

        if (Registry.MapLoaded == Constants.THE_FORBIDDEN_MAP)
        {
            DontDestroyOnLoad(Registry.Map);
        }
        else
        {
            Registry.CoreGameInfrastructureObject.LoadMap(Constants.THE_FORBIDDEN_MAP);
        }
        Registry.CoreGameInfrastructureObject.ChangeMenu(Constants.CLICK_TO_JOIN_MENU);
    }

    private void Update()
    {
        if (Gamepad.current != null && Gamepad.current.buttonEast.wasPressedThisFrame)
        {
            Registry.CoreGameInfrastructureObject.ChangeMenu(Constants.MAIN_MENU);
        }
    }
}