using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(EnemyStats))]
public class EnemyBase : MonoBehaviour
{
    [Header("Layer(s)")]
    [SerializeField] LayerMask whatIsGround;
    [SerializeField] LayerMask whatIsObstacle;

    [Header("Movement Settings")]
    [SerializeField] private float turnCooldown;
    [SerializeField] Transform target;
    private float timer = 0;

    [Header("Raycasts")]
    [SerializeField] private float yOffset;
    private List<string> dir;
    private List<float> dist;

    // Shortest direction to reach the target
    private int minIndex;
    // Move or attack ?
    private int needToMove;
    // Ref
    private EnemyStats enemyStats;
    // If it's his turn, he can make a move
    public bool canMove = false;

    private Rigidbody rb;

    void Awake()
    {
        // Set variables
        dir = new List<string>();
        dist = new List<float>();
        needToMove = 0;
        timer = 0;
        enemyStats = gameObject.GetComponent<EnemyStats>();
        rb = gameObject.GetComponent<Rigidbody>();
    }

    void Update()
    {
        //Debug.Log(timer + " " + gameObject.name);

        // If the enemy is'nt falling in a hole
        if (rb.velocity.y >= 0)
        {
            // Start timer, when it's his turn
            if (canMove)
            {
                timer += Time.deltaTime;

                if (timer >= turnCooldown)
                {
                    CheckDir();
                }
            }
            else
            {
                timer = 0;
            }
        }
        else
        {
            Destroy(this.gameObject,1f);
            //Debug.Log("is falling");
        }
    }

    /// <summary>
    /// Check possiblities of mouvement
    /// </summary>
    private void CheckDir()
    {
        #region Can AI move ?

        #region Detect ground forward ?
        RaycastHit hitGround;
        if (Physics.Raycast(transform.position + transform.forward, transform.TransformDirection(-Vector3.up), out hitGround, Mathf.Infinity, whatIsGround))
        {
            //Debug.DrawRay(transform.position + transform.up * yOffset + transform.forward, transform.TransformDirection(-Vector3.up) * hit.distance, Color.yellow, 5f);
            //Debug.Log("Did Hit " + hit.collider.gameObject.name);

            #region Detect obstacle ?
            RaycastHit hitObstacle;
            if (Physics.Raycast(transform.position + transform.up * yOffset + transform.forward, transform.TransformDirection(-Vector3.up), out hitObstacle, Mathf.Infinity, whatIsObstacle))
            {
                dir.Remove("Forward");

                // Detect Player ?
                if(hitObstacle.collider.transform != target)
                {
                    needToMove++;
                }
            }
            else
            {
                dir.Add("Forward");

                float distF = Vector3.Distance(transform.position + transform.forward, target.position);
                dist.Add(distF);

                needToMove++;
            }
            #endregion
        }
        else
        {
            dir.Remove("Forward");

            needToMove++;
        }
        #endregion

        #region Detect ground backward ?
        if (Physics.Raycast(transform.position - transform.forward, transform.TransformDirection(-Vector3.up), out hitGround, Mathf.Infinity, whatIsGround))
        {
            #region Detect obstacle ?
            RaycastHit hitObstacle;
            if (Physics.Raycast(transform.position + transform.up * yOffset - transform.forward, transform.TransformDirection(-Vector3.up), out hitObstacle, Mathf.Infinity, whatIsObstacle))
            {
                dir.Remove("Backward");

                // Detect Player ?
                if (hitObstacle.collider.transform != target)
                {
                    needToMove++;
                }
            }
            else
            {
                dir.Add("Backward");

                float distB = Vector3.Distance(transform.position - transform.forward, target.position);
                dist.Add(distB);

                needToMove++;
            }
            #endregion
        }
        else
        {
            dir.Remove("Backward");

            needToMove++;
        }
        #endregion

        #region Detect ground right ?
        if (Physics.Raycast(transform.position + transform.right, transform.TransformDirection(-Vector3.up), out hitGround, Mathf.Infinity, whatIsGround))
        {
            #region Detect obstacle ?
            RaycastHit hitObstacle;
            if (Physics.Raycast(transform.position + transform.up * yOffset + transform.right, transform.TransformDirection(-Vector3.up), out hitObstacle, Mathf.Infinity, whatIsObstacle))
            {
                dir.Remove("Right");

                // Detect Player ?
                if (hitObstacle.collider.transform != target)
                {
                    needToMove++;
                }
            }
            else
            {
                dir.Add("Right");

                float distR = Vector3.Distance(transform.position + transform.right, target.position);
                dist.Add(distR);

                needToMove++;
            }
            #endregion
        }
        else
        {
            dir.Remove("Right");

            needToMove++;
        }
        #endregion

        #region Detect ground left ?
        if (Physics.Raycast(transform.position - transform.right, transform.TransformDirection(-Vector3.up), out hitGround, Mathf.Infinity, whatIsGround))
        {
            #region Detect obstacle ?
            RaycastHit hitObstacle;
            if (Physics.Raycast(transform.position + transform.up * yOffset - transform.right, transform.TransformDirection(-Vector3.up), out hitObstacle, Mathf.Infinity, whatIsObstacle))
            {
                dir.Remove("Left");

                // Detect Player ?
                if (hitObstacle.collider.transform != target)
                {
                    needToMove++;
                }
            }
            else
            {
                dir.Add("Left");

                float distL = Vector3.Distance(transform.position - transform.right, target.position);
                dist.Add(distL);

                needToMove++;
            }
            #endregion
        }
        else
        {
            dir.Remove("Left");

            needToMove++;
        }
        #endregion

        #endregion

        // if needToMove is < 4, this mean the AI is next to the player and don't need to move
        // if needToMove is >= 4, he need to move towards the player
        if (needToMove >= 4)
        {
            //Debug.Log("no player around");

            float minValue = dist.Min();
            minIndex = dist.IndexOf(minValue);
            //Debug.Log(minIndex);

            // If player is'nt next to him, he can't attack
            enemyStats.canAttack = false;

            ChooseDir();
        }
        else
        {
            //Debug.Log("player around");

            // Reset
            dir.Clear();
            dist.Clear();
            minIndex = 0;
            needToMove = 0;

            // If player is next to him, he can attack
            enemyStats.canAttack = true;
        }
    }
    
    /// <summary>
    /// Choose a random direction to move
    /// </summary>
    private void ChooseDir()
    {
        string direction = dir[minIndex];
        //Debug.Log(directions);

        switch (direction)
        {
            case "Forward":
                Move(Vector3.forward);
                break;
            case "Backward":
                Move(-Vector3.forward);
                break;
            case "Right":
                Move(Vector3.right);
                break;
            case "Left":
                Move(-Vector3.right);
                break;
        }
    }

    /// <summary>
    /// Move in the chosen direction
    /// </summary>
    /// <param name="moveDir"></param>
    /// <param name="rayVect"></param>
    private void Move(Vector3 moveDir)
    {
        // Move the enemy one square
        transform.Translate(moveDir, Space.Self);

        // Reset
        dir.Clear();
        dist.Clear();
        minIndex = 0;
        needToMove = 0;

        // End turn
        SetcanMove(false);
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
}
