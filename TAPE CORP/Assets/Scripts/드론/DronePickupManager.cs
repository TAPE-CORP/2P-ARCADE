using UnityEngine;
using System.Collections.Generic;

public class DronePickupManager : MonoBehaviour
{
    public GameObject dronePrefab;
    public int poolSize = 2;

    private List<GameObject> dronePool = new List<GameObject>();
    private HashSet<Transform> rescuedPlayers = new HashSet<Transform>();

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
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (var player in players)
        {
            // y <= 0 이하면 드론 출동
            if (player.transform.position.y <= 0f && !rescuedPlayers.Contains(player.transform))
            {
                GameObject availableDrone = GetAvailableDrone();
                if (availableDrone != null)
                {
                    availableDrone.SetActive(true);
                    DronePickupController dpc = availableDrone.GetComponent<DronePickupController>();
                    dpc.Initialize(player.transform);
                    rescuedPlayers.Add(player.transform);
                }
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
}
