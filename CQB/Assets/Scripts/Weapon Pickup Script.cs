using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public bool isInteracted = false;

    public Firearm firearm;
    public int weaponIndexToUnlock;

    public void Interact()
    {
        Debug.Log("Interact called on: " + gameObject.name);
        if (!isInteracted)
        {
            isInteracted = true;
            UnlockWeapon();
        }
        else
        {
            Debug.Log("Already interacted with: " + gameObject.name);
        }
    }

    private void UnlockWeapon()
    {
        if (firearm != null)
        {
            firearm.UnlockWeapon(weaponIndexToUnlock);
            firearm.SetupWeapon(weaponIndexToUnlock); 
            Debug.Log($"Unlocked weapon at index: {weaponIndexToUnlock}");
        }
        else
        {
            Debug.LogError("Firearm reference is not set!");
        }
    }

}

