using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FinishStairsBehaviour : MonoBehaviour
{
    private MoneyPoolManager pool;
    private CharacterMovementBehaviour character;


    private void Start()
    {
        pool = EventManager.GetPoolScript?.Invoke();
        character = EventManager.GetCharacterScript?.Invoke();
    }
    private bool isOnce;
    IEnumerator DelayHaptic()
    {
        yield return new WaitForSeconds(.2f);
        isOnce = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Money"))
        {
            other.transform.parent = null;
            pool.activeList.Remove(other.transform);

            if(pool.activeList.Count <= 3)
            {
                character.confetti1.transform.position = other.transform.position + Vector3.up * 2 + other.transform.right * 2;
                character.confetti2.transform.position = other.transform.position + Vector3.up * 2 - other.transform.right * 2;
                character.confetti1.SetActive(true);
                character.confetti2.SetActive(true);

                DOTween.Kill(5);
                DOTween.Kill(6);
                character.isFinished = true;
                character.isOnStairs = false;
                EventManager.showWinPanel?.Invoke();
            }

            if (isOnce)
                return;

            isOnce = true;
            EventManager.invokeHaptic?.Invoke(vibrationTypes.MediumImpact);
            StartCoroutine(DelayHaptic());
        }
    }
}
