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

    public int ShotgunBB;

    [Header("Weapon Library")]
    public bool useWeapon0 = true; // M1911
    public bool useWeapon1 = false; // M4
    public bool useWeapon2 = false; // FN SCAR
    public bool useWeapon3 = false; 
    public bool useWeapon4 = false; 
    public bool useWeapon5 = false; 
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


    [Header("Shake")]
    public float gunShakeIntensity = 2f;
    public float shakeDuration = 0.5f;

    [Header("Weapon Locational Data")]
    public GameObject[] bulletInstantiators;
    public GameObject[] casingInstantiators;

    [Header("Weapon Mags")]
    public List<List<Magazine>> weaponMagazines;
    private int currentMagazineIndex = 0;


    private void Start()
    {
        weaponUnlocked = new bool[weapons.Length];
        for (int i = 0; i < weaponUnlocked.Length; i++)
        {
            weaponUnlocked[i] = false;
            weaponModels[i].SetActive(false);
        }

        weaponMagazines = new List<List<Magazine>>();

        var m1911Magazines = new List<Magazine>
        {
            new Magazine(21)
        };
        weaponMagazines.Add(m1911Magazines);

        var m4Magazines = new List<Magazine>
        {
            new Magazine(30),
            new Magazine(30),
            new Magazine(30)
        };
        weaponMagazines.Add(m4Magazines);

        var fnScarMagazines = new List<Magazine>
        {
            new Magazine(20),
            new Magazine(20)
        };
        weaponMagazines.Add(fnScarMagazines);

        var hk416Magazines = new List<Magazine>
        {
            new Magazine(30),
            new Magazine(30)
        };
        weaponMagazines.Add(hk416Magazines);

        var sigMpxMagazines = new List<Magazine>
        {
            new Magazine(20),
            new Magazine(20)
        };
        weaponMagazines.Add(sigMpxMagazines);


        var m906Magazines = new List<Magazine>
        {
            new Magazine(15)
        };
        weaponMagazines.Add(m906Magazines);

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

        if(Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }

        if (Input.GetMouseButtonDown(0) && currentAmmo > 0)
        {
            if(currentClip > 0)
            {
                Debug.Log("Firing weapon...");
                Fire();
            }
            else
            {
                Reload();
            }
            
        }
    }

    public void SetupWeapon(int id)
    {
        switch (id)
        {
            case 0 when useWeapon0: // M1911
                weaponID = 0;
                shotVel = 300f;
                fireMode = 0;
                fireRate = 0.5f;
                currentClip = 7;
                clipSize = 7;
                maxAmmo = 14;
                currentAmmo = 21;
                reloadAmt = 7;
                bulletLifespan = 1.5f;
                break;

            case 1 when useWeapon1: // M4
                weaponID = 1;
                shotVel = 900f;
                fireMode = 1;
                fireRate = 0.083f;
                currentClip = 30;
                clipSize = 30;
                maxAmmo = 60;
                currentAmmo = 90;
                reloadAmt = 30;
                bulletLifespan = 2f;
                break;

            case 2 when useWeapon2: // FN SCAR
                weaponID = 2;
                shotVel = 600f;
                fireMode = 2;
                fireRate = 0.067f;
                currentClip = 20;
                clipSize = 20;
                maxAmmo = 40;
                currentAmmo = 60;
                reloadAmt = 20;
                bulletLifespan = 1.0f;
                break;

            case 3 when useWeapon3: // HK416
                weaponID = 3;
                shotVel = 700f;
                fireMode = 2;
                fireRate = 0.1f;
                currentClip = 30;
                clipSize = 30;
                maxAmmo = 60;
                currentAmmo = 90;
                reloadAmt = 30;
                bulletLifespan = 1.0f;
                break;

            case 4 when useWeapon4: // SIG MPX
                weaponID = 4;
                shotVel = 600f;
                fireMode = 2;
                fireRate = 0.067f;
                currentClip = 20;
                clipSize = 20;
                maxAmmo = 40;
                currentAmmo = 60;
                reloadAmt = 20;
                bulletLifespan = 1.0f;
                break;

            
            case 5 when useWeapon5: // M-906 
                weaponID = 5;
                shotVel = 800f;
                fireMode = 2;
                fireRate = 0.083f;
                currentClip = 15;
                clipSize = 15;
                maxAmmo = 30;
                currentAmmo = 30;
                reloadAmt = 15;
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

            Transform cameraTransform = Camera.main.transform;
            float intensity = 2f;
            float duration = 0.5f;

            playerAmmo.TriggerShake(cameraTransform, intensity, duration);

            GameObject bulletInstantiator = bulletInstantiators[weaponID];
            GameObject projectile = Instantiate(shot, bulletInstantiator.transform.position, bulletInstantiator.transform.rotation);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(camera.transform.forward * shotVel, ForceMode.Impulse);
            }

            currentClip--;
            CanFire = false;

            Destroy(projectile, bulletLifespan);
            Destroy(muzzleFlash, 0.1f);

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


