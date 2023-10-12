using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSpines : MonoBehaviour
{
    [SerializeField] GameObject spines;
    public enum SpinesType { notMoving, timed, trap}
    public SpinesType spinesType;
    
    bool spinesOut;
    bool spinesTrapCounting;
    public bool SetSpinesTrapCounting
    {
        set { spinesTrapCounting = value; }
    }
    [SerializeField] float spinesCounter;
    [SerializeField] float spinesTimeSpan = 5;
    [SerializeField] float spinesHidTimeSpan = 2;
    bool adviceSent = false;
    private void Start()
    {
        //SETTING SPINES
        if(spinesType != SpinesType.notMoving)
        {
            spines.SetActive(false);
        }
        if(spinesType == SpinesType.timed)
        {
            spinesTrapCounting = true;
        }

        if (spines.activeInHierarchy)
        {
            spinesOut = true;
        }
        else
        {
            spinesOut = false;
        }
    }
    private void Update()
    {
        if(spinesType != SpinesType.notMoving)
        {
            if (spinesTrapCounting && spinesOut == false)
            {
                spinesCounter += Time.deltaTime;
                if(spinesCounter>=spinesTimeSpan)
                {
                    ActivateSpines();
                }
                if(spinesCounter>= (spinesTimeSpan*0.75f) && !adviceSent)
                {
                    adviceSent = true;
                    //MUST ADD BOOL TO CONDITION
                    //Debug.Log("AVISO PUAS");

                    //CHANGE MATERIAL
                }

            }
            else if(spinesTrapCounting && spinesOut == true && spinesType == SpinesType.timed)
            {
                spinesCounter += Time.deltaTime;
                if(spinesCounter >= spinesHidTimeSpan)
                {
                    DeactivateSpines();
                }
            }

        }

    }

    public void ActivateSpines()
    {
        spinesCounter = 0;
        spines.SetActive(true);
        spinesOut = true;
        //CHANGE MATERIAL



    }
    public void DeactivateSpines()
    {
        spinesCounter = 0;
        spines.SetActive(false);
        spinesOut = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Hook" && !spinesOut)
        {
            spinesTrapCounting = true;
        }
        
    }
    private void OnCollisionStay(Collision collision)
    {
        Debug.Log("Collision");
        if(collision.gameObject.tag == "Hook" && spinesOut)
        {
            Debug.Log("COLLISION W HOOK");
            PlayerController playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            switch (collision.gameObject.GetComponent<HookProjectile>().HookOrigin)
            {
                case HookProjectile.hookOrigin.left:
                    playerController.EliminateLeftHook();
                    break;
                case HookProjectile.hookOrigin.right:
                    playerController.EliminateRightHook();
                    break;
                default:
                    break;
            }
        }
    }
}
