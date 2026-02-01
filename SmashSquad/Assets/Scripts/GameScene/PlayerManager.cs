using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GeomancyChunkData
{
    public GameObject ObjectInstance;
    public Vector3 Velocity;
    public float lifeTime;
    public float height;
    public bool linked = true;

    public void Unlink()
    {
        linked = false;
    }
}

public class PlayerManager : MonoBehaviour
{
    [Header("Camera")]
    public CinemachineCamera CinemachineCam;

    public Camera PlayerCamera;
    private Vector2 cameraSensitivity;
    private Vector2 CurrentCameraSensitivity;
    private CinemachineOrbitalFollow orbitalFollow;
    private CinemachineFollowZoom followZoom;
    public Volume CameraVolumeProfile;

    [Header("Physics")]
    public Rigidbody PlayerRigidbody;

    public float moveSpeed = 5.0f;
    public float jumpSpeed = 5.0f;
    public float sprintSpeedModifier = 1.5f;
    private bool DoJump = false;
    private bool DoSprint = false;
    private bool Grounded = false;
    public bool SpawnLock = true;
    public Vector3 HitVector = Vector3.zero;

    [Header("Player Model")]
    private GameObject PlayerModel;

    [Header("Input")]
    public float controllerDeadZone = 0.01f;

    private Vector3 PlayerMovementDir;

    [Header("PowerUp")]
    public float PowerUpMaxChargeTime = 30.0f;

    private int PowerUpCount = 0;
    private float CurrentPowerUpChargeTime;

    [Header("PowerUp - Launch")]
    private bool DoLaunchPowwerUp = false;

    [Header("PowerUp - Ninja")]
    public float InvisibleTimer = 0.0f;

    [Header("PowerUp - Geomancy")]
    public List<GameObject> Candyland_GeomancyGroundChunks;

    public List<GeomancyChunkData> GeomancyChunks = new List<GeomancyChunkData>();
    public float GeomancyChunkMaxHeightFromGround = 1.3f;
    public float GeomancyChunkLifetime = 45;

    [Header("PowerUp - Warp")]
    public GameObject WarpballPrefab;

    [Header("UI Sprites for button indicators")]
    public Sprite Active_A_ButtonImage;

    public Sprite Inactive_A_ButtonImage;

    public Sprite Active_B_ButtonImage;
    public Sprite Inactive_B_ButtonImage;

    public Sprite Active_X_ButtonImage;
    public Sprite Inactive_X_ButtonImage;

    public Sprite Active_Y_ButtonImage;
    public Sprite Inactive_Y_ButtonImage;

    public Sprite Active_RB_ButtonImage;
    public Sprite Inactive_RB_ButtonImage;

    [Header("UI Elements")]
    public TMP_Text SpecialAbilityText;

    public TMP_Text RangeText;

    public Image SpecialAbilityButtonImage;
    public Image AttackButtonImage;
    public Image JumpButtonImage;
    public Image RangeButtonImage;

    public GameObject Crosshair;

    public Image OppositePlayerImage;
    public Image CurrentPlayerImage;

    [Header("Tutorial")]
    public GameObject HelpPrompt;

    public TMP_Text HelpText;

    [Header("Arrow")]
    public float ArrowPullback;

    public GameObject ArrowPrefab;
    public int ArrowCount;
    public float ArrowMaxRestockTime = 15.0f;
    private float ArrowRestockTime;
    private Vignette ArrowChargeVignette;

    [Header("Footstep SFX")]
    public List<AudioClip> FootstepSounds;

    [Range(0.0f, 1.0f)] public float FootstepVolume = 1.0f;
    [Range(0.0f, 0.3f)] public float FootstepPitchRange = 0.15f;
    [Range(0.0f, 1.0f)] public float FootstepVolumeRange = 0.1f;
    public float FootstepDelay = 0.1f;
    private float CurrentFootstepDelay;

    [Header("Explosion SFX")]
    public AudioClip ExplosionSound;

    [Range(0.0f, 1.0f)] public float ExplosionSoundVolume = 1.0f;
    [Range(0.0f, 0.15f)] public float ExplosionSoundPitchVariance = 0.15f;

    [Header("Power Up SFX")]
    public AudioClip PowerUpSound;

    [Range(0.0f, 1.0f)] public float PowerUpSoundVolume = 1.0f;
    [Range(0.0f, 0.15f)] public float PowerUpSoundPitchVariance = 0.15f;

    [Header("Arrow Fire SFX")]
    public AudioClip ArrowFireSound;

    [Range(0.0f, 1.0f)] public float ArrowFireSoundVolume = 1.0f;
    [Range(0.0f, 0.15f)] public float ArrowFireSoundPitchVariance = 0.15f;

    [Header("Launch Explosion Effect")]
    public GameObject BigExplosionPrefab;

    private List<GameObject> BigExplosionInstances = new List<GameObject>();

    [Header("Attack Effect")]
    public GameObject AttackShockWavePrefab;

    private List<GameObject> AttackShockWaveInstances = new List<GameObject>();

    [Header("Misc")]
    public string PlayerID;

    private float AirTime = 0.0f;

    public void StartPlay()
    {
        Vector3 Position = Constants.CANDYLAND_PLAYER_SPAWN_POSITIONS[Random.Range(0, Constants.CANDYLAND_PLAYER_SPAWN_POSITIONS.Length)];
        while (Position == Registry.PlayerOneStartPosition)
        {
            Position = Constants.CANDYLAND_PLAYER_SPAWN_POSITIONS[Random.Range(0, Constants.CANDYLAND_PLAYER_SPAWN_POSITIONS.Length)];
        }
        if (PlayerID == Constants.PLAYER_ONE)
        {
            Registry.PlayerOneStartPosition = Position;
        }
        else
        {
            Registry.PlayerTwoStartPosition = Position;
        }

        PlayerRigidbody.position = Position;

        PlayerRigidbody.angularVelocity = Vector3.zero;
        PlayerRigidbody.linearVelocity = Vector3.zero;

        SpawnLock = true;

        CurrentPowerUpChargeTime = PowerUpMaxChargeTime;
        ArrowRestockTime = ArrowMaxRestockTime;
        PowerUpCount = 0;
        ArrowCount = 0;

        InvisibleTimer = 0.0f;

        CurrentFootstepDelay = FootstepDelay;

        foreach (GeomancyChunkData item in GeomancyChunks)
        {
            Destroy(item.ObjectInstance);
        }
        GeomancyChunks.Clear();

        foreach (GameObject BigExplosionInstance in BigExplosionInstances)
        {
            Destroy(BigExplosionInstance);
        }
        BigExplosionInstances.Clear();

        foreach (GameObject AttackShockWaveInstance in AttackShockWaveInstances)
        {
            Destroy(AttackShockWaveInstance);
        }
        AttackShockWaveInstances.Clear();

        StartCoroutine(ShowTutorial());
    }

    private IEnumerator DoBigExplosion()
    {
        GameObject BigExplosionInstance = Instantiate(BigExplosionPrefab, this.transform.position, this.transform.rotation);
        BigExplosionInstances.Add(BigExplosionInstance);

        yield return new WaitForSeconds(2.0f);

        Destroy(BigExplosionInstance);
        BigExplosionInstances.Remove(BigExplosionInstance);
    }

    private IEnumerator DoAttackShockWave()
    {
        GameObject AttackShockWaveInstance = Instantiate(AttackShockWavePrefab, this.transform.position, Quaternion.Euler(0, 90, 0));
        AttackShockWaveInstances.Add(AttackShockWaveInstance);

        yield return new WaitForSeconds(2.0f);

        Destroy(AttackShockWaveInstance);
        AttackShockWaveInstances.Remove(AttackShockWaveInstance);
    }

    private IEnumerator ShowTutorial()
    {
        HelpPrompt.SetActive(true);

        if (PlayerID == Constants.PLAYER_ONE)
        {
            if (Registry.PlayerOneSelectedClass == Constants.GEOMANCY_CLASS)
            {
                HelpText.text = Constants.GEOMANCY_CLASS_GUIDE;
            }
            else if (Registry.PlayerOneSelectedClass == Constants.WARP_CLASS)
            {
                HelpText.text = Constants.WARP_CLASS_GUIDE;
            }
            else if (Registry.PlayerOneSelectedClass == Constants.NINJA_CLASS)
            {
                HelpText.text = Constants.NINJA_CLASS_GUIDE;
            }
            else
            {
                HelpText.text = Constants.LAUNCH_CLASS_GUIDE;
            }
        }
        else
        {
            if (Registry.PlayerTwoSelectedClass == Constants.GEOMANCY_CLASS)
            {
                HelpText.text = Constants.GEOMANCY_CLASS_GUIDE;
            }
            else if (Registry.PlayerTwoSelectedClass == Constants.WARP_CLASS)
            {
                HelpText.text = Constants.WARP_CLASS_GUIDE;
            }
            else if (Registry.PlayerTwoSelectedClass == Constants.NINJA_CLASS)
            {
                HelpText.text = Constants.NINJA_CLASS_GUIDE;
            }
            else
            {
                HelpText.text = Constants.LAUNCH_CLASS_GUIDE;
            }
        }

        yield return new WaitForSeconds(7.0f);

        HelpText.text = "The aim of the game is to knock the other player off the map by any means possible!";

        yield return new WaitForSeconds(5.0f);

        HelpPrompt.SetActive(false);
    }

    private void Start()
    {
        if (PlayerID == Constants.PLAYER_ONE)
        {
            Registry.PlayerOneGameObject = this;
        }
        else
        {
            Registry.PlayerTwoGameObject = this;
        }

        orbitalFollow = CinemachineCam.GetComponent<CinemachineOrbitalFollow>();
        followZoom = CinemachineCam.GetComponent<CinemachineFollowZoom>();

        CameraVolumeProfile.profile.TryGet(out ArrowChargeVignette);

        CurrentCameraSensitivity = cameraSensitivity;

        if (orbitalFollow == null)
        {
            Debug.LogError("CinemachineOrbitalFollow is missing on the camera.");
        }

        if (PlayerID == Constants.PLAYER_ONE)
        {
            if (Registry.PlayerOneGameObject == null)
            {
                Debug.LogError("Player One GameObject is not assigned in the Registry.");
            }
        }
        else
        {
            if (Registry.PlayerTwoGameObject == null)
            {
                Debug.LogError("Player Two GameObject is not assigned in the Registry.");
            }
        }

        PlayerModel = transform.GetChild(0).gameObject;

        CinemachineCam.Follow = PlayerRigidbody.transform;

        followZoom.Width = 5.0f;

        StartPlay();
    }

    public float SmoothStepDifferential(float value)
    {
        return 4.0f * value * (1.0f - value);
    }

    private IEnumerator GeomancyChunkRise(GeomancyChunkData chunk)
    {
        float RiseTime = 1.0f;

        chunk.height = PlayerRigidbody.position.y - 1.0f;

        while (RiseTime > 0.0f)
        {
            float Offset = Mathf.SmoothStep(1.0f, -0.1f, 1 - (RiseTime / 1.0f));
            float OffsetDifferential = SmoothStepDifferential(1 - (RiseTime / 1.0f));
            chunk.height = PlayerRigidbody.position.y - Offset;
            RiseTime -= Time.deltaTime;

            if (PlayerID == Constants.PLAYER_ONE)
            {
                if (Registry.PlayerOneInput != null)
                {
                    Registry.PlayerOneInput.SetMotorSpeeds(0.2f * OffsetDifferential, 0.2f * OffsetDifferential);
                }
            }
            else
            {
                if (Registry.PlayerTwoInput != null)
                {
                    Registry.PlayerTwoInput.SetMotorSpeeds(0.2f * OffsetDifferential, 0.2f * OffsetDifferential);
                }
            }
            yield return null;
        }

        if (PlayerID == Constants.PLAYER_ONE)
        {
            if (Registry.PlayerOneInput != null)
            {
                Registry.PlayerOneInput.SetMotorSpeeds(0.0f, 0.0f);
            }
        }
        else
        {
            if (Registry.PlayerTwoInput != null)
            {
                Registry.PlayerTwoInput.SetMotorSpeeds(0.0f, 0.0f);
            }
        }

        chunk.height = PlayerRigidbody.position.y + 0.1f;
    }

    private void UnlinkGeomancyChunk(GeomancyChunkData chunk)
    {
        if (chunk != null)
        {
            if (PlayerID == Constants.PLAYER_ONE)
            {
                chunk.ObjectInstance.tag = "PlayerOneGeomancyChunk";
            }
            else
            {
                chunk.ObjectInstance.tag = "PlayerTwoGeomancyChunk";
            }
        }

        //chunk.ObjectRigidBody.useGravity = false;
        //chunk.ObjectRigidBody.constraints = RigidbodyConstraints.FreezeRotation;
        //chunk.ObjectRigidBody.linearDamping = 1.0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        string GeomancyTagName;
        if (PlayerID == Constants.PLAYER_ONE)
        {
            GeomancyTagName = "PlayerTwoGeomancyChunk"; // flipped
        }
        else
        {
            GeomancyTagName = "PlayerOneGeomancyChunk"; // flipped
        }
        if (other.gameObject.CompareTag(GeomancyTagName))
        {
            Vector3 KnockbackDirection = Vector3.Normalize(other.transform.position - transform.position);
            KnockbackDirection.Normalize();
            KnockbackDirection.y = Mathf.Abs(KnockbackDirection.y) + 10.0f;
            KnockbackDirection.x *= 300.0f;
            KnockbackDirection.z *= 300.0f;
            HitVector = KnockbackDirection;
            int index = 0;
            bool found = false;
            for (index = 0; index < GeomancyChunks.Count; index++)
            {
                if (other.gameObject == GeomancyChunks[index].ObjectInstance)
                {
                    found = true;
                    break;
                }
            }

            if (found)
            {
                Destroy(GeomancyChunks[index].ObjectInstance);
                GeomancyChunks.RemoveAt(index);
            }
        }
    }

    private void Update()
    {
        if (Registry.GamePaused)
        {
            return;
        }

        if (PlayerID == Constants.PLAYER_ONE)
        {
            cameraSensitivity = new Vector2(Registry.P1_X_CamSense, Registry.P1_Y_CamSense);

            if (Registry.P1_X_InvertAxis)
            {
                cameraSensitivity.x *= -1;
            }
            if (Registry.P1_Y_InvertAxis)
            {
                cameraSensitivity.y *= -1;
            }
        }
        else
        {
            cameraSensitivity = new Vector2(Registry.P2_X_CamSense, Registry.P2_Y_CamSense);

            if (Registry.P2_X_InvertAxis)
            {
                cameraSensitivity.x *= -1;
            }
            if (Registry.P2_Y_InvertAxis)
            {
                cameraSensitivity.y *= -1;
            }
        }

        CurrentCameraSensitivity = cameraSensitivity;

        // Player movement
        Vector2 move;
        if (PlayerID == Constants.PLAYER_ONE)
        {
            move = Registry.PlayerOneInput.leftStick.ReadValue();
        }
        else
        {
            move = Registry.PlayerTwoInput.leftStick.ReadValue();
        }

        CurrentFootstepDelay -= Time.deltaTime * move.magnitude; // already normalized to 0-1 range
        if (CurrentFootstepDelay < 0 && Grounded)
        {
            CurrentFootstepDelay = FootstepDelay;

            if (InvisibleTimer <= 0.0f) // Ninja hides footsteps when invisible
            {
                Registry.CoreGameInfrastructureObject.Play_SFX_ExtendedOneShot(
                FootstepSounds[Random.Range(0, FootstepSounds.Count)],
                FootstepVolume * Registry.MasterVolume * Registry.SFXVolume * Random.Range(1.0f - FootstepVolumeRange, 1.0f),
                0.0f,
                Random.Range(1.0f - FootstepPitchRange, 1.0f + FootstepPitchRange));
            }
        }

        // Player Arrow
        if (PlayerID == Constants.PLAYER_ONE)
        {
            ArrowPullback = Registry.PlayerOneInput.leftTrigger.ReadValue();
        }
        else
        {
            ArrowPullback = Registry.PlayerTwoInput.leftTrigger.ReadValue();
        }
        followZoom.FovRange = new Vector2(10.0f, 10.0f + (1.0f - ArrowPullback) * 20.0f);
        ArrowChargeVignette.intensity.value = Mathf.Lerp(0.0f, 0.231f, ArrowPullback);
        CurrentCameraSensitivity *= Mathf.Lerp(1.0f, 0.1f, ArrowPullback);
        if (ArrowPullback > 0)
        {
            Crosshair.SetActive(true);
            bool FireArrow = false;

            if (PlayerID == Constants.PLAYER_ONE) // Hide that controller's player (unlike later when we hide opposite)
            {
                FireArrow = Registry.PlayerOneInput.rightShoulder.wasPressedThisFrame;
            }
            else
            {
                FireArrow = Registry.PlayerTwoInput.rightShoulder.wasPressedThisFrame;
            }

            if (FireArrow && ArrowCount > 0)
            {
                ArrowCount--;
                GameObject ArrowProjectile = Instantiate(ArrowPrefab);
                ArrowProjectile.transform.position = PlayerRigidbody.position + PlayerCamera.transform.forward * 2.0f + PlayerRigidbody.transform.up * 1.5f;
                ArrowProjectile.GetComponent<ArrowManager>().PlayerID = PlayerID;
                Rigidbody WarpballRigidbody = ArrowProjectile.GetComponent<Rigidbody>();
                WarpballRigidbody.AddForce(PlayerCamera.transform.forward * 150.0f * ArrowPullback, ForceMode.Impulse);

                Registry.CoreGameInfrastructureObject.Play_SFX_ExtendedOneShot(
                    ArrowFireSound,
                    ArrowFireSoundVolume * Registry.MasterVolume * Registry.SFXVolume,
                    0.0f,
                    Random.Range(1.0f - ArrowFireSoundPitchVariance, 1.0f + ArrowFireSoundPitchVariance));
            }
        }
        else
        {
            Crosshair.SetActive(false);
        }

        PlayerMovementDir = Vector3.zero;

        if (move.sqrMagnitude > controllerDeadZone)
        {
            // Get camera forward/right projected onto ground
            Vector3 camForward = CinemachineCam.transform.forward;
            camForward.y = 0;
            camForward.Normalize();

            Vector3 camRight = CinemachineCam.transform.right;
            camRight.y = 0;
            camRight.Normalize();

            // Calculate movement direction
            Vector3 moveDir = camForward * move.y + camRight * move.x;

            PlayerMovementDir = moveDir;
        }

        // Camera orbit
        Vector2 look;
        if (PlayerID == Constants.PLAYER_ONE)
        {
            look = Registry.PlayerOneInput.rightStick.ReadValue();
        }
        else
        {
            look = Registry.PlayerTwoInput.rightStick.ReadValue();
        }

        if (look.magnitude > controllerDeadZone)
        {
            // Horizontal orbit (Yaw)
            orbitalFollow.HorizontalAxis.Value += look.x * CurrentCameraSensitivity.x * Time.deltaTime;

            // Vertical orbit (Pitch)
            orbitalFollow.VerticalAxis.Value += look.y * CurrentCameraSensitivity.y * Time.deltaTime;

            // Clamp vertical (so camera doesn't flip)
            orbitalFollow.VerticalAxis.Value = Mathf.Clamp(orbitalFollow.VerticalAxis.Value, -75, 75);
        }

        // Pause button pressed
        if (PlayerID == Constants.PLAYER_ONE)
        {
            if (Registry.PlayerOneInput.startButton.IsPressed())
            {
                Registry.PlayerOnePaused = true;
            }
        }
        else
        {
            if (Registry.PlayerTwoInput.startButton.IsPressed())
            {
                Registry.PlayerTwoPaused = true;
            }
        }

        // Sprint button pressed
        if (PlayerID == Constants.PLAYER_ONE)
        {
            DoSprint = Registry.PlayerOneInput.leftStickButton.IsPressed();
        }
        else
        {
            DoSprint = Registry.PlayerTwoInput.leftStickButton.IsPressed();
        }

        if (DoSprint)
        {
            followZoom.Width = 7.0f;
        }
        else
        {
            followZoom.Width = 5.0f;
        }

        RaycastHit GroundedRay;
        bool NewGrounded = Physics.Raycast(PlayerRigidbody.position, transform.up * -1, out GroundedRay, 0.25f);

        if (NewGrounded && !Grounded) // just landed
        {
            if (InvisibleTimer <= 0.0f) // Ninja hides footsteps when invisible
            {
                Registry.CoreGameInfrastructureObject.Play_SFX_ExtendedOneShot(
                    FootstepSounds[Random.Range(0, FootstepSounds.Count)],
                    FootstepVolume * Registry.MasterVolume * Registry.SFXVolume * AirTime / 2.3f,
                    0.0f,
                    Random.Range(1.0f - FootstepPitchRange, 1.0f + FootstepPitchRange));
            }

            AirTime = 0.0f;
        }
        Grounded = NewGrounded;

        if (!Grounded)
        {
            AirTime += Time.deltaTime;
        }

        if (PlayerID == Constants.PLAYER_ONE)
        {
            DoJump = Registry.PlayerOneInput.buttonSouth.IsPressed() && Grounded;
        }
        else
        {
            DoJump = Registry.PlayerTwoInput.buttonSouth.IsPressed() && Grounded;
        }

        if (PlayerRigidbody.position.y < -50)
        {
            if (PlayerID == Constants.PLAYER_ONE) // Opposite player wins
            {
                Registry.GameOver = Constants.PLAYER_TWO;
            }
            else
            {
                Registry.GameOver = Constants.PLAYER_ONE;
            }

            Registry.GamePaused = true;
            Registry.CoreGameInfrastructureObject.ChangeMenu(Constants.GAME_OVER_MENU);
        }

        float DistanceToTarget = Vector3.Distance(Registry.PlayerOneGameObject.PlayerRigidbody.position, Registry.PlayerTwoGameObject.PlayerRigidbody.position);

        Vector3 NormalTargetDirection;
        if (PlayerID == Constants.PLAYER_ONE)
        {
            NormalTargetDirection = Vector3.Normalize(Registry.PlayerTwoGameObject.transform.position - PlayerRigidbody.position);
        }
        else
        {
            NormalTargetDirection = Vector3.Normalize(Registry.PlayerOneGameObject.transform.position - PlayerRigidbody.position);
        }

        float TargetAngle = Vector3.Dot(PlayerModel.transform.forward, NormalTargetDirection);
        bool AttackButtonPressed;
        if (PlayerID == Constants.PLAYER_ONE)
        {
            AttackButtonPressed = Registry.PlayerOneInput.buttonWest.wasPressedThisFrame;
        }
        else
        {
            AttackButtonPressed = Registry.PlayerTwoInput.buttonWest.wasPressedThisFrame;
        }

        if (AttackButtonPressed)
        {
            StartCoroutine(DoAttackShockWave());
        }

        if (DistanceToTarget < 2.3f && TargetAngle > Constants.COS_45)
        {
            if (AttackButtonPressed)
            {
                Vector3 LaunchVector = NormalTargetDirection;
                LaunchVector.y = Mathf.Abs(LaunchVector.y) + 10.0f;
                LaunchVector.x *= 300.0f;
                LaunchVector.z *= 300.0f;

                if (PlayerID == Constants.PLAYER_ONE)
                {
                    Registry.PlayerTwoGameObject.HitVector = LaunchVector;
                }
                else
                {
                    Registry.PlayerOneGameObject.HitVector = LaunchVector;
                }
            }
        }

        Vector3 screenPos;
        if (PlayerID == Constants.PLAYER_ONE) // Opposite player wins
        {
            screenPos = PlayerCamera.WorldToScreenPoint(Registry.PlayerTwoGameObject.transform.position);
        }
        else
        {
            screenPos = PlayerCamera.WorldToScreenPoint(Registry.PlayerOneGameObject.transform.position);
        }

        if (screenPos.z > 0 && DistanceToTarget > 132.0f)
        {
            if (PlayerID == Constants.PLAYER_ONE) // Opposite player wins
            {
                OppositePlayerImage.enabled = Registry.PlayerTwoGameObject.InvisibleTimer <= 0.0f;
            }
            else
            {
                OppositePlayerImage.enabled = Registry.PlayerOneGameObject.InvisibleTimer <= 0.0f;
            }

            Vector2 anchoredPos = OppositePlayerImage.rectTransform.anchoredPosition;
            anchoredPos.x = screenPos.x - (Screen.width / 2f);
            OppositePlayerImage.rectTransform.anchoredPosition = anchoredPos;
        }
        else
        {
            OppositePlayerImage.enabled = false;
        }

        foreach (GeomancyChunkData chunk in GeomancyChunks)
        {
            DistanceToTarget = Vector3.Distance(PlayerRigidbody.position, chunk.ObjectInstance.transform.position);
            NormalTargetDirection = Vector3.Normalize(chunk.ObjectInstance.transform.position - PlayerRigidbody.position);
            TargetAngle = Vector3.Dot(PlayerModel.transform.forward, NormalTargetDirection);

            if (DistanceToTarget < 2.3f && TargetAngle > Constants.COS_45)
            {
                if (AttackButtonPressed)
                {
                    Vector3 LaunchVector = NormalTargetDirection;
                    LaunchVector.y = 0.0f;
                    LaunchVector.x *= 75.0f;
                    LaunchVector.z *= 75.0f;
                    chunk.Velocity = LaunchVector;
                }
            }
        }

        if (Grounded && SpawnLock)
        {
            PlayerRigidbody.useGravity = false;
        }
        else
        {
            PlayerRigidbody.useGravity = true;
        }

        bool NorthButtonPressed;
        string SelectedClass;

        if (PlayerID == Constants.PLAYER_ONE) // Opposite player wins
        {
            NorthButtonPressed = Registry.PlayerOneInput.buttonNorth.wasPressedThisFrame;
            SelectedClass = Registry.PlayerOneSelectedClass;
        }
        else
        {
            NorthButtonPressed = Registry.PlayerTwoInput.buttonNorth.wasPressedThisFrame;
            SelectedClass = Registry.PlayerTwoSelectedClass;
        }

        if (NorthButtonPressed && PowerUpCount > 0)
        {
            PowerUpCount--;
            SpawnLock = false;

            Registry.CoreGameInfrastructureObject.Play_SFX_ExtendedOneShot(
                PowerUpSound,
                PowerUpSoundVolume * Registry.MasterVolume * Registry.SFXVolume,
                0.0f,
                Random.Range(1.0f - PowerUpSoundPitchVariance, 1.0f + PowerUpSoundPitchVariance));

            if (SelectedClass == Constants.LAUNCH_CLASS)
            {
                DoLaunchPowwerUp = true;
                StartCoroutine(DoBigExplosion());
            }
            else if (SelectedClass == Constants.NINJA_CLASS)
            {
                InvisibleTimer = 20.0f;
            }
            else if (SelectedClass == Constants.GEOMANCY_CLASS)
            {
                GeomancyChunkData chunk = new GeomancyChunkData();
                chunk.ObjectInstance = Instantiate(Candyland_GeomancyGroundChunks[Random.Range(0, Candyland_GeomancyGroundChunks.Count)]);
                chunk.lifeTime = GeomancyChunkLifetime;
                GeomancyChunks.Add(chunk);

                StartCoroutine(GeomancyChunkRise(chunk));
            }
            else // Warp class
            {
                GameObject Warpball = Instantiate(WarpballPrefab);
                Warpball.transform.position = PlayerRigidbody.position + PlayerCamera.transform.forward * 2.0f + PlayerRigidbody.transform.up * 1.5f;
                Warpball.GetComponent<WarpballManager>().PlayerID = PlayerID;
                Rigidbody WarpballRigidbody = Warpball.GetComponent<Rigidbody>();
                WarpballRigidbody.AddForce(PlayerCamera.transform.forward * 150.0f * ArrowPullback, ForceMode.Impulse);
            }
        }

        // Power up recharging -------------------------------------------------------------------------------------------------------------------------------------
        if (PowerUpCount < 5)
        {
            CurrentPowerUpChargeTime -= Time.deltaTime;
            if (CurrentPowerUpChargeTime < 0)
            {
                PowerUpCount++;
                CurrentPowerUpChargeTime = PowerUpMaxChargeTime;
            }
        }
        else
        {
            CurrentPowerUpChargeTime = PowerUpMaxChargeTime;
        }

        // Arrow recharging -------------------------------------------------------------------------------------------------------------------------------------
        if (ArrowCount < 5)
        {
            ArrowRestockTime -= Time.deltaTime;
            if (ArrowRestockTime < 0)
            {
                ArrowCount++;
                ArrowRestockTime = ArrowMaxRestockTime;
            }
        }
        else
        {
            ArrowRestockTime = ArrowMaxRestockTime;
        }

        // Ninja Power Up -------------------------------------------------------------------------------------------------------------------------------------
        if (InvisibleTimer > 0)
        {
            InvisibleTimer -= Time.deltaTime;
        }

        // Geomancy Chunk Manager ---------------------------------------------------------------------------------------------------------------------
        for (int i = GeomancyChunks.Count - 1; i >= 0; i--)
        {
            GeomancyChunkData chunk = GeomancyChunks[i];
            chunk.lifeTime -= Time.deltaTime;

            if (chunk.lifeTime < 0)
            {
                Destroy(chunk.ObjectInstance);
                GeomancyChunks.RemoveAt(i);
            }
            else
            {
                Vector3 ChunkPosition;
                if (chunk.linked)
                {
                    ChunkPosition = new Vector3(transform.position.x, chunk.height, transform.position.z);
                }
                else
                {
                    ChunkPosition = new Vector3(chunk.ObjectInstance.transform.position.x, chunk.height, chunk.ObjectInstance.transform.position.z);
                }

                ChunkPosition += chunk.Velocity * Time.deltaTime;
                chunk.ObjectInstance.transform.position = ChunkPosition + Constants.GEOMANCY_CHUNK_OFFSET;
            }
        }

        // UI Updates -------------------------------------------------------------------------------------------------------------------------------------
        SpecialAbilityText.text = $"{PowerUpCount}x Special";
        RangeText.text = $"{ArrowCount}x Range";

        if (PowerUpCount > 0)
        {
            SpecialAbilityButtonImage.sprite = Active_Y_ButtonImage;
        }
        else
        {
            SpecialAbilityButtonImage.sprite = Inactive_Y_ButtonImage;
        }

        if (ArrowCount > 0)
        {
            RangeButtonImage.sprite = Active_RB_ButtonImage;
        }
        else
        {
            RangeButtonImage.sprite = Inactive_RB_ButtonImage;
        }

        if (Grounded)
        {
            JumpButtonImage.sprite = Active_A_ButtonImage;
        }
        else
        {
            JumpButtonImage.sprite = Inactive_A_ButtonImage;
        }

        AttackButtonImage.sprite = Active_X_ButtonImage;

        // Player Visibility Management -------------------------------------------------------------------------------------------------------------------------------------
        if (InvisibleTimer > 0 && ArrowPullback > 0)
        {
            Registry.CoreGameInfrastructureObject.SetLayer(PlayerModel.transform, Constants.LAYER_INVISIBLE_TO_BOTH_PLAYERS);
        }
        else if (InvisibleTimer > 0)
        {
            if (PlayerID == Constants.PLAYER_ONE)
            {
                Registry.CoreGameInfrastructureObject.SetLayer(PlayerModel.transform, Constants.LAYER_INVISIBLE_TO_PLAYER_TWO);
            }
            else
            {
                Registry.CoreGameInfrastructureObject.SetLayer(PlayerModel.transform, Constants.LAYER_INVISIBLE_TO_PLAYER_ONE);
            }
        }
        else if (ArrowPullback > 0)
        {
            if (PlayerID == Constants.PLAYER_ONE)
            {
                Registry.CoreGameInfrastructureObject.SetLayer(PlayerModel.transform, Constants.LAYER_INVISIBLE_TO_PLAYER_ONE);
            }
            else
            {
                Registry.CoreGameInfrastructureObject.SetLayer(PlayerModel.transform, Constants.LAYER_INVISIBLE_TO_PLAYER_TWO);
            }
        }
        else
        {
            Registry.CoreGameInfrastructureObject.SetLayer(PlayerModel.transform, Constants.LAYER_DEFAULT);
        }
    }

    private IEnumerator MeleeHitRumble(Gamepad EffectedController)
    {
        EffectedController.SetMotorSpeeds(0.5f, 0.5f);
        yield return new WaitForSeconds(0.2f);
        if (EffectedController != null)
        {
            EffectedController.SetMotorSpeeds(0.0f, 0.0f);
        }
    }

    private void FixedUpdate()
    {
        if (Registry.GamePaused) return;

        if (DoJump && Grounded)
        {
            PlayerRigidbody.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
            DoJump = false;
            SpawnLock = false;

            for (int i = 0; i < GeomancyChunks.Count; i++)
            {
                GeomancyChunkData chunk = GeomancyChunks[i];
                if (chunk.linked)
                {
                    UnlinkGeomancyChunk(chunk);
                    chunk.Unlink();
                }
            }
        }

        Vector3 horizontalVelocity = new Vector3(PlayerRigidbody.linearVelocity.x, 0, PlayerRigidbody.linearVelocity.z);
        Vector3 Velocity = PlayerRigidbody.linearVelocity;
        if (Velocity.y < 0)
        {
            Velocity.y = 0;
        }

        if (PlayerMovementDir.magnitude > controllerDeadZone)
        {
            PlayerRigidbody.MoveRotation(Quaternion.LookRotation(PlayerMovementDir));
            SpawnLock = false;
        }

        PlayerMovementDir.y = 0;

        if (PlayerMovementDir.magnitude > controllerDeadZone)
        {
            float targetSpeed = moveSpeed * (1.0f - ArrowPullback); // slow doen movement when shooting
            if (DoSprint)
            {
                targetSpeed *= sprintSpeedModifier;
            }
            Vector3 desiredVelocity = PlayerMovementDir * targetSpeed;
            Vector3 velocityChange = desiredVelocity - horizontalVelocity;

            velocityChange = velocityChange * 15.0f;
            velocityChange.y = -Velocity.y * 10.0f;
            PlayerRigidbody.AddForce(velocityChange, ForceMode.Acceleration);
        }
        else
        {
            PlayerRigidbody.AddForce(-Velocity * 10.0f, ForceMode.Acceleration);
        }

        if (HitVector.magnitude > 0)
        {
            PlayerRigidbody.AddForce(HitVector, ForceMode.Impulse);
            HitVector = Vector3.zero;
            SpawnLock = false;

            if (PlayerID == Constants.PLAYER_ONE)
            {
                Registry.CoreGameInfrastructureObject.StartCoroutine(MeleeHitRumble(Registry.PlayerOneInput));
            }
            else
            {
                Registry.CoreGameInfrastructureObject.StartCoroutine(MeleeHitRumble(Registry.PlayerTwoInput));
            }
        }

        if (DoLaunchPowwerUp)
        {
            if (PlayerID == Constants.PLAYER_ONE) // Opposite player wins
            {
                PlayerRigidbody.AddExplosionForce(50000.0f, PlayerRigidbody.position - new Vector3(0, 1, 0), 50.0f);
                Registry.PlayerTwoGameObject.PlayerRigidbody.AddExplosionForce(50000.0f, PlayerRigidbody.position - new Vector3(0, 1, 0), 50.0f);
            }
            else
            {
                PlayerRigidbody.AddExplosionForce(50000.0f, PlayerRigidbody.position - new Vector3(0, 1, 0), 50.0f);
                Registry.PlayerOneGameObject.PlayerRigidbody.AddExplosionForce(50000.0f, PlayerRigidbody.position - new Vector3(0, 1, 0), 50.0f);
            }

            Registry.PlayerOneGameObject.SpawnLock = false;
            Registry.PlayerTwoGameObject.SpawnLock = false;
            DoLaunchPowwerUp = false;

            Registry.CoreGameInfrastructureObject.Play_SFX_ExtendedOneShot(
                ExplosionSound,
                ExplosionSoundVolume * Registry.MasterVolume * Registry.SFXVolume,
                0.0f,
                Random.Range(1.0f - ExplosionSoundPitchVariance, 1.0f + ExplosionSoundPitchVariance));
        }
    }
}