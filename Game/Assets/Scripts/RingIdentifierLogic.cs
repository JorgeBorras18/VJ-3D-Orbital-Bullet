using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingIdentifierLogic : MonoBehaviour
{
    public bool external;
    public int ringId;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    RingIdentifierLogic() { }

    

    public void setRingId(int id) {
        ringId = id;
    }

    public void setExternal(bool isExternal) {
        external = isExternal;
    }
    
    public bool sameRingAs(RingIdentifierLogic ringIdentifierLogic)
    {
        return ringIdentifierLogic.ringId == ringId && ringIdentifierLogic.external == external;
    }

    public int getRingId()
    {
        return ringId;
    }

    public bool isExternal()
    {
        return external;
    }

}
