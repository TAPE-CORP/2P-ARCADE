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
            if (player.transform.position.y <= 0f && !rescuedPlayers.Contains(player.transform))
            {
                GameObject availableDrone = GetAvailableDrone();
                if (availableDrone != null)
                {
                    availableDrone.SetActive(true);
                    DronePickupController dpc = availableDrone.GetComponent<DronePickupController>();
                    dpc.Initialize(player.transform, this);  // 매니저 넘기기
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
            {
                return drone;
            }
        }
        return null;
    }

    // 다시 구조 가능하게 만드는 메서드
    public void UnmarkRescuedPlayer(Transform playerTransform)
    {
        if (rescuedPlayers.Contains(playerTransform))
        {
            rescuedPlayers.Remove(playerTransform);
        }
    }
}
