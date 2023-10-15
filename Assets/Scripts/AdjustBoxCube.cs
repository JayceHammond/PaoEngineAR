using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;


public class AdjustBoxCube : MonoBehaviour
{
    public Camera ARCam;

    // Start is called before the first frame update

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(ARCam.transform.position.x + 8.6f, ARCam.transform.position.y - 2, transform.position.z);
    }
}
