using UnityEngine;
using System.Collections.Generic;

public class DronePickupManager : MonoBehaviour
{
    public GameObject dronePrefab;
    public int poolSize = 2;

    private List<GameObject> dronePool = new List<GameObject>();
    private Dictionary<Transform, GameObject> activePickups = new Dictionary<Transform, GameObject>();

    void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject drone = Instantiate(dronePrefab, Vector3.zero, Quaternion.identity);
            drone.SetActive(false);
            dronePool.Add(drone);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            TryTogglePickup("Grab2P"); // Player2
        }
        else if (Input.GetKeyDown(KeyCode.RightShift))
        {
            TryTogglePickup("Grab1P"); // Player1
        }
    }

    void TryTogglePickup(string layerName)
    {
        int layer = LayerMask.NameToLayer(layerName);
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (var player in players)
        {
            if (player.layer == layer)
            {
                if (activePickups.ContainsKey(player.transform))
                {
                    // 해제
                    DronePickupController dpc = activePickups[player.transform].GetComponent<DronePickupController>();
                    dpc.ForceRelease();
                    activePickups.Remove(player.transform);
                }
                else
                {
                    GameObject drone = GetAvailableDrone();
                    if (drone != null)
                    {
                        drone.SetActive(true);
                        DronePickupController dpc = drone.GetComponent<DronePickupController>();
                        dpc.Initialize(player.transform, this);
                        activePickups[player.transform] = drone;
                    }
                }

                break; // 한 명만 처리
            }
        }
    }

    GameObject GetAvailableDrone()
    {
        foreach (var drone in dronePool)
        {
            if (!drone.activeInHierarchy)
                return drone;
        }
        return null;
    }

    public void UnmarkPickup(Transform player)
    {
        if (activePickups.ContainsKey(player))
        {
            activePickups.Remove(player);
        }
    }
}
