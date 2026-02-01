using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class PlayerOneClassSelectionMenuManager : MonoBehaviour
{
    public PlayerInput PlayerOneInput;

    public EventSystem PlayerOneEventSystem;

    public InputSystemUIInputModule PlayerOne_InputSystem;

    public List<Button> CharacterSelectButtons;

    [Header("Click Button Sound")]
    public AudioClip ClickedButtonSound;

    [Range(0.0f, 1.0f)] public float ClickedButtonSoundVolume = 1.0f;
    [Range(0.0f, 0.15f)] public float ClickedButtonSoundPitchVariance = 0.15f;

    private void Start()
    {
        PlayerOneInput.SwitchCurrentControlScheme("Gamepad", Registry.PlayerOneInput);

        PlayerOne_InputSystem.actionsAsset = PlayerOneInput.actions;

        PlayerOne_InputSystem.point = InputActionReference.Create(PlayerOneInput.actions.FindAction("Point"));
        PlayerOne_InputSystem.move = InputActionReference.Create(PlayerOneInput.actions.FindAction("Navigate"));
        PlayerOne_InputSystem.submit = InputActionReference.Create(PlayerOneInput.actions.FindAction("Submit"));
        PlayerOne_InputSystem.cancel = InputActionReference.Create(PlayerOneInput.actions.FindAction("Cancel"));
        PlayerOne_InputSystem.leftClick = InputActionReference.Create(PlayerOneInput.actions.FindAction("Click"));
        PlayerOne_InputSystem.rightClick = InputActionReference.Create(PlayerOneInput.actions.FindAction("RightClick"));
        PlayerOne_InputSystem.middleClick = InputActionReference.Create(PlayerOneInput.actions.FindAction("MiddleClick"));
        PlayerOne_InputSystem.scrollWheel = InputActionReference.Create(PlayerOneInput.actions.FindAction("ScrollWheel"));
        PlayerOne_InputSystem.trackedDevicePosition = InputActionReference.Create(PlayerOneInput.actions.FindAction("TrackedDevicePosition"));
        PlayerOne_InputSystem.trackedDeviceOrientation = InputActionReference.Create(PlayerOneInput.actions.FindAction("TrackedDeviceOrientation"));

        PlayerOneEventSystem.SetSelectedGameObject(CharacterSelectButtons[Random.Range(0, CharacterSelectButtons.Count)].gameObject);
    }

    public void OnPlayerOneLaunchClassSelected()
    {
        Registry.CoreGameInfrastructureObject.Play_SFX_ExtendedOneShot(
            ClickedButtonSound,
            ClickedButtonSoundVolume * Registry.MasterVolume * Registry.SFXVolume,
            0.0f,
            Random.Range(1.0f - ClickedButtonSoundPitchVariance, 1.0f + ClickedButtonSoundPitchVariance));

        Registry.PlayerOneSelectedClass = Constants.LAUNCH_CLASS;
        Registry.CoreGameInfrastructureObject.ChangeMenu(Constants.PLAYER_TWO_CLASS_SELECTION_MENU);
    }

    public void OnPlayerOneWarpClassSelected()
    {
        Registry.CoreGameInfrastructureObject.Play_SFX_ExtendedOneShot(
            ClickedButtonSound,
            ClickedButtonSoundVolume * Registry.MasterVolume * Registry.SFXVolume,
            0.0f,
            Random.Range(1.0f - ClickedButtonSoundPitchVariance, 1.0f + ClickedButtonSoundPitchVariance));

        Registry.PlayerOneSelectedClass = Constants.WARP_CLASS;
        Registry.CoreGameInfrastructureObject.ChangeMenu(Constants.PLAYER_TWO_CLASS_SELECTION_MENU);
    }

    public void OnPlayerOneGeomancyClassSelected()
    {
        Registry.CoreGameInfrastructureObject.Play_SFX_ExtendedOneShot(
            ClickedButtonSound,
            ClickedButtonSoundVolume * Registry.MasterVolume * Registry.SFXVolume,
            0.0f,
            Random.Range(1.0f - ClickedButtonSoundPitchVariance, 1.0f + ClickedButtonSoundPitchVariance));

        Registry.PlayerOneSelectedClass = Constants.GEOMANCY_CLASS;
        Registry.CoreGameInfrastructureObject.ChangeMenu(Constants.PLAYER_TWO_CLASS_SELECTION_MENU);
    }

    public void OnPlayerOneNinjaClassSelected()
    {
        Registry.CoreGameInfrastructureObject.Play_SFX_ExtendedOneShot(
            ClickedButtonSound,
            ClickedButtonSoundVolume * Registry.MasterVolume * Registry.SFXVolume,
            0.0f,
            Random.Range(1.0f - ClickedButtonSoundPitchVariance, 1.0f + ClickedButtonSoundPitchVariance));

        Registry.PlayerOneSelectedClass = Constants.NINJA_CLASS;
        Registry.CoreGameInfrastructureObject.ChangeMenu(Constants.PLAYER_TWO_CLASS_SELECTION_MENU);
    }
}