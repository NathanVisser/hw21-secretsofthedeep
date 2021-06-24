using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Reactor : MonoBehaviour
{
    [SerializeField]
    private float m_timePerCrystal = 1.5f;
    [SerializeField]
    private AudioSource m_sourceA;
    [SerializeField]
    private AudioSource m_sourceB;
    [SerializeField]
    private AudioSource m_sourceReactor;

    [SerializeField]
    private ReactorSocket[] m_sockets;
    private bool m_attemptingStartup;

    private bool m_startingReactor = false;
    private float m_reactorStartup = 0f;
    [SerializeField]
    private float m_reactorStartupTime = 5f;
    [SerializeField]
    private float m_reactorMinPitch = .4f;
    [SerializeField]
    private float m_reactorMaxPitch = 1f;
    [SerializeField]
    private float m_reactorMaxVolume = 1f;

    [SerializeField]
    private ObjectActivator activator;

    private bool m_reactorRunning = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_startingReactor)
        {
            m_reactorStartup += Time.deltaTime / m_reactorStartupTime;

            if (m_reactorStartup >= 1f)
            {
                m_reactorStartup = 1f;
                m_startingReactor = false;
            }

            m_sourceReactor.volume = m_reactorStartup * m_reactorMaxVolume;
            m_sourceReactor.pitch = Mathf.Lerp(m_reactorMinPitch, m_reactorMaxPitch, m_reactorStartup);
        }
    }


    public void AttemptStart()
    {
        if (!m_attemptingStartup && !m_startingReactor && !m_reactorRunning)
        {
            LockAll(true);

            m_attemptingStartup = true;
            StartCoroutine(RunStartupSequence());
        }
    }

    private IEnumerator RunStartupSequence()
    {
        m_sourceA.Play();
        m_sourceB.Play();

        for (int i = 0; i < 5; i++)
        {
            m_sockets[i].Light.enabled = true;
            m_sourceA.pitch = Data.Instance.crystalSoundSettings[i].pitchA;
            m_sourceB.pitch = Data.Instance.crystalSoundSettings[i].pitchB;
            yield return new WaitForSeconds(m_timePerCrystal);
            m_sockets[i].Light.enabled = false;
        }

        m_sourceA.Stop();
        m_sourceB.Stop();

        bool match = true;

        for (int i = 0; i < 5; i++)
        {
            if (m_sockets[i].CrystalID != i)
                match = false;
        }

        if (match)
        {
            // Reactor started
            Activated();
        }
        else
        {
            LockAll(false);
        }

        m_attemptingStartup = false;
    }

    private void Activated()
    {
        m_sourceReactor.pitch = m_reactorMinPitch;
        m_sourceReactor.volume = 0f;
        m_sourceReactor.Play();
        m_startingReactor = true;
        m_reactorRunning = true;

        if (activator!= null)
            activator.Activated();
    }

    private void LockAll(bool locked)
    {
        foreach (ReactorSocket reactorSocket in m_sockets)
        {
            reactorSocket.SetLock(locked);
        }
    }
}
