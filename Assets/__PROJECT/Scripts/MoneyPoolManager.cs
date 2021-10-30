using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MoneyPoolManager : MonoBehaviour
{
    [System.Serializable]
    public class MoneysOnZ
    {
        public Transform tra;
    }
    public List<MoneysOnZ> moneysOnZs = new List<MoneysOnZ>();

    private Transform tr;
    private Transform Transform { get { return (tr == null) ? tr = GetComponent<Transform>() : tr; } }

    public Dictionary<float, Queue<Transform>> zObjects = new Dictionary<float, Queue<Transform>>();

    private CharacterMovementBehaviour character;

    [SerializeField] private int prefabCount;
    [SerializeField] private GameObject moneyPrefab;
    public List<Transform> prefabList = new List<Transform>();

    [SerializeField] private int lengthX;
    [SerializeField] private int lengthY;
    [SerializeField] private int lengthZ;

    public float offsetX;
    public float offsetY;
    [SerializeField] private float offsetZ;

    public float initAltitude;

    [SerializeField] private float placementDelay;

    [HideInInspector] public int currentXLength;
    [HideInInspector] public int currentYLength;
    private int currentTotalMoney;
    private int nextXLength;
    private int nextYLength;
    private int nextTotalMoney;

    [SerializeField] private List<GameObject> moneyPrefabs = new List<GameObject>();

    [HideInInspector] public List<Transform> activeList = new List<Transform>();


    private void OnEnable()
    {
        EventManager.StackNewMoneys += CalculateDifference;
        EventManager.GetPoolScript += () => this;
        EventManager.SetMoneyAfterVehicle += SetMoneyAfterVehiclePart;
        EventManager.AfterVehicle += AfterVehicle;
    }
    private void OnDisable()
    {
        EventManager.StackNewMoneys -= CalculateDifference;
        EventManager.GetPoolScript -= () => this;
        EventManager.SetMoneyAfterVehicle -= SetMoneyAfterVehiclePart;
        EventManager.AfterVehicle -= AfterVehicle;
    }

    private void AfterVehicle(int x, int y)
    {
        StartCoroutine(DelayForCalculation(x, y));

    }
    private List<GameObject> willdeletedList = new List<GameObject>();
    private void SetMoneyAfterVehiclePart(int x, int y)
    {
        currentXLength = 1;
        currentYLength = 1;
        foreach (var item in character.moneyTransformList)
        {
            willdeletedList.Add(item.gameObject);
        }
        foreach (var itm in willdeletedList)
        {
            Destroy(itm);
        }
        character.moneyTransformList.Clear();
        willdeletedList.Clear();

        CalculateDifference(x, y);
    }

    private void Start()
    {
        DOTween.SetTweensCapacity(100000, 1000);

        character = EventManager.GetCharacterScript?.Invoke();

        for (int i = 0; i < prefabCount; i++)
        {
            var tmp = Instantiate(moneyPrefabs[Random.Range(0,3)], Transform);
            prefabList.Add(tmp.transform);
        }

        //for (int i = 0; i < character.moneyParentList[0].childCount; i++)
        //{
        //    character.moneyParentList[0].GetChild(i).tag = "FrontMoney";
        //}

        CheckMoneyLength(true);
    }

    [SerializeField]private List<float> listY = new List<float>();
    [SerializeField]private List<float> listX = new List<float>();

    public void CheckMoneyLength(bool isFirst)
    {
        listX.Clear();
        listY.Clear();

        if (isFirst)
        {
            foreach (var item in character.moneyParentList)
                for (int i = 0; i < item.childCount; i++)
                    character.moneyTransformList.Add(item.GetChild(i));
        }

        if(character.moneyTransformList.Count != 0)
        {
            for (int i = 0; i < character.moneyParentList[0].childCount; i++)
            {
                if (!listY.Contains(character.moneyParentList[0].GetChild(i).localPosition.y))
                    listY.Add(character.moneyParentList[0].GetChild(i).localPosition.y);
                if (!listX.Contains(character.moneyParentList[0].GetChild(i).localPosition.x))
                    listX.Add(character.moneyParentList[0].GetChild(i).localPosition.x);
            }
            currentYLength = listY.Count;
            currentXLength = listX.Count;
        }
        else
        {
            currentXLength = 0;
            currentYLength = 0;
        }
        currentTotalMoney = currentXLength * currentYLength * lengthZ;

        if (character.moneyTransformList.Count == 0) currentYLength = 0;

        if (currentXLength < 6)
            character.clamp = 2;
    }

    private void CalculateDifference(int nextX, int nextY)
    {
        CheckMoneyLength(false);

        StartCoroutine(DelayForCalculation(nextX, nextY));
    }
    public void StartCoroutine(int x, int y)=> StartCoroutine(DelayForCalculation(x, y));
    IEnumerator DelayForCalculation(int nextX, int nextY)
    {
        yield return new WaitForSeconds(.1f);
        nextXLength = currentXLength + nextX;
        nextYLength = currentYLength + nextY;
        nextTotalMoney = nextXLength * nextYLength * lengthZ;
        //Debug.Log(currentTotalMoney + " " + nextTotalMoney);

        int difference = nextTotalMoney - character.moneyTransformList.Count;

        for (int i = 0; i < difference; i++)
        {
            character.moneyTransformList.Add(prefabList[i]);
            prefabList[i].transform.position = character.transform.position;
            prefabList[i].gameObject.SetActive(true);
            prefabList.RemoveAt(i);
        }

        PlaceNewVoxel();

    }

    private void PlaceNewVoxel()
    {
        StartCoroutine(Delay());

        int index = 0;

        float rightX = nextXLength * .5f;
        float leftX = -rightX;

        activeList.Clear();

        for (int i = 0; i < lengthZ; i++)
        {
            for (float j = leftX; j < rightX; j++)
                for (int k = 0; k < nextYLength; k++)
                {
                    if (i == 0)
                        character.moneyTransformList[index].tag = "FrontMoney";

                    character.moneyTransformList[index].parent = character.moneyParentList[i];
                    character.moneyTransformList[index].DOLocalMove(new Vector3(j * offsetX, k * offsetY + initAltitude, 0), placementDelay).SetEase(Ease.Linear);
                    character.moneyTransformList[index].localRotation = Quaternion.Euler(Vector3.zero);

                    EventManager.invokeHaptic?.Invoke(vibrationTypes.LightImpact);

                    if (k == nextYLength - 1 || i == lengthZ - 1 || j + 1 >= rightX - 1 || j == leftX)
                    {
                        character.moneyTransformList[index].gameObject.SetActive(true);
                        character.moneyTransformList[index].GetChild(0).GetComponent<MeshRenderer>().enabled = true;

                        activeList.Add(character.moneyTransformList[index]);
                    }
                    else
                    {
                        character.moneyTransformList[index].GetChild(0).GetComponent<MeshRenderer>().enabled = false;

                        if (activeList.Contains(character.moneyTransformList[index]))
                            activeList.Remove(character.moneyTransformList[index]);
                    }

                    index++;
                }
        }
    }

    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(.65f);
        FakeDictionary();
    }
    public List<Key> keys = new List<Key>();

    [System.Serializable]
    public class Key
    {
        public float tagX;
        public float tagY;

        public List<Transform> elements = new List<Transform>();
    }

    private void FakeDictionary()
    {
        lengthX = nextXLength;
        lengthY = nextYLength;

        if (lengthX >= 6)
            character.clamp = .85f;
        else
            character.clamp = 2;

        keys.Clear();

        for (int i = 0; i < character.moneyParentList.Count; i++)
        {
            for (int j = 0; j < character.moneyParentList[i].childCount; j++)
            {
                float keyX = character.moneyParentList[i].GetChild(j).localPosition.x / offsetX;
                float keyY = (character.moneyParentList[i].GetChild(j).localPosition.y - initAltitude) / offsetY + 1;

                bool isThere = false;

                foreach (var item in keys)
                {
                    if (item != null && item.tagX == keyX && item.tagY == keyY)
                    {
                        item.elements.Add(character.moneyParentList[i].GetChild(j));
                        isThere = true;
                    }
                }

                if (!isThere)
                {
                    Key ky = new Key();
                    ky.tagX = keyX;
                    ky.tagY = keyY;
                    ky.elements.Add(character.moneyParentList[i].GetChild(j));
                    keys.Add(ky);
                }
            }
        }
    }

}
