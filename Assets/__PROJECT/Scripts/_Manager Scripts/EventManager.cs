using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class EventManager
{
    #region GameManagerEvents
    public static Action startGame;
    public static Action winGame;
    public static Action loseGame;
    #endregion

    #region LevelManagerEvents
    public static Action loadNextScene;
    public static Action loadOpeningScene;
    public static Action loadSameScene;
    #endregion

    #region UserInterfaceEvents
    public static Func<bool> checkingSceneType;
    public static Action showWinPanel;
    public static Action showFailPanel;
    public static Action<float> changeProgressBarFillAmount;
    #endregion

    #region HapticEvents
    public static Action<vibrationTypes> invokeHaptic;
    #endregion

    public static Func<Transform> GetPlayerTransform;


    public static Action<int, int> StackNewMoneys;
    public static Action<List<Transform>, int, int> MoneyTransforms;

    public static Func<CharacterMovementBehaviour> GetCharacterScript;
    public static Func<MoneyPoolManager> GetPoolScript;

    public static Action<int, int> SetMoneyAfterVehicle;

    public static Action OnStairStatus;

    public static Func<CharacterCollisionBehaviour> GetCollisionBehaviour;
    public static Func<MoneyParticleManager> GetParticleManager;

    public static Action CollisionFinished;

    public static Action CheckListRender;

    public static Action<int,int> AfterVehicle;

    public static Action DeactiveVehicleLens;

    public static Func<Transform> GetGObjTr;


}