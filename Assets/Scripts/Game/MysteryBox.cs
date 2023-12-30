using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MysteryBox : MonoBehaviour
{
    [Range(1,4)]
    [SerializeField] private int powerIndex;
    [SerializeField] private bool isRandom = false;

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

            FindObjectOfType<PowerSystem>().owner = other.gameObject;
            Destroy(gameObject);
        }
    }
}
