using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class PlayerTwoClassSelectionMenuManager : MonoBehaviour
{
    public PlayerInput PlayerTwoInput;

    public EventSystem PlayerTwoEventSystem;

    public InputSystemUIInputModule PlayerTwo_InputSystem;

    public List<Button> CharacterSelectButtons;

    [Header("Click Button Sound")]
    public AudioClip ClickedButtonSound;

    [Range(0.0f, 1.0f)] public float ClickedButtonSoundVolume = 1.0f;
    [Range(0.0f, 0.15f)] public float ClickedButtonSoundPitchVariance = 0.15f;

    private void Start()
    {
        PlayerTwoInput.SwitchCurrentControlScheme("Gamepad", Registry.PlayerTwoInput);

        PlayerTwo_InputSystem.actionsAsset = PlayerTwoInput.actions;

        PlayerTwo_InputSystem.point = InputActionReference.Create(PlayerTwoInput.actions.FindAction("Point"));
        PlayerTwo_InputSystem.move = InputActionReference.Create(PlayerTwoInput.actions.FindAction("Navigate"));
        PlayerTwo_InputSystem.submit = InputActionReference.Create(PlayerTwoInput.actions.FindAction("Submit"));
        PlayerTwo_InputSystem.cancel = InputActionReference.Create(PlayerTwoInput.actions.FindAction("Cancel"));
        PlayerTwo_InputSystem.leftClick = InputActionReference.Create(PlayerTwoInput.actions.FindAction("Click"));
        PlayerTwo_InputSystem.rightClick = InputActionReference.Create(PlayerTwoInput.actions.FindAction("RightClick"));
        PlayerTwo_InputSystem.middleClick = InputActionReference.Create(PlayerTwoInput.actions.FindAction("MiddleClick"));
        PlayerTwo_InputSystem.scrollWheel = InputActionReference.Create(PlayerTwoInput.actions.FindAction("ScrollWheel"));
        PlayerTwo_InputSystem.trackedDevicePosition = InputActionReference.Create(PlayerTwoInput.actions.FindAction("TrackedDevicePosition"));
        PlayerTwo_InputSystem.trackedDeviceOrientation = InputActionReference.Create(PlayerTwoInput.actions.FindAction("TrackedDeviceOrientation"));

        PlayerTwoEventSystem.SetSelectedGameObject(CharacterSelectButtons[Random.Range(0, CharacterSelectButtons.Count)].gameObject);
    }

    public void OnPlayerTwoLaunchClassSelected()
    {
        Registry.CoreGameInfrastructureObject.Play_SFX_ExtendedOneShot(
            ClickedButtonSound,
            ClickedButtonSoundVolume * Registry.MasterVolume * Registry.SFXVolume,
            0.0f,
            Random.Range(1.0f - ClickedButtonSoundPitchVariance, 1.0f + ClickedButtonSoundPitchVariance));

        Registry.PlayerTwoSelectedClass = Constants.LAUNCH_CLASS;
        Registry.CoreGameInfrastructureObject.CloseMenu();
        StartCoroutine(WaitForSceneLoad());
    }

    public void OnPlayerTwoWarpClassSelected()
    {
        Registry.CoreGameInfrastructureObject.Play_SFX_ExtendedOneShot(
            ClickedButtonSound,
            ClickedButtonSoundVolume * Registry.MasterVolume * Registry.SFXVolume,
            0.0f,
            Random.Range(1.0f - ClickedButtonSoundPitchVariance, 1.0f + ClickedButtonSoundPitchVariance));

        Registry.PlayerTwoSelectedClass = Constants.WARP_CLASS;
        Registry.CoreGameInfrastructureObject.CloseMenu();
        StartCoroutine(WaitForSceneLoad());
    }

    public void OnPlayerTwoGeomancyClassSelected()
    {
        Registry.CoreGameInfrastructureObject.Play_SFX_ExtendedOneShot(
            ClickedButtonSound,
            ClickedButtonSoundVolume * Registry.MasterVolume * Registry.SFXVolume,
            0.0f,
            Random.Range(1.0f - ClickedButtonSoundPitchVariance, 1.0f + ClickedButtonSoundPitchVariance));

        Registry.PlayerTwoSelectedClass = Constants.GEOMANCY_CLASS;
        Registry.CoreGameInfrastructureObject.CloseMenu();
        StartCoroutine(WaitForSceneLoad());
    }

    public void OnPlayerTwoNinjaClassSelected()
    {
        Registry.CoreGameInfrastructureObject.Play_SFX_ExtendedOneShot(
            ClickedButtonSound,
            ClickedButtonSoundVolume * Registry.MasterVolume * Registry.SFXVolume,
            0.0f,
            Random.Range(1.0f - ClickedButtonSoundPitchVariance, 1.0f + ClickedButtonSoundPitchVariance));

        Registry.PlayerTwoSelectedClass = Constants.NINJA_CLASS;
        Registry.CoreGameInfrastructureObject.CloseMenu();
        StartCoroutine(WaitForSceneLoad());
    }

    private IEnumerator WaitForSceneLoad()
    {
        bool DoRun = true;
        do
        {
            if (Registry.MapLoaded != Constants.MAP_NOT_LOADED)
            {
                DoRun = false;
                Registry.CoreGameInfrastructureObject.ChangeScene(Constants.GAME_SCENE);
            }
            yield return null;
        } while (DoRun);
    }
}