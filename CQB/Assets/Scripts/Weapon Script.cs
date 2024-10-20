using NUnit;
using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Firearm : MonoBehaviour
{

    [Header("Weapon Stats")]
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


    [Header("Weapon Library")]
    public bool useWeapon1 = true;
    public bool useWeapon2 = false;
    public bool useWeapon3 = false;
    public bool CanFire = true;
    public Transform camera;


    [Header("Weapon Objects")]
    public GameObject shot;
    public GameObject muzzleFlashPrefab;
    public GameObject[] casingPrefabs;
    public bool gunshake;
    public PlayerMovement playerAmmo;
    public Transform gunTransform;


    [Header("Weapon Models")]
    public GameObject[] weapons;
    public GameObject[] weaponModels;
    public Transform weaponslot;
    public GameObject[] weaponpickups;
    private int currentWeaponIndex = -1;
    public bool[] weaponUnlocked;


    [Header("Weapon Locational Data")]
    public GameObject[] bulletInstantiators;
    public GameObject[] casingInstantiators;


    private void Start()
    {
        weaponUnlocked = new bool[weapons.Length];
        for (int i = 0; i < weaponUnlocked.Length; i++)
        {
            weaponUnlocked[i] = false;
            weaponModels[i].SetActive(false);
        }
    }

    void Update()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons.Length > i && weaponUnlocked[i])
            {
                Debug.Log($"Weapon {i} is unlocked and can be switched.");
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    SwitchWeapon(i);
                }
            }
            else
            {
                Debug.Log($"Weapon {i} is locked.");
            }
        }

        if (Input.GetMouseButtonDown(0) && currentClip > 0 && currentAmmo > 0)
        {
            Debug.Log("Firing weapon...");
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

            case 2 when useWeapon3:
                weaponID = 2;
                shotVel = 600f;
                fireMode = 2;
                fireRate = 1f / 15f;
                currentClip = 20;
                clipSize = 20;
                maxAmmo = 150;
                currentAmmo = 80;
                reloadAmt = 20;
                bulletLifespan = 1.0f;
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

            
            GameObject bulletInstantiator = bulletInstantiators[weaponID];
            GameObject projectile = Instantiate(shot, bulletInstantiator.transform.position, bulletInstantiator.transform.rotation );
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(camera.transform.forward * shotVel, ForceMode.Impulse);
            }

            
            GameObject casingInstantiator = casingInstantiators[weaponID];
            GameObject casing1 = Instantiate(casingPrefabs[weaponID], casingInstantiator.transform.position, casingInstantiator.transform.rotation * Quaternion.Euler(0, -90, 0));
            Rigidbody casingRb = casing1.GetComponent<Rigidbody>();
            casingRb.AddForce(camera.transform.right * casingspeed, ForceMode.Impulse);

            
            currentClip--;
            CanFire = false;

            Destroy(projectile, bulletLifespan);
            Destroy(muzzleFlash, 0.1f);
            Destroy(casing1, 1f);

            StartCoroutine(CooldownFire());
            StartCoroutine(GunAction());
        }
    }

    public void SwitchWeapon(int weaponIndex)
    {
        Debug.Log($"Attempting to switch to weapon index: {weaponIndex}");

        if (weaponIndex >= 0 && weaponIndex < weapons.Length && weaponUnlocked[weaponIndex])
        {
            Debug.Log($"Switching to weapon: {weapons[weaponIndex].name}");

            if (currentWeaponIndex >= 0)
            {
                weapons[currentWeaponIndex].SetActive(false);
                weaponModels[currentWeaponIndex].SetActive(false);
            }

            weapons[weaponIndex].SetActive(true);
            weaponModels[weaponIndex].SetActive(true);
            currentWeaponIndex = weaponIndex;

            SetupWeapon(weaponIndex);
        }
        else
        {
            Debug.LogError("Cannot switch to weapon: either the index is invalid or the weapon is locked.");
        }
    }

    public void UnlockWeapon(int weaponIndex)
    {
        if (weaponIndex >= 0 && weaponIndex < weaponUnlocked.Length)
        {
            weaponUnlocked[weaponIndex] = true;
            SwitchWeapon(weaponIndex);
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
    }

    private IEnumerator camshake()
    {
        yield return new WaitForSeconds(.2f);
        gunshake = false;
    }
}


