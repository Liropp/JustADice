using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CInematicPlayer : MonoBehaviour
{
    [SerializeField] private float cinematicWaitTime = 1;
    [SerializeField] private PlayableDirector pB;

    private void Start()
    {
        Invoke("PlayCine", cinematicWaitTime);
    }

    private void PlayCine()
    {
        pB.Play();
    }
}
