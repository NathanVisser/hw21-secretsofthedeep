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

    private XRBaseInteractable m_currentInteractable;

    public void OnCrystalSocketed(XRBaseInteractable interactor)
    {
        Debug.Log("Slotted");
        var crystal = interactor.gameObject.GetComponent<Crystal>();
        if (crystal != null)
        {
            crystalID = crystal.ID;
            interactor.GetComponent<Rigidbody>().isKinematic = false;
            m_currentInteractable = interactor;
        }
    }

    public void OnCrystalUnsocketed(XRBaseInteractable interactor)
    {
        crystalID = -1;
        interactor.GetComponent<Rigidbody>().isKinematic = false;
        m_currentInteractable = null;
    }

    public void SetLock(bool locked)
    {
        if (m_currentInteractable != null)
        {
            var colliders = m_currentInteractable.GetComponents<Collider>();

            foreach (Collider collider in colliders)
            {
                collider.enabled = !locked;
            }
        }
    }
}
