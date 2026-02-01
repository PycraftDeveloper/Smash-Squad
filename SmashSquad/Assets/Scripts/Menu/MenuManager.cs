using UnityEngine;

public class MenuManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        int MapIndex = Random.Range(0, Constants.MAPS.Length);
        MapIndex = 0; // Temporarily force Candyland map for intro sequence
        Registry.MenuBackgroundScene = Constants.MAPS[MapIndex];
        Registry.CoreGameInfrastructureObject.LoadMap(Constants.MAPS[MapIndex]);

        if (Registry.MenuBackgroundScene == Constants.CANDYLAND_MAP)
        {
            Vector3 initialPosition = Constants.CANDYLAND_ORBIT_CAMERA_POSITIONS[Random.Range(0, Constants.CANDYLAND_ORBIT_CAMERA_POSITIONS.Length)];
            Camera.main.transform.position = initialPosition;
            Camera.main.transform.LookAt(Vector3.zero);
        }
    }
}