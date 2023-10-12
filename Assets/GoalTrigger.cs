using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" || other.tag == "Hook")
        {
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().GameWin();
            Destroy(gameObject);
        }
    }
}
