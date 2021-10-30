using System;
using System.Collections;
using System.Collections.Generic;                                               //Elephant SDK'yı kurduktan sonra, bu scripti yorumdan çıkarmalıyız. Elephant Scene'deki Elephatn UI scriptinde
using ElephantSDK;                                                              //bir GameObject tanımlayıp bu tanımladığımız field'a prefab klasöründeki ElephantManager prefabını sürüklemeliyiz.
using UnityEngine;                                                              //Son olarak ElephantUI scriptinde PlayGame methodundaki sahne yükleme işleminin hemen üstünde daha önce tanımladığımız prefabı Instantiate etmemiz gerekiyor.

public class ElephantManager : MonoBehaviour                                    //Elephant ile ilgili işlemlerin yapıldığı script. (A/B, Eventler vs.)
{
    public static ElephantManager instance;

    private void Awake()
    {
        instance = this;
    }

    // this is can be on any GameObject instance 
    void Start()
    {
        ElephantCore.onOpen += OnOpen;
    }

    private void OnDisable()
    {
        ElephantCore.onOpen -= OnOpen;
    }

    void OnOpen(bool gdprRequired)
    {

    }
    public void SendPlayerStartsPlayLevel(int level)
    {
        Elephant.LevelStarted(level);
    }

    public void SendPlayerCompletesLevel(int level)
    {
        Elephant.LevelCompleted(level);
    }

    public void SendPlayerFailsLevel(int level)
    {
        Elephant.LevelFailed(level);
    }
}