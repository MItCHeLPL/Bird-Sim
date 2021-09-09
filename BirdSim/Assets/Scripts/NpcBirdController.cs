using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcBirdController : MonoBehaviour
{
    private Animator anim;

    [SerializeField] private int frameCooldown = 1000; //Cooldown between animations
    [SerializeField] private List<string> animations; //Animations to be played

    private int animationOffset; //Offset between animations
    private float IdleAgitated = 0; //Random idle animation

    void Awake()
    {
        anim = GetComponent<Animator>();

        TriggerRandomAnimation(); //Launch starting animation

        animationOffset = Random.Range(frameCooldown / 4, frameCooldown / 2); //Random offset between animations

        //Random idle animation
        IdleAgitated = Random.Range(0, 1);
        anim.SetFloat("IdleAgitated", IdleAgitated); 
    }

    void FixedUpdate()
    {
        //play random animation every set amount of frames with offset
        if((Time.frameCount + animationOffset) % frameCooldown == 0)
		{
            TriggerRandomAnimation();
        }
    }

    private void TriggerRandomAnimation()
	{
        int rand = Random.Range(0, animations.Count); //Pick random animation
        anim.SetTrigger(animations[rand]); //Play animation
    }
}
