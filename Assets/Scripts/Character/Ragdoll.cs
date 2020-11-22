using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    Transform parentBackup;

    Animator animator;
    Collider[] boneColliders;
    Rigidbody[] boneRigidbodies;

    public bool isPlayerObjectRagdoll = false;

    // Start is called before the first frame update
    void Start()
    {
        parentBackup = transform.parent;

        animator = GetComponent<Animator>();
        boneColliders = GetComponentsInChildren<Collider>();
        boneRigidbodies = GetComponentsInChildren<Rigidbody>();

        // Initialization
        ToggleRagdoll(false);
    }

    public void ToggleRagdoll(bool state)
    {
        // Ragdoll on -> Animator off
        animator.enabled = !state;

        // Parent setting : Don't chase parent's movement.
        // Position reset
        if (state)
        {
            // Be Ragdoll
            transform.parent = null;
        }
        else
        {
            if (!isPlayerObjectRagdoll)
            {
                // Reset Ragdoll
                transform.parent = parentBackup;
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
                transform.localScale = Vector3.one;
            }
        }

        // Collider and Rigidbody setting
        foreach (Rigidbody rigi in boneRigidbodies)
        {
            // Except myself
            if (rigi.transform == transform)
                continue;

            rigi.isKinematic = !state;
        }
        foreach (Collider bone in boneColliders)
        {
            // Except myself
            if (bone.transform == transform)
                continue;

            bone.enabled = state;
        }
    }
}
