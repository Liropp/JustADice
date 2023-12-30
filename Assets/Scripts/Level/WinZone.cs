using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(CallPlayerWon(0.75f));
        }
    }

    private IEnumerator CallPlayerWon(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        FindObjectOfType<InGameMenu>().PlayerWon();
    }
}
