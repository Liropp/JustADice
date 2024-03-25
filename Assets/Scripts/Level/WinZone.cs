using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinZone : MonoBehaviour
{
    [SerializeField] private float waitTimeTP = 0.75f;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(CallPlayerWon(waitTimeTP));
        }
    }

    private IEnumerator CallPlayerWon(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        // this make the player return right at the levels menu, and not to the main menu, before.
        PlayerPrefs.SetInt("Story", 1);

        FindObjectOfType<InGameMenu>().PlayerWon();
    }
}
