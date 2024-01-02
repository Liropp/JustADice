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
                hit.collider.gameObject.transform.position = transform.position;
                Destroy(this.gameObject);
            }
        }
    }
}
