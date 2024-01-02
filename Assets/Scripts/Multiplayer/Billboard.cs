using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    [SerializeField] private Transform camDir;

    private void Update()
    {
        transform.LookAt(camDir);
    }
}
