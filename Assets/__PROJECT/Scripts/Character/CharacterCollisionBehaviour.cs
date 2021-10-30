using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CharacterCollisionBehaviour : MonoBehaviour
{
    private CharacterMovementBehaviour charMove;
    private CharacterMovementBehaviour CharacterMovementBehaviour { get { return (charMove == null) ? charMove = GetComponent<CharacterMovementBehaviour>() : charMove; } }


    private CharacterMovementBehaviour character = EventManager.GetCharacterScript?.Invoke();

    private bool avoidSecCol;

    [HideInInspector] public bool motor;
    [HideInInspector] public bool car;
    [HideInInspector] public bool plane;

    [HideInInspector] public int moneyCount;

    private MoneyPoolManager pool;


    private void OnEnable()
    {
        EventManager.GetCollisionBehaviour += () => this;
    }
    private void OnDisable()
    {
        EventManager.GetCollisionBehaviour -= () => this;
    }

    private void Start()
    {
        pool = EventManager.GetPoolScript?.Invoke();
    }
    private List<float> xValues = new List<float>();
    private List<float> yValues = new List<float>();
    private VehicleType oldVehicleType;
    public void HorVerCollision(int horizontal, int vertical)
    {
        if (!avoidSecCol)
        {
            avoidSecCol = true;
            StartCoroutine(AvoidSecondInteraction());
            EventManager.StackNewMoneys?.Invoke(horizontal, vertical); //bu satırı yoruma al
            //------------------//------------------//------------------ BURADA DICTIONARY'YLE YAPACAGIN ISLEMI YAZ //------------------//------------------//------------------

            //if(horizontal != 0)
            //{
            //    for (int i = 0; i < pool.keys.Count; i++)
            //    {
            //        if (!xValues.Contains(pool.keys[i].tagX))
            //            xValues.Add(pool.keys[i].tagX);
            //    }
            //    pool.currentXLength = xValues.Count;
            //    pool.StartCoroutine(horizontal, vertical);
            //}
            //else if(vertical != 0)
            //{
            //    for (int i = 0; i < pool.keys.Count; i++)
            //    {
            //        if (!yValues.Contains(pool.keys[i].tagY))
            //            yValues.Add(pool.keys[i].tagY);
            //    }
            //    pool.currentYLength = yValues.Count;
            //    pool.StartCoroutine(horizontal, vertical);
            //}
        }

    }

    IEnumerator AvoidSecondInteraction()
    {
        yield return new WaitForSeconds(.3f);
        avoidSecCol = false;
    }
    List<LensBehaviour> lenses = new List<LensBehaviour>();
    public void Execute(Transform otherTr, LensBehaviour lens, VehicleType vehicleType, LensType lensType)
    {
        lenses.Add(lens);
        StartCoroutine(VehicleLensCollision(otherTr, vehicleType, lensType));
    }

    public IEnumerator VehicleLensCollision(Transform otherTr, VehicleType vehicleType, LensType lensType)
    {
        yield return new WaitForSeconds(.2f);
        
        if (plane && lensType == LensType.planeLens)
        {
            if (!plane1)
            {
                oldVehicleType = vehicleType;
                plane1 = true;
                CharacterMovementBehaviour.characterType = CharacterType.plane;
                CharacterMovementBehaviour.Execute(CharacterType.plane, otherTr, 0, vehicleType);
                foreach (var item in lenses)
                {
                    if (item.lensType == LensType.planeLens)
                        item.gameObject.SetActive(false);
                }
            }
        }
        else if (car && lensType == LensType.carLens && !plane)
        {
            if (!car1)
            {
                oldVehicleType = vehicleType;
                car1 = true;
                CharacterMovementBehaviour.characterType = CharacterType.car;
                CharacterMovementBehaviour.Execute(CharacterType.car, otherTr, 0, vehicleType);
                foreach (var item in lenses)
                {
                    if (item.lensType == LensType.carLens)
                        item.gameObject.SetActive(false);
                }
            }
        }
        else if (motor && lensType == LensType.motorLens && !car)
        {
            if (!motor1)
            {
                oldVehicleType = vehicleType;

                motor1 = true;
                CharacterMovementBehaviour.characterType = CharacterType.motor;
                CharacterMovementBehaviour.Execute(CharacterType.motor, otherTr, 0, vehicleType);
                foreach (var item in lenses)
                {
                    if (item.lensType == LensType.motorLens)
                        item.gameObject.SetActive(false);
                }
            }
        }

        yield return new WaitForSeconds(2);

        motor = false;
        car = false;
        plane = false;
        motor1 = false;
        car1 = false;
        plane1 = false;
    }
    private bool car1, motor1, plane1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FinishLine"))
        {
            EventManager.OnStairStatus?.Invoke();
            EventManager.invokeHaptic?.Invoke(vibrationTypes.LightImpact);

        }
        else if (other.CompareTag("EndOfVehicle"))
        {
            EventManager.invokeHaptic?.Invoke(vibrationTypes.LightImpact);
            CharacterMovementBehaviour.Execute(CharacterType.money, CharacterMovementBehaviour.transform, moneyCount, oldVehicleType);

        }
    }
}
