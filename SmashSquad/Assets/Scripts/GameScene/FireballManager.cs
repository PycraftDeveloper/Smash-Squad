using System.Collections;
using UnityEngine;

public class FireballManager : MonoBehaviour
{
    public GameObject FireballCore;
    public GameObject FireballExplosion;

    public ParticleSystem FlameStream;
    public ParticleSystem FireEmbers;

    [Header("Explosion SFX")]
    public AudioClip ExplosionSound;

    [Range(0.0f, 1.0f)] public float ExplosionSoundVolume = 1.0f;
    [Range(0.0f, 0.15f)] public float ExplosionSoundPitchVariance = 0.15f;

    public void OnCollisionEnter(Collision collision)
    {
        DelayedGarbageCollect();
        StartCoroutine(DoExplosion());

        Registry.PlayerOneGameObject.PlayerRigidbody.AddExplosionForce(10000.0f, transform.position, 50.0f);
        Registry.PlayerTwoGameObject.PlayerRigidbody.AddExplosionForce(10000.0f, transform.position, 50.0f);
    }

    public IEnumerator DoExplosion()
    {
        FireballExplosion.SetActive(true);

        float DistanceToPlayerOne = (FireballCore.transform.position - Registry.PlayerOneGameObject.transform.position).magnitude;
        float DistanceToPlayerTwo = (FireballCore.transform.position - Registry.PlayerTwoGameObject.transform.position).magnitude;
        float MinDistance = Mathf.Min(DistanceToPlayerTwo, DistanceToPlayerOne);
        float Amplitude = 1 - (Mathf.Clamp(MinDistance, 0.0f, 100.0f) / 100.0f);

        Registry.CoreGameInfrastructureObject.Play_SFX_ExtendedOneShot(
                ExplosionSound,
                ExplosionSoundVolume * Registry.MasterVolume * Registry.SFXVolume * Amplitude,
                0.0f,
                Random.Range(1.0f - ExplosionSoundPitchVariance, 1.0f + ExplosionSoundPitchVariance));

        yield return new WaitForSeconds(1.5f);
        ParticleSystem particleSystem = FireballExplosion.GetComponent<ParticleSystem>();
        particleSystem.Stop();
    }

    public IEnumerator DelayedDestroy()
    {
        yield return new WaitForSeconds(15.0f);
        Destroy(gameObject);
    }

    public void DelayedGarbageCollect()
    {
        Destroy(FireballCore.gameObject);
        FlameStream.Stop();
        FireEmbers.Stop();
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        SphereCollider sc = GetComponent<SphereCollider>();
        sc.enabled = false;
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