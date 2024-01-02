using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    [SerializeField] private VFX[] vfx_Prefabs;
    private GameObject moveInstance;
    private GameObject spawnInstance;
    private Vector3 _startPos;
    private Vector3 _endPos;
    private float time = 0;
    private bool canMove = false;

    private void Update()
    {
        if (canMove)
        {
            time += Time.deltaTime*2f;
            moveInstance.transform.position = Vector3.Lerp(_startPos, _endPos, time);

            Vector3 rot = new Vector3(_endPos.x, moveInstance.transform.position.y, _endPos.z);
            moveInstance.transform.LookAt(rot);

            if (time >= 1)
            {
                Destroy(moveInstance);
                time = 0;
                canMove = false;
            }
        }
    }

    public void Spawn(string vfxName, Vector3 spawnTransform, float duration)
    {
        VFX vfx = Array.Find(vfx_Prefabs, vfx => vfx.vfx_Name == vfxName);
        spawnInstance = Instantiate(vfx.vfx_Prefab, spawnTransform, vfx.vfx_Prefab.transform.rotation);

        if(duration < 9999f)
        {
            Destroy(spawnInstance, duration);
        }
    }

    public void SpawnMoving(string vfxName, Vector3 startPos, Vector3 endPos)
    {
        _startPos = startPos;
        _endPos = endPos;

        VFX vfx = Array.Find(vfx_Prefabs, vfx => vfx.vfx_Name == vfxName);
        moveInstance = Instantiate(vfx.vfx_Prefab, _startPos, vfx.vfx_Prefab.transform.rotation);

        canMove = true;
    }

    public void DestroySpawnInstance()
    {
        Destroy(spawnInstance);
    }

    public void SetParentSpawnInstance(Transform parent)
    {
        spawnInstance.transform.SetParent(parent);
    }
}
