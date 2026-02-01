using UnityEngine;
using UnityEngine.InputSystem;

public class GameSceneManager : MonoBehaviour
{
    public GameObject PlayerOne;
    public GameObject PlayerTwo;

    public GameObject LaunchCharacter;
    public GameObject WarpCharacterModel;
    public GameObject GeomancyCharacterModel;
    public GameObject NinjaCharacterModel;

    public GameObject FireballPrefab;
    public GameObject FireballParent;

    private float RunTime = 0.0f;
    public float FireBallSpawnStartTime = 120.0f;
    public float FireBallLeadUpTime = 420.0f;

    public void StartPlay()
    {
        RunTime = 0.0f;
        foreach (Transform child in FireballParent.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void Start()
    {
        Registry.CurrentGameManager = this;

        InputSystem.onDeviceChange += OnDeviceChange;

        StartPlay();
    }

    public void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (device == Registry.PlayerOneInput)
        {
            switch (change)
            {
                case InputDeviceChange.Removed:
                    Registry.PlayerOneInput = null;
                    Registry.ControllerDisconnect = true;
                    Registry.GamePaused = true;
                    Registry.CoreGameInfrastructureObject.ChangeMenu(Constants.CLICK_TO_JOIN_MENU);
                    break;
            }
        }
        else if (device == Registry.PlayerTwoInput)
        {
            switch (change)
            {
                case InputDeviceChange.Removed:
                    Registry.PlayerTwoInput = null;
                    Registry.ControllerDisconnect = true;
                    Registry.GamePaused = true;
                    Registry.CoreGameInfrastructureObject.ChangeMenu(Constants.CLICK_TO_JOIN_MENU);
                    break;
            }
        }
    }

    private void Awake()
    {
        if (Registry.PlayerOneSelectedClass == Constants.LAUNCH_CLASS)
        {
            Instantiate(LaunchCharacter, PlayerOne.transform);
        }
        else if (Registry.PlayerOneSelectedClass == Constants.WARP_CLASS)
        {
            Instantiate(WarpCharacterModel, PlayerOne.transform);
        }
        else if (Registry.PlayerOneSelectedClass == Constants.GEOMANCY_CLASS)
        {
            Instantiate(GeomancyCharacterModel, PlayerOne.transform);
        }
        else if (Registry.PlayerOneSelectedClass == Constants.NINJA_CLASS)
        {
            Instantiate(NinjaCharacterModel, PlayerOne.transform);
        }

        if (Registry.PlayerTwoSelectedClass == Constants.LAUNCH_CLASS)
        {
            Instantiate(LaunchCharacter, PlayerTwo.transform);
        }
        else if (Registry.PlayerTwoSelectedClass == Constants.WARP_CLASS)
        {
            Instantiate(WarpCharacterModel, PlayerTwo.transform);
        }
        else if (Registry.PlayerTwoSelectedClass == Constants.GEOMANCY_CLASS)
        {
            Instantiate(GeomancyCharacterModel, PlayerTwo.transform);
        }
        else if (Registry.PlayerTwoSelectedClass == Constants.NINJA_CLASS)
        {
            Instantiate(NinjaCharacterModel, PlayerTwo.transform);
        }
    }

    public void Update()
    {
        if (Registry.GamePaused)
        {
            return;
        }

        RunTime += Time.deltaTime;

        if (RunTime > FireBallSpawnStartTime)
        {
            float SpawnChance = Random.Range(0.0f, 1.0f);
            if (SpawnChance < (RunTime - FireBallSpawnStartTime) / (FireBallLeadUpTime - FireBallSpawnStartTime))
            {
                int SpawnNumber = Random.Range(0, 10);
                if (SpawnNumber == 0)
                {
                    Vector3 RandomPosition = new Vector3(
                        Random.Range(-250.0f, 250.0f),
                        100.0f,
                        Random.Range(-250.0f, 250.0f));

                    GameObject SpawnedFireBall = Instantiate(FireballPrefab, RandomPosition, transform.rotation);
                    SpawnedFireBall.transform.parent = FireballParent.transform;
                }
            }
        }

        if (Registry.PlayerOnePaused || Registry.PlayerTwoPaused)
        {
            Registry.GamePaused = true;
            Registry.CoreGameInfrastructureObject.ChangeMenu(Constants.PAUSE_MENU);
        }
    }
}