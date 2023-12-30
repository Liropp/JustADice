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

    private void Start()
    {
        powerName.text = "Nothing";
        currPowerBtn = this.gameObject;
        colors.SetActive(false);
    }

    public void UsePower()
    {
        if (_powerIndex <= 0)
        {
            //Debug.Log("NO POWER");
            powerName.text = "Nothing";
        }
        else
        {
            //Debug.Log("USE POWER : " + _powerIndex + "!");
            currPowerBtn.GetComponent<Image>().enabled = false;
            usingPower = true;
        }
    }

    private void Update()
    {
        if (usingPower && Input.GetKeyDown(KeyCode.Mouse0))
        {
            if(_powerIndex != 3 && _powerIndex != 4)
            {
                // Raycast to detect the gameObject at the mouse position
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    // An object is hit
                    GameObject hitObject = hit.collider.gameObject;

                    switch (_powerIndex)
                    {
                        case 1:
                            if (hitObject.CompareTag("Player") || hitObject.CompareTag("Enemy") && hitObject != owner)
                            {
                                Fireball(hitObject);
                            }
                            break;
                        case 2:
                            if (hitObject.layer == 3)
                            {
                                CursedGround(hitObject);
                            }
                            break;
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

        if (isInv)
        {
            if (FindObjectOfType<GameManager>().GetTurnCount() == invEndTurn)
            {
                owner.GetComponent<PlayerHP>().canTakeDamage = true;

                currPowerBtn.GetComponent<Image>().enabled = true;
                _powerIndex = 0;
                powerName.text = "Nothing";
                usingPower = false;

                Debug.Log("VINCIBLE");
                isInv = false;
                invEndTurn = 0;
            }
        }        
    }

    public void NewPower(int index)
    {
        _powerIndex = index;
        //Debug.Log("LOOT POWER : " + _powerIndex);

        switch (_powerIndex)
        {
            case 1:
                powerName.text = "Fireball";
                break;
            case 2:
                powerName.text = "Cursed ground";
                break;
            case 3:
                powerName.text = "Invincible";
                break;
            case 4:
                powerName.text = "Chains";
                break;
            case 5:
                powerName.text = "Choose a color";
                break;
            case 6:
                powerName.text = "Swap";
                break;
        }
    }

    private void Fireball(GameObject target)
    {
        if (target.CompareTag("Player"))
        {
            target.GetComponent<PlayerHP>().PlayerTakeDamage(30, false);
            _powerIndex = 0;
            powerName.text = "Nothing";
            currPowerBtn.GetComponent<Image>().enabled = true;
            usingPower = false;
        }

        if (target.CompareTag("Enemy"))
        {
            target.GetComponent<EnemyStats>().TakeDamage(30, false);
            _powerIndex = 0;
            powerName.text = "Nothing";
            currPowerBtn.GetComponent<Image>().enabled = true;
            usingPower = false;
        }
    }

    private void CursedGround(GameObject target)
    {
        cgCount--;

        Instantiate(cursedGround, target.transform.position, target.transform.rotation);
        Destroy(target);
        currPowerBtn.GetComponent<Image>().enabled = true;

        if (cgCount <= 0)
        {
            _powerIndex = 0;
            powerName.text = "Nothing";
            usingPower = false;
            cgCount = 3;
        }
    }

    private void Invincible()
    {
        if(invEndTurn == 0)
        invEndTurn = FindObjectOfType<GameManager>().GetTurnCount() + 1;

        owner.GetComponent<PlayerHP>().canTakeDamage = false;
        isInv = true;

        Debug.Log("INVINCIBLE");
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
        currPowerBtn.GetComponent<Image>().enabled = true;
        usingPower = false;
    }
}
