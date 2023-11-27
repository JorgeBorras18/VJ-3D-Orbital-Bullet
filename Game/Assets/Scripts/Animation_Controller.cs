using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animation_Controller : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator player_animator;
    public Animator healpot_animator;
    public SpriteRenderer player_sprite;
    public SpriteRenderer healpot_sprite;
    [SerializeField] int ActualAnimation;

    public void changeAnimation(int new_Animation)
    {
        if (new_Animation == ActualAnimation) return;

        string animation_string = "";
        switch (new_Animation)
        {
            case 0:
                animation_string = "_Idle";
                break;
            case 1:
                animation_string = "_Run";
                break;
            case 2:
                animation_string = "_Jump";
                break;
            case 3:
                animation_string = "_Mid_Air_Jump";
                break;
            case 4:
                animation_string = "_Air_Fall";
                break;

        }
        player_animator.Play("Player" + animation_string, 0, 0f);
        healpot_animator.Play("Healpot" + animation_string, 0, 0f);
        ActualAnimation = new_Animation;
    }

    public void flipX(bool facingRight)
    {
        player_sprite.flipX = facingRight;
        healpot_sprite.flipX = facingRight;
    }

    public void setAlive(bool player_is_alive)
    {
        player_animator.Play("Player_Death", 0, 0f);
        healpot_animator.Play("Healpot_Death", 0, 0f);
    }
}
