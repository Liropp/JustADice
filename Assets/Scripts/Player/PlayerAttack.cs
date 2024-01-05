using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviour
{
    //Ref
    private PlayerController playerController;

    // Can attack ?
    private bool canAttack = false;

    // Stats
    private int damage = 20;
    private int healPoints = 0;
    private bool isPoisoned = false;

    // Attack Btn
    public GameObject attackBtn;

    [Header("Spells")]
    [SerializeField] private Spell_Object[] spl_objects;
    [SerializeField] LayerMask whatIsGround;
    [SerializeField] LayerMask whatToAttack;
    [SerializeField] private RectMask2D spl_cooldownMask;
    [SerializeField] private GameObject spl_Lock;
    [SerializeField] private Sprite[] spellSprites;
    [SerializeField] private MeshRenderer[] spl_MeshRs;
    [SerializeField] private Material[] spl_MatUnlocked;
    [SerializeField] private Material[] spl_MatLocked;
    private bool enemyAround = false;
    private bool isDistantAttack = false;
    private bool canTeleport = false;
    private bool canHeal = false;
    private Spell_Object spl_selected;
    string groundColor;

    private void Awake()
    {
        playerController = gameObject.GetComponent<PlayerController>();

        foreach (var spell in spl_objects)
        {
            spell.curUseSpellCooldown = 0;
            spell.canUseSpell = true;
            spell.enable = true;
        }
    }

    private void Update()
    {
        if (!playerController.isTuto)
        {
            DetectTargetAround();
        }

        // If player can attack & choose a target
        if (canAttack && Input.GetKeyDown(KeyCode.Mouse0) && playerController.GetcanMove())
        {
            DetectTarget();
        }

        SetupCurSpell();

        //Debug.Log(spl_Black.curUseSpellCooldown);

        if (playerController.GetcanMove())
        {
            if (canDecreaseCooldown)
            {
                DecreaseCooldown();
                canDecreaseCooldown = false;
            }
        }
    }

    /// <summary>
    /// detect target, who player choose to attack
    /// </summary>
    private void DetectTarget()
    {
        // Raycast to detect the gameObject at the mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, whatToAttack))
        {
            // An object is hit
            GameObject hitObject = hit.collider.gameObject;

            GameManager gM = FindObjectOfType<GameManager>();
            if (!gM.isMulti)
            {
                // Check if the object is an enemy and if he next to player, in order to give him damage
                if (hitObject.CompareTag("Enemy") && !canTeleport && !canHeal)
                {
                    float dist = Mathf.Round(Vector3.Distance(hitObject.transform.position, transform.position));
                    //Debug.Log(dist);

                    if (dist <= 1 || isDistantAttack)
                    {
                        //Debug.Log(hitObject.name + " hit");
                        hitObject.GetComponent<EnemyStats>().TakeDamage(damage, isPoisoned);

                        switch (playerController.DiceUpColor())
                        {
                            case "Green":
                                //Debug.Log("Poison");
                                FindObjectOfType<AudioManager>().Play("GreenSpell");

                                break;
                            case "Red":
                                //Debug.Log("Poison");
                                FindObjectOfType<AudioManager>().Play("RedSpell");
                                #region particles
                                Vector3 RstartPos = new Vector3(this.transform.position.x, this.transform.position.y + 0.5f, this.transform.position.z);
                                Vector3 Rpos = hitObject.transform.position;
                                FindObjectOfType<VFXManager>().SpawnMoving("Punch", RstartPos, Rpos);
                                #endregion

                                break;
                            case "Yellow":
                                //Debug.Log("Poison");
                                FindObjectOfType<AudioManager>().Play("YellowSpell");
                                #region particles
                                Vector3 Ypos = hitObject.transform.position;
                                FindObjectOfType<VFXManager>().SpawnMoving("Arrow", this.transform.position, Ypos);
                                #endregion

                                break;
                            case "Blue":
                                //Debug.Log("Poison");
                                FindObjectOfType<AudioManager>().Play("BlueSpell");
                                #region particles
                                Vector3 BstartPos = new Vector3(this.transform.position.x, this.transform.position.y + 0.5f, this.transform.position.z);
                                Vector3 Bpos = hitObject.transform.position;
                                FindObjectOfType<VFXManager>().SpawnMoving("Punchs", BstartPos, Bpos);
                                #endregion

                                break;
                        }

                        attackBtn.SetActive(true);

                        // End turn
                        playerController.SetcanMove(false);

                        canAttack = false;
                    }
                }
            }
            else
            {
                // Check if the object is another player and if he next to this player, in order to give him damage
                if (hitObject.CompareTag("Player") && hitObject.gameObject != this.gameObject && !canTeleport && !canHeal)
                {
                    float dist = Mathf.Round(Vector3.Distance(hitObject.transform.position, transform.position));
                    //Debug.Log(dist);

                    if (dist <= 1 || isDistantAttack)
                    {
                        //Debug.Log(hitObject.name + " hit");
                        hitObject.GetComponent<PlayerHP>().PlayerTakeDamage(damage, isPoisoned);

                        switch (playerController.DiceUpColor())
                        {
                            case "Green":
                                //Debug.Log("Poison");
                                FindObjectOfType<AudioManager>().Play("GreenSpell");

                                break;
                            case "Red":
                                //Debug.Log("Poison");
                                FindObjectOfType<AudioManager>().Play("RedSpell");
                                #region particles
                                Vector3 RstartPos = new Vector3(this.transform.position.x, this.transform.position.y + 0.5f, this.transform.position.z);
                                Vector3 Rpos = hitObject.transform.position;
                                FindObjectOfType<VFXManager>().SpawnMoving("Punch", RstartPos, Rpos);
                                #endregion

                                break;
                            case "Yellow":
                                //Debug.Log("Poison");
                                FindObjectOfType<AudioManager>().Play("YellowSpell");
                                #region particles
                                Vector3 Ypos = hitObject.transform.position;
                                FindObjectOfType<VFXManager>().SpawnMoving("Arrow", this.transform.position, Ypos);
                                #endregion

                                break;
                            case "Blue":
                                //Debug.Log("Poison");
                                FindObjectOfType<AudioManager>().Play("BlueSpell");
                                #region particles
                                Vector3 BstartPos = new Vector3(this.transform.position.x, this.transform.position.y + 0.5f, this.transform.position.z);
                                Vector3 Bpos = hitObject.transform.position;
                                FindObjectOfType<VFXManager>().SpawnMoving("Punchs", BstartPos, Bpos);
                                #endregion

                                break;
                        }

                        attackBtn.SetActive(true);

                        // End turn
                        playerController.SetcanMove(false);

                        canAttack = false;
                    }
                }
            }

            if (hitObject.layer == 3 && canTeleport)
            {
                Teleportation(hit.point);
            }
        }
    }

    /// <summary>
    /// player can tp with the black spell
    /// </summary>
    /// <param name="_hitPoint"></param>
    private void Teleportation(Vector3 _hitPoint)
    {
        //Debug.Log("player teleport");
        FindObjectOfType<AudioManager>().Play("BlackSpell");
        #region particles
        Vector3 pos = new Vector3(this.transform.position.x, this.transform.position.y - .5f, this.transform.position.z);
        FindObjectOfType<VFXManager>().Spawn("TP", pos, 3f);
        #endregion

        Vector3 centeredPos = new Vector3(Mathf.RoundToInt(_hitPoint.x), transform.position.y, Mathf.RoundToInt(_hitPoint.z));
        //Debug.Log(centeredPos);
        //Debug.Log(_hitPoint);
        transform.position = centeredPos;

        attackBtn.SetActive(true);

        // End turn
        playerController.SetcanMove(false);

        canAttack = false;
    }

    /// <summary>
    /// detect if there is an enemy around the player in order to attack
    /// </summary>
    private void DetectTargetAround()
    {
        Vector3 forwardPos = transform.position + Vector3.forward;
        Vector3 backPos = transform.position + Vector3.back;
        Vector3 rightPos = transform.position + Vector3.right;
        Vector3 leftPos = transform.position + Vector3.left;
        float rad = 0.35f;

        Collider[] hitForward = Physics.OverlapSphere(forwardPos, rad);
        Collider[] hitBack = Physics.OverlapSphere(backPos, rad);
        Collider[] hitRight = Physics.OverlapSphere(rightPos, rad);
        Collider[] hitLeft = Physics.OverlapSphere(leftPos, rad);
        
        if(hitForward.Length > 0)
        {
            foreach (var hitCollider in hitForward)
            {
                // An object is hit
                GameObject hitObject = hitCollider.gameObject;
                GameManager gM = FindObjectOfType<GameManager>();

                if (!gM.isMulti)
                {
                    // Check if the object is an Enemy
                    if (hitObject.CompareTag("Enemy"))
                    {
                        //Debug.Log("enemy around");
                        enemyAround = true;
                    }
                }
                else
                {
                    // Check if the object is another Player
                    if (hitObject.CompareTag("Player") && hitObject.gameObject != this.gameObject)
                    {
                        //Debug.Log("another player around");
                        enemyAround = true;
                    }
                }
            }
        }
        
        if (hitBack.Length > 0)
        {
            foreach (var hitCollider in hitBack)
            {
                // An object is hit
                GameObject hitObject = hitCollider.gameObject;
                GameManager gM = FindObjectOfType<GameManager>();

                if (!gM.isMulti)
                {
                    // Check if the object is an Enemy
                    if (hitObject.CompareTag("Enemy"))
                    {
                        //Debug.Log("enemy around");
                        enemyAround = true;
                    }
                }
                else
                {
                    // Check if the object is another Player
                    if (hitObject.CompareTag("Player") && hitObject.gameObject != this.gameObject)
                    {
                        //Debug.Log("another player around");
                        enemyAround = true;
                    }
                }
            }
        }
        
        if (hitRight.Length > 0)
        {
            foreach (var hitCollider in hitRight)
            {
                // An object is hit
                GameObject hitObject = hitCollider.gameObject;
                GameManager gM = FindObjectOfType<GameManager>();

                if (!gM.isMulti)
                {
                    // Check if the object is an Enemy
                    if (hitObject.CompareTag("Enemy"))
                    {
                        //Debug.Log("enemy around");
                        enemyAround = true;
                    }
                }
                else
                {
                    // Check if the object is another Player
                    if (hitObject.CompareTag("Player") && hitObject.gameObject != this.gameObject)
                    {
                        //Debug.Log("another player around");
                        enemyAround = true;
                    }
                }
            }
        }

        if (hitLeft.Length > 0)
        {
            foreach (var hitCollider in hitLeft)
            {
                // An object is hit
                GameObject hitObject = hitCollider.gameObject;
                GameManager gM = FindObjectOfType<GameManager>();

                if (!gM.isMulti)
                {
                    // Check if the object is an Enemy
                    if (hitObject.CompareTag("Enemy"))
                    {
                        //Debug.Log("enemy around");
                        enemyAround = true;
                    }
                }
                else
                {
                    // Check if the object is another Player
                    if (hitObject.CompareTag("Player") && hitObject.gameObject != this.gameObject)
                    {
                        //Debug.Log("another player around");
                        enemyAround = true;
                    }
                }
            }
        }

        if(hitForward.Length <= 0 && hitBack.Length <= 0 && hitRight.Length <= 0 && hitLeft.Length <= 0)
        {
            enemyAround = false;
        }

        // Bug fix
        if (!enemyAround && !attackBtn.activeInHierarchy && !canHeal && !canTeleport && !isDistantAttack)
        {
            //Debug.Log("Bug");

            canAttack = false;
            attackBtn.SetActive(true);
            spl_selected.canUseSpell = true;
            spl_selected.curUseSpellCooldown = 0;
        }
    }

    /// <summary>
    /// player select attack button
    /// </summary>
    public void Attack()
    {
        Debug.Log("spl_selected.canUseSpell : " + spl_selected.canUseSpell);
        Debug.Log("spl_selected.curUseSpellCooldown : " + spl_selected.curUseSpellCooldown);

        //Debug.Log("spl_selected.canUseSpell " + spl_selected.canUseSpell);
        //Debug.Log("spl_selected.enable " + spl_selected.enable);
        if (spl_selected.canUseSpell && spl_selected.enable)
        {
            // only if there is an enemy next to player
            if (enemyAround || isDistantAttack || canHeal || canTeleport && playerController.GetcanMove())
            {
                canAttack = true;
                attackBtn.SetActive(false);

                if (canHeal)
                {
                    FindObjectOfType<AudioManager>().Play("PinkSpell");
                    #region particles
                    Vector3 pos = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
                    FindObjectOfType<VFXManager>().Spawn("Heal", pos, 2f);
                    #endregion
                    gameObject.GetComponent<PlayerHP>().Heal(healPoints);
                    attackBtn.SetActive(true);

                    // End turn
                    playerController.SetcanMove(false);

                    canAttack = false;
                }

                spl_selected.curUseSpellCooldown = spl_selected.useSpellMaxCooldown;
                spl_selected.canUseSpell = false;
            }
            else
            {
                attackBtn.SetActive(true);
                canAttack = false;
            }
        }

        if (!spl_selected.canUseSpell)
        {
            if(spl_selected.curUseSpellCooldown <= 0)
            {
                spl_selected.canUseSpell = true;
            }
        }
    }

    /// <summary>
    /// setup color and stats for the selected spell
    /// </summary>
    private void SetupCurSpell()
    {
        RaycastHit down;
        if (Physics.Raycast(transform.position, -transform.up, out down, 1f, whatIsGround))
        {
            groundColor = down.collider.gameObject.name;
            //Debug.Log(groundColor);
        }

        switch (playerController.DiceUpColor())
        {
            case "Green":
                //Debug.Log("Poison");

                // ref
                spl_selected = spl_objects[2];

                if (spl_selected.enable)
                {
                    // stats
                    #region cooldown mask
                    if (spl_selected.useSpellMaxCooldown <= 0)
                    {
                        RefreshSpellCdText(50);
                    }
                    else
                    {
                        float result = spl_selected.useSpellMaxCooldown - spl_selected.curUseSpellCooldown;
                        if (result <= 0)
                        {
                            RefreshSpellCdText(0);
                        }
                        else if (result >= spl_selected.useSpellMaxCooldown)
                        {
                            RefreshSpellCdText(50);
                        }
                        else
                        {
                            RefreshSpellCdText(25);
                        }
                    }
                    #endregion

                    attackBtn.GetComponent<Image>().sprite = spellSprites[0];
                    attackBtn.GetComponent<Image>().color = Color.green;
                    spl_Lock.SetActive(false);
                    spl_MeshRs[0].material = spl_MatUnlocked[0];

                    if (groundColor == "Green")
                    {
                        damage = spl_objects[2].damage * 2;
                    }
                    else
                    {
                        damage = spl_objects[2].damage;
                    }

                    healPoints = spl_objects[2].healPoints;
                    isPoisoned = spl_objects[2]._isPoisoned;
                    isDistantAttack = spl_objects[2]._isDistantAttack;
                    canTeleport = spl_objects[2]._canTeleport;
                    canHeal = spl_objects[2]._canHeal;
                }
                else
                {
                    attackBtn.GetComponent<Image>().sprite = spellSprites[0];
                    attackBtn.GetComponent<Image>().color = Color.grey;
                    spl_Lock.SetActive(true);
                    spl_MeshRs[0].material = spl_MatLocked[0];
                    RefreshSpellCdText(0);
                    damage = 0;
                    healPoints = 0;
                    isPoisoned = false;
                    isDistantAttack = false;
                    canTeleport = false;
                    canHeal = false;
                }

                break;
            case "Black":
                //Debug.Log("TP");

                // ref
                spl_selected = spl_objects[0];

                if (spl_selected.enable)
                {
                    // stats
                    #region cooldown mask
                    if (spl_selected.useSpellMaxCooldown <= 0)
                    {
                        RefreshSpellCdText(50);
                    }
                    else
                    {
                        float result = spl_selected.useSpellMaxCooldown - spl_selected.curUseSpellCooldown;
                        if (result <= 0)
                        {
                            RefreshSpellCdText(0);
                        }
                        else if (result >= spl_selected.useSpellMaxCooldown)
                        {
                            RefreshSpellCdText(50);
                        }
                        else
                        {
                            RefreshSpellCdText(25);
                        }
                    }
                    #endregion

                    attackBtn.GetComponent<Image>().sprite = spellSprites[1];
                    attackBtn.GetComponent<Image>().color = Color.black;
                    spl_Lock.SetActive(false);
                    spl_MeshRs[1].material = spl_MatUnlocked[1];

                    damage = spl_objects[0].damage;
                    healPoints = spl_objects[0].healPoints;
                    isPoisoned = spl_objects[0]._isPoisoned;
                    isDistantAttack = spl_objects[0]._isDistantAttack;
                    canTeleport = spl_objects[0]._canTeleport;
                    canHeal = spl_objects[0]._canHeal;
                }
                else
                {
                    attackBtn.GetComponent<Image>().sprite = spellSprites[1];
                    attackBtn.GetComponent<Image>().color = Color.grey;
                    spl_Lock.SetActive(true);
                    spl_MeshRs[1].material = spl_MatLocked[1];
                    RefreshSpellCdText(0);
                    damage = 0;
                    healPoints = 0;
                    isPoisoned = false;
                    isDistantAttack = false;
                    canTeleport = false;
                    canHeal = false;
                }

                break;
            case "Pink":
                //Debug.Log("Heal");

                // ref
                spl_selected = spl_objects[3];

                if (spl_selected.enable)
                {
                    // stats
                    #region cooldown mask
                    if (spl_selected.useSpellMaxCooldown <= 0)
                    {
                        Debug.Log("no cooldown");
                        RefreshSpellCdText(50);
                    }
                    else
                    {
                        float result = spl_selected.useSpellMaxCooldown - spl_selected.curUseSpellCooldown;
                        if (result <= 0)
                        {
                            Debug.Log("cooldown start");

                            RefreshSpellCdText(0);
                        }
                        else if (result >= spl_selected.useSpellMaxCooldown)
                        {
                            RefreshSpellCdText(50);
                        }
                        else
                        {
                            RefreshSpellCdText(25);
                        }
                    }
                    #endregion

                    attackBtn.GetComponent<Image>().sprite = spellSprites[2];
                    attackBtn.GetComponent<Image>().color = Color.magenta;
                    spl_Lock.SetActive(false);
                    spl_MeshRs[2].material = spl_MatUnlocked[2];

                    damage = spl_objects[3].damage;

                    if (groundColor == "Pink")
                    {
                        healPoints = spl_objects[3].healPoints * 2;
                    }
                    else
                    {
                        healPoints = spl_objects[3].healPoints;
                    }

                    isPoisoned = spl_objects[3]._isPoisoned;
                    isDistantAttack = spl_objects[3]._isDistantAttack;
                    canTeleport = spl_objects[3]._canTeleport;
                    canHeal = spl_objects[3]._canHeal;
                }
                else
                {
                    attackBtn.GetComponent<Image>().sprite = spellSprites[2];
                    attackBtn.GetComponent<Image>().color = Color.grey;
                    spl_Lock.SetActive(true);
                    spl_MeshRs[2].material = spl_MatLocked[2];
                    RefreshSpellCdText(0);
                    damage = 0;
                    healPoints = 0;
                    isPoisoned = false;
                    isDistantAttack = false;
                    canTeleport = false;
                    canHeal = false;
                }

                break;
            case "Yellow":
                //Debug.Log("Bow");

                // ref
                spl_selected = spl_objects[5];

                if (spl_selected.enable)
                {
                    // stats
                    #region cooldown mask
                    if (spl_selected.useSpellMaxCooldown <= 0)
                    {
                        RefreshSpellCdText(50);
                    }
                    else
                    {
                        float result = spl_selected.useSpellMaxCooldown - spl_selected.curUseSpellCooldown;
                        if (result <= 0)
                        {
                            RefreshSpellCdText(0);
                        }
                        else if (result >= spl_selected.useSpellMaxCooldown)
                        {
                            RefreshSpellCdText(50);
                        }
                        else
                        {
                            RefreshSpellCdText(25);
                        }
                    }
                    #endregion

                    attackBtn.GetComponent<Image>().sprite = spellSprites[3];
                    attackBtn.GetComponent<Image>().color = Color.yellow;
                    spl_Lock.SetActive(false);
                    spl_MeshRs[3].material = spl_MatUnlocked[3];

                    if (groundColor == "Yellow")
                    {
                        damage = spl_objects[5].damage * 3;
                    }
                    else
                    {
                        damage = spl_objects[5].damage;
                    }

                    healPoints = spl_objects[5].healPoints;
                    isPoisoned = spl_objects[5]._isPoisoned;
                    isDistantAttack = spl_objects[5]._isDistantAttack;
                    canTeleport = spl_objects[5]._canTeleport;
                    canHeal = spl_objects[5]._canHeal;
                }
                else
                {
                    attackBtn.GetComponent<Image>().sprite = spellSprites[3];
                    attackBtn.GetComponent<Image>().color = Color.grey;
                    spl_Lock.SetActive(true);
                    spl_MeshRs[3].material = spl_MatLocked[3];
                    RefreshSpellCdText(0);
                    damage = 0;
                    healPoints = 0;
                    isPoisoned = false;
                    isDistantAttack = false;
                    canTeleport = false;
                    canHeal = false;
                }

                break;
            case "Red":
                //Debug.Log("Ultime");

                // ref
                spl_selected = spl_objects[4];

                if (spl_selected.enable)
                {
                    // stats
                    #region cooldown mask
                    if (spl_selected.useSpellMaxCooldown <= 0)
                    {
                        RefreshSpellCdText(50);
                    }
                    else
                    {
                        float result = spl_selected.useSpellMaxCooldown - spl_selected.curUseSpellCooldown;
                        if (result <= 0)
                        {
                            RefreshSpellCdText(0);
                        }
                        else if (result >= spl_selected.useSpellMaxCooldown)
                        {
                            RefreshSpellCdText(50);
                        }
                        else
                        {
                            RefreshSpellCdText(25);
                        }
                    }
                    #endregion

                    attackBtn.GetComponent<Image>().sprite = spellSprites[4];
                    attackBtn.GetComponent<Image>().color = Color.red;
                    spl_Lock.SetActive(false);
                    spl_MeshRs[4].material = spl_MatUnlocked[4];

                    if (groundColor == "Red")
                    {
                        damage = spl_objects[4].damage * 2;
                    }
                    else
                    {
                        damage = spl_objects[4].damage;
                    }

                    healPoints = spl_objects[4].healPoints;
                    isPoisoned = spl_objects[4]._isPoisoned;
                    isDistantAttack = spl_objects[4]._isDistantAttack;
                    canTeleport = spl_objects[4]._canTeleport;
                    canHeal = spl_objects[4]._canHeal;
                }
                else
                {
                    attackBtn.GetComponent<Image>().sprite = spellSprites[4];
                    attackBtn.GetComponent<Image>().color = Color.grey;
                    spl_Lock.SetActive(true);
                    spl_MeshRs[4].material = spl_MatLocked[4];
                    RefreshSpellCdText(0);
                    damage = 0;
                    healPoints = 0;
                    isPoisoned = false;
                    isDistantAttack = false;
                    canTeleport = false;
                    canHeal = false;
                }

                break;
            case "Blue":
                //Debug.Log("Base attack");

                // ref
                spl_selected = spl_objects[1];

                if (spl_selected.enable)
                {
                    // stats
                    #region cooldown mask
                    if (spl_selected.useSpellMaxCooldown <= 0)
                    {
                        RefreshSpellCdText(50);
                    }
                    else
                    {
                        float result = spl_selected.useSpellMaxCooldown - spl_selected.curUseSpellCooldown;
                        if (result <= 0)
                        {
                            RefreshSpellCdText(0);
                        }
                        else if (result >= spl_selected.useSpellMaxCooldown)
                        {
                            RefreshSpellCdText(50);
                        }
                        else
                        {
                            RefreshSpellCdText(25);
                        }
                    }
                    #endregion

                    attackBtn.GetComponent<Image>().sprite = spellSprites[5];
                    attackBtn.GetComponent<Image>().color = Color.blue;
                    spl_Lock.SetActive(false);
                    spl_MeshRs[5].material = spl_MatUnlocked[5];

                    if (groundColor == "Blue")
                    {
                        damage = spl_objects[1].damage * 3;
                    }
                    else
                    {
                        damage = spl_objects[1].damage;
                    }

                    healPoints = spl_objects[1].healPoints;
                    isPoisoned = spl_objects[1]._isPoisoned;
                    isDistantAttack = spl_objects[1]._isDistantAttack;
                    canTeleport = spl_objects[1]._canTeleport;
                    canHeal = spl_objects[1]._canHeal;
                }
                else
                {
                    attackBtn.GetComponent<Image>().sprite = spellSprites[5];
                    attackBtn.GetComponent<Image>().color = Color.grey;
                    spl_Lock.SetActive(true);
                    spl_MeshRs[5].material = spl_MatLocked[5];
                    RefreshSpellCdText(0);
                    damage = 0;
                    healPoints = 0;
                    isPoisoned = false;
                    isDistantAttack = false;
                    canTeleport = false;
                    canHeal = false;
                }

                break;
        }
    }

    /// <summary>
    /// refresh the text on the attack button showing each cooldown for spells
    /// </summary>
    /// <param name="txt"></param>
    private void RefreshSpellCdText(int value)
    {
        if(spl_cooldownMask != null)
            spl_cooldownMask.padding = new Vector4(0, value, 0, 0);
    }

    /// <summary>
    /// get spells list, from other scripts
    /// </summary>
    /// <returns></returns>
    public Spell_Object[] GetSpells()
    {
        return spl_objects;
    }

    int times = 0;
    public void UnableRandomSpells(int count)
    {
        times = count;

        if (count <= 6 && count >= 0)
        {
            while (times > 0)
            {
                int value = Mathf.RoundToInt(Random.Range(0, spl_objects.Length));

                for (int i = 0; i < spl_objects.Length; i++)
                {
                    if (i == value && spl_objects[i].enable)
                    {
                        spl_objects[i].enable = false;
                        times--;
                        Debug.Log(spl_objects[i].name + " is disabled");
                    }
                }
            }
        }
    }

    [HideInInspector] public bool canDecreaseCooldown = false;
    private void DecreaseCooldown()
    {
        foreach (var spell in spl_objects)
        {
            if (!spell.canUseSpell && spell.curUseSpellCooldown > 0)
            {
                spell.curUseSpellCooldown--;
                //Debug.Log(spell.name + " " + spell.curUseSpellCooldown);
            }
        }
    }

    /// <summary>
    /// draw gizmos debug
    /// </summary>
    private void OnDrawGizmos()
    {
        Vector3 forwardPos = transform.position + Vector3.forward;
        Vector3 backPos = transform.position + Vector3.back;
        Vector3 rightPos = transform.position + Vector3.right;
        Vector3 leftPos = transform.position + Vector3.left;
        float rad = 0.35f;

        Gizmos.DrawSphere(forwardPos, rad);
        Gizmos.DrawSphere(backPos, rad);
        Gizmos.DrawSphere(rightPos, rad);
        Gizmos.DrawSphere(leftPos, rad);
    }
}
