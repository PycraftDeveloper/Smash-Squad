using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CloudData
{
    public GameObject Instance;
    public float TargetScale;
    public float DecaySpeed;
    public float MovementSpeed;
    public bool IsSpawning;
    public float InitialAltitude;
    public float AccumulatedTime;
    public float UpdateInterval;
}

public class CloudMaker : MonoBehaviour
{
    public GameObject[] CloudModels;

    public Vector2 SpawnRange;
    public Vector2 HeightRange;
    public float SpawnHeight;
    public Vector2 ScaleRange;

    public Transform CloudParent;
    public int MaxNumberOfCloudModels = 50;

    [Header("Spawn Animation")]
    public float SpawnLerpTime = 2.0f;

    [Header("Curvature Effect")]
    public float HorizonRadius = 500.0f;

    public float CurvatureStrength = 0.5f;

    private List<CloudData> CloudInstances = new List<CloudData>();
    private Vector2 Direction;
    private float XSeed;
    private float YSeed;

    private void Start()
    {
        XSeed = Random.Range(0f, 1000f);
        YSeed = Random.Range(0f, 1000f);

        for (int i = 0; i < MaxNumberOfCloudModels; i++)
        {
            SpawnCloud(true);
        }
    }

    private void SpawnCloud(bool Start = false)
    {
        Vector3 spawnPos = new Vector3(
            Random.Range(SpawnRange.x, SpawnRange.y),
            SpawnHeight + Random.Range(HeightRange.x, HeightRange.y),
            Random.Range(SpawnRange.x, SpawnRange.y)
        );

        GameObject cloud = Instantiate(
            CloudModels[Random.Range(0, CloudModels.Length)],
            spawnPos,
            Quaternion.Euler(0f, Random.Range(0, 4) * 90f, 0f),
            CloudParent
        );

        float targetScale = Random.Range(ScaleRange.x, ScaleRange.y);
        cloud.transform.localScale = Vector3.zero;

        CloudData data = new CloudData
        {
            Instance = cloud,
            TargetScale = targetScale,
            DecaySpeed = Random.Range(1f, 6f),
            MovementSpeed = Random.Range(0.1f, 1f),
            IsSpawning = true,
            InitialAltitude = spawnPos.y,
            UpdateInterval = Random.Range(1.0f / 20.0f, 1.0f / 10.0f),
            AccumulatedTime = 0
        };

        CloudInstances.Add(data);
        if (!Start)
        {
            StartCoroutine(SpawnLerp(data));
        }
        else
        {
            data.Instance.transform.localScale = Vector3.one * data.TargetScale;
        }
    }

    private IEnumerator SpawnLerp(CloudData data)
    {
        float t = 0f;

        while (t < 1f && data.Instance != null)
        {
            t += Time.deltaTime / SpawnLerpTime;
            float scale = Mathf.Lerp(0f, data.TargetScale, t);
            data.Instance.transform.localScale = Vector3.one * scale;
            yield return null;
        }

        if (data.Instance != null)
        {
            data.Instance.transform.localScale = Vector3.one * data.TargetScale;
        }

        data.IsSpawning = false;
    }

    private void Update()
    {
        Direction.x = Mathf.PerlinNoise(XSeed + Time.time, 0f) * 3f;
        Direction.y = Mathf.PerlinNoise(0f, YSeed + Time.time) * 3f;

        for (int i = CloudInstances.Count - 1; i >= 0; i--)
        {
            CloudData cloud = CloudInstances[i];
            cloud.AccumulatedTime += Time.deltaTime;

            if (cloud.AccumulatedTime > cloud.UpdateInterval)
            {
                float FrameTime = cloud.AccumulatedTime;
                cloud.AccumulatedTime = 0;

                Transform CloudTransform = cloud.Instance.transform;

                // Movement
                CloudTransform.position += new Vector3(Direction.x, 0f, Direction.y) * FrameTime * cloud.MovementSpeed;

                // Horizon effect
                Vector2 flatPos = new Vector2(CloudTransform.position.x, CloudTransform.position.z);
                float distance = flatPos.magnitude;

                if (distance > HorizonRadius && CloudTransform.position.y > 0)
                {
                    CloudTransform.position = new Vector3(
                        CloudTransform.position.x,
                        Mathf.SmoothStep(cloud.InitialAltitude, 0, (distance - 500.0f) / 500.0f),
                        CloudTransform.position.z);
                }

                // Decay out (was originally going to use transparency)
                if (!cloud.IsSpawning)
                {
                    CloudTransform.localScale -= Vector3.one * cloud.DecaySpeed * FrameTime;

                    if (CloudTransform.localScale.x <= 0f)
                    {
                        Destroy(cloud.Instance);
                        CloudInstances.RemoveAt(i);
                    }
                }
            }
        }

        // Maintain cloud count
        while (CloudInstances.Count < MaxNumberOfCloudModels)
        {
            SpawnCloud();
        }
    }
}