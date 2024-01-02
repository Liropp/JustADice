using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(EnemyStats))]
public class EnemyBase : MonoBehaviour
{
    [Header("Layer(s)")]
    [SerializeField] LayerMask whatIsGround;
    [SerializeField] LayerMask whatIsObstacle;

    [Header("Movement Settings")]
    private Vector3 moveDir;
    private float moveRot;
    private float timeElapsed = 0;
    private Vector3 startPos;
    [SerializeField] private float turnCooldown;
    public Transform target;
    private float timer = 0;
    private float timerM = 0;

    public int moveDist { get; set; }

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

    [Header("Random_TP")]
    [SerializeField] private bool rdm_enable = true;
    [SerializeField] [Range(0,1)] private float rdm_change;
    private float rdm_nbr = 0;
    private bool rdm_canTP = false;
    private bool rdm_once = true;

    void Awake()
    {
        // Set variables
        dir = new List<string>();
        dist = new List<float>();
        needToMove = 0;
        timer = 0;
        enemyStats = gameObject.GetComponent<EnemyStats>();
    }

    void FixedUpdate()
    {
        //Debug.Log(timer + " " + gameObject.name);

        // If the enemy is'nt falling in a hole
        if (transform.position.y >= 0)
        {
            MyTurn();
        }
        else if(transform.position.y <= -0.75f)
        {
            Destroy(this.gameObject,1f);
            //Debug.Log("is falling");
        }

        //Debug.Log(transform.position.y);
    }

    private void MyTurn()
    {
        // Start timer, when it's his turn
        if (canMove)
        {
            if (timer >= turnCooldown / 2)
            {
                if(rdm_enable)
                RandomSkill();

                if (!rdm_canTP || !rdm_enable)
                {
                    //Debug.Log("Move");

                    CheckDir();
                }

                timer = turnCooldown / 2;
            }
            else
            {
                startPos = transform.position;
                timer += Time.deltaTime;
            }
        }
        else
        {
            timer = 0;
            rdm_once = true;
        }
    }

    /// <summary>
    /// this skill depend on chance, to create unexcepted actions, in order to surprise the player
    /// </summary>
    private void RandomSkill()
    {
        if (rdm_once)
        {
            // random between 0% and 100%
            rdm_nbr = Random.value;
            //Debug.Log(rdm_nbr);

            if (rdm_nbr <= rdm_change)
            {
                TP();
            }
            else
            {
                rdm_canTP = false;
            }

            rdm_once = false;
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
        if (Physics.Raycast(transform.position + transform.forward * moveDist, transform.TransformDirection(-Vector3.up), out hitGround, Mathf.Infinity, whatIsGround))
        {
            //Debug.DrawRay(transform.position + transform.forward * moveDist, transform.TransformDirection(-Vector3.up) * hitGround.distance, Color.red, 10f);
            //Debug.Log("Did Hit " + hitGround.collider.gameObject.name);

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

                float distF = Vector3.Distance(transform.position + transform.forward * moveDist, target.position);

                if (Physics.Raycast(transform.position + transform.forward * moveDist, transform.TransformDirection(Vector3.right), out hitObstacle, 2f, whatIsObstacle) || Physics.Raycast(transform.position + transform.forward * moveDist, transform.TransformDirection(-Vector3.right), out hitObstacle, 2f, whatIsObstacle))
                {
                    dist.Add(distF+1);

                    //Debug.Log("wall, not the best path");
                }
                else
                {
                    dist.Add(distF);
                }

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
        if (Physics.Raycast(transform.position - transform.forward * moveDist, transform.TransformDirection(-Vector3.up), out hitGround, Mathf.Infinity, whatIsGround))
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

                float distB = Vector3.Distance(transform.position - transform.forward * moveDist, target.position);

                //Debug.DrawRay(transform.position - transform.forward * moveDist, transform.TransformDirection(Vector3.right) * 1f, Color.red, 1f);
                Debug.DrawRay(transform.position - (transform.forward * moveDist) + transform.TransformDirection(Vector3.right), transform.TransformDirection(-Vector3.up) * 1f, Color.red, 1f);
                Debug.DrawRay(transform.position - (transform.forward * moveDist) + transform.TransformDirection(-Vector3.right), transform.TransformDirection(-Vector3.up) * 1f, Color.red, 1f);
                if (Physics.Raycast(transform.position - transform.forward * moveDist, transform.TransformDirection(Vector3.right), out hitObstacle, 2f, whatIsObstacle) || Physics.Raycast(transform.position - transform.forward * moveDist, transform.TransformDirection(-Vector3.right), out hitObstacle, 2f, whatIsObstacle))
                {
                    dist.Add(distB + 1);

                    //Debug.Log("wall, not the best path");
                }
                else
                {
                    dist.Add(distB);
                }

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

            MoveDir();
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
    private void MoveDir()
    {
        timerM += Time.fixedDeltaTime;
        //Debug.Log(timerM);

        if (minIndex <= dir.Count)
        {
            if(timeElapsed == 0)
            {
                string direction = dir[minIndex];
                //Debug.Log(direction);

                RaycastHit hitPlayer;
                RaycastHit hitGround;
                Vector3 ray_StartPos = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);

                switch (direction)
                {
                    case "Forward":
                        moveRot = 0;
                        transform.Rotate(0.0f, moveRot, 0.0f, Space.Self);
                        break;
                    case "Backward":
                        moveRot = 180;
                        transform.Rotate(0.0f, moveRot, 0.0f, Space.Self);
                        break;
                    case "Right":
                        moveRot = 90;
                        transform.Rotate(0.0f, moveRot, 0.0f, Space.Self);
                        break;
                    case "Left":
                        moveRot = -90;
                        transform.Rotate(0.0f, moveRot, 0.0f, Space.Self);
                        break;
                }

                if (Physics.Raycast(ray_StartPos, transform.forward, out hitPlayer, moveDist, whatIsObstacle))
                {
                    //Debug.DrawRay(ray_StartPos, transform.TransformDirection(Vector3.forward) * moveDist, Color.red, 10f);
                    //Debug.Log("Did Hit " + hitGround.collider.gameObject.name);

                    //Debug.Log("hitPlayer.distance: " + Mathf.FloorToInt(hitPlayer.distance));

                    Vector3 vect = hitPlayer.point - transform.forward/2f;
                    Debug.DrawRay(vect, -Vector3.up * 2f, Color.red, 10f);
                    if (Physics.Raycast(vect, -Vector3.up, out hitGround, 2f, whatIsGround))
                    {
                        moveDir = transform.forward * Mathf.FloorToInt(hitPlayer.distance);
                        Debug.Log("je me colle au joueur");
                    }
                    else
                    {
                        moveDir = Vector3.zero;
                        Debug.Log("je ne bouge pas");
                    }
                }
                else if(direction == "Forward" || direction == "Backward")
                {
                    moveDir = transform.forward * moveDist;
                    Debug.Log("je dash");
                }
                else if (direction == "Right" || direction == "Left")
                {
                    moveDir = transform.forward;
                    Debug.Log("j'avance doucement");
                }
            }

            // Move the enemy one square
            if (timerM < turnCooldown / 2)
            {
                var endPos = new Vector3(startPos.x + moveDir.x, startPos.y, startPos.z + moveDir.z);
                //Debug.Log(moveDir);
                //Debug.Log(endPos);
                float totalDistance = Vector3.Distance(startPos, endPos);
                float speed = totalDistance / (turnCooldown / 2);
                float t = timeElapsed * speed;
                //Debug.Log(t);
                //Debug.Log(totalDistance);
                //Debug.Log(speed);
                transform.position = Vector3.Lerp(startPos, endPos, t);
                timeElapsed += Time.fixedDeltaTime;
            }
            else
            {
                //Debug.Log("end");

                var endPos = startPos + moveDir;
                transform.position = Vector3.Lerp(startPos, endPos, 1);
                Played();
            }
        }
    }

    /// <summary>
    /// AI teleport next to the player
    /// </summary>
    private void TP()
    {
        //Debug.Log("TP");
        FindObjectOfType<AudioManager>().Play("BlackSpell");

        RaycastHit hitGround;
        int rdm_x = Random.Range(0, 4);
        switch (rdm_x)
        {
            case 0:
                Vector3 fwdPos = target.position + Vector3.forward;
                if (Physics.Raycast(fwdPos, -transform.up, out hitGround, 1f, whatIsGround) && !Physics.Raycast(target.position, transform.forward, out hitGround, Mathf.Infinity, whatIsObstacle))
                {
                    transform.position = fwdPos;
                }
                else
                {
                    //Debug.Log("void or wall forward");
                }
                break;
            case 1:
                Vector3 rPos = target.position + Vector3.right;
                if (Physics.Raycast(rPos, -transform.up, out hitGround, 1f, whatIsGround) && !Physics.Raycast(target.position, transform.right, out hitGround, Mathf.Infinity, whatIsObstacle))
                {
                    transform.position = rPos;
                }
                else
                {
                    //Debug.Log("void or wall right");
                }
                break;
            case 2:
                Vector3 bwdPos = target.position + -Vector3.forward;
                if (Physics.Raycast(bwdPos, -transform.up, out hitGround, 1f, whatIsGround) && !Physics.Raycast(target.position, -transform.forward, out hitGround, Mathf.Infinity, whatIsObstacle))
                {
                    transform.position = bwdPos;
                }
                else
                {
                    //Debug.Log("void or wall backward");
                }
                break;
            case 3:
                Vector3 lPos = target.position + -Vector3.right;
                if (Physics.Raycast(lPos, -transform.up, out hitGround, 1f, whatIsGround) && !Physics.Raycast(target.position, -transform.right, out hitGround, Mathf.Infinity, whatIsObstacle))
                {
                    transform.position = lPos;
                }
                else
                {
                    //Debug.Log("void or wall left");
                }
                break;
        }

        rdm_canTP = true;

        enemyStats.canAttack = false;
        Played();
    }

    /// <summary>
    /// AI played his turn
    /// </summary>
    /// <param name="moveDir"></param>
    /// <param name="rayVect"></param>
    private void Played()
    {
        // Reset
        dir.Clear();
        dist.Clear();
        minIndex = 0;
        needToMove = 0;
        timeElapsed = 0;
        timerM = 0;

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