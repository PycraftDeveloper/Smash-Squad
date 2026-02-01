using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ArrowManager : MonoBehaviour
{
    public GameObject ArrowModel;
    private Rigidbody ArrowRigidBody;

    public List<TrailRenderer> TrailEffects;

    public string PlayerID;

    [Header("Arrow Hit Success SFX")]
    public AudioClip ArrowHitSuccessSound;

    [Range(0.0f, 1.0f)] public float ArrowHitSuccessSoundVolume = 1.0f;
    [Range(0.0f, 0.15f)] public float ArrowHitSuccessSoundPitchVariance = 0.15f;

    public void Start()
    {
        ArrowRigidBody = GetComponent<Rigidbody>();
    }

    private IEnumerator ArrowHitRumble(Gamepad EffectedController)
    {
        EffectedController.SetMotorSpeeds(0.5f, 0.5f);
        yield return new WaitForSeconds(0.2f);
        if (EffectedController != null)
        {
            EffectedController.SetMotorSpeeds(0.0f, 0.0f);
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PlayerManager hitplayer = collision.gameObject.GetComponent<PlayerManager>();
            if (hitplayer.PlayerID == PlayerID)
            {
                // Don't hit yourself.
                return;
            }
            Vector3 ArrowLinearVelocityDirection = ArrowRigidBody.linearVelocity.normalized;
            ArrowLinearVelocityDirection.y = Mathf.Abs(ArrowLinearVelocityDirection.y) + 10.0f;
            ArrowLinearVelocityDirection.x *= 300.0f;
            ArrowLinearVelocityDirection.z *= 300.0f;
            hitplayer.PlayerRigidbody.AddForce(ArrowLinearVelocityDirection, ForceMode.Impulse); // force based on direction.

            for (int i = 0; i < hitplayer.GeomancyChunks.Count; i++)
            {
                GeomancyChunkData chunk = hitplayer.GeomancyChunks[i];
                chunk.linked = false; // Allows the player to be knocked off geomancy platforms.
            }

            if (hitplayer.PlayerID == Constants.PLAYER_ONE)
            {
                Registry.CoreGameInfrastructureObject.StartCoroutine(ArrowHitRumble(Registry.PlayerOneInput));
            }
            else
            {
                Registry.CoreGameInfrastructureObject.StartCoroutine(ArrowHitRumble(Registry.PlayerTwoInput));
            }

            // Play hit success sound
            Registry.CoreGameInfrastructureObject.Play_SFX_ExtendedOneShot(
                ArrowHitSuccessSound,
                ArrowHitSuccessSoundVolume * Registry.MasterVolume * Registry.SFXVolume,
                0.0f,
                Random.Range(1.0f - ArrowHitSuccessSoundPitchVariance, 1.0f + ArrowHitSuccessSoundPitchVariance));
        }

        DelayedGarbageCollect();
    }

    public void Update()
    {
        if (transform.position.y < -75.0f)
        {
            DelayedGarbageCollect();
        }
    }

    public IEnumerator DelayedDestroy()
    {
        yield return new WaitForSeconds(15.0f);
        Destroy(gameObject);
    }

    public void DelayedGarbageCollect()
    {
        Destroy(ArrowModel.gameObject);
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        CapsuleCollider sc = GetComponent<CapsuleCollider>();
        sc.enabled = false;
        foreach (TrailRenderer trail in TrailEffects)
        {
            trail.emitting = false;
        }
        StartCoroutine(DelayedDestroy());
    }

    public void FixedUpdate()
    {
        Vector3 Velocity = ArrowRigidBody.linearVelocity;

        if (Velocity.sqrMagnitude > 0.0001f)
        {
            Quaternion rotation = Quaternion.LookRotation(Velocity.normalized, Vector3.up);

            ArrowRigidBody.MoveRotation(rotation);
        }
    }
}