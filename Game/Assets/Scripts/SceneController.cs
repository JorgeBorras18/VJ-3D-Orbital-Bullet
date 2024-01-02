using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Numerics;
//using System;

public class SceneController : MonoBehaviour
{
    private int currentRing;
    private int number_of_rings = 3; // to be 5

    private bool gameIsFinished = false;
    private bool gameIsStarted = false;


    //////////// Game elements ////////////
    
    // Player
    private PlayerLogic player;
    
    // Rings
    private List<Ring> internalRings = new List<Ring>();
    private List<Ring> externalRings = new List<Ring>();


    // Start is called before the first frame update
    void Start()
    {
       currentRing = 0;
       init_rings();
       init_player();
        gameIsStarted = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameIsStarted)
        {
            if (currentRing != (number_of_rings - 1) && currentRingIsFinished()) {
                externalRings[currentRing].turnIndicadorOn();
                if (Input.GetKeyDown(KeyCode.L) && playerIsInPositionToGoUp()) {
                    externalRings[currentRing + 1].triggerPlatformMovementToStart();
                    player.triggerToJumpToTheNextRing();
                    ++currentRing;
                }
            }
        }
        
    }

    private void init_player()
    {
        player = GameObject.Find("2D_Player").GetComponent<PlayerLogic>();
    }


    private void init_rings() {
       
        //string ringId = "level" + i.ToString();
        externalRings.Add(GameObject.Find("collisionable/externes/" + "level0").GetComponent<Ring>());
        externalRings.Add(GameObject.Find("collisionable/externes/" + "level1").GetComponent<Ring>());
        externalRings.Add(GameObject.Find("collisionable/externes/" + "level2").GetComponent<Ring>());
        // externalRings.Add(GameObject.Find("collisionable/externes/" + "level3").GetComponent<Ring>());
        // externalRings.Add(GameObject.Find("collisionable/externes/" + "level4").GetComponent<Ring>());
        externalRings[0].setPlatformById();
        externalRings[1].setPlatformById();
        externalRings[2].setPlatformById();
        // externalRings[3].setPlatformById();
        // externalRings[4].setPlatformById();
    }


    private bool playerIsInPositionToGoUp() {
        return externalRings[currentRing + 1].playerIsInPositionToGoUp(GameObject.Find("2D_Player").transform.position, currentRing);
        
    }
    private bool currentRingIsFinished()
    {
        return externalRings[currentRing].isFinished();
    }


}