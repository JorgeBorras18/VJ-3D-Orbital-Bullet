using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.VisualScripting.Metadata;

// Class that handles raise of every pilar of Light (Harvester Boss Fight)
public class LightPilar : MonoBehaviour
{
    enum PILAR { INACTIVE, SLEEPING, WARNING, RISING, RAISED }

    [SerializeField] GameObject _Preview;
    [SerializeField] GameObject _PilarChildShort;
    [SerializeField] GameObject _PilarChild;
    [SerializeField] GameObject choosen_Pilar_Type;
    [SerializeField] float rising_duration;
    [SerializeField] float StayRaisedDuration = 1f;

    private float sleepTimeout = 0;
    private float raise_event_timestamp = 0;
    private float raise_warning_duration = 0;
    private float max_scale = 7f;
    private PILAR actual_state;

    private void Start()
    {
        actual_state = PILAR.INACTIVE;
        _Preview.SetActive(false);
        _PilarChildShort.SetActive(false);
        _PilarChild.SetActive(false);
        transform.LookAt(new Vector3(0, transform.position.y, 0));
    }

    void FixedUpdate()
    {
        if (actual_state != PILAR.INACTIVE)
        {
            if (actual_state == PILAR.SLEEPING && Time.time - raise_event_timestamp > sleepTimeout)
            {
                actual_state = PILAR.WARNING;
                _Preview.SetActive(true);
                raise_event_timestamp = Time.time;
            }
            else if (actual_state == PILAR.WARNING && Time.time - raise_event_timestamp > raise_warning_duration)
            {
                actual_state = PILAR.RISING;
                raise_event_timestamp = Time.time;
                choosen_Pilar_Type.SetActive(true);
            }
            else if (actual_state == PILAR.RISING)
            {
                Vector3 prev_Scale = choosen_Pilar_Type.transform.lossyScale;
                if (Time.time - raise_event_timestamp < rising_duration)
                {
                    choosen_Pilar_Type.transform.localScale = new Vector3(prev_Scale.x, ((Time.time - raise_event_timestamp) * max_scale / rising_duration), prev_Scale.z);
                }
                else
                {
                    actual_state = PILAR.RAISED;
                    choosen_Pilar_Type.transform.localScale = new Vector3(prev_Scale.x, max_scale, prev_Scale.z);
                    raise_event_timestamp = Time.time;
                }
            }
            else if (actual_state == PILAR.RAISED && Time.time - raise_event_timestamp > StayRaisedDuration)
            {
                actual_state = PILAR.INACTIVE;
                _Preview.SetActive(false);
                choosen_Pilar_Type.transform.localScale = new Vector3(choosen_Pilar_Type.transform.localScale.x, 0.1f, choosen_Pilar_Type.transform.localScale.z);
                choosen_Pilar_Type.SetActive(false);
            }
        }
    }

    public void RaisePillar(float event_delay, float warning_delay, bool is_short)
    {
        if (actual_state != PILAR.INACTIVE) return;
        actual_state = PILAR.SLEEPING;
        sleepTimeout = event_delay;
        if (is_short)
        {
            max_scale = 2.5f;
            choosen_Pilar_Type = _PilarChildShort;
            StayRaisedDuration = 0.5f;
        }
        else
        {
            max_scale = 7f;
            choosen_Pilar_Type = _PilarChild;
            StayRaisedDuration = 1f;
        }
        raise_warning_duration = warning_delay;
        raise_event_timestamp = Time.time;
    }
}
