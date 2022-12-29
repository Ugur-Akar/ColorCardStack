using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerClick : MonoBehaviour
{
    //Settings
    
    // Connections
    
    // State Variables
    
    // Start is called before the first frame update
	void Awake()
	{
		//InitConnections();
	}
    void Start()
    {
        //InitState();
    }
    void InitConnections(){
    }
    void InitState(){
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f))
            {

                if (hit.transform != null)
                {
                    Destroy(hit.collider.gameObject);
                }
            }
        }

    }
}

