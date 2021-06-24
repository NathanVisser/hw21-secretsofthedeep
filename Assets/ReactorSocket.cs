using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Bson;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ReactorSocket : MonoBehaviour
{
    [SerializeField]
    private Light light;

    public Light Light => light;

    private int crystalID;
    public int CrystalID => crystalID;

    private bool m_locking = false;

    private XRBaseInteractable m_currentInteractable;

    public void OnCrystalSocketed(XRBaseInteractable interactor)
    {
        var crystal = interactor.gameObject.GetComponent<Crystal>();
        if (crystal != null)
        {
            crystalID = crystal.ID;
            interactor.GetComponent<Rigidbody>().isKinematic = false;
            m_currentInteractable = interactor;

            if(m_locking)
                LockInteractable(interactor, true);
        }
    }

    public void OnCrystalUnsocketed(XRBaseInteractable interactor)
    {
        crystalID = -1;
        interactor.GetComponent<Rigidbody>().isKinematic = false;
        m_currentInteractable = null;
    }

    private void LockInteractable(XRBaseInteractable interactable, bool lockActor)
    {
        var colliders = interactable.GetComponents<Collider>();

        foreach (Collider collider in colliders)
        {
            collider.enabled = !lockActor;
        }

        interactable.transform.parent = transform;
    }

    public void SetLock(bool locked)
    {
        m_locking = locked;

        if (m_currentInteractable != null)
        {
            LockInteractable(m_currentInteractable, locked);
        }
    }
}
