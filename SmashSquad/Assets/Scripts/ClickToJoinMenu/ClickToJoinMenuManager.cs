using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

public class ClickToJoinMenuManager : MonoBehaviour
{
    public TMP_Text PlayerOneText;
    public TMP_Text PlayerTwoText;

    public Image PlayerOneSideBackgroundImage;
    public Image PlayerTwoSideBackgroundImage;

    [Range(0.05f, 0.5f)]
    public float AnalogDeadzone = 0.2f;

    [Header("Click Button Sound")]
    public AudioClip ClickedButtonSound;

    [Range(0.0f, 1.0f)] public float ClickedButtonSoundVolume = 1.0f;
    [Range(0.0f, 0.15f)] public float ClickedButtonSoundPitchVariance = 0.15f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        if (!Registry.ControllerDisconnect)
        {
            Registry.PlayerOneInput = null;
            Registry.PlayerTwoInput = null;
        }
        OnJoin();
    }

    private void OnJoin()
    {
        if (Registry.PlayerOneInput == null)
        {
            PlayerOneText.text = "Player One\n\nPress any button to join";
            PlayerTwoText.text = "Player Two\n\nWait for player one to join";
            PlayerOneSideBackgroundImage.color = new Color(0, 0, 0, 0);
            PlayerTwoSideBackgroundImage.color = new Color(0, 0, 0, 0.69f);
        }
        else if (Registry.PlayerTwoInput == null)
        {
            PlayerOneText.text = "Player One\n\nReady to play";
            PlayerTwoText.text = "Player Two\n\nPress any button to join";
            PlayerOneSideBackgroundImage.color = new Color(0, 0, 0, 0.69f);
            PlayerTwoSideBackgroundImage.color = new Color(0, 0, 0, 0);
        }
        else
        {
            if (Registry.ControllerDisconnect)
            {
                Registry.ControllerDisconnect = false;
                Registry.GamePaused = false;
                Registry.CoreGameInfrastructureObject.CloseMenu();
            }
            else
            {
                Registry.CoreGameInfrastructureObject.ChangeMenu(Constants.PLAYER_ONE_CLASS_SELECTION_MENU);
            }
        }
    }

    private bool IsGamepadInteracted(Gamepad gamepad)
    {
        foreach (var control in gamepad.allControls)
        {
            if (control is ButtonControl btn && btn.wasPressedThisFrame)
                return true;
        }

        if (Mathf.Abs(gamepad.leftStick.ReadValue().x) > AnalogDeadzone ||
            Mathf.Abs(gamepad.leftStick.ReadValue().y) > AnalogDeadzone ||
            Mathf.Abs(gamepad.rightStick.ReadValue().x) > AnalogDeadzone ||
            Mathf.Abs(gamepad.rightStick.ReadValue().y) > AnalogDeadzone ||
            gamepad.leftTrigger.ReadValue() > AnalogDeadzone ||
            gamepad.rightTrigger.ReadValue() > AnalogDeadzone)
        {
            return true;
        }

        return false;
    }

    private IEnumerator FirstSelectedGamePadRumble(Gamepad SelectedGamePad)
    {
        SelectedGamePad.SetMotorSpeeds(0.5f, 0.5f);
        yield return new WaitForSeconds(0.75f);
        SelectedGamePad.SetMotorSpeeds(0, 0);
    }

    private IEnumerator SecondSelectedGamePadRumble(Gamepad SelectedGamePad)
    {
        SelectedGamePad.SetMotorSpeeds(0.5f, 0.5f);
        yield return new WaitForSeconds(0.25f);
        SelectedGamePad.SetMotorSpeeds(0, 0);
        yield return new WaitForSeconds(0.25f);
        SelectedGamePad.SetMotorSpeeds(0.5f, 0.5f);
        yield return new WaitForSeconds(0.25f);
        SelectedGamePad.SetMotorSpeeds(0, 0);
    }

    private void StopAllRumble()
    {
        foreach (var CurrentGamePad in Gamepad.all)
        {
            CurrentGamePad.SetMotorSpeeds(0, 0);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (Registry.PlayerOneInput == null || Registry.PlayerTwoInput == null)
        {
            var ConnectedGamePads = Gamepad.all;

            for (int i = 0; i < ConnectedGamePads.Count; i++)
            {
                Gamepad CurrentGamePad = ConnectedGamePads[i];
                if (CurrentGamePad == Registry.PlayerOneInput || CurrentGamePad == Registry.PlayerTwoInput)
                {
                    continue;
                }
                if (IsGamepadInteracted(CurrentGamePad))
                {
                    if (Registry.PlayerOneInput == null)
                    {
                        Registry.CoreGameInfrastructureObject.Play_SFX_ExtendedOneShot(
                            ClickedButtonSound,
                            ClickedButtonSoundVolume * Registry.MasterVolume * Registry.SFXVolume,
                            0.0f,
                            Random.Range(1.0f - ClickedButtonSoundPitchVariance, 1.0f + ClickedButtonSoundPitchVariance));

                        Registry.PlayerOneInput = CurrentGamePad;
                        Registry.CoreGameInfrastructureObject.StartCoroutine(FirstSelectedGamePadRumble(Registry.PlayerOneInput));
                        OnJoin();
                    }
                    else if (Registry.PlayerTwoInput == null)
                    {
                        Registry.CoreGameInfrastructureObject.Play_SFX_ExtendedOneShot(
                            ClickedButtonSound,
                            ClickedButtonSoundVolume * Registry.MasterVolume * Registry.SFXVolume,
                            0.0f,
                            Random.Range(1.0f - ClickedButtonSoundPitchVariance, 1.0f + ClickedButtonSoundPitchVariance));

                        Registry.PlayerTwoInput = CurrentGamePad;
                        Registry.CoreGameInfrastructureObject.StartCoroutine(SecondSelectedGamePadRumble(Registry.PlayerTwoInput));
                        OnJoin();
                    }

                    if (Registry.PlayerOneInput != null && Registry.PlayerTwoInput != null)
                        break;
                }
            }
        }
    }
}