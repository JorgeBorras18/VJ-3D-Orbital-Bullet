using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animation_Controller : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator player_animator;
    public Animator healpot_animator;
    public Animator gun_animator;
    public SpriteRenderer player_sprite;
    public SpriteRenderer healpot_sprite;
    public SpriteRenderer shoulder_sprite;
    public SpriteRenderer gun_sprite;
    [SerializeField] string ActualAnimation;

    public void changeAnimation(string new_Animation)
    {
        /*
        if (new_Animation == ActualAnimation) return;
        player_animator.Play("Player_" + new_Animation, 0, 0f);
        healpot_animator.Play("Healpot_" + new_Animation, 0, 0f);
        gun_animator.Play("Gun_" + new_Animation, 0, 0f);
        ActualAnimation = new_Animation;*/
        
        if (new_Animation == ActualAnimation) return;
        ActualAnimation = new_Animation;
        //Debug.Log(Time.time + " " + new_Animation);
        if (new_Animation == "Fast_Roll" || new_Animation == "Roll" || new_Animation == "Death")
        {
            transform.GetChild(0).gameObject.SetActive(false);
            player_animator.Play("Player_" + new_Animation, 0, 0f);
            healpot_animator.Play("Healpot_" + new_Animation, 0, 0f);
            return;
        }
        else if (transform.GetChild(0).gameObject.activeSelf == false) transform.GetChild(0).gameObject.SetActive(true);
        player_animator.Play("Player_" + new_Animation, 0, 0f);
        healpot_animator.Play("Healpot_" + new_Animation, 0, 0f);
        gun_animator.Play("Gun_" + new_Animation, 0, 0f);
        
    }

    public void flipX(bool facingRight)
    {
        player_sprite.flipX = facingRight;
        healpot_sprite.flipX = facingRight;
        if (facingRight)
        {
            transform.Rotate(new Vector3(0, 180f, 0));
        }
        else if (!facingRight)
        {
            transform.Rotate(new Vector3(0, -180f, 0));
        }
    }

    public void setAlive(bool player_is_alive)
    {
        player_animator.PlayInFixedTime("Player_Death", 0, 0f);
        healpot_animator.PlayInFixedTime("Healpot_Death", 0, 0f);
        gun_animator.PlayInFixedTime("Gun_Death", 0, 0f);
    }

    public string getActualState()
    {
        return ActualAnimation;
    }

    public bool animationHasFinished()
    {
        return player_animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !player_animator.IsInTransition(0);
    }
}
