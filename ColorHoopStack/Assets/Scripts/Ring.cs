using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ring : MonoBehaviour
{
    //Platform where the ring is located
    public GameObject parentPlatform;
    //Ring socket which the ring is placement
    public GameObject parentRingSocket;
    public bool isCanMove;
    public string ringColor;
    public GameManager gameManager;

    private GameObject movementPosition;
    private GameObject targetPlatform;
    private bool ringSelected;
    private bool isChangedRingPostion;
    private bool insertToSocket;
    private bool backToSocket;

    public void MoveRings(string process, GameObject stand = null, GameObject socket = null, GameObject movedObject = null)
    {
        switch (process)
        {
            case "Selection":
                movementPosition = movedObject;
                ringSelected = true;
                break;
            case "ChangePosition":
                targetPlatform = stand;
                parentRingSocket = socket;
                movementPosition = movedObject;
                isChangedRingPostion = true;
                break;
            case "BackToSocket":
                backToSocket = true;
                break;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (ringSelected)
        {
            transform.position = Vector3.Lerp(transform.position, movementPosition.transform.position, .5f);
            if (Vector3.Distance(transform.position, movementPosition.transform.position) < .10)
            {
                ringSelected = false;
            }
        }

        if (isChangedRingPostion)
        {
            transform.position = Vector3.Lerp(transform.position, movementPosition.transform.position, .5f);
            if (Vector3.Distance(transform.position, movementPosition.transform.position) < .10)
            {
                isChangedRingPostion = false;
                insertToSocket = true;
            }
        }
        if (insertToSocket)
        {
            //Lerp to stand ring socket
            transform.position = Vector3.Lerp(transform.position, parentRingSocket.transform.position, .5f);
            if (Vector3.Distance(transform.position, parentRingSocket.transform.position) < .10)
            {
                //When it close enough to stang, it will take the socket position
                transform.position = parentRingSocket.transform.position;
                insertToSocket = false;

                //Assaigning to object's parent platform is the target platform (The platform which player clicked)
                parentPlatform = targetPlatform;

                if (parentPlatform.GetComponent<StandController>().rings.Count > 1)
                {
                    parentPlatform.GetComponent<StandController>().rings[^2].GetComponent<Ring>().isCanMove = false;
                }
                gameManager.isMoving = false;
            }
        }
        if (backToSocket)
        {
            //Lerp to stand ring socket
            transform.position = Vector3.Lerp(transform.position, parentRingSocket.transform.position, .5f);
            if (Vector3.Distance(transform.position, parentRingSocket.transform.position) < .10)
            {
                //When it close enough to stang, it will take the socket position
                transform.position = parentRingSocket.transform.position;
                backToSocket = false;
                gameManager.isMoving = false;
            }
        }
    }
}
