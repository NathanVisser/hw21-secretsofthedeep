using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class SteeringWheelInteractor : XRBaseInteractable
{
    [System.Serializable]
    public class DragDistanceEvent : UnityEvent<float> { }
    public DragDistanceEvent OnSteering;

    [SerializeField]
    private float m_maxSteeringAmount = 180f;
    [SerializeField]
    private float m_velocitySpinAmp = 1;

    private Vector3 m_interactorStartPos;
    private bool m_beingHeld = false;
    private XRBaseInteractor m_currentInteractor;
    [SerializeField]
    private Transform m_wheelGraphic;
    private float m_currentAngle = 0f;
    private float m_lastAngle;

    [SerializeField]
    private float m_displayDailRotationAmount = 56f;
    [SerializeField]
    private Transform m_displayDail;

    void Update()
    {
        if (m_beingHeld)
        {
            var currentInterctorPos = Flatten(transform.InverseTransformPoint(m_currentInteractor.transform.position));
            var angle = Vector3.SignedAngle(m_interactorStartPos, currentInterctorPos, Vector3.forward);
            var totalAngle = Mathf.Clamp(m_currentAngle + angle, -m_maxSteeringAmount, m_maxSteeringAmount);
            m_currentAngle = totalAngle;
            m_wheelGraphic.localRotation = Quaternion.Euler(0f, 0f, totalAngle);
            m_interactorStartPos = currentInterctorPos;
            m_displayDail.transform.localRotation = Quaternion.Euler(0f, (totalAngle / m_maxSteeringAmount) * m_displayDailRotationAmount, 0f);

            if (OnSteering != null)
                OnSteering.Invoke(totalAngle / m_maxSteeringAmount);
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
