using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ReactorSocket : MonoBehaviour
{
    [SerializeField]
    private Light light;

    public Light Light => light;

    private int crystalID;
    public int CrystalID => crystalID;

    public void OnCrystalSocketed(XRBaseInteractable interactor)
    {
        Debug.Log("Slotted");
        var crystal = interactor.gameObject.GetComponent<Crystal>();
        if (crystal != null)
        {
            crystalID = crystal.ID;
            interactor.GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    public void OnCrystalUnsocketed(XRBaseInteractable interactor)
    {
        crystalID = -1;
        interactor.GetComponent<Rigidbody>().isKinematic = false;
    }

}
