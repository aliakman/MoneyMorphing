using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyParticleManager : MonoBehaviour
{
    [SerializeField] private GameObject particlePrefab;
    public List<GameObject> particleList = new List<GameObject>();
    public int particleCount;


    private void OnEnable()
    {
        EventManager.GetParticleManager += () => this;
        EventManager.CollisionFinished += SetListBack;
    }
    private void OnDisable()
    {
        EventManager.GetParticleManager -= () => this;
        EventManager.CollisionFinished -= SetListBack;
    }


    private void Start()
    {
        for (int i = 0; i < particleCount; i++)
            particleList.Add(Instantiate(particlePrefab, transform));
    }

    private void SetListBack()
    {
        foreach (var item in particleList)
        {
            item.SetActive(false);
        }
    }

}
