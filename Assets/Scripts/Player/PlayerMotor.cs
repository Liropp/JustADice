using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerMotor : MonoBehaviour
{
    // Ref(s)
    private PlayerController playerController;

    [Header("Rotation Settings")]
    [SerializeField] private float rotSpeed;
    [SerializeField] private float rotCooldown;
    private float t;
    private float timer;

    //Rotation : End rotation
    private Quaternion rotEnd;

    private int multi_moveMaxPerTurn = 3;
    [HideInInspector] public int multi_movePerTurn = 0;

    private void Awake()
    {
        // Reset
        t = 0.0f;

        // Set ref(s)
        playerController = gameObject.GetComponent<PlayerController>();
    }

    public void Move(Vector3 dir)
    {
        // The cooldown is finish ?
        if (timer >= rotCooldown)
        {
            // Move the dice one square
            transform.Translate(dir, Space.Self);
        }
    }

    public void Rotate(Quaternion _rotEnd)
    {
        // The cooldown is finish ?
        if (timer >= rotCooldown)
        {
            rotEnd = _rotEnd;

            // Reset
            timer = 0;
            t = 0.0f;

            // End turn
            if (!playerController.isTuto)
            {
                if (FindObjectOfType<GameManager>().isMulti)
                {
                    multi_movePerTurn++;
                    //Debug.Log(multi_movePerTurn + "/" + multi_moveMaxPerTurn);

                    if (multi_movePerTurn >= multi_moveMaxPerTurn)
                    {
                        playerController.SetcanMove(false);
                    }
                }
                else
                {
                    playerController.SetcanMove(false);
                }
            }
        }
    }

    public void FixedUpdate()
    {
        //Debug.Log(rotEnd.eulerAngles);
        //Debug.Log(playerController.playerGFX.transform.rotation.eulerAngles);

        timer += Time.deltaTime;
        t = (timer * rotSpeed) * rotCooldown;
        t = Mathf.Clamp01(t); // Make sure it stay between 0 & 1

        // The cooldown start ?
        if (timer < rotCooldown && rotEnd.eulerAngles != playerController.playerGFX.transform.rotation.eulerAngles)
        {
            // Rotate the dice in a direction
            playerController.playerGFX.transform.rotation = Quaternion.Lerp(playerController.playerGFX.transform.rotation, rotEnd, t);
        }
    }
}
