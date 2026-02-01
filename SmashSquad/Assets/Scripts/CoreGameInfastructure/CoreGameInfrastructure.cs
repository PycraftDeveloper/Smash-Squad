using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

public class CoreGameInfrastructure : MonoBehaviour
{
    [Header("Menu Prefabs")]
    public GameObject MainMenuPrefab;

    public GameObject SettingsMenuPrefab;
    public GameObject CreditsMenuPrefab;
    public GameObject LevelSelectionMenuPrefab;
    public GameObject ClickToJoinMenuPrefab;
    public GameObject PlayerOneClassSelectionMenuPrefab;
    public GameObject PlayerTwoClassSelectionMenuPrefab;
    public GameObject GameOverMenuPrefab;
    public GameObject GameIntroMenuPrefab;
    public GameObject PauseMenuPrefab;

    [Header("Map Prefabs")]
    public GameObject CandylandMap;

    public GameObject TheForbiddenMap;

    [Header("Audio Sources")]
    public AudioSource MusicAudioSource;

    public AudioSource SFX_AudioSource;

    [Header("Music")]
    public AudioClip[] MusicTracks;

    [Header("Misc")]
    public GameObject CurrentMenu;

    public string MenuToChangeToAfterSceneChange = "";
    public bool SceneChanging = false;
    private bool PlayMusic = false;
    public AsyncOperationHandle<GameObject> PreviousMapLoader;
    private SavedDataManager PersistantDataStorage;

    public void Play_SFX_ExtendedOneShot(AudioClip Audio, float Volume, float StereoPan = 0.0f, float Pitch = 1.0f)
    // This method is used to play a one shot with additional settings for pan and pitch adjustment which is not
    // otherwise available when playing a one-shot sound. This is done by making new AudioSource objects.
    {
        GameObject NewAudioSource = new GameObject("SFX_ExtendedOneShot");
        AudioSource ExtendedAudioSourceComponent = NewAudioSource.AddComponent<AudioSource>();
        ExtendedOneShot ExtendedOneShotComponent = NewAudioSource.AddComponent<ExtendedOneShot>();
        ExtendedAudioSourceComponent.clip = Audio;
        ExtendedAudioSourceComponent.volume = Volume;
        ExtendedAudioSourceComponent.panStereo = StereoPan;
        ExtendedAudioSourceComponent.pitch = Pitch;
        ExtendedAudioSourceComponent.Play();
        ExtendedOneShotComponent.Lifetime = Audio.length;
        NewAudioSource.transform.parent = SFX_AudioSource.transform;
    }

    public void SetLayer(Transform obj, int LayerID)
    {
        obj.gameObject.layer = LayerID;

        foreach (Transform child in obj.transform)
        {
            SetLayer(child, LayerID);
        }
    }

    private void Awake()
    {
        if (Registry.CoreGameInfrastructureObject == null)
        {
            DontDestroyOnLoad(gameObject);
            Registry.CoreGameInfrastructureObject = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PersistantDataStorage = new SavedDataManager();
        PersistantDataStorage.Load();
        CurrentMenu = Instantiate(GameIntroMenuPrefab);
    }

    public IEnumerator AsynchronouslyLoadMap(string mapName)
    {
        AsyncOperationHandle<GameObject> AsyncMapLoader = Addressables.InstantiateAsync(mapName);
        yield return AsyncMapLoader;

        if (AsyncMapLoader.Status == AsyncOperationStatus.Succeeded)
        {
            if (PreviousMapLoader.IsValid())
            {
                Addressables.Release(PreviousMapLoader);
            }

            GameObject CurrentMap = AsyncMapLoader.Result;
            Destroy(Registry.Map);
            Registry.Map = CurrentMap;
            PreviousMapLoader = AsyncMapLoader;
        }
        else
        {
            Debug.LogError($"Failed to load map: {mapName}");
        }

        Registry.MapLoaded = mapName;

        yield return null;
    }

    public void LoadMap(string mapName)
    {
        if (Registry.Map != null)
        {
            Destroy(Registry.Map);
            Registry.Map = null;
        }
        Registry.MapLoaded = Constants.MAP_NOT_LOADED;
        StartCoroutine(AsynchronouslyLoadMap(mapName));
    }

    private void MenuSplitter(string MenuName)
    {
        if (MenuName == Constants.MAIN_MENU)
        {
            PlayMusic = true;
            CurrentMenu = Instantiate(MainMenuPrefab);
        }
        else if (MenuName == Constants.LEVEL_SELECTION_MENU)
        {
            CurrentMenu = Instantiate(LevelSelectionMenuPrefab);
        }
        else if (MenuName == Constants.SETTINGS_MENU)
        {
            CurrentMenu = Instantiate(SettingsMenuPrefab);
        }
        else if (MenuName == Constants.CREDITS_MENU)
        {
            CurrentMenu = Instantiate(CreditsMenuPrefab);
        }
        else if (MenuName == Constants.CLICK_TO_JOIN_MENU)
        {
            CurrentMenu = Instantiate(ClickToJoinMenuPrefab);
        }
        else if (MenuName == Constants.PLAYER_ONE_CLASS_SELECTION_MENU)
        {
            CurrentMenu = Instantiate(PlayerOneClassSelectionMenuPrefab);
        }
        else if (MenuName == Constants.PLAYER_TWO_CLASS_SELECTION_MENU)
        {
            CurrentMenu = Instantiate(PlayerTwoClassSelectionMenuPrefab);
        }
        else if (MenuName == Constants.GAME_OVER_MENU)
        {
            CurrentMenu = Instantiate(GameOverMenuPrefab);
        }
        else if (MenuName == Constants.PAUSE_MENU)
        {
            CurrentMenu = Instantiate(PauseMenuPrefab);
        }
    }

    public void LoadMenu(string MenuName)
    {
        if (SceneChanging)
        {
            MenuToChangeToAfterSceneChange = MenuName;
            return;
        }

        if (MenuName == Constants.PREVIOUS_MENU)
        {
            MenuName = Registry.MenuStack.Pop();
        }

        MenuSplitter(MenuName);
    }

    public void ChangeMenu(string MenuName)
    {
        if (SceneChanging)
        {
            MenuToChangeToAfterSceneChange = MenuName;
            return;
        }

        if (MenuName == Constants.PREVIOUS_MENU)
        {
            MenuName = Registry.MenuStack.Pop();
        }

        if (CurrentMenu != null)
        {
            Destroy(CurrentMenu);
        }

        MenuSplitter(MenuName);
    }

    public void CloseMenu()
    {
        if (CurrentMenu != null)
        {
            Destroy(CurrentMenu);
        }
    }

    public void ChangeScene(string SceneName)
    {
        SceneChanging = true;
        SceneManager.LoadScene(SceneName);
    }

    private void SelectAndPlayMusicTrack()
    {
        MusicAudioSource.clip = MusicTracks[Random.Range(0, MusicTracks.Length)];
        MusicAudioSource.Play();
    }

    public void Quit()
    {
        PersistantDataStorage.Save();
        Application.Quit();
    }

    public void Update()
    {
        if (Registry.GamePaused)
        {
            Time.timeScale = 0.0f;
        }
        else
        {
            Time.timeScale = 1.0f;
        }

        MusicAudioSource.volume = Registry.MusicVolume * Registry.MasterVolume;

        if (MenuToChangeToAfterSceneChange != "" && SceneChanging == false)
        {
            ChangeMenu(MenuToChangeToAfterSceneChange);
            MenuToChangeToAfterSceneChange = "";
        }

        SceneChanging = false;

        if (PlayMusic && !MusicAudioSource.isPlaying)
        {
            SelectAndPlayMusicTrack();
        }
    }
}