using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard_Facing_Player : MonoBehaviour
{
    [SerializeField] private Angular_Physics angular_physics_player;
    [SerializeField] private Transform Player;
    private bool facingRight;

    // Start is called before the first frame update
    void Start()
    {
        if (angular_physics_player == null) angular_physics_player = GameObject.Find("Player").GetComponent<Angular_Physics>();
        if (Player == null) Player = GameObject.Find("Player").GetComponent<Transform>();
    }

    //like an update, but must be called after moving object
    // offset angle is to cancel out rotate depending on desired facing dir (away or facing player)
    public void turn_to_camera(float new_angle, float offset_angle, bool just_look_at_player)
    {
        //Should be facing Left or Right?
        float player_angle = angular_physics_player.getRelativeAngle(new_angle);
        Debug.Log(player_angle * 180 / Mathf.PI + " - " + offset_angle);
        if (!facingRight && ((player_angle < Mathf.PI / 2f) || (player_angle > Mathf.PI && player_angle < Mathf.PI * 3f / 2f)))
            facingRight = true;
        else if (facingRight && ((player_angle > Mathf.PI * 3f / 2f) || (player_angle > Mathf.PI / 2f && player_angle < Mathf.PI)))
             facingRight = false;


        //Billboard facing Player
        transform.LookAt(transform.position + Player.forward);
        if (player_angle > Mathf.PI || just_look_at_player)
        {
            if (facingRight) transform.Rotate(0, 180 + offset_angle, 0);
            else if (offset_angle != 0) transform.Rotate(0, offset_angle, 0);
        }
        else
        {
            if (!facingRight) transform.Rotate(0, 180 + offset_angle, 0);
            else if (offset_angle != 0) transform.Rotate(0, offset_angle, 0);
        }
        
    }

    public bool isFacingRight()
    {
        return facingRight;
    }

}
