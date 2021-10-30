using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Swerve;
using DG.Tweening;
using PathCreation.Examples;

public enum CharacterType
{
    changing,
    money,
    car,
    motor,
    plane
};

public class CharacterMovementBehaviour : MonoBehaviour
{
    private Transform tr;
    private Transform Transform { get { return (tr == null) ? tr = GetComponent<Transform>() : tr; } }

    private Transform playerTr;
    private Transform playerTransform { get { return (playerTr == null) ? playerTr = Transform.GetChild(3).transform : playerTr; } }

    private BoxCollider col;
    private BoxCollider BoxCollider { get { return (col == null) ? col = GetComponent<BoxCollider>() : col; } }

    private PathFollower path;
    public PathFollower PathFollower { get { return (path == null) ? path = Transform.parent.GetComponent<PathFollower>() : path; } }

    [SerializeField] private Transform endOfTr;

    [SerializeField] private GameObject sideCam;
    [SerializeField] private GameObject backCam;

    [SerializeField] private Transform gObj;

    public CharacterType characterType;
    public Transform carTr, motorTr, planeTr;
    [SerializeField] private GameObject vehicleParticle1, vehicleParticle2;

    [Header("Movement Stats")]
    [Range(0,50)]
    [SerializeField] private float speed;
    [Range(0, 15)]
    public float clamp;
    [Range(0, 50)]
    [SerializeField] private float horizontalSpeed;
    [Range(0, 40)]
    [SerializeField] private float targetRot;
    [Range(0, 10)]
    [SerializeField] private float rotSpeed;
    [SerializeField] private float stairSpeed;
    [SerializeField] private float stairMoneyDelay;

    [SerializeField] private float planeClamp;
    [SerializeField] private float carClamp;
    [SerializeField] private float motorClamp;

    [SerializeField] private float offsetX;
    [SerializeField] private float offsetY;

    [SerializeField] private float movementDelayOnX; 
    public List<Transform> moneyParentList = new List<Transform>();
    public List<Transform> moneyTransformList = new List<Transform>();
    private bool isFirstInput;
    [HideInInspector] public bool isFinished;
    [HideInInspector] public bool isOnStairs;

    private MoneyPoolManager pool;
    [SerializeField] private Transform stairLastStep;

    public GameObject confetti1, confetti2;


    private void OnEnable()
    {
        EventManager.GetGObjTr += () => gObj;
        EventManager.GetPlayerTransform += () => Transform;
        EventManager.showWinPanel += () => isFinished = true;
        EventManager.showFailPanel += () => isFinished = true;
        EventManager.GetCharacterScript += () => this;
        EventManager.OnStairStatus += OnStairStatus;
        EventManager.CheckListRender += CheckList;
    }
    private void OnDisable()
    {
        EventManager.GetGObjTr -= () => gObj;
        EventManager.GetPlayerTransform -= () => Transform;
        EventManager.showWinPanel -= () => isFinished = true;
        EventManager.showFailPanel -= () => isFinished = true;
        EventManager.GetCharacterScript -= () => this;
        EventManager.OnStairStatus -= OnStairStatus;
        EventManager.CheckListRender -= CheckList;
    }
    private bool isTriggered;

    private void CheckList()
    {
        if(moneyTransformList.Count <= 300)
            foreach (var item in moneyTransformList)
                item.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
    }

    private void Start()
    {
        vehicleParticle1.transform.parent = null;
        vehicleParticle2.transform.parent = null;

        pool = EventManager.GetPoolScript?.Invoke();
        PathFollower.OnPathChanged();
    }
    private bool isOnStatus;
    private void OnStairStatus()
    {
        if (isTriggered)
            return;

        isOnStatus = true;
        isTriggered = true;
        Transform.DOLocalMoveX(0, .2f).
            OnComplete(() =>
            {
                StartCoroutine(OnStairMoney());
            });
        PathFollower.speed = 0;
        PathFollower.enabled = false;

        foreach (var itm in pool.activeList)
        {
            itm.tag = "Money";
        }
    }
    [SerializeField] private bool isMuch;
    private float delayValue;
    private bool isOnce;
    private float valueDel;
    private IEnumerator OnStairMoney()
    {
        yield return new WaitForSeconds(.1f);

        int index = 0;

        isMuch = true;
        delayValue = 1;
        //sideCam.SetActive(false);
        sideCam.GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow = gObj;
        sideCam.GetComponent<Cinemachine.CinemachineVirtualCamera>().LookAt = gObj;
        foreach (var item in pool.activeList)
        {
            EventManager.invokeHaptic?.Invoke(vibrationTypes.SoftImpact);
            item.parent = endOfTr;
            item.DOMove(Transform.position + Vector3.up * index * .2f, delayValue).SetEase(Ease.Linear);
            //yield return new WaitForFixedUpdate();
            if(!isOnce)
            {
                yield return new WaitForSeconds(.5f);
                isOnce = true;
                valueDel = 1;
            }
            index++;
        }
        yield return new WaitForSeconds(valueDel);
        isOnStairs = true;

        Transform.DOMove(new Vector3(stairLastStep.position.x, Transform.position.y, stairLastStep.position.z), 5).SetEase(Ease.Linear).SetId(5)
            .OnComplete(() =>
            {
                if (isFinished)
                    return;
                isFinished = true;
                isOnStairs = false;
                EventManager.showWinPanel?.Invoke();

                confetti1.transform.position = stairLastStep.position + Vector3.up * 2 + stairLastStep.transform.right * 2;
                confetti2.transform.position = stairLastStep.position + Vector3.up * 2 + stairLastStep.transform.right * 2;
                confetti1.SetActive(true);
                confetti2.SetActive(true);
            });


        gObj.DOMove(stairLastStep.position - Vector3.up * 3, 5).SetEase(Ease.Linear).SetId(6)
            .OnComplete(() =>
            {
                if (isFinished)
                    return;
                isFinished = true;
                isOnStairs = false;
                EventManager.showWinPanel?.Invoke();
            });
    }

    private void Update()
    {
        if (isOnStairs)
            return; 

        if (isOnStatus)
            return;

        else if (!isFirstInput)
        {
            PathFollower.speed = 0;
            if (Input.GetMouseButton(0))
            {
                isFirstInput = true;
                PathFollower.speed = 10;
            }
            return;
        }
        else if (isFinished)
        {
            PathFollower.speed = 0;
            return;
        }

        switch (characterType)
        {
            case CharacterType.money:
                SwerveController.MoveAndRotateOnAxis(Transform, clamp, false, true, false, EnumHolder.Axis.x, horizontalSpeed, targetRot, rotSpeed);
                break;
            case CharacterType.motor:
                SwerveController.MoveAndRotateOnAxis(motorTr, motorClamp, true, true, false, EnumHolder.Axis.z, horizontalSpeed, -targetRot, rotSpeed);
                break;
            case CharacterType.car:
                SwerveController.MoveAndRotateOnAxis(carTr, carClamp, true, true, false, EnumHolder.Axis.x, horizontalSpeed, targetRot *.5f, rotSpeed);
                break;
            case CharacterType.plane:
                SwerveController.MoveAndRotateOnAxis(planeTr, planeClamp, true, true, false, EnumHolder.Axis.z, horizontalSpeed, -targetRot * .5f, rotSpeed);
                break;
            case CharacterType.changing:
                break;
        }

    }

    int indexVeh = 0;
    [HideInInspector] public VehicleType vehType;
    private bool isEntered;
    private CharacterType type;

    public void Execute(CharacterType characterType0, Transform otherTr, int a, VehicleType vehicleType) => StartCoroutine(ChangeCharacterType(characterType0, a, vehicleType));
    public IEnumerator ChangeCharacterType(CharacterType characterType1, int value, VehicleType vehicleType)
    {
        if (vehicleType == VehicleType.first) indexVeh = 0;
        else if (vehicleType == VehicleType.second) indexVeh = 1;
        else if (vehicleType == VehicleType.third) indexVeh = 2;
        else if (vehicleType == VehicleType.fourth) indexVeh = 3;
        else if (vehicleType == VehicleType.fifth) indexVeh = 4;

        if (characterType1 != CharacterType.money && !isEntered)
        {
            isEntered = true;

            clamp = 2;
            for (int i = 0; i < moneyTransformList.Count; i++)
            {
                moneyTransformList[i].parent = null;
                moneyTransformList[i].gameObject.SetActive(false);
            }

            backCam.SetActive(true);
            sideCam.SetActive(false);

            EventManager.DeactiveVehicleLens?.Invoke();


            switch (characterType1)
            {
                case CharacterType.motor:
                    OnVehicleStatus(motorTr);
                    type = CharacterType.motor;
                    break;
                case CharacterType.car:
                    OnVehicleStatus(carTr);
                    type = CharacterType.car;
                    break;
                case CharacterType.plane:
                    OnVehicleStatus(planeTr);
                    type = CharacterType.plane;
                    break;
            }
        }
        else if(!isEntered)
        {
            characterType = characterType1;

            isEntered = true;

            switch (type)
            {
                case CharacterType.motor:
                    vehicleParticle1.transform.position = motorTr.position;
                    vehicleParticle1.SetActive(true);
                    motorTr.gameObject.SetActive(false);
                    vehicleParticle2.transform.position = Transform.position;
                    vehicleParticle2.gameObject.SetActive(true);
                    break;
                case CharacterType.car:
                    vehicleParticle1.transform.position = carTr.position;
                    vehicleParticle1.SetActive(true);
                    carTr.gameObject.SetActive(false);
                    vehicleParticle2.transform.position = Transform.position;
                    vehicleParticle2.gameObject.SetActive(true);
                    break;
                case CharacterType.plane:
                    vehicleParticle1.transform.position = planeTr.position;
                    vehicleParticle1.SetActive(true);
                    planeTr.gameObject.SetActive(false);
                    vehicleParticle2.transform.position = Transform.position;
                    vehicleParticle2.gameObject.SetActive(true);
                    break;
            }

            switch (characterType)
            {
                case CharacterType.money:
                    moneyTransformList.Clear();

                    if (value > 10)
                        EventManager.AfterVehicle?.Invoke(0, 3);
                    else if (value >= 6)
                        EventManager.AfterVehicle?.Invoke(0, 2);
                    else if (value > 0)
                        EventManager.AfterVehicle?.Invoke(0, 1);
                    break;
            }
            sideCam.SetActive(true);
            backCam.SetActive(false);
        }
        yield return new WaitForSeconds(.5f);
        isEntered = false;
    }

    public void RemoveCollidedParents(Transform other)
    {
        moneyTransformList.Remove(other);
    }

    private void OnVehicleStatus(Transform vehicle)
    {
        vehicleParticle1.transform.position = vehicle.position;
        vehicleParticle1.SetActive(true);
        vehicleParticle2.transform.position = Transform.position;
        vehicleParticle2.SetActive(true);

        pool.keys.Clear();

        for (int i = 0; i < vehicle.childCount; i++)
        {
            vehicle.GetChild(i).gameObject.SetActive(false);
        }

        vehicle.GetChild(indexVeh).gameObject.SetActive(true);

        vehicle.gameObject.SetActive(true);
        Transform.DOLocalMoveX(0, 1);
    }

}
