using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour
{
    [Header("Ref(s)")]
    public GameObject playerGFX;
    PlayerMotor playerMotor;

    [Header("Layer(s)")]
    [SerializeField] LayerMask whatIsGround;
    [SerializeField] LayerMask whatIsObstacle;
    [SerializeField] LayerMask whatIsDiceSide;

    [Header("Raycasts")]
    [SerializeField] private float yOffset;

    // If it's his turn, he can make a move
    private bool canMove = true;

    void Awake()
    {
        // Set ref(s)
        playerMotor = gameObject.GetComponent<PlayerMotor>();
    }

    /// <summary>
    /// move the dice forward
    /// </summary>
    public void MoveForward()
    {
        #region Detect ground ?
        RaycastHit hit;
        if (Physics.Raycast(transform.position + transform.forward, transform.TransformDirection(-Vector3.up), out hit, Mathf.Infinity, whatIsGround))
        {
            #region Debug(s)
            //Debug.DrawRay(transform.position + transform.forward, transform.TransformDirection(-Vector3.up) * hit.distance, Color.yellow, 5f);
            //Debug.Log("Did Hit");
            #endregion

            #region Detect obstacle ?
            RaycastHit hitObstacle;
            if (!Physics.Raycast(transform.position + transform.up * yOffset + transform.forward, transform.TransformDirection(-Vector3.up), out hitObstacle, Mathf.Infinity, whatIsObstacle))
            {
                Move(transform.forward, Vector3.right);
            }
            #endregion
        }
        else
        {
            #region Debug(s)
            //Debug.DrawRay(transform.position + transform.forward, transform.TransformDirection(-Vector3.up) * 100, Color.white, 5f);
            //Debug.Log("Did not Hit");
            #endregion
        }
        #endregion
    }

    /// <summary>
    /// move the dice backward
    /// </summary>
    public void MoveBackward()
    {
        #region Dectect ground ?
        RaycastHit hit;
        if (Physics.Raycast(transform.position - transform.forward, transform.TransformDirection(-Vector3.up), out hit, Mathf.Infinity, whatIsGround))
        {
            #region Debug(s)
            //Debug.DrawRay(transform.position - transform.forward, transform.TransformDirection(-Vector3.up) * hit.distance, Color.yellow, 5f);
            //Debug.Log("Did Hit");
            #endregion

            #region Detect obstacle ?
            RaycastHit hitObstacle;
            if (!Physics.Raycast(transform.position + transform.up * yOffset - transform.forward, transform.TransformDirection(-Vector3.up), out hitObstacle, Mathf.Infinity, whatIsObstacle))
            {
                Move(-transform.forward, -Vector3.right);
            }
            #endregion
        }
        else
        {
            #region Debug(s)
            //Debug.DrawRay(transform.position - transform.forward, transform.TransformDirection(-Vector3.up) * 100, Color.white, 5f);
            //Debug.Log("Did not Hit");
            #endregion
        }
        #endregion
    }

    /// <summary>
    /// move the dice right
    /// </summary>
    public void MoveRight()
    {
        #region Detect ground ?
        RaycastHit hit;
        if (Physics.Raycast(transform.position + transform.right, transform.TransformDirection(-Vector3.up), out hit, Mathf.Infinity, whatIsGround))
        {
            #region Debug(s)
            //Debug.DrawRay(transform.position + transform.forward, transform.TransformDirection(-Vector3.up) * hit.distance, Color.yellow, 5f);
            //Debug.Log("Did Hit");
            #endregion

            #region Detect obstacle ?
            RaycastHit hitObstacle;
            if (!Physics.Raycast(transform.position + transform.up * yOffset + transform.right, transform.TransformDirection(-Vector3.up), out hitObstacle, Mathf.Infinity, whatIsObstacle))
            {
                Move(transform.right, -Vector3.forward);
            }
            #endregion
        }
        else
        {
            #region Debug(s)
            //Debug.DrawRay(transform.position + transform.right, transform.TransformDirection(-Vector3.up) * 100, Color.white, 5f);
            //Debug.Log("Did not Hit");
            #endregion
        }
        #endregion
    }

    /// <summary>
    /// move the dice left
    /// </summary>
    public void MoveLeft()
    {
        #region Dectect ground ?
        RaycastHit hit;
        if (Physics.Raycast(transform.position - transform.right, transform.TransformDirection(-Vector3.up), out hit, Mathf.Infinity, whatIsGround))
        {
            #region Debug(s)
            //Debug.DrawRay(transform.position - transform.right, transform.TransformDirection(-Vector3.up) * hit.distance, Color.yellow, 5f);
            //Debug.Log("Did Hit");
            #endregion

            #region Detect obstacle ?
            RaycastHit hitObstacle;
            if (!Physics.Raycast(transform.position + transform.up * yOffset - transform.right, transform.TransformDirection(-Vector3.up), out hitObstacle, Mathf.Infinity, whatIsObstacle))
            {
                Move(-transform.right, Vector3.forward);
            }
            #endregion
        }
        else
        {
            #region Debug(s)
            //Debug.DrawRay(transform.position - transform.right, transform.TransformDirection(-Vector3.up) * 100, Color.white, 5f);
            //Debug.Log("Did not Hit");
            #endregion
        }
        #endregion
    }

    /// <summary>
    /// send movement values to PlayerMotor
    /// </summary>
    void Move(Vector3 moveDir, Vector3 rotDir)
    {
        // My turn ?
        if (canMove)
        {
            // Move
            playerMotor.Move(moveDir);

            // Rotate
            Quaternion end = Quaternion.AngleAxis(90f, rotDir) * playerGFX.transform.rotation;
            playerMotor.Rotate(end);
        }
    }

    bool playOnce = true;

    /// <summary>
    /// choose between : start or end turn
    /// </summary>
    /// <param name="state"></param>
    public void SetcanMove(bool state)
    {
        canMove = state;
        //Debug.Log(canMove);

        if (!state && playOnce)
        {
            FindObjectOfType<GameManager>().pawnPlayed.Invoke();
            playOnce = false;
        }
        else
        {
            playOnce = true;
        }
    }

    /// <summary>
    /// get the value : start or end turn
    /// </summary>
    /// <returns></returns>
    public bool GetcanMove()
    {
        return canMove;
    }

    public string DiceUpColor()
    {
        RaycastHit hitColor;
        //Debug.DrawRay(transform.position, Vector3.up * 1f, Color.red, 3f);
        if (Physics.Raycast(transform.position, Vector3.up, out hitColor, 1f, whatIsDiceSide))
        {
            //Debug.Log("Up color = " + hitColor.collider.gameObject.name);
            return hitColor.collider.gameObject.name;
        }
        else
        {
            return "null";
        }
    }
}
