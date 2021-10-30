using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using DG.Tweening;

public class ObstacleBehaviour : MonoBehaviour
{
    private CharacterMovementBehaviour character;
    private CharacterCollisionBehaviour charCollision;
    private MoneyPoolManager pool;
    private List<Transform> frontList = new List<Transform>();
    private bool isTriggered;
    private List<GameObject> deletedObjs = new List<GameObject>();
    private MoneyParticleManager particleManager;
    private int modInt = -1;
    private int index = -1;

    private void Start()
    {
        character = EventManager.GetCharacterScript?.Invoke();
        pool = EventManager.GetPoolScript?.Invoke();

        particleManager = EventManager.GetParticleManager?.Invoke();
        charCollision = EventManager.GetCollisionBehaviour?.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FrontMoney"))
        {
            other.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
            frontList.Add(other.transform);

            if (!isTriggered)
            {
                isTriggered = true;
                ExecuteDelay();
            }

            other.GetComponent<MeshRenderer>().enabled = false;
        }
        else if (other.CompareTag("Money"))
            foreach (var item in frontList)
                if (Mathf.Approximately(item.transform.localPosition.x, other.transform.localPosition.x) &&
                    Mathf.Approximately(item.transform.localPosition.y, other.transform.localPosition.y))
                {
                    EventManager.invokeHaptic?.Invoke(vibrationTypes.LightImpact);

                    other.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
                    modInt++;
                    index++;

                    if (index > 999)
                    {
                        EventManager.CollisionFinished?.Invoke();
                        index = 0;
                    }

                    if (modInt % 3 == 0)
                    {
                        particleManager.particleList[index].transform.position = other.transform.position;
                        particleManager.particleList[index].SetActive(true);
                    }
                    break;
                }
        if (other.CompareTag("MoneyCol"))
        {

            EventManager.invokeHaptic?.Invoke(vibrationTypes.LightImpact);

            index++;

            if (index > 999)
            {
                EventManager.CollisionFinished?.Invoke();
                index = 0;
            }

            charCollision.moneyCount++;
            other.gameObject.SetActive(false);

            particleManager.particleList[index].transform.position = other.transform.position;
            particleManager.particleList[index].SetActive(true);
        }
    }

    public List<MoneyPoolManager.Key> keys = new List<MoneyPoolManager.Key>();

    void ExecuteDelay() => StartCoroutine(enumerator());

    private IEnumerator enumerator()
    {
        yield return new WaitForSeconds(.3f);

        foreach (var item in frontList)
        {
            for (int i = 0; i < pool.keys.Count; i++)
            {
                if (Mathf.Approximately(item.transform.localPosition.x / pool.offsetX, pool.keys[i].tagX) &&
                    Mathf.Approximately((item.transform.localPosition.y - pool.initAltitude) / pool.offsetY + 1, pool.keys[i].tagY))
                {
                    foreach (var itm in pool.keys[i].elements)
                        deletedObjs.Add(itm.gameObject);

                    pool.keys[i].elements.Clear();
                    keys.Add(pool.keys[i]);
                }
            }
        }

        yield return new WaitForSeconds(.1f);


        foreach (var i in keys)
            pool.keys.Remove(i);

        foreach (var item in deletedObjs)
        {
            character.moneyTransformList.Remove(item.transform);
            //yield return new WaitForSeconds(.005f);
            Destroy(item);
        }
        frontList.Clear();

        yield return new WaitForSeconds(.1f);

        pool.CheckMoneyLength(false);

        if (character.moneyTransformList.Count == 0)
            EventManager.showFailPanel?.Invoke();

        EventManager.CheckListRender?.Invoke();

        yield return new WaitForSeconds(3);
        EventManager.CollisionFinished?.Invoke();


    }

}
