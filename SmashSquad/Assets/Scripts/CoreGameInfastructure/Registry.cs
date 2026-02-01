using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public static class Registry
{
    public static CoreGameInfrastructure CoreGameInfrastructureObject = null;

    public static Stack<string> MenuStack;

    public static string MapLoaded = Constants.MAP_NOT_LOADED;
    public static GameObject Map = null;

    public static Gamepad PlayerOneInput = null;
    public static Gamepad PlayerTwoInput = null;

    public static string PlayerOneSelectedClass = null;
    public static string PlayerTwoSelectedClass = null;

    public static PlayerManager PlayerOneGameObject;
    public static PlayerManager PlayerTwoGameObject;
    public static GameSceneManager CurrentGameManager;

    public static string MenuBackgroundScene = null;

    public static bool ControllerDisconnect = false;

    public static bool GamePaused = false;
    public static string GameOver = null;

    public static Vector3 PlayerOneStartPosition;
    public static Vector3 PlayerTwoStartPosition;

    public static bool PlayerOnePaused = false;
    public static bool PlayerTwoPaused = false;

    public static float MasterVolume = 1.0f;
    public static float MusicVolume = 0.25f;
    public static float SFXVolume = 1.0f;
    public static float P1_X_CamSense = 100.0f;
    public static float P1_Y_CamSense = 100.0f;
    public static float P2_X_CamSense = 100.0f;
    public static float P2_Y_CamSense = 100.0f;
    public static bool P1_X_InvertAxis = false;
    public static bool P1_Y_InvertAxis = true;
    public static bool P2_X_InvertAxis = false;
    public static bool P2_Y_InvertAxis = true;
}