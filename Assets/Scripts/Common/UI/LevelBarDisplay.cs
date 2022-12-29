using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelBarDisplay : MonoBehaviour
{

	#region Attributes
	// Enter your attributes which are used to specify this single instance of this class
	#endregion
	#region Connections
	public Transform targetTransform;
	public TextMeshProUGUI moveText;
	public Slider slider;

	#endregion
	#region State Variables
	// Enter your state variables used to store data in your algorithm
	float progressTarget = 3;
	float progressPerSecond = 2f;
	float errorMargin = 0.001f;
	bool change = false;
	#endregion
	
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (change)
        {
			slider.value = Mathf.Lerp(slider.value, progressTarget, Time.deltaTime * progressPerSecond);
			if(Mathf.Abs(slider.value - progressTarget) <= errorMargin)
            {
				change = false;
            }
        }
    }
	
	public void DisplayProgress(float progress)
    {
		if(progress <= progressTarget)
        {
			change = true;
			progressTarget = progress;
        }
	}

	public void SetMoves(int movesLeft)
    {
		moveText.text = movesLeft.ToString();
	}

	public void GetCurrentProgress()
    {

    }

	#region Init Functions
	void InitState(){
		// Ali veli hasan hüseyin
	}
	void InitConnections(){
	}
	
	#endregion
	
	
}
