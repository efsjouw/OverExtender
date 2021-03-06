using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour {

    public Vector3 rotation;
    public bool randomRotation;

	void Start () {
        System.Random random = new System.Random();
        if (randomRotation)
        {
            Vector3 rotateVector = new Vector3(random.Next(0, 2), random.Next(0, 2), random.Next(0, 2));
            if(rotateVector == Vector3.zero)
            {
                int rotate = random.Next(0, 2);
                switch(rotate)
                {
                    //x
                    case 0:
                        rotateVector = new Vector3(1, random.Next(0, 2), random.Next(0, 2));
                        break;
                    //y
                    case 1:
                        rotateVector = new Vector3(random.Next(0, 2), 1, random.Next(0, 2));
                        break;
                    //z
                    case 2:
                        rotateVector = new Vector3(random.Next(0, 2), random.Next(0, 2), 1);
                        break;
                }
            }
            rotation = rotateVector;
        }
	}
	
	void Update () {
        gameObject.transform.Rotate(rotation);
	}
}

