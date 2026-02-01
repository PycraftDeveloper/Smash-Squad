using System.Collections;
using UnityEngine;

public class CelestialLightSource : MonoBehaviour
{
    private Light DirectionalLight;
    public Material CelestialObjectMaterial;
    private Color CelestialColor;
    public float LengthOfDay = 60 * 20;
    public float Timer = 0;
    private float ShadowStrength;
    private bool FadeRunning = false;

    private void Start()
    {
        DirectionalLight = GetComponent<Light>();
        ShadowStrength = DirectionalLight.shadowStrength;

        transform.position = new Vector3(0, 0, 5000);
        transform.rotation = Quaternion.LookRotation(Vector3.zero - transform.position);

        if (CelestialObjectMaterial != null)
        {
            CelestialColor = CelestialObjectMaterial.color;
            StartCoroutine(FadeOut());
            FadeRunning = true;
        }
    }

    private IEnumerator FadeOut()
    {
        float Fade = 1.0f;
        float Duration = 20.0f;
        while (Fade > 0.0f)
        {
            Fade = Duration / 20.0f;
            CelestialObjectMaterial.color = new Color(CelestialColor.r, CelestialColor.g, CelestialColor.b, Fade);
            Duration -= Time.deltaTime;
            yield return null;
        }
        FadeRunning = false;
        yield break;
    }

    private IEnumerator FadeIn()
    {
        float Fade = 0.0f;
        float Duration = 0;
        while (Fade < 1.0f)
        {
            Fade = Duration / 20.0f;
            CelestialObjectMaterial.color = new Color(CelestialColor.r, CelestialColor.g, CelestialColor.b, Fade);
            Duration += Time.deltaTime;
            yield return null;
        }
        FadeRunning = false;
        yield break;
    }

    private void Update()
    {
        Vector3 NewSunPosition = Vector3.zero;
        NewSunPosition.x = Mathf.Cos((Timer / LengthOfDay) * Constants.TAU) * 5000;
        NewSunPosition.y = Mathf.Sin((Timer / LengthOfDay) * Constants.TAU) * 5000;
        NewSunPosition.z = Mathf.Sin((Timer / LengthOfDay) * Constants.TAU) * 2500;
        Timer += Time.deltaTime;

        transform.position = NewSunPosition;
        transform.rotation = Quaternion.LookRotation(Vector3.zero - transform.position);

        if (!FadeRunning && CelestialObjectMaterial != null)
        {
            if (Timer % LengthOfDay >= LengthOfDay / 2.0f && Timer % LengthOfDay < (LengthOfDay / 2.0f) + 1.0f)
            {
                FadeRunning = true;
                StartCoroutine(FadeOut());
            }
            else if (Timer % LengthOfDay >= LengthOfDay - 20)
            {
                FadeRunning = true;
                StartCoroutine(FadeIn());
            }
        }

        if (transform.position.y > 0)
        {
            DirectionalLight.enabled = true;
            DirectionalLight.shadowStrength = (ShadowStrength * 0.1f) + ((1.0f + Mathf.Sin(((Timer - (LengthOfDay / 8.0f)) / (LengthOfDay / 2.0f)) * Constants.TAU)) / 2.0f) * (ShadowStrength * 0.9f);
        }
        else
        {
            DirectionalLight.enabled = false;
            DirectionalLight.shadowStrength = (ShadowStrength * 0.1f);
        }
    }
}