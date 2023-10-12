using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public PlayerControls inputActions;
    [Header("GAMEPAD CONTROLS")]
    Gamepad gamepad;

    bool isUsingGamepad = false;

    [Header("STICKS VALUES")]
    [SerializeField] Vector2 aimLeftVector;
    [SerializeField] Vector2 aimRightVector;
    [Header("Aim Origins")]
    [SerializeField] Transform aimLeftOrigin;
    [SerializeField] Transform aimRightOrigin;
    [Header("Line Renderers")]
    [SerializeField] LineRenderer aimLeftRenderer;
    [SerializeField] LineRenderer aimRightRenderer;

    [SerializeField] LineRenderer leftHookRenderer;
    [SerializeField] LineRenderer rightHookRenderer;

    [Header("HOOK")]
    [SerializeField] GameObject hookProjectile;
    HookProjectile hookProjectileLeft;
    HookProjectile hookProjectileRight;
    [SerializeField] float hookSpeed;
    [SerializeField] float hookLifeSpan;
    [SerializeField] float hookDistanceSpan;
    [SerializeField] float lowLaserColor = 0.3f;

    [Header("FX")]
    [SerializeField] ParticleSystem leftParticles;
    [SerializeField] ParticleSystem rightParticles;
    //AUDIO SOURCE LEFT
    [SerializeField] AudioSource leftHookAudio;
    //AUDIO SOURCE RIGHT
    [SerializeField] AudioSource rightHookAudio;
    
    //AUDIO CLIP DESTROYHOOK
    [SerializeField] AudioSource hookDestroyAudioSource;

    //[SerializeField]
   // float shootingDistance = 5;
    [SerializeField]
    float initialSpring = 8;
    int layer_mask;
    [SerializeField] float tensingForce = 10;

    [SerializeField] bool mustDoSlowDown = false;

    SpringJoint springJointLeft;
    SpringJoint springJointRight;

    //How strong the slowmo is
    public float slowdownFactor = 0.05f;
    //How long is the transition to slowmo and inverse
    public float slowdownLenth = 2f;

    GameManager gameManager;

    public SpringJoint GetLeftSpring
    {
        get { return springJointLeft;}
    }
    public SpringJoint GetRightSpring
    {
        get { return springJointRight; }
    }



    private void Awake()
    {
        inputActions = new PlayerControls();
    }

    
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        layer_mask = LayerMask.GetMask("Scenario"); //"Player","Enemy"

        gamepad = Gamepad.current;

        if(aimLeftRenderer == enabled)
        {
            aimLeftRenderer.enabled = false;
        }
        if(aimRightRenderer == enabled)
        {
            aimRightRenderer.enabled = false;
        }
        if(leftHookRenderer.enabled)
        {
            leftHookRenderer.enabled = false;
        }
        if(rightHookRenderer.enabled)
        {
            rightHookRenderer.enabled = false;
        }

    }

    
    void Update()
    {

       if(!gameManager.IsGamePaused && !gameManager.IsGameOver && !gameManager.IsGameWin)
        {
            //IF CONTROLLER GETS UNPLUGGED DURING PLAY
            if (Gamepad.current == null)
            {
                // Debug.Log("Juego con teclado");
                isUsingGamepad = false;
                gamepad = null;
            }
            //IF CONTROLLER GETS PLUGGED DURING PLAY
            else if (Gamepad.current != null)
            {
                isUsingGamepad = true;
                gamepad = Gamepad.current;
                // Debug.Log("uso mando");
            }


            if (mustDoSlowDown)
            {
                Time.timeScale -= (1f / (slowdownLenth / 4)) * Time.unscaledDeltaTime;
                Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f);
                if (Time.timeScale <= slowdownFactor)
                {
                    mustDoSlowDown = false;
                }
            }
            else
            {
                Time.timeScale += (1f / slowdownLenth) * Time.unscaledDeltaTime;
                Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f);
            }
            Time.fixedDeltaTime = Time.timeScale * 0.02f;


            if (leftHookRenderer.enabled)
            {
                Vector3 leftHookRendererEndPosition = hookProjectileLeft.transform.GetChild(0).position - leftHookRenderer.transform.position;
                leftHookRenderer.SetPosition(1, leftHookRendererEndPosition);
            }
            if (rightHookRenderer.enabled)
            {
                Vector3 rightHookRendererEndPosition = hookProjectileRight.transform.GetChild(0).position - rightHookRenderer.transform.position;
                rightHookRenderer.SetPosition(1, rightHookRendererEndPosition);
            }


            if (springJointLeft != null && aimLeftRenderer.material.color.a != lowLaserColor)
            {
                //Debug.Log("Color actual es " + aimLeftRenderer.material.color.r + " R," + aimLeftRenderer.material.color.g + " G," + aimLeftRenderer.material.color.b + " B," + aimLeftRenderer.material.color.a + " A");
                aimLeftRenderer.material.color = new Color(aimLeftRenderer.material.color.r, aimLeftRenderer.material.color.g, aimLeftRenderer.material.color.b, lowLaserColor);

                //Debug.Log("Cambio color a " + aimLeftRenderer.material.color.r + " R," + aimLeftRenderer.material.color.g + " G," + aimLeftRenderer.material.color.b + " B," + aimLeftRenderer.material.color.a + " A");
            }
            else if (springJointLeft == null && aimLeftRenderer.material.color.a == lowLaserColor)
            {
                aimLeftRenderer.material.color = new Color(aimLeftRenderer.material.color.r, aimLeftRenderer.material.color.g, aimLeftRenderer.material.color.b, 1);


            }

            if (springJointRight != null && aimRightRenderer.material.color.a != lowLaserColor)
            {
                aimRightRenderer.material.color = new Color(aimRightRenderer.material.color.r, aimRightRenderer.material.color.g, aimRightRenderer.material.color.b, lowLaserColor);
            }
            else if (springJointRight == null && aimRightRenderer.material.color.a == lowLaserColor)
            {
                aimRightRenderer.material.color = new Color(aimRightRenderer.material.color.r, aimRightRenderer.material.color.g, aimRightRenderer.material.color.b, 1);


            }
        }

    }
    private void FixedUpdate()
    {
        if (aimLeftVector.magnitude >= 0.25f && !gameManager.IsGamePaused && !gameManager.IsGameOver && !gameManager.IsGameWin)
        {
            RaycastHit hitLeft;
            if (Physics.Raycast(aimLeftOrigin.transform.position, aimLeftVector, out hitLeft, hookDistanceSpan, layer_mask))
            {
                aimLeftRenderer.SetPosition(1, new Vector3(hitLeft.distance, 0, -0.05f));
            }
            else
            {
                aimLeftRenderer.SetPosition(1, new Vector3(10, 0, -0.05f));
            }
        }
        if (aimRightVector.magnitude >= 0.25f)
        {
            RaycastHit hitRight;
            if (Physics.Raycast(aimRightOrigin.transform.position, aimRightVector, out hitRight, hookDistanceSpan, layer_mask))
            {
                aimRightRenderer.SetPosition(1, new Vector3(hitRight.distance, 0, -0.05f));
            }
            else
            {
                aimRightRenderer.SetPosition(1, new Vector3(10, 0, -0.05f));
            }
        }
    }

   
    public void DoSlowMotion()
    {
        Time.timeScale = slowdownFactor;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;

    }

    public void CreateLeftJoint(Vector3 impactPoint, Rigidbody rigidbodyToAttach)
    {

        //Debug.Log("Nuevo Spring LEFT");
        springJointLeft = gameObject.AddComponent<SpringJoint>();
        //SETTING PARAMETERS
        springJointLeft.autoConfigureConnectedAnchor = false;
        springJointLeft.enableCollision = true;
        springJointLeft.connectedBody = rigidbodyToAttach;
        springJointLeft.spring = initialSpring;//initialSpring;
        //SETTING ANCHORS
        springJointLeft.anchor = aimLeftOrigin.localPosition;

        //ANCHOR TO BLOCK
        //Vector3 hitPoint = impactPoint - rigidbodyToAttach.transform.position;
        //Debug.Log("THE HITPOINT IS " + hit.point);
        //Debug.Log("THE RELATIVE HITPOINT IS " + hitPoint);
        //springJointLeft.connectedAnchor = new Vector3(hitPoint.x, hitPoint.y, 0);

        //ANCHOR TO HARPOON
        Vector3 hitPoint = hookProjectileLeft.transform.GetChild(0).position - rigidbodyToAttach.transform.position;
        springJointLeft.connectedAnchor = new Vector3(hitPoint.x, hitPoint.y, 0);

    }
    public void CreateRightJoint(Vector3 impactPoint, Rigidbody rigidbodyToAttach)
    {
        //Debug.Log("gotcha RIGHT!");
        //Debug.Log("Nuevo Spring RIGHT");
        springJointRight = gameObject.AddComponent<SpringJoint>();
        //SETTING PARAMETERS
        springJointRight.autoConfigureConnectedAnchor = false;
        springJointRight.enableCollision = true;
        springJointRight.connectedBody = rigidbodyToAttach;
        springJointRight.spring = initialSpring;
        //SETTING ANCHORS
        springJointRight.anchor = aimRightOrigin.localPosition;

        //ANCHOR TO BLOCK
        //Vector3 hitPoint = impactPoint - rigidbodyToAttach.transform.position;
        //Debug.Log("THE HITPOINT IS " + hit.point);
        //Debug.Log("THE RELATIVE HITPOINT IS " + hitPoint);
        //springJointRight.connectedAnchor = new Vector3(hitPoint.x, hitPoint.y, 0);

        //ANCHOR TO HARPOON
        Vector3 hitPoint = hookProjectileRight.transform.GetChild(0).position - rigidbodyToAttach.transform.position;
        springJointRight.connectedAnchor = new Vector3(hitPoint.x, hitPoint.y, 0);
    }

    public void DisableLeftHarpoonRenderer()
    {
        leftHookRenderer.enabled = false;
    }
    public void DisableRightHarpoonRenderer()
    {
        rightHookRenderer.enabled = false;
    }

        //CONTROLS
    void OnAimLeft(InputValue joystickValue)
    {
        if(!gameManager.IsGamePaused && !gameManager.IsGameOver && !gameManager.IsGameWin)
        {
            aimLeftVector = joystickValue.Get<Vector2>();
            if (aimLeftVector.magnitude >= 0.35f)
            {
                aimLeftRenderer.enabled = true;
            }
            else
            {
                aimLeftRenderer.enabled = false;
            }
            float angle = Mathf.Atan2(aimLeftVector.y, aimLeftVector.x) * Mathf.Rad2Deg;
            aimLeftOrigin.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
        


    }
    void OnAimRight(InputValue joystickValue)
    {
        if (!gameManager.IsGamePaused && !gameManager.IsGameOver && !gameManager.IsGameWin)
        {

            aimRightVector = joystickValue.Get<Vector2>();
            if (aimRightVector.magnitude >= 0.35f)
            {
                aimRightRenderer.enabled = true;
            }
            else
            {
                aimRightRenderer.enabled = false;
            }
            float angle = Mathf.Atan2(aimRightVector.y, aimRightVector.x) * Mathf.Rad2Deg;
            aimRightOrigin.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
       
    }

    
    void OnShootLeft(InputValue buttonValue)
    {
        if (!gameManager.IsGamePaused && !gameManager.IsGameOver && !gameManager.IsGameWin)
        { 

            if (springJointLeft != null)
            {
                Destroy(springJointLeft);
                DestroyLeftHook();
                //DoSlowMotion();
                mustDoSlowDown = true;
                return;

            }

            if ( hookProjectileLeft != null)
            {
                DestroyLeftHook();
                FireLeftHook();
            }
            else
            {
                FireLeftHook();


                //RaycastHit hit;

                //if (Physics.Raycast(aimLeftOrigin.transform.position, aimLeftVector, out hit, shootingDistance,layer_mask)) // 7 = layer scenario
                //{
                //    if (hit.collider.tag == "Scenario")
                //    {
                //        Debug.Log("gotcha LEFT!");
                //        Debug.Log("Nuevo Spring LEFT");
                //        springJointLeft = gameObject.AddComponent<SpringJoint>();
                //        //SETTING PARAMETERS
                //        springJointLeft.autoConfigureConnectedAnchor = false;
                //        springJointLeft.enableCollision = true;
                //        springJointLeft.connectedBody = hit.collider.attachedRigidbody;
                //        springJointLeft.spring = initialSpring;
                //        //SETTING ANCHORS
                //        springJointLeft.anchor = aimLeftOrigin.localPosition;
                //        Vector3 hitPoint = hit.point - hit.transform.position;
                //        Debug.Log("THE HITPOINT IS " + hit.point);
                //        Debug.Log("THE RELATIVE HITPOINT IS "+ hitPoint);
                //        springJointLeft.connectedAnchor = new Vector3(hitPoint.x, hitPoint.y,0);


                //    }
                //}

            }

        }
       

    }

    private void DestroyLeftHook()
    {
        //SOUND?
        //DESTROY HOOOK CLIP PLAY
        PlayHookDestroySoundEffect();
        hookProjectileLeft.DestroyHookGameObject();
        DisableLeftHarpoonRenderer();
    }
    public void EliminateLeftHook()
    {
        //SOUND?
        //DESTROY HOOOK CLIP PLAY
        PlayHookDestroySoundEffect();
        hookProjectileLeft.DestroyHookGameObject();
        DisableLeftHarpoonRenderer();
        if(springJointLeft != null)
        {
            Destroy(springJointLeft);
        }
    }

    private void FireLeftHook()
    {
        GameObject leftHook = Instantiate(hookProjectile, aimLeftOrigin.position, new Quaternion(aimLeftOrigin.transform.localRotation.x, aimLeftOrigin.transform.localRotation.y, aimLeftOrigin.localRotation.z, aimLeftOrigin.transform.rotation.w), null);
        HookProjectile leftHookProjectile = leftHook.GetComponent<HookProjectile>();
        leftHookProjectile.SetSpeed = hookSpeed;
        leftHookProjectile.SetPlayer = gameObject;
        //leftHookProjectile.SetHookLifeSpan = hookLifeSpan;
        leftHookProjectile.SetDistanceSpan = hookDistanceSpan;
        leftHookProjectile.SetPlayerController = this;
        leftHookProjectile.HookOrigin = HookProjectile.hookOrigin.left;
        hookProjectileLeft = leftHookProjectile;
        leftHookRenderer.enabled = true;
        leftHookRenderer.SetPosition(0, Vector3.zero);

        leftParticles.Play();
        float newPitch = Random.Range(0.85f, 1.15f);
        leftHookAudio.pitch = newPitch;
        leftHookAudio.Play();

    }

    void OnShootRight(InputValue buttonValue)
    {
        if (!gameManager.IsGamePaused && !gameManager.IsGameOver && !gameManager.IsGameWin)
        {



            if (springJointRight != null)
            {
                Destroy(springJointRight);
                DestroyRightHook();
                mustDoSlowDown = true;
                //DoSlowMotion(); // TO DO-> CHOOSE BEST ONE

                return;
            }
            if (hookProjectileRight != null)//(springJointRight != null)
            {

                DestroyRightHook();
                FireRightHook();
            }
            else
            {
                FireRightHook();



                //RaycastHit hit;

                //if (Physics.Raycast(aimRightOrigin.transform.position, aimRightVector, out hit, shootingDistance, layer_mask)) // 7 = layer scenario
                //{
                //    if (hit.collider.tag == "Scenario")
                //    {
                //        Debug.Log("gotcha RIGHT!");
                //        Debug.Log("Nuevo Spring RIGHT");
                //        springJointRight = gameObject.AddComponent<SpringJoint>();
                //        //SETTING PARAMETERS
                //        springJointRight.autoConfigureConnectedAnchor = false;
                //        springJointRight.enableCollision = true;
                //        springJointRight.connectedBody = hit.collider.attachedRigidbody;
                //        springJointRight.spring = initialSpring;
                //        //SETTING ANCHORS
                //        springJointRight.anchor = aimLeftOrigin.localPosition;
                //        Vector3 hitPoint = hit.point - hit.transform.position;
                //        Debug.Log("THE HITPOINT IS " + hit.point);
                //        Debug.Log("THE RELATIVE HITPOINT IS " + hitPoint);
                //        springJointRight.connectedAnchor = new Vector3(hitPoint.x, hitPoint.y, 0);


                //    }
                //}
            }
        }
    }

    void OnTenseLeft(InputValue axisValue)
    {
        if (!gameManager.IsGamePaused && !gameManager.IsGameOver && !gameManager.IsGameWin)
        {


            float axis = axisValue.Get<float>();
            Debug.Log(axis);

            if (springJointLeft != null)
            {
                springJointLeft.spring += axis * tensingForce;
                if (axis == 0)
                {
                    springJointLeft.spring = initialSpring;
                }
            }
        }
    }
    void OnTenseRight(InputValue axisValue)
    {
        if (!gameManager.IsGamePaused && !gameManager.IsGameOver && !gameManager.IsGameWin)
        {

            float axis = axisValue.Get<float>();
            //Debug.Log(axis);

            if (springJointRight != null)
            {

                springJointRight.spring += axis * tensingForce;
                if (axis == 0)
                {
                    springJointRight.spring = initialSpring;
                }
            }
        }
    }

    private void FireRightHook()
    {
        GameObject rightHook = Instantiate(hookProjectile, aimRightOrigin.position, new Quaternion(aimRightOrigin.transform.localRotation.x, aimRightOrigin.transform.localRotation.y, aimRightOrigin.localRotation.z, aimRightOrigin.transform.rotation.w), null);
        HookProjectile rightHookProjectile = rightHook.GetComponent<HookProjectile>();
        rightHookProjectile.SetSpeed = hookSpeed;
        rightHookProjectile.SetPlayer = gameObject;
        //rightHookProjectile.SetHookLifeSpan = hookLifeSpan;
        rightHookProjectile.SetDistanceSpan = hookDistanceSpan;
        rightHookProjectile.SetPlayerController = this;
        rightHookProjectile.HookOrigin = HookProjectile.hookOrigin.right;
        hookProjectileRight = rightHookProjectile;
        rightHookRenderer.enabled = true;
        rightHookRenderer.SetPosition(0, Vector3.zero);

        rightParticles.Play();
        float newPitch = Random.Range(0.85f, 1.15f);
        rightHookAudio.pitch = newPitch;
        rightHookAudio.Play();
    }

    private void DestroyRightHook()
    {
        //SOUND?
        //DESTROY HOOOK CLIP PLAY
        PlayHookDestroySoundEffect();
        hookProjectileRight.DestroyHookGameObject();
        DisableRightHarpoonRenderer();
    }

    private void PlayHookDestroySoundEffect()
    {
        float newPitch = Random.Range(0.85f, 1.15f);
        hookDestroyAudioSource.pitch = newPitch;
        hookDestroyAudioSource.Play();
    }

    public void EliminateRightHook()
    {
        //SOUND?
        //DESTROY HOOOK CLIP PLAY
        PlayHookDestroySoundEffect();
        hookProjectileRight.DestroyHookGameObject();
        DisableRightHarpoonRenderer();
        if (springJointRight!= null)
        {
            Destroy(springJointRight);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "TimeTrigger")
        {
            gameManager.ActivateTimer();
        }
       
    }

    void OnPause(InputValue buttonValue)
    {
        gameManager.PauseGame();
    }
    void OnRestart(InputValue buttonValue)
    {
        if(gameManager.IsGamePaused || gameManager.IsGameOver || gameManager.IsGameWin)
        {
            gameManager.PlayAgain();
        }
    }

    void OnAccept(InputValue buttonValue)
    {
        if(gameManager.IsGameWin)
        {
            gameManager.ContinuePlayingAfterWin();
        }
    }

    //DPRCTD??
    //EL ENABLE Y DISABLE PARA inputActions sirve para que pueda coger ScreenToWorldPoint. Sin ellos, no funciona.
    //private void OnEnable()
    //{
    //    inputActions.Enable();

    //}
    //private void OnDisable()
    //{
    //    inputActions.Disable();
    //}
}
