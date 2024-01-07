using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Ring : MonoBehaviour
{
    PlatformLogic platformLogic;
    public string platformId;
    public int missingEnemies;
    public Vector3 positionToGoUp;
    RingIdentifierLogic identifierLogic;
    IndicadorLogic indicadorLogic = new IndicadorLogic();
    public bool finishedRing = false;


    // Start is called before the first frame update
    void Start()
    {
        identifierLogic = GetComponent<RingIdentifierLogic>();
        int ringNum = identifierLogic.getRingId();
        if (ringNum != 4)
            indicadorLogic = GameObject.Find("indicador" + ringNum.ToString()).GetComponent<IndicadorLogic>();
    }

    // Update is called once per frame
    void Update() {
    }

    public void setPlatformById() {
        if (platformId != "none") platformLogic = GameObject.Find(platformId).gameObject.GetComponent<PlatformLogic>();
        else platformLogic = null;
    }

    public bool isFinished() {
        int ringNum = identifierLogic.getRingId();
        int inside = (identifierLogic.isExternal()) ? 0 : 1;
        missingEnemies = GameObject.Find("enemics_" + ringNum.ToString() + "." + inside.ToString()).gameObject.transform.childCount;
        finishedRing = (platformId == "none" || platformLogic.isFinished()) && ((ringNum != 4) || (missingEnemies == 1 && ringNum == 4));
        return finishedRing;
    }

    public void triggerPlatformMovementToStart() { 
        platformLogic.triggerPlatformMovementToStart();
    }

    public bool playerIsInPositionToGoUp(Vector3 playerPosition, int last_ring_number) {
        int ringNum = identifierLogic.getRingId();
        return Math.Abs(playerPosition.x - positionToGoUp.x) <= 1 && Math.Abs(playerPosition.z - positionToGoUp.z) <= 1 && last_ring_number + 1 == ringNum;

    }

    public void turnIndicadorOn() {
        indicadorLogic.turnIndicadorOn();
    }

}
