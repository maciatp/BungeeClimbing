using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField] bool mustDoAction = false;
    [SerializeField] float moveSpeed = 2;
                     float moveCounter = 0;
    [SerializeField] float moveTimeSpan = 5;
                     float disappearCounter = 0;
    [SerializeField] float disappearTimeSpan = 3;
    [SerializeField] float rotationSpeed = 10;
    Rigidbody rb;
    MeshRenderer meshRenderer;
    [SerializeField] Color mustMoveColor;
    [SerializeField] float adviceThreshold = 0.65f;
    [SerializeField] Color adviceColor;
    [SerializeField] Color finalColor;

    bool adviceSent = false;



    public bool GetMustDoAction
    {
        get { return mustDoAction; }
    }

    public enum Blocks { nonMoving, falling, rising, disappearing, rotatingContinuously}; // ADD ALL BLOCKS
    public Blocks blockType;

    public enum State { none, mustMove, isMoving};

    State currentState = State.none;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        meshRenderer = GetComponentInChildren<MeshRenderer>();
    }
    private void Update()
    {
        switch (blockType)
        {
            case Blocks.nonMoving:
                break;
            case Blocks.falling:
                if(currentState == State.mustMove)
                {
                    moveCounter += Time.deltaTime;
                    if(moveCounter >= moveTimeSpan)
                    {
                        ActivateIsMoving();
                    }
                }
                if (currentState == State.isMoving)
                {   
                    //MOVEMENT
                    Vector3 newPos = gameObject.transform.position + new Vector3(0, moveSpeed * Time.deltaTime, 0);
                    gameObject.transform.position = newPos;

                    disappearCounter += Time.deltaTime;                    
                    if (disappearCounter >= (disappearTimeSpan * 0.75f) && !adviceSent)
                    {
                        adviceSent = true;
                        Debug.Log("AVISO ME VOY");

                        //CHANGE MATERIAL
                        meshRenderer.material.color = finalColor;
                    }
                    if (disappearCounter >= disappearTimeSpan)
                    {
                        DestroyBlock();
                    }



                }
                break;

            case Blocks.rising:
               
                if(currentState == State.isMoving)
                {
                    Vector3 newPos = gameObject.transform.position + new Vector3(0, moveSpeed * Time.deltaTime, 0);
                    gameObject.transform.position = newPos;

                    disappearCounter += Time.deltaTime;
                    if(disappearCounter >= (disappearTimeSpan * adviceThreshold) && (meshRenderer.material.color != finalColor))
                    {
                        meshRenderer.material.color = finalColor;
                    }
                    if(disappearCounter >= disappearTimeSpan)
                    {
                        DestroyBlock();
                    }
                }
                break;

            case Blocks.disappearing:
                {
                    if(mustDoAction)
                    {
                        disappearCounter += Time.deltaTime;
                        if(disappearCounter >= disappearTimeSpan)
                        {
                            DestroyBlock();
                        }
                        if (disappearCounter >= (disappearTimeSpan * adviceThreshold) && !adviceSent)
                        {
                            adviceSent = true;
                            Debug.Log("AVISO ME VOY");

                            //CHANGE MATERIAL
                            meshRenderer.material.color = finalColor;
                        }
                    }
                }
                break;
            case Blocks.rotatingContinuously:
                transform.GetChild(0).Rotate(new Vector3(0, 0, rotationSpeed * Time.deltaTime));
                break;
        }
        
    }

    private void DestroyBlock()
    {
        //DISAPPEAR
        PlayerController playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        //DESTROY HOOK IF HOOKED
        if (playerController.GetLeftSpring != null)
        {
            if (playerController.GetLeftSpring.connectedBody == this.rb)
            {
                playerController.EliminateLeftHook();
            }
        }
        if (playerController.GetRightSpring != null)
        {
            if (playerController.GetRightSpring.connectedBody == this.rb)
            {
                playerController.EliminateRightHook();
            }
        }
        //ADD FX
        //DESTROY GAMEOBJECT
        Destroy(gameObject);
    }

    public void ActivateMustMove()
    {
        if(currentState != State.mustMove)
        {
            currentState = State.mustMove;
            meshRenderer.material.color = mustMoveColor;
        }
    }
    public void ActivateIsMoving()
    {
        if(currentState != State.isMoving)
        {
            currentState = State.isMoving;
            meshRenderer.material.color = adviceColor;
        }
    }
    private void ActivateMustDoAction()
    {
        if(!mustDoAction)
        {
            mustDoAction = true;
            meshRenderer.material.color = adviceColor;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            Destroy(gameObject);
        }
        if(collision.gameObject.tag == "Hook" || collision.gameObject.tag == "Player")
        {

            switch (blockType)
            {
                case Blocks.nonMoving:
                    break;
                case Blocks.falling:
                    if(currentState == State.none)
                    {
                        ActivateMustMove();
                    }
                    break;
                case Blocks.rising:
                    if(currentState == State.none)
                    {
                        ActivateIsMoving();
                    }
                    break;
                case Blocks.disappearing:
                    ActivateMustDoAction();
                    break;
                default:
                    break;
            }
        }
        
    }

    
}
