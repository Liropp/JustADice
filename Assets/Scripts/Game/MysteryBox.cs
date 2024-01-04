using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MysteryBox : MonoBehaviour
{
    [Range(1,4)]
    [SerializeField] private int powerIndex;
    [SerializeField] private bool isRandom = false;
    [SerializeField] private bool canRespawn = false;
    [SerializeField] private int respawnMaxCooldown = 7;
    private int respawnCooldown = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (isRandom)
            {
                int random = Random.Range(1, 4 + 1);
                other.gameObject.GetComponent<PlayerController>().myPS.NewPower(random);
            }
            else
            {
                other.gameObject.GetComponent<PlayerController>().myPS.NewPower(powerIndex);
            }

            if (!canRespawn)
            {
                Destroy(gameObject);
            }
            else
            {
                respawnCooldown = FindObjectOfType<GameManager>().GetTurnCount() + respawnMaxCooldown;
                gameObject.GetComponent<BoxCollider>().enabled = false;
                gameObject.GetComponent<MeshRenderer>().enabled = false;
            }
        }
    }

    private void Update()
    {
        if (canRespawn)
        {
            if (FindObjectOfType<GameManager>().GetTurnCount() >= respawnCooldown)
            {
                gameObject.GetComponent<BoxCollider>().enabled = true;
                gameObject.GetComponent<MeshRenderer>().enabled = true;
            }
        }
    }
}
