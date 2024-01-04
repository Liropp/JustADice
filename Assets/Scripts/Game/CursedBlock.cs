using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursedBlock : MonoBehaviour
{
    [SerializeField] private LayerMask whatIsObstacle;

    private void Update()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.up, out hit, 1f, whatIsObstacle))
        {
            if(hit.collider.gameObject.CompareTag("Player"))
            {
                hit.collider.gameObject.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                Destroy(this.gameObject);
            }
        }
    }
}
