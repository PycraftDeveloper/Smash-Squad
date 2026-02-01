using TMPro;
using UnityEngine;

public static class Constants
{
    public const float TAU = Mathf.PI * 2;

    public const int INTRO_SEQU_START = 0;
    public const int INTRO_SEQU_WAIT_FOR_LOAD = 1;
    public const int INTRO_SEQU_END = 2;

    public const string MAIN_MENU = "main menu";
    public const string SETTINGS_MENU = "settings menu";
    public const string CREDITS_MENU = "credits menu";
    public const string LEVEL_SELECTION_MENU = "level selection menu";
    public const string PLAYER_ONE_CLASS_SELECTION_MENU = "player one class selection menu";
    public const string PLAYER_TWO_CLASS_SELECTION_MENU = "player two class selection menu";
    public const string CLICK_TO_JOIN_MENU = "click to join menu";
    public const string GAME_OVER_MENU = "game over menu";
    public const string PAUSE_MENU = "pause menu";
    public const string PREVIOUS_MENU = "previous";

    public const string CANDYLAND_MAP = "CandylandMap";
    public const string THE_FORBIDDEN_MAP = "TheForbiddenMap";
    public const string MAP_NOT_LOADED = "";
    public static readonly string[] MAPS = new string[] { CANDYLAND_MAP, THE_FORBIDDEN_MAP };

    public const string LAUNCH_CLASS = "Launch";
    public const string WARP_CLASS = "Warp";
    public const string GEOMANCY_CLASS = "Geomancy";
    public const string NINJA_CLASS = "Ninja";

    public const string MENU_SCENE = "Menu";
    public const string GAME_SCENE = "Game";

    public static readonly Vector3[] CANDYLAND_ORBIT_CAMERA_POSITIONS = new Vector3[]
    {
        new Vector3(32.95f, 5.26f + 20.0f, -108.2f),
        new Vector3(-22.48f, 5.26f + 20.0f, -98.79f),
        new Vector3(-30.72f, 9.77f, -18.93f),
        new Vector3(-7.36f, 5.26f + 20.0f, 45.12f),
        new Vector3(0.51f, 9.77f + 20.0f, 37.08f),
        new Vector3(39.07f, 6.98f + 20.0f, -74.86f),
        new Vector3(85.2f, 6.98f + 20.0f, 28.2f),
        new Vector3(-0.26f, 51.37f + 30.0f, -0.03f)
    };

    public static readonly Vector3[] CANDYLAND_PLAYER_SPAWN_POSITIONS = new Vector3[]
    {
        new Vector3(36.7f, 9.147f, 0.63f),
        new Vector3(3.7f, 9.223f, 37.9f),
        new Vector3(-35.55f, 11.65f, 5.0f),
        new Vector3(-0.42f, 9.58f, -38.08f),
        new Vector3(-18.4f, 12.13f, -124.63f),
        new Vector3(45.51f, 8.62f, -81.8f), //
        new Vector3(76.53f, 7.0f, -27.86f),
        new Vector3(117.65f, 6.23f, 33.12f),
        new Vector3(92.14f, 4.9f, 106.52f),
        new Vector3(28.5f, 5.92f, 106.9f),
        new Vector3(-26.81f, 8.06f, 134.15f),
        new Vector3(-58.23f, 7.59f, 80.1f),
        new Vector3(-114.99f, 7.22f, 76.88f),
        new Vector3(-114.99f, 4.9f, 8.03f),
        new Vector3(-109.2f, 3.42f, -58.71f),
        new Vector3(-61.13f, 5.7f, -65.02f),
        new Vector3(-13.7f, 8.38f, -161.2f)
    };

    public const string PLAYER_ONE = "player one";
    public const string PLAYER_TWO = "player two";

    public const float COS_45 = 0.707107f;

    public const int LAYER_INVISIBLE_TO_PLAYER_ONE = 6;
    public const int LAYER_INVISIBLE_TO_PLAYER_TWO = 7;
    public const int LAYER_INVISIBLE_TO_BOTH_PLAYERS = 8;
    public const int LAYER_DEFAULT = 0;

    public static Vector3 GEOMANCY_CHUNK_OFFSET = new Vector3(0.0f, -0.369f, 0.0f);

    public static string GEOMANCY_CLASS_GUIDE = "Your special ability allows you to spawn walkable platforms in the air that can be launched forwards when attacked";
    public static string LAUNCH_CLASS_GUIDE = "Your special ability allows you to create explosions at your feet, knocking you into the air and other players nearby";
    public static string WARP_CLASS_GUIDE = "Your special ability allows you to teleport around the map";
    public static string NINJA_CLASS_GUIDE = "Your special ability allows you to become invisible to other players";
}