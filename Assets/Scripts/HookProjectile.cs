using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookProjectile : MonoBehaviour
{
    [SerializeField] float speed;
    Rigidbody rb;
    GameObject player;
    PlayerController playerController;
    [SerializeField]
    Material[] ropeMaterials;
    Vector3 initialPoint;
    Vector3 endPoint;
    [SerializeField] ParticleSystem particles;

    [Header("PARAMETERS")]
    [SerializeField] float lifeTime;
    [SerializeField] float lifeSpan;
    [SerializeField] float distanceSpan;

    [SerializeField] AudioSource hookImpactAudio;

    [SerializeField] bool hasHooked = false;
    public enum hookOrigin { left, right };

    hookOrigin hookOriginType;

    // Start is called before the first frame update
    public float SetSpeed
    {
        get { return speed; }
        set { speed = value;}
    }
    public GameObject SetPlayer
    {
        set { player = value; }
    }
    public PlayerController SetPlayerController
    {
        set { playerController = value; }
    }
    public hookOrigin HookOrigin
    {
        get { return hookOriginType; }
        set { hookOriginType = value; }
    }
    public float SetHookLifeSpan
    {
        set { lifeSpan = value; }
    }
    public float SetDistanceSpan
    {
        set { distanceSpan = value; }
    }

    void Start()
    {
        initialPoint = transform.position;
        rb = GetComponent<Rigidbody>();
        switch (hookOriginType)
        {
            case hookOrigin.left:
                GetComponent<MeshRenderer>().material = ropeMaterials[0];
                break;
            case hookOrigin.right:
                GetComponent<MeshRenderer>().material = ropeMaterials[1];
                break;
            
        }
    }

    private void Update()
    {
        if(!hasHooked)
        {
            //lifeTime += Time.unscaledDeltaTime;
            float distance = (initialPoint - transform.position).magnitude;
            if(distance >= distanceSpan)//(lifeTime>=lifeSpan)
            {
                switch (hookOriginType)
                {
                    case hookOrigin.left:
                        playerController.DisableLeftHarpoonRenderer();
                        break;
                    case hookOrigin.right:
                        playerController.DisableRightHarpoonRenderer();
                        break;
                }

                DestroyHookGameObject();
            }
        }
    }

    public void DestroyHookGameObject()
    {
        if(hasHooked)
        {

        particles.gameObject.transform.parent = null;
        var main = particles.main;
        main.stopAction = ParticleSystemStopAction.Destroy;
        particles.Play();
        }
        
        Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        if(!hasHooked)
        {
            Vector3 currentPosition = gameObject.transform.position;
            Vector3 nextPosition = currentPosition + transform.right * speed * Time.unscaledDeltaTime;
            gameObject.transform.position = nextPosition;
            //rb.velocity = transform.right * speed;
        }
       
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(!hasHooked && (collision.gameObject.tag == "Scenario")||(collision.gameObject.tag == "Floor"))
        {
            FixedJoint fixedJoint = gameObject.AddComponent<FixedJoint>();
            switch (hookOriginType)
            {
                case hookOrigin.left:
                    playerController.CreateLeftJoint(collision.GetContact(0).point, collision.rigidbody);

                    break;
                case hookOrigin.right:
                    playerController.CreateRightJoint(collision.GetContact(0).point, collision.rigidbody);
                    break;
              
            }
            hasHooked = true;
            fixedJoint.connectedBody = collision.rigidbody;
            particles.Play();
            float newPitch = Random.Range(0.85f, 1.15f);
            hookImpactAudio.pitch = newPitch;
            hookImpactAudio.Play();
           
        }
    }
}
