using TMPro;
using UnityEngine;

public class GameOverMenuManager : MonoBehaviour
{
    public TMP_Text WhoWonText;

    [Header("Click Button Sound")]
    public AudioClip ClickedButtonSound;

    [Range(0.0f, 1.0f)] public float ClickedButtonSoundVolume = 1.0f;
    [Range(0.0f, 0.15f)] public float ClickedButtonSoundPitchVariance = 0.15f;

    public void Start()
    {
        if (Registry.GameOver == Constants.PLAYER_ONE)
        {
            WhoWonText.text = "Player one wins!";
        }
        else
        {
            WhoWonText.text = "Player two wins!";
        }

        Registry.PlayerOneGameObject.enabled = false;
        Registry.PlayerTwoGameObject.enabled = false;

        Registry.PlayerOneGameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        Registry.PlayerTwoGameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
    }

    public void OnPlayAgainButtonClicked()
    {
        Registry.CoreGameInfrastructureObject.Play_SFX_ExtendedOneShot(
            ClickedButtonSound,
            ClickedButtonSoundVolume * Registry.MasterVolume * Registry.SFXVolume,
            0.0f,
            Random.Range(1.0f - ClickedButtonSoundPitchVariance, 1.0f + ClickedButtonSoundPitchVariance));

        Registry.PlayerOneGameObject.enabled = true;
        Registry.PlayerTwoGameObject.enabled = true;

        Registry.PlayerOneGameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        Registry.PlayerTwoGameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;

        Registry.PlayerOneGameObject.StartPlay();
        Registry.PlayerTwoGameObject.StartPlay();
        Registry.CurrentGameManager.StartPlay();

        Registry.GamePaused = false;
        Registry.GameOver = null;
        Registry.CoreGameInfrastructureObject.CloseMenu();
    }

    public void OnMainMenuButtonClicked()
    {
        Registry.CoreGameInfrastructureObject.Play_SFX_ExtendedOneShot(
            ClickedButtonSound,
            ClickedButtonSoundVolume * Registry.MasterVolume * Registry.SFXVolume,
            0.0f,
            Random.Range(1.0f - ClickedButtonSoundPitchVariance, 1.0f + ClickedButtonSoundPitchVariance));

        Registry.GamePaused = false;
        Registry.GameOver = null;
        Registry.CoreGameInfrastructureObject.ChangeScene(Constants.MENU_SCENE);
        Registry.CoreGameInfrastructureObject.ChangeMenu(Constants.MAIN_MENU);
    }

    public void OnQuitButtonPressed()
    {
        Registry.CoreGameInfrastructureObject.Quit();
    }
}