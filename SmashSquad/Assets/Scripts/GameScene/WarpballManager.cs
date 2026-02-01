using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpballManager : MonoBehaviour
{
    public GameObject WarpballCore;
    public GameObject WarpballExplosion;

    public List<TrailRenderer> TrailEffects;

    public string PlayerID;

    [Header("Warp SFX")]
    public AudioClip WarpSound;

    [Range(0.0f, 1.0f)] public float WarpSoundVolume = 1.0f;
    [Range(0.0f, 0.15f)] public float WarpSoundPitchVariance = 0.15f;

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Player")
        {
            DelayedGarbageCollect();
            StartCoroutine(DoExplosion());

            // Do teleport here
            ContactPoint contact = collision.contacts[0];
            Vector3 startPos = contact.point + Vector3.up * 100f; // start above
            Ray ray = new Ray(startPos, Vector3.down);

            if (Physics.Raycast(ray, out RaycastHit hit, 200f))
            {
                Vector3 newPosition = hit.point;
                if (PlayerID == Constants.PLAYER_ONE)
                {
                    newPosition.y += Registry.PlayerOneGameObject.GetComponent<Collider>().bounds.extents.y;
                    Registry.PlayerOneGameObject.PlayerRigidbody.MovePosition(newPosition);
                    //Registry.PlayerOneGameObject.transform.position = newPosition;
                }
                else
                {
                    newPosition.y += Registry.PlayerTwoGameObject.GetComponent<Collider>().bounds.extents.y;
                    Registry.PlayerTwoGameObject.PlayerRigidbody.MovePosition(newPosition);
                    //Registry.PlayerTwoGameObject.transform.position = newPosition;
                }

                // Play warp sound
                Registry.CoreGameInfrastructureObject.Play_SFX_ExtendedOneShot(
                    WarpSound,
                    WarpSoundVolume * Registry.MasterVolume * Registry.SFXVolume,
                    0.0f,
                    Random.Range(1.0f - WarpSoundPitchVariance, 1.0f + WarpSoundPitchVariance));
            }
        }
    }

    public IEnumerator DoExplosion()
    {
        WarpballExplosion.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        ParticleSystem particleSystem = WarpballExplosion.GetComponent<ParticleSystem>();
        particleSystem.Stop();
    }

    public IEnumerator DelayedDestroy()
    {
        yield return new WaitForSeconds(15.0f);
        Destroy(gameObject);
    }

    public void DelayedGarbageCollect()
    {
        Destroy(WarpballCore.gameObject);
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        SphereCollider sc = GetComponent<SphereCollider>();
        sc.enabled = false;
        foreach (TrailRenderer trail in TrailEffects)
        {
            trail.emitting = false;
        }
        StartCoroutine(DelayedDestroy());
    }

    public void Update()
    {
        if (transform.position.y < -75.0f)
        {
            DelayedGarbageCollect();
        }
    }
}