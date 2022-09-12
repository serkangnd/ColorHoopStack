using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandController : MonoBehaviour
{
    //When player touch to rings, ring will automatically move to up
    public GameObject autoMovePlace;
    //Sockets need for ring placement correctly
    public GameObject[] sockets;
    //We put the rings the right place and also we need compare colors and counts with emptySocket
    public int emptySocket;
    //Our ring list
    public List<GameObject> rings = new();
    //We create a gameManager for catching movements and rings places
    [SerializeField] private GameManager gameManager;
    //When compeleteRingCount reach max ring count our stand will be close
    [SerializeField] private int compeleteRingCount;

    //When player wants to move rings we need to take toppest ring
    public GameObject GetToppomRingOfPlatform()
    {
        //It will return the last object
        return rings[^1]; //this is a index operator, you can also use => return rings[rings.count-1];
    }

    public GameObject GetAvaliableSocket()
    {
        //The logic here is to send the empty Socket variable as an index to our sockets list
        return sockets[emptySocket];
    }

    //This method work for when player choose another stand for ring
    //We need to remove ring from parentPlatform and we calculate the emptySockets of parentPlatform
    public void SocketsChangingProcess(GameObject removedRing)
    {
        //Delete from the list which moved ring
        rings.Remove(removedRing);
        //Controller
        if (rings.Count != 0)
        {
            //Calculate the empty sockets
            //we have a list by name of sockets. Lists start from 0 index.
            //At the moment, we have 4 circles and when we remove a circle from the platform,
            //the new circle that will come to that area is removed so that it fits into the correct socket.
            //4-1 = 3 so that the appropriate socket there is actually a 3-index socket.
            emptySocket--;
            //When we delete the moved ring, we need to change new ring's isCanMove situation for it can be movable
            rings[^1].GetComponent<Ring>().isCanMove = true;
        }
        else
        {
            //ıf there is no rings on platform our empty socket place will be 0 automatically
            emptySocket = 0;
        }
    }

    public void RingsColorControl()
    {
        //If our stand is full now we can check our rings are compelete
        if (rings.Count == 4)
        {
            string refColor = rings[0].GetComponent<Ring>().ringColor;
            foreach (var item in rings)
            {
                if (refColor == item.GetComponent<Ring>().ringColor)
                {
                    compeleteRingCount++;
                }
            }

            if (compeleteRingCount == 4)
            {
                gameManager.PlatformCompleted();
                ClosingFinishedPlatform();
            }
            else
            {
                compeleteRingCount = 0;
            }
        }
    }

    void ClosingFinishedPlatform()
    {
        foreach (var item in rings)
        {
            item.GetComponent<Ring>().isCanMove = false;
            Color32 color = item.GetComponent<MeshRenderer>().material.GetColor("_Color");
            color.a = 150;
            item.GetComponent<MeshRenderer>().material.SetColor("_Color", color);
            //We were finding our platforms with their tags. We are changing the tag so that no action is taken
            gameObject.tag = "Untagged";
        }
    }
}
