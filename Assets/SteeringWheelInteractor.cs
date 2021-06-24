using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SteeringWheelInteractor : XRBaseInteractable
{
    [SerializeField]
    private float m_maxSteeringAmount = 180f;
    [SerializeField]
    private float m_velocitySpinAmp = 1;

    private Vector3 m_interactorStartPos;
    private bool m_beingHeld = false;
    private XRBaseInteractor m_currentInteractor;
    [SerializeField]
    private Transform m_wheelGraphic;

    void Update()
    {
        if (m_beingHeld)
        {
            var currentInterctorPos = Flatten(transform.InverseTransformPoint(m_currentInteractor.transform.position));
            var angle = Vector3.SignedAngle(m_interactorStartPos, currentInterctorPos, Vector3.forward);
            Debug.Log(angle);
            m_wheelGraphic.localRotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    protected override void OnSelectEnter(XRBaseInteractor interactor)
    {
        base.OnSelectEnter(interactor);
        m_interactorStartPos = Flatten(transform.InverseTransformPoint(interactor.transform.position));
        m_beingHeld = true;
        m_currentInteractor = interactor;
    }

    protected override void OnSelectExit(XRBaseInteractor interactor)
    {
        m_beingHeld = false;
        base.OnSelectExit(interactor);
    }

    private Vector3 Flatten(Vector3 input)
    {
        input.z = 0f;
        return input;
    }
}
