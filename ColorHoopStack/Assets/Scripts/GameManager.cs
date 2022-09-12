using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    private GameObject selectedRing;
    private GameObject selectedPlatform;
    private Ring _ring;
    public bool isMoving;

    //Count of target puzzle (Level Target)
    public int targetFinishPlatformCount;

    //Finished platforms
    //If compeletedPlatformCount equal to target we will finish the game or next level
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

                    //Sending rings to other platforms, we are catching platform with selected platform collider
                    if (selectedRing != null && selectedPlatform != hit.collider.gameObject)
                    {
                        StandController _standController = hit.collider.GetComponent<StandController>();

                        //If stand is full our ring will not move to new stand --Controller
                        if (_standController.rings.Count != 4 && _standController.rings.Count != 0)
                        {
                            //Our ring color if same with last ring color of choosen stand
                            if (_ring.ringColor == _standController.rings[^1].GetComponent<Ring>().ringColor)
                            {

                                //Here (in our standController class) we delete the selected object with the SocketsChangingProcess method
                                //If we are going to send the selected ring to another platform. We set the required free sockets status
                                selectedPlatform.GetComponent<StandController>().SocketsChangingProcess(selectedRing);

                                _ring.MoveRings("ChangePosition", hit.collider.gameObject, _standController.GetAvaliableSocket(), _standController.autoMovePlace);

                                //When a circle goes to a new platform, we need to increase the appropriate socket of that platform as an index,
                                //so if a new circle comes, it should not be nested.
                                _standController.emptySocket++;
                                _standController.rings.Add(selectedRing);

                                //Control for closing stands - winning situation
                                _standController.RingsColorControl();

                                //After we adding rings to new platform, we cleaning our selected ring and platform
                                selectedRing = null;
                                selectedPlatform = null;
                            }
                            else
                            {
                                //If colors are not matched with last ring our ring will back own stand
                                _ring.MoveRings("BackToSocket");
                                selectedRing = null;
                                selectedPlatform = null;
                            }
                        }

                        //If stand is empty
                        else if (_standController.rings.Count == 0)
                        {
                            selectedPlatform.GetComponent<StandController>().SocketsChangingProcess(selectedRing);
                            _ring.MoveRings("ChangePosition", hit.collider.gameObject, _standController.GetAvaliableSocket(), _standController.autoMovePlace);
                            _standController.emptySocket++;
                            _standController.rings.Add(selectedRing);
                            _standController.RingsColorControl();
                            selectedRing = null;
                            selectedPlatform = null;
                        }

                        //If stand is full
                        else
                        {
                            _ring.MoveRings("BackToSocket");
                            selectedRing = null;
                            selectedPlatform = null;
                        }
                    }

                    //If player choose same platform againg
                    else if (selectedPlatform == hit.collider.gameObject)
                    {
                        _ring.MoveRings("BackToSocket");
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

    public void PlatformCompleted()
    {
        compeletedPlatformCount++;
        if (compeletedPlatformCount == targetFinishPlatformCount)
        {
            Debug.Log("wing game");
        }
    }
}
