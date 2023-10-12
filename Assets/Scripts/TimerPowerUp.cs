using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerPowerUp : MonoBehaviour
{
    [SerializeField] int timeToAdd;
    [SerializeField] TMPro.TMP_Text text;
    private void Start()
    {
        if(timeToAdd > 0)
        {
            text.text = "+"+timeToAdd.ToString();

        }
        else
        {
            text.text = timeToAdd.ToString();
        }
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if(timeToAdd >= 0)
        {
            if(other.tag == "Player"|| other.tag == "Hook")
            {
               GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().AddToCurrentTime(timeToAdd);

                Destroy(gameObject);

            }
        }
        else
        {
            if (other.tag == "Player")
            {
                GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().AddToCurrentTime(timeToAdd);

                Destroy(gameObject);

            }
        }

    }
}
