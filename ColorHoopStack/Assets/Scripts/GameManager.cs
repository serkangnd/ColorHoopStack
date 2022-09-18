using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private GameManager gameManager;
    private GameObject selectedRing;
    private GameObject selectedPlatform;
    private Ring _ring;
    public bool isMoving;

    //Count of target puzzle (Level Target)
    public int targetFinishPlatformCount;

    //Finished platforms
    //If compeletedPlatformCount equal to target we will finish the game or next level
    private int compeletedPlatformCount;

    //SFX
    [SerializeField] private AudioSource gameSoundTrack;
    [SerializeField] private AudioSource backToSocketEffect;
    [SerializeField] private AudioSource insertToSocketEffect;

    //Level Index
    [SerializeField] TextMeshProUGUI currentLevelText;
    [SerializeField] private static int currentLevelIndex;
    public GameObject nextLevelPanel;

    //tapTo play and game situation
    public GameObject tapToPlay;
    public bool isGameStart = false;
    
    void Start()
    {
        gameSoundTrack.Play();
        //Active scenes start from 0 so we need to add 1 
        currentLevelIndex = SceneManager.GetActiveScene().buildIndex + 1;
        SetLevelTexts();
    }

    void Update()
    {
        if (!isGameStart)
        {
            tapToPlay.SetActive(true);
        }
        else
        {
            tapToPlay.SetActive(false);
        }
        if (Input.GetMouseButtonDown(0) && Input.touchCount < 2)
        {
            isGameStart = true;
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
                                insertToSocketEffect.Play();
                            }
                            else if (selectedRing != null && hit.collider.CompareTag("Untagged"))
                            {
                                _ring.MoveRings("BackToSocket");
                                selectedRing = null;
                                selectedPlatform = null;
                                backToSocketEffect.Play();
                            }
                            else
                            {
                                //If colors are not matched with last ring our ring will back own stand
                                _ring.MoveRings("BackToSocket");
                                selectedRing = null;
                                selectedPlatform = null;
                                backToSocketEffect.Play();
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
                            insertToSocketEffect.Play();
                        }

                        //If stand is full
                        else
                        {
                            _ring.MoveRings("BackToSocket");
                            selectedRing = null;
                            selectedPlatform = null;
                            backToSocketEffect.Play();
                        }
                    }

                    //If player choose same platform againg
                    else if (selectedPlatform == hit.collider.gameObject)
                    {
                        _ring.MoveRings("BackToSocket");
                        selectedRing = null;
                        selectedPlatform = null;
                        backToSocketEffect.Play();
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

                if (selectedRing != null && hit.collider.CompareTag("Untagged"))
                {
                    _ring.MoveRings("BackToSocket");
                    selectedRing = null;
                    selectedPlatform = null;
                    backToSocketEffect.Play();
                }
            }
        }
    }

    public void PlatformCompleted()
    {
        compeletedPlatformCount++;
        if (compeletedPlatformCount == targetFinishPlatformCount)
        {
            gameSoundTrack.Stop();
            nextLevelPanel.SetActive(true);
        }
    }

    public void SetLevelTexts()
    {
        currentLevelText.text = "Level: " + currentLevelIndex.ToString();
    }
}
