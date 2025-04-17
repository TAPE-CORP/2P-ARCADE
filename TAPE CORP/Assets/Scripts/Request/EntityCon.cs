using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityCon : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y < -10)
        {
            Debug.Log("Ãß¶ô, ÆÄ±«" + this.gameObject.name);
            Destroy(gameObject);
        }
    }
}
