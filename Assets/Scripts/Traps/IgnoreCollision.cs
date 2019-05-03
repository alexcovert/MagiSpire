﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreCollision : MonoBehaviour
{
    private GameObject projectile;
    private CapsuleCollider col;

    // Use this for initialization
    void Start()
    {

    }
    void Update()
    {
        Debug.Log(projectile);
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Projectile")
        {
            projectile = other.gameObject;
            col = projectile.GetComponentInChildren<CapsuleCollider>();
            col.enabled = true;
        }
    }
}
