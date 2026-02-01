using UnityEngine;

public class CameraRotator : MonoBehaviour
{
    public float RotationSpeed = 1.0f;

    // Update is called once per frame
    private void Update()
    {
        transform.Rotate(Vector3.up, Time.deltaTime * RotationSpeed, Space.World);
    }
}