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
    public int ringNum;
    IndicadorLogic indicadorLogic = new IndicadorLogic();


    // Start is called before the first frame update
    void Start()
    {
        missingEnemies = 0;
        indicadorLogic = GameObject.Find("indicador" + ringNum.ToString()).GetComponent<IndicadorLogic>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void setPlatformById() {
        if (platformId != "none") platformLogic = GameObject.Find(platformId).gameObject.GetComponent<PlatformLogic>();
        else platformLogic = null;
    }

    public bool isFinished() {
        return platformId == "none" || (platformLogic.isFinished() && missingEnemies == 0);
    }

    public void triggerPlatformMovementToStart() { 
        platformLogic.triggerPlatformMovementToStart();
    }

    public bool playerIsInPositionToGoUp(Vector3 playerPosition, int last_ring_number)
    {
        return Math.Abs(playerPosition.x - positionToGoUp.x) <= 1 && Math.Abs(playerPosition.z - positionToGoUp.z) <= 1 && last_ring_number + 1 == ringNum;

    }

    public void turnIndicadorOn()
    {
        indicadorLogic.turnIndicadorOn();
    }

}
