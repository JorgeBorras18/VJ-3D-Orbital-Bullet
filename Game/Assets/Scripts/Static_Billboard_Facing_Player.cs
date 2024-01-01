using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Static_Billboard_Facing_Player : MonoBehaviour
{
    [SerializeField] private Angular_Physics angular_physics_player;
    [SerializeField] private Transform Player;
    private float actual_angle;
    private bool facingRight;

    // Start is called before the first frame update
    void Start()
    {
        actual_angle = (Mathf.Atan(transform.position.z / transform.position.x) + Mathf.PI * 2) % (Mathf.PI * 2);
        if (angular_physics_player != null) GameObject.Find("Player").GetComponent<Angular_Physics>();
        if (Player == null) Player = GameObject.Find("Player").GetComponent<Transform>();
    }

    void FixedUpdate()
    {
        //Billboard facing Player
        transform.LookAt(transform.position + Player.forward);
        if (facingRight) transform.Rotate(0, 180, 0);

        //Should be facing Left or Right?
        float player_angle = angular_physics_player.getRelativeAngle(actual_angle);
        if (!facingRight && ((player_angle < Mathf.PI / 2f) || (player_angle > Mathf.PI && player_angle < Mathf.PI * 3f / 2f)))
            facingRight = true;
        else if (facingRight && ((player_angle > Mathf.PI * 3f / 2f) || (player_angle > Mathf.PI / 2f && player_angle < Mathf.PI)))
             facingRight = false;
    }

    public bool isFacingRight()
    {
        return facingRight;
    }

}
