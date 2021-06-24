using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuseBox : MonoBehaviour
{
    public ParticleSystem[] SparkleFuseVFX;
    public ParticleSystem[] SwitchedOnVFX;
    public ParticleSystem[] SwitchedOffVFX;
    public ObjectActivator Activator;
    bool m_FusePresent = false;

    public void Switched(int step)
    {
        if (!m_FusePresent)
            return;

        if (Activator != null)
            Activator.Activated();

        if (step == 0)
        {
            foreach (var s in SwitchedOffVFX)
            {
                s.Play();
            }
        }
        else
        {
            foreach (var s in SwitchedOnVFX)
            {
                s.Play();
            }
        }
    }
    
    public void FuseSocketed(bool socketed)
    {
        m_FusePresent = socketed;

        if (m_FusePresent)
        {
            foreach (var s in SparkleFuseVFX)
            {
                s.Play();
            }
        }
    }
}
