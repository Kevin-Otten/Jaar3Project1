﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : InteractableObject {
    public float sphereRadius = 5;
    public Transform soldierInsidePos;

    public Camera barrelCam;
    public GameObject turret;
    public GameObject barrel;
    public float barrelRotationSpeed;

    public float screenBoundary;
    private Vector3 soldierOutsidePos;
    private Soldier currentSoldier;
    public bool soldierInside;

    private Weapon previousWeapon;
    public Camera thirdPersonCamera;

    [Header("Clamp properties")]
    public bool clamp;
    public float clampX;
    public float clampY;
    private float xRotInput;
    private float yRotInput;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if(GetComponentInChildren<Weapon>().currentClip > 0)
        {
            Interact();
        }

        if (TeamManager.instance.turnTime <= .1F)
        {
            if (soldierInside)
            {
                ExitTank();
            }
        }

        if (Input.GetButtonDown("Enter"))
        {
            if (soldierInside)
            {
                ExitTank(); 
            }
        }
    }

    private void FixedUpdate()
    {
        if (soldierInside)
        {
            Movement();
            if (Input.GetMouseButtonDown(1) && !currentSoldier.canShoot)
            {
                currentSoldier.CombatToggle();
                thirdPersonCamera.depth = -1;
                Camera.main.depth = 1;
                GetComponentInChildren<Weapon>().SpecialFunctionalityToggle();
            }
        }
    }

    void Movement()
    {
        Vector2 mousePos = Input.mousePosition;

        float h = Input.GetAxis("Mouse X");
        float v = Input.GetAxis("Mouse Y");

        if (!clamp)
        {
            turret.transform.Rotate(transform.up * h * barrelRotationSpeed * Time.deltaTime);
            barrel.transform.Rotate(Vector3.forward * v * barrelRotationSpeed * Time.deltaTime);
        }
        else
        {
            xRotInput += Input.GetAxis("Mouse X") * Time.deltaTime * barrelRotationSpeed;
            xRotInput = Mathf.Clamp(xRotInput, -clampX, clampX);
            turret.transform.localRotation = Quaternion.Euler(0, xRotInput, 0);

            yRotInput += Input.GetAxis("Mouse Y") * Time.deltaTime * barrelRotationSpeed;
            yRotInput = Mathf.Clamp(yRotInput, -clampY, clampY);
            barrel.transform.localRotation = Quaternion.Euler(0, 0, -yRotInput);

        }




    }

    public override void Interact()
    {
        if (soldierNearby())
        {
            if (Input.GetKeyDown("e") && !currentSoldier.canShoot)
            {
                previousWeapon = currentSoldier.equippedWeapon;

                soldierOutsidePos = currentSoldier.gameObject.transform.position;
                currentSoldier.gameObject.transform.position = soldierInsidePos.position;
                currentSoldier.isActive = false;
                currentSoldier.soldierMovement.canMoveAndRotate = false;

                soldierInside = true;
                currentSoldier.equippedWeapon = GetComponentInChildren<Weapon>();
                GetComponentInChildren<Weapon>().mySoldier = currentSoldier;

                thirdPersonCamera.depth = 1;
                Camera.main.depth = -1;
               
            }
        }
        else if (soldierInside)
        {
            if (Input.GetKeyDown("e") && !currentSoldier.canShoot)
            {
                ExitTank();
            }
        }
    }

    public void ExitTank()
    {
        currentSoldier.gameObject.transform.position = soldierOutsidePos;
        currentSoldier.isActive = true;
        currentSoldier.soldierMovement.canMoveAndRotate = true;

        

        currentSoldier.equippedWeapon = previousWeapon;
        previousWeapon = null;
        GetComponentInChildren<Weapon>().mySoldier = null;

        GetComponentInChildren<Weapon>().SpecialFunctionalityToggle();
        thirdPersonCamera.depth = -1;
        Camera.main.depth = 1;
        soldierInside = false;
    }

    public bool soldierNearby()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, sphereRadius);
        foreach (Collider c in cols)
        {
            if (c.GetComponentInParent<Soldier>())
            {
                if (c.GetComponentInParent<Soldier>().isActive)
                {
                    currentSoldier = c.GetComponentInParent<Soldier>();
                    return true;
                }
            }
        }

        return false;
    }

}
