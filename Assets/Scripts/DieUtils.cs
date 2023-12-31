using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieUtils : MonoBehaviour
{

    [SerializeField] private Transform[] dieSides;
    [SerializeField] private GameObject hitParticle;
    [SerializeField] private GameObject beamParticle;
    private static GameObject beamFX;
    private bool beamInstantiated = false;
    public int result = 1;
    public bool availableResult = false;

    // Start is called before the first frame update
    void Start()
    {
        beamInstantiated = false;    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision other) {
        ContactPoint contact = other.contacts[0];
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Vector3 position = contact.point;

        if(hitParticle != null){
            var hitFX = Instantiate(hitParticle, position, rotation);
        }
    }

    private void OnCollisionStay(Collision other) {
        CheckDieResult();
        availableResult = true;

        ContactPoint contact = other.contacts[0];
        Quaternion rotation = Quaternion.Euler(-90, 0, 0);
        Vector3 position = transform.position;

        if(beamParticle != null && !beamInstantiated){
            beamFX = Instantiate(beamParticle, position, rotation);
            beamInstantiated = true;
        }
        if(beamInstantiated){
            beamFX.transform.position = position;
        }
    }

    private void OnCollisionExit(Collision other) {
        availableResult = false;
        if(beamInstantiated){
            Destroy(beamFX);
        }
    }

    private void OnDestroy(){
        if(beamInstantiated){
            Destroy(beamFX);
        }
    }

    private void CheckDieResult(){
        for(int i = 0; i < dieSides.Length; i++){
            if(dieSides[i].position.y > dieSides[result - 1].position.y){
                result = i + 1;
            }
        }
    }
}
