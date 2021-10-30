using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    private Transform tr;
    private Transform Transform { get { return (tr == null) ? tr = GetComponent<Transform>() : tr; } }

    private Transform playerTransform;


    [Range(0,1)]
    [SerializeField] private float cameraFollowDelay;

    [Header("Offset Parameters")]
    private Vector3 offset;
    private Vector3 desiredPosition;
    private Vector3 smoothPosition;
    public bool isFinishStatus;

    void Start()
    {
        playerTransform = EventManager.GetGObjTr?.Invoke();
        offset = Transform.position - playerTransform.position;
    }

    void LateUpdate()
    {
        //if (isFinishStatus)
        //    return;
        desiredPosition = offset + playerTransform.position;
        //smoothPosition = Vector3.Lerp(smoothPosition, desiredPosition, cameraFollowDelay);

        //Transform.position = new Vector3(desiredPosition.x, Transform.position.y, Transform.position.z);
        Transform.position = desiredPosition;

    }

}
