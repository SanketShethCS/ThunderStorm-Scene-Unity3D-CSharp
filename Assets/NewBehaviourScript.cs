using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour {

    // Game object for cloud particle system
    GameObject obj1;

    // Game object for point light
    GameObject obj2;

    //Keyframing current time.
    public float currentTime = 0.0f;

    //Keyframing start time.
    public float startTime = 0.0f;

    //Keyframing index of time.
    public int timeIndex = 0;
    private float time = 0.0f;
    public float interpolationPeriod = 30.0f;
    
    
    // Use this for initialization
    void Start () {
        
    }
	

	// Update is called once per frame
	void Update ()
    {

        //Dust storm particle system that represents cloud
        obj2 = GameObject.Find("DustStorm2");

        //Point light object for illumination from lightning
        obj1 = GameObject.Find("Point light");

        //Get currenttime in keyframe
        currentTime = Time.time - startTime;
        int t = (int)currentTime;

        //For some part of the simulation show clouds and its movement.
        if (t > 5 && t < 160)
        {
            float val = 0.01f;

            // reduce the speed of the moving clouds with time.
            Vector3 dist = (new Vector3(1.0f - val, 0, 0));

            // translate the cloud particle system
            obj2.transform.position += dist;

            // increment the reduction value
            if( val > 0.1f )
                val = val + 0.1f;

            // step distance
            Vector3 distlight = (new Vector3(0, 3f, 0));

            // move point light away from the terrain to simulate
            // increasing darkness with cluds moving in. Synced.
            if(obj1.transform.position.y < 7500)

                // translate the point light
                obj1.transform.position += distlight;

        }
    }
}
