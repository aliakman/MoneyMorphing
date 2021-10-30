using System.Collections;
using System.Collections.Generic;
using GameAnalyticsSDK;
using UnityEngine;

public class GameAnalyticsInitialize : MonoBehaviour                            //GA SDK eklendikten sonra yorum satırları kaldırılıp loading scene'e GameAnalyticsManager prefabı atılmalı.
{
    void Start()
    {
        GameAnalytics.Initialize();
    }
}