using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehiclePartLensDeactive : MonoBehaviour
{
    private bool isTriggered;
    [SerializeField] private GameObject lens1, lens2, lens3;


    private void OnEnable()
    {
        EventManager.DeactiveVehicleLens += Deactivation;
    }
    private void OnDisable()
    {
        EventManager.DeactiveVehicleLens -= Deactivation;
    }


    private void Deactivation()
    {
        if (isTriggered)
        {
            StartCoroutine(Delay());
        }
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(.5f);
        lens1.SetActive(false);
        lens2.SetActive(false);
        lens3.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        CharacterCollisionBehaviour character = other.GetComponent<CharacterCollisionBehaviour>();

        if(character != null && !isTriggered)
        {
            isTriggered = true;
        }
    }


}
