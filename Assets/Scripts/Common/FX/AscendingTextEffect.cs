using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class AscendingTextEffect : MonoBehaviour
{
    //Settings
    public float ascendRange = 1.0f;
    public float ascendTime = 2.0f;
    public float postAscendTime = 0.5f;
    // Connections
    public TextMeshPro textMesh;
    // State Variables
    Vector3 initialPosition;
    // Start is called before the first frame update
    void Start()
    {
        InitConnections();
        InitState();
    }
    void InitConnections(){
      
    }
    void InitState(){
        initialPosition = transform.position;
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayText(string text)
    {
        transform.position = initialPosition;
        textMesh.text =  text;
        gameObject.SetActive(true);
        float endPositionY = transform.position.y + ascendRange;
        transform.DOMoveY(endPositionY, ascendTime)
            .SetEase(Ease.OutCubic)
            .OnComplete(FadeOut);
    }

    private void FadeOut()
    {
        textMesh.DOFade(0, postAscendTime).OnComplete(HideText);
    }
    
    public void HideText()
    {
        gameObject.SetActive(false);
    }


}

