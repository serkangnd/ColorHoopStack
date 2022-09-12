using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    private GameObject selectedRing;
    private GameObject selectedPlatform;
    private Ring _ring;
    public bool isMoving;
    public int targetPlatformCount;
    private int compeletedPlatformCount;
    
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit,100))
            {
                if(hit.collider != null && hit.collider.CompareTag("Stand"))
                {
                    Debug.Log("hit");
                    //Sending rings to other platforms
                    if (selectedRing != null && selectedPlatform != hit.collider.gameObject)
                    {
                        StandController _standController = hit.collider.GetComponent<StandController>();
                        //Here (in our standController class) we delete the selected object with the SocketsChangingProcess method
                        //If we are going to send the selected ring to another platform. We set the required free sockets status
                        selectedPlatform.GetComponent<StandController>().SocketsChangingProcess(selectedRing);

                        _ring.MoveRings("ChangePosition", hit.collider.gameObject, _standController.GetAvaliableSocket(), _standController.autoMovePlace);
                        //When a circle goes to a new platform, we need to increase the appropriate socket of that platform as an index,
                        //so if a new circle comes, it should not be nested.
                        _standController.emptySocket++;
                        _standController.rings.Add(selectedRing);
                        //After we adding rings to new platform, we cleaning our selected ring and platform
                        selectedRing = null;
                        selectedPlatform = null;
                    }
                    //If there is no selected platform that code block will work
                    else
                    {
                        StandController _standController = hit.collider.GetComponent<StandController>();
                        //Selected ring will be last object of platform
                        selectedRing = _standController.GetToppomRingOfPlatform();
                        _ring = selectedRing.GetComponent<Ring>();
                        //If there is a selected object we are enable moving, for control the situation
                        isMoving = true;

                        //If ring is movable, ring lerp to autoMovePlace which is little bit up of ring (just a effect like ring upping from stand)
                        if (_ring.isCanMove)
                        {
                            _ring.MoveRings("Selection", null, null, _ring.parentPlatform.GetComponent<StandController>().autoMovePlace);
                            selectedPlatform = _ring.parentPlatform;
                        }
                    }
                }
            }
        }
    }
}
