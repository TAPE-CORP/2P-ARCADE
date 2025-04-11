using System.Collections.Generic;
using UnityEngine;

public class ConveyorBeltLineDrawer : MonoBehaviour
{
    public GameObject conveyorSegmentPrefab;
    public Transform conveyorParent;

    [Header("경로 설정")]
    public int maxAttempts = 1000;
    public int maxSegmentLength = 10;

    public Vector2Int start = new Vector2Int(0, 50);
    public int endX = 200;
    public int allowedYError = 10;

    private HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
    private List<Vector2Int> finalPath = new List<Vector2Int>();

    void Start()
    {
        GeneratePath();
    }

    void GeneratePath()
    {
        foreach (Transform child in conveyorParent)
            Destroy(child.gameObject);

        visited.Clear();
        finalPath.Clear();

        List<Vector2Int> tempPath = new List<Vector2Int>();
        tempPath.Add(start);
        visited.Add(start);

        if (FindPath(start, tempPath, 0))
        {
            for (int i = 0; i < finalPath.Count - 1; i++)
            {
                CreateSegment(finalPath[i], finalPath[i + 1]);
            }
        }
        else
        {
            Debug.LogWarning("경로를 찾을 수 없습니다.");
        }
    }

    bool FindPath(Vector2Int current, List<Vector2Int> path, int attempts)
    {
        if (attempts > maxAttempts) return false;
        if (current.x == endX && Mathf.Abs(current.y - start.y) <= allowedYError)
        {
            finalPath = new List<Vector2Int>(path);
            return true;
        }

        List<Vector2Int> directions = new List<Vector2Int> {
            Vector2Int.right, Vector2Int.up, Vector2Int.down
        };
        Shuffle(directions);

        foreach (Vector2Int dir in directions)
        {
            for (int len = maxSegmentLength; len >= 1; len--)
            {
                Vector2Int next = current + dir * len;
                bool valid = true;
                List<Vector2Int> temp = new List<Vector2Int>();

                for (int i = 1; i <= len; i++)
                {
                    Vector2Int step = current + dir * i;
                    if (visited.Contains(step))
                    {
                        valid = false;
                        break;
                    }
                    temp.Add(step);
                }

                if (!valid) continue;

                path.Add(next);
                foreach (var p in temp) visited.Add(p);

                if (FindPath(next, path, attempts + 1)) return true;

                // 백트래킹
                path.RemoveAt(path.Count - 1);
                foreach (var p in temp) visited.Remove(p);
            }
        }
        return false;
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rnd = Random.Range(i, list.Count);
            T temp = list[i];
            list[i] = list[rnd];
            list[rnd] = temp;
        }
    }

    void CreateSegment(Vector2Int from, Vector2Int to)
    {
        Vector3 start = new Vector3(from.x, from.y, 0);
        Vector3 end = new Vector3(to.x, to.y, 0);
        Vector3 center = (start + end) / 2f;
        Vector3 dir = end - start;

        GameObject seg = Instantiate(conveyorSegmentPrefab, center, Quaternion.identity, conveyorParent);

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        seg.transform.rotation = Quaternion.Euler(0, 0, angle);
        seg.transform.localScale = new Vector3(dir.magnitude, 1f, 1f);

        var box = seg.GetComponent<BoxCollider2D>();
        if (box != null)
        {
            box.size = new Vector2(1f, 1f);
            box.offset = Vector2.zero;
            box.isTrigger = true;
        }

        var conveyor = seg.GetComponent<Conveyor>();
        if (conveyor != null)
        {
            conveyor.isRight = dir.x >= 0;
        }
    }
}
