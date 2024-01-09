using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PowerSystem : MonoBehaviour
{
    private int _powerIndex;
    private GameObject currPowerBtn;
    [HideInInspector] public GameObject owner;
    private bool usingPower = false;
    [SerializeField] private TextMeshProUGUI powerName;
    [SerializeField] private GameObject cursedGround;
    private int cgCount = 3;
    private int invEndTurn = 0;
    private bool isInv = false;
    [SerializeField] private GameObject colors;
    [SerializeField] private Transform playerGFX;
    [SerializeField] private LayerMask whatToAttack;
    [SerializeField] private Button powerBtn;
    [SerializeField] private Sprite[] sprites;

    [Header("Feedbacks")]
    [SerializeField] private GameObject fb_distantAttack;
    [SerializeField] private GameObject fb_teleport;
    [SerializeField] private TextMeshProUGUI fb_countTxt;
    private Animator fb_animator;

    private void Start()
    {
        powerName.text = "Nothing";
        powerBtn.GetComponent<Image>().sprite = sprites[4];
        powerBtn.GetComponent<Image>().color = Color.magenta;
        currPowerBtn = this.gameObject;
        colors.SetActive(false);
        owner = playerGFX.transform.parent.gameObject;
        fb_animator = this.gameObject.GetComponent<Animator>();
        fb_countTxt.enabled = false;
    }

    public void UsePower()
    {
        if (owner.GetComponent<PlayerController>().GetcanMove() && owner.GetComponent<PlayerAttack>().attackBtn.activeSelf)
        {
            if (_powerIndex <= 0)
            {
                //Debug.Log("NO POWER");
                powerName.text = "Nothing";
                powerBtn.GetComponent<Image>().sprite = sprites[4];
                powerBtn.GetComponent<Image>().color = Color.magenta;
            }
            else
            {
                //Debug.Log("USE POWER : " + _powerIndex + "!");
                currPowerBtn.GetComponent<Image>().enabled = false;
                usingPower = true;

                if (_powerIndex == 1)
                {
                    owner.GetComponent<PlayerAttack>().FeedbackArrowSetup(fb_distantAttack);
                }

                if (_powerIndex == 2)
                {
                    owner.GetComponent<PlayerAttack>().FeedbackPlaneSetup(fb_teleport);
                }
            }
        }
    }

    private void Update()
    {
        if (usingPower && Input.GetKeyDown(KeyCode.Mouse0))
        {
            if(_powerIndex == 1 || _powerIndex == 2)
            {
                // Raycast to detect the gameObject at the mouse position
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, whatToAttack))
                {
                    // An object is hit
                    GameObject hitObject = hit.collider.gameObject;
                    //Debug.Log("hitObject : " + hitObject);
                    //Debug.Log("owner : " + owner);

                    if(_powerIndex == 1)
                    {
                        if (hitObject.CompareTag("Player") && hitObject != owner || hitObject.CompareTag("Enemy"))
                        {
                            Fireball(hitObject);
                        }
                    }

                    if (_powerIndex == 2)
                    {
                        if (hitObject.layer == 3)
                        {
                            Debug.Log("hitObject : " + hitObject.name);

                            RaycastHit up;
                            //Debug.DrawRay(hitObject.transform.position, transform.up * Mathf.Infinity, Color.red, 5f);
                            if (Physics.Raycast(hitObject.transform.position, transform.up, out up, Mathf.Infinity, whatToAttack))
                            {
                                GameObject hitPlayer = up.collider.gameObject;
                                Debug.Log("hitPlayer : " + hitPlayer.name);

                                if (!hitPlayer.CompareTag("Player"))
                                {
                                    CursedGround(hitObject);
                                }
                            }
                            else
                            {
                                CursedGround(hitObject);
                            }
                        }
                    }
                }
            }
        }

        if (usingPower && _powerIndex == 3 || usingPower && _powerIndex == 4)
        {
            switch (_powerIndex)
            {
                case 3:
                    Invincible();
                    break;
                case 4:
                    ChangeColor();
                    break;
            }
        }

        //Debug.Log("isInv : " + isInv);
        if (isInv)
        {
            if (FindObjectOfType<GameManager>().GetTurnCount() >= invEndTurn)
            {
                owner.GetComponent<PlayerHP>().canTakeDamage = true;

                currPowerBtn.GetComponent<Image>().enabled = true;
                _powerIndex = 0;
                powerName.text = "Nothing";
                powerBtn.GetComponent<Image>().sprite = sprites[4];
                powerBtn.GetComponent<Image>().color = Color.magenta;
                usingPower = false;

                #region particles
                foreach (Transform child in owner.transform)
                {
                    Debug.Log(child.name);
                    if(child.gameObject.layer == 8)
                    {
                        Destroy(child.gameObject);
                    }
                }
                #endregion

                Debug.Log("VINCIBLE");
                isInv = false;
                invEndTurn = 0;
            }
        }
    }

    public void NewPower(int index)
    {
        if(_powerIndex <= 0)
        {
            _powerIndex = index;
            //Debug.Log("LOOT POWER : " + _powerIndex);
            FindObjectOfType<AudioManager>().Play("PickupPower");
            fb_animator.SetTrigger("PickupTrigger");

            switch (_powerIndex)
            {
                case 1:
                    powerName.text = "Fireball";
                    powerBtn.GetComponent<Image>().sprite = sprites[0];
                    powerBtn.GetComponent<Image>().color = Color.magenta;
                    fb_countTxt.enabled = false;

                    break;
                case 2:
                    powerName.text = "Cursed ground";
                    powerBtn.GetComponent<Image>().sprite = sprites[1];
                    powerBtn.GetComponent<Image>().color = Color.white;
                    fb_countTxt.enabled = true;
                    fb_countTxt.text = "x" + cgCount;

                    break;
                case 3:
                    powerName.text = "Invincible";
                    powerBtn.GetComponent<Image>().sprite = sprites[2];
                    powerBtn.GetComponent<Image>().color = Color.white;
                    fb_countTxt.enabled = false;

                    break;
                case 4:
                    powerName.text = "Choose a color";
                    powerBtn.GetComponent<Image>().sprite = sprites[3];
                    powerBtn.GetComponent<Image>().color = Color.white;
                    fb_countTxt.enabled = false;

                    break;
            }
        }
    }

    private void Fireball(GameObject target)
    {
        #region particles
        Vector3 endPos = target.transform.position;
        FindObjectOfType<VFXManager>().SpawnMoving("FireBall", owner.transform.position, endPos);
        #endregion

        if (target.CompareTag("Player"))
        {
            target.GetComponent<PlayerHP>().PlayerTakeDamage(30, false);
            _powerIndex = 0;
            powerName.text = "Nothing";
            powerBtn.GetComponent<Image>().sprite = sprites[4];
            powerBtn.GetComponent<Image>().color = Color.magenta;
            currPowerBtn.GetComponent<Image>().enabled = true;
            usingPower = false;
        }

        if (target.CompareTag("Enemy"))
        {
            target.GetComponent<EnemyStats>().TakeDamage(30, false);
            _powerIndex = 0;
            powerName.text = "Nothing";
            powerBtn.GetComponent<Image>().sprite = sprites[4];
            powerBtn.GetComponent<Image>().color = Color.magenta;
            currPowerBtn.GetComponent<Image>().enabled = true;
            usingPower = false;
        }

        owner.GetComponent<PlayerAttack>().DeleteFeedbacks();
    }

    private void CursedGround(GameObject target)
    {
        cgCount--;
        fb_countTxt.text = "x" + cgCount;
        owner.GetComponent<PlayerAttack>().DeleteFeedbacks();

        Instantiate(cursedGround, target.transform.position, target.transform.rotation);
        Destroy(target);
        currPowerBtn.GetComponent<Image>().enabled = true;

        if (cgCount <= 0)
        {
            _powerIndex = 0;
            powerName.text = "Nothing";
            powerBtn.GetComponent<Image>().sprite = sprites[4];
            powerBtn.GetComponent<Image>().color = Color.magenta;
            fb_countTxt.enabled = false;
            usingPower = false;
            cgCount = 3;
        }
        else
        {
            usingPower = false;
        }
    }

    private void Invincible()
    {
        if(owner != null)
        {
            if (invEndTurn == 0)
            {
                invEndTurn = FindObjectOfType<GameManager>().GetTurnCount() + 1;

                #region particles
                FindObjectOfType<VFXManager>().Spawn("Shield", owner.transform.position, 9999f);
                FindObjectOfType<VFXManager>().SetParentSpawnInstance(owner.transform);
                #endregion
            }

            owner.GetComponent<PlayerHP>().canTakeDamage = false;
            isInv = true;

            Debug.Log("INVINCIBLE");
        }
    }

    private void ChangeColor()
    {
        colors.SetActive(true);
        currPowerBtn.GetComponent<Image>().enabled = false;
    }

    public void ChooseColor(int rotIndex)
    {
        switch (rotIndex)
        {
            case 1:
                playerGFX.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case 2:
                playerGFX.rotation = Quaternion.Euler(-90, 0, 0);
                break;
            case 3:
                playerGFX.rotation = Quaternion.Euler(0, 0, -90);
                break;
            case 4:
                playerGFX.rotation = Quaternion.Euler(0, 0, 90);
                break;
            case 5:
                playerGFX.rotation = Quaternion.Euler(90, 0, 0);
                break;
            case 6:
                playerGFX.rotation = Quaternion.Euler(180, 0, 0);
                break;
        }

        colors.SetActive(false);
        _powerIndex = 0;
        powerName.text = "Nothing";
        powerBtn.GetComponent<Image>().sprite = sprites[4];
        powerBtn.GetComponent<Image>().color = Color.magenta;
        currPowerBtn.GetComponent<Image>().enabled = true;
        usingPower = false;
    }
}
