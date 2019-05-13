﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogProjectile : MonoBehaviour {
    private TrapBase trapBase;

    // custom to this trap
    [SerializeField] private int knockBackValue = 75;
    [SerializeField] private int knockUpValue = 25;
    [SerializeField] private float stunDuration = 0.75f;
    //Time before log disappears
    [SerializeField] private float lifeTime = 2f;

    [SerializeField] private float animationSpeed;

    [SerializeField] private float speedForKnockback = 0.05f;

    // let the FixedUpdate method know that there was a collision with player
    private bool hit = false;
    // the player (or whatever collided with this trap)
    private GameObject player = null;
    // keep track of how many frames of knockback have passed
    private int knockTimer = 0;
    //Player's animator for knockback animation
    private Animator anim = null;
    //Transform of log model
    private GameObject child;
    //see if log is still moving
    private Rigidbody rb;

    //Log collider
    private BoxCollider box;

    //Figure out which face this is on
    private CameraOneRotator playerOne;

    //So this doesn't die prematurely
    private bool canDie = false;

    void Awake()
    {
        playerOne = GameObject.Find("Player 1").GetComponent<CameraOneRotator>();
    }

    // Use this for initialization
    void Start () {
        trapBase = GetComponent<TrapBase>();
        child = transform.parent.gameObject.transform.GetChild(1).gameObject;
        rb = child.GetComponent<Rigidbody>();
        box = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    private void Update()
    {
        this.transform.position = child.transform.position;

        //So cannot die prematurely
        if(rb.velocity.y != 0)
        {
            canDie = true;
        }

        //only needs to have 0 velocity once.
        if ((rb.velocity.x <= 0.00001 && rb.velocity.x >= -0.00001) && (rb.velocity.z <= 0.00001 && rb.velocity.z >= -0.00001))
        {
            box.enabled = false;
            if (canDie == true)
            {
                StartCoroutine(Death());
            }
        }
        switch (playerOne.GetState())
        {
            case 1:
                if (rb.velocity.x > 0)
                {
                    box.enabled = false;
                }
                if (rb.velocity.x < -speedForKnockback)
                {
                    box.enabled = true;
                }
                break;
            case 2:
                if(rb.velocity.z > 0)
                {
                    box.enabled = false;
                }
                if(rb.velocity.z < -speedForKnockback)
                {
                    box.enabled = true;
                }
                break;
            case 3:
                if(rb.velocity.x > speedForKnockback)
                {
                    box.enabled = true;
                }
                if(rb.velocity.x < 0)
                {
                    box.enabled = false;
                }
                break;
            case 4:
                if (rb.velocity.z > speedForKnockback)
                {
                    box.enabled = true;
                }
                if (rb.velocity.z < 0)
                {
                    box.enabled = false;
                }
                break;
        }
    }
    void FixedUpdate () {
        
        if (player != null)
        {
            if (hit)
            {
                if (hit && knockTimer < 7 && knockTimer >= 5)
                {
                    trapBase.KnockBack(player, knockBackValue, 0);
                    knockTimer++;
                }
                else if (hit && knockTimer < 7)
                {
                    trapBase.KnockBack(player, 0, knockUpValue);
                    trapBase.Stun(player.gameObject, stunDuration);
                    knockTimer++;
                }
                else
                {
                    hit = false;
                    anim.SetBool("Knockback", hit);
                    knockTimer = 0;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            player = col.gameObject;
            hit = true;
            anim = player.GetComponent<PlayerOneMovement>().GetAnim();
            if (player.GetComponent<PlayerOneMovement>().IsCrouched() == false)
            {
                anim.Play("Knockback", 0);
            }
        }
    }

    private IEnumerator Death()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(this.transform.parent.gameObject);
    }
    
}
