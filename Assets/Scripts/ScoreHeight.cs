using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreHeight : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI currentHeightText;
    //[SerializeField] TMPro.TextMeshProUGUI currentPlayTopHeight;
    [SerializeField] TMPro.TextMeshProUGUI topHeightText;
    

   

    public void SetHeight(float height)
    {
        currentHeightText.text = height.ToString("0000") + " m";
    }
    public void SetTopHeightText(float topHeight)
    {
        topHeightText.text = topHeight.ToString("0000") + " m";
    }

    //public void SetCurrentTopHeight(float currentTopHeight)
    //{
    //    currentPlayTopHeight.text = currentTopHeight.ToString("0000") + " m";

    //}
}
