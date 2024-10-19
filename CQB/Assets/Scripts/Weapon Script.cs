using NUnit;
using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using UnityEngine.Events;
using UnityEngine.UIElements.Experimental;
using System;

public class Firearm : MonoBehaviour
{
    public int weaponID;
    public float shotVel;
    public int fireMode;
    public float fireRate;
    public int currentClip;
    public int clipSize;
    public int maxAmmo;
    public int currentAmmo;
    public int reloadAmt;
    public float bulletLifespan;
    public float casingspeed;

    public bool useWeapon1 = true;
    public bool useWeapon2 = false;
    public bool CanFire = true;
    public Transform camera;


    [Header("Weapon Stats")]
    public GameObject shot;
    public GameObject casing;
    public GameObject muzzleFlashPrefab;
    public bool gunshake;
    public PlayerMovement playerAmmo;
    public Transform gunTransform;


    void Update()
    {
        if (Input.GetMouseButtonDown(0) && currentClip > 0 && currentAmmo > 0)
        {
            Fire();
        }
    }

    public void SetupWeapon(int id)
    {
        switch (id)
        {
            case 0 when useWeapon1: 
                weaponID = 0;
                shotVel = 300f; 
                fireMode = 0; 
                fireRate = 1f / 45f; 
                currentClip = 7;
                clipSize = 7;
                maxAmmo = 100;
                currentAmmo = 50; 
                reloadAmt = 7;
                bulletLifespan = 1.5f; 
                break;

            case 1 when useWeapon2: 
                weaponID = 1;
                shotVel = 900f; 
                fireMode = 1; 
                fireRate = 1f / 12f; 
                currentClip = 30;
                clipSize = 30;
                maxAmmo = 210;
                currentAmmo = 120; 
                reloadAmt = 30;
                bulletLifespan = 2f; 
                break;

            default:
                Debug.Log("Invalid weapon ID or weapon not available.");
                break;
        }
    }

    public void Fire()
    {
        if (Time.timeScale == 1)
        {

            GameObject muzzleFlash = Instantiate(muzzleFlashPrefab, gunTransform.position, gunTransform.rotation);


            gunshake = true;
            StartCoroutine(camshake());


            GameObject projectile = Instantiate(shot, gunTransform.position, gunTransform.rotation * Quaternion.Euler(90, 0, 0));
            Rigidbody rb = projectile.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.AddForce(camera.transform.forward * shotVel, ForceMode.Impulse);
            }


            currentClip--;
            CanFire = false;


            Destroy(projectile, 2f);
            Destroy(muzzleFlash, 0.1f);


            StartCoroutine(CooldownFire());
            StartCoroutine(GunAction());
        }
    }

    public void Reload()
    {
        if (currentClip >= clipSize) return;

        int reloadCount = clipSize - currentClip;
        int availableAmmo = playerAmmo.GetCurrentAmmo(weaponID);

        if (availableAmmo < reloadCount)
        {
            currentClip += availableAmmo;
            playerAmmo.DecreaseAmmo(weaponID, availableAmmo);
        }
        else
        {
            currentClip += reloadCount;
            playerAmmo.DecreaseAmmo(weaponID, reloadCount);
        }
    }

    private IEnumerator CooldownFire()
    {
        yield return new WaitForSeconds(fireRate);
        CanFire = true;
    }

    IEnumerator GunAction()
    {
        yield return new WaitForSeconds(0.01f);

        GameObject casing1 = Instantiate(casing, gunTransform.position, gunTransform.rotation * Quaternion.Euler(90, 0, 0));
        Rigidbody rb = casing1.GetComponent<Rigidbody>();
        rb.AddForce(camera.transform.right * casingspeed, ForceMode.Impulse);

        Destroy(casing1, 1f);
    }

    private IEnumerator camshake()
    {
        yield return new WaitForSeconds(.2f);
        gunshake = false;
    }


}


