using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public enum LensType
{
    horizontal,
    vertical,
    motorLens,
    carLens,
    planeLens
}

public enum VehicleType
{
    first,
    second,
    third,
    fourth,
    fifth
}

public class LensBehaviour : MonoBehaviour
{
    private Transform tr;
    private Transform Transform { get { return (tr == null) ? tr = GetComponent<Transform>() : tr; } }

    [SerializeField] private TextMeshPro textMesh;
    private CharacterCollisionBehaviour charCol;

    [SerializeField] private GameObject motor;
    [SerializeField] private GameObject car;
    [SerializeField] private GameObject plane;

    public LensType lensType;
    public VehicleType vehicleType;
    public int lensValue;

    private MoneyPoolManager pool;


    private void Start()
    {
        pool = EventManager.GetPoolScript?.Invoke();
        charCol = EventManager.GetCollisionBehaviour?.Invoke();
        textMesh.text = lensValue.ToString();
        //CharacterCollisionBehaviour characterCollisionBehaviour = other.GetComponent<CharacterCollisionBehaviour>();


        if (lensType == LensType.motorLens)
            motor.SetActive(true);
        else if (lensType == LensType.carLens)
            car.SetActive(true);
        else if (lensType == LensType.planeLens)
            plane.SetActive(true);
    }
    private bool isTriggered;
    private bool isCollided;

    private void OnTriggerEnter(Collider other)
    {
        CharacterCollisionBehaviour characterCollisionBehaviour = other.GetComponent<CharacterCollisionBehaviour>();

        if (characterCollisionBehaviour != null && !isTriggered)
        {
            isTriggered = true;
            EventManager.invokeHaptic?.Invoke(vibrationTypes.MediumImpact);
            switch (lensType)
            {
                case LensType.horizontal:
                    characterCollisionBehaviour.HorVerCollision(lensValue, 0);
                    DownLens();
                    break;
                case LensType.vertical:
                    characterCollisionBehaviour.HorVerCollision(0, lensValue);
                    DownLens();
                    break;
            }
        }

        if (other.CompareTag("FrontMoney") && !isCollided && lensType != LensType.vertical ||
            other.CompareTag("FrontMoney") && !isCollided && lensType != LensType.horizontal)
        {
            isCollided = true;
            switch (lensType)
            {
                case LensType.motorLens:
                    if (charCol.plane || charCol.car)
                        return;
                    charCol.motor = true;
                    charCol.Execute(other.transform, this, vehicleType, lensType);
                    break;
                case LensType.carLens:

                    if (charCol.plane)
                        return;
                    charCol.car = true;
                    charCol.Execute(other.transform, this, vehicleType, lensType);
                    break;
                case LensType.planeLens:

                    charCol.plane = true;
                    charCol.Execute(other.transform, this, vehicleType, lensType);
                    break;
            }

            pool.CheckMoneyLength(false);
        }
    }

    private void DownLens() => Transform.DOMoveY(-3, 1).SetEase(Ease.Linear);

}
