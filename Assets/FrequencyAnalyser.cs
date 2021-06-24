using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class FrequencyAnalyser : MonoBehaviour
{
    [SerializeField]
    private LineRenderer m_lineRendererCrystal;
    [SerializeField]
    private LineRenderer m_lineRendererInputs;

    [SerializeField]
    private MeshRenderer m_ledA, m_ledB;

    [SerializeField]
    private Material m_ledMatRed, m_ledMatGreen;

    [SerializeField]
    private float m_maxCrystalVolume = .7f;

    private float m_pointXMin = -0.5f;
    private float m_pointXMax = 0.5f;
    private float m_pointYMin = -0.5f;
    private float m_pointYMax = 0.5f;
    private int m_points = 100;

    private float m_minFreq = 1f;
    private float m_maxFreq = 40f;

    private float freqA = .5f;
    private float AmplA = 0f;
    private float PhaseA = 0f;
    private float freqB = .5f;
    private float AmplB = 0f;
    private float PhaseB = 0f;

    private float m_maxFreqDeviation = 1f;
    private float m_maxAmplDeviation = .2f;
    private float m_maxPhaseDeviation = .2f;

    private Crystal m_currentCrystal;

    private bool m_Amatched = false;
    private bool m_Bmatched = false;
    private float m_crystalAVolume = 0f;
    private float m_crystalBVolume = 0f;
    private float m_staticVolume = 0f;
    private bool m_hasCrystal = false;


    [SerializeField]
    private AudioSource sourceA, sourceB, sourceBuzz;

    public void SetFreqA(DialInteractable dail)
    {
        freqA = m_minFreq + (dail.CurrentAngle / dail.RotationAngleMaximum) * m_maxFreq;
        DrawScreen();
    }

    public void SetAmplA(DialInteractable dail)
    {
        AmplA = dail.CurrentAngle / dail.RotationAngleMaximum;
        DrawScreen();
    }

    public void SetPhaseA(DialInteractable dail)
    {
        PhaseA = (dail.CurrentAngle / dail.RotationAngleMaximum);
        DrawScreen();
    }

    public void SetFreqB(DialInteractable dail)
    {
        freqB = m_minFreq + (dail.CurrentAngle / dail.RotationAngleMaximum) * m_maxFreq;
        DrawScreen();
    }

    public void SetAmplB(DialInteractable dail)
    {
        AmplB = dail.CurrentAngle / dail.RotationAngleMaximum;
        DrawScreen();
    }

    public void SetPhaseB(DialInteractable dail)
    {
        PhaseB = (dail.CurrentAngle / dail.RotationAngleMaximum);
        DrawScreen();
    }

    private void Start()
    {
        Toggle(false);
        DrawScreen();
    }

    private void Update()
    {
        if (m_hasCrystal)
        {
            HandleSound();
        }
    }

    private void HandleSound()
    {
        FadeSound(ref m_crystalAVolume, (m_Amatched && m_Bmatched)? m_maxCrystalVolume : 0f);
        FadeSound(ref m_crystalBVolume, (m_Amatched || m_Bmatched)? m_maxCrystalVolume : 0f);
        FadeSound( ref m_staticVolume, (m_Amatched ? 0f : .12f) + (m_Bmatched ? 0f : .12f));

        sourceA.volume = m_crystalAVolume;
        sourceB.volume = m_crystalBVolume;
        sourceBuzz.volume = m_staticVolume;
    }

    private void FadeSound(ref float volume, float targetVolume)
    {
        if(volume == targetVolume)
            return;

        if (volume < targetVolume)
        {
            volume += Time.deltaTime;

            if (volume > targetVolume)
                volume = targetVolume;
        }
        else
        {
            volume -= Time.deltaTime;

            if (volume < targetVolume)
                volume = targetVolume;
        }
    }

    private void DrawScreen()
    {
        Vector3[] inputPoints = new Vector3[m_points];
        Vector3[] crystalPoints = new Vector3[m_points];

        for (int i = 0; i < m_points; i++)
        {
            float position = (float) i / (float)m_points;

            var inputPointA = CalculatePoint(position, freqA, AmplA, PhaseA);
            var inputPointB = CalculatePoint(position, freqB, AmplB, PhaseB);

            inputPoints[i] = (inputPointA + inputPointB) / 2f;

            if (m_currentCrystal != null)
            {
                var crystalPointA = CalculatePoint(position, m_currentCrystal.FreqA, m_currentCrystal.AmplitudeA,
                    m_currentCrystal.PhaseA);
                var crystalPointB = CalculatePoint(position, m_currentCrystal.FreqB, m_currentCrystal.AmplitudeB,
                    m_currentCrystal.PhaseB);

                crystalPoints[i] = (crystalPointA + crystalPointB) / 2f;
            }
            else
            {
                crystalPoints[i] = new Vector3(Mathf.Lerp(m_pointXMin, m_pointXMax, position), 0.5f, 0f);
            }
        }

        m_lineRendererInputs.SetPositions(inputPoints);

        m_lineRendererCrystal.SetPositions(crystalPoints);

        if(m_hasCrystal)
            CheckMatch();
    }

    public void Toggle(bool on)
    {
        m_ledA.enabled = on;
        m_ledB.enabled = on;
        m_lineRendererCrystal.enabled = on;
        m_lineRendererInputs.enabled = on;
    }

    private void CheckMatch()
    {
        bool BankAMatchCrystalA = MatchesA(freqA, AmplA, PhaseA);
        bool BankAMatchCrystalB = MatchesB(freqA, AmplA, PhaseA);
        bool BankBMatchCrystalA = MatchesA(freqB, AmplB, PhaseB);
        bool BankBMatchCrystalB = MatchesB(freqB, AmplB, PhaseB);

        m_Amatched = (BankAMatchCrystalA || BankBMatchCrystalA);
        m_Bmatched = (BankAMatchCrystalB || BankBMatchCrystalB);

        if ((BankAMatchCrystalA && !BankBMatchCrystalA) ||
            (BankAMatchCrystalB && !BankBMatchCrystalB))
        {
            m_ledA.material = m_ledMatGreen;
        }
        else
        {
            m_ledA.material = m_ledMatRed;
        }

        if ((BankBMatchCrystalA && !BankAMatchCrystalA) ||
            (BankBMatchCrystalB && !BankAMatchCrystalB))
        {
            m_ledB.material = m_ledMatGreen;
        }
        else
        {
            m_ledB.material = m_ledMatRed;
        }
    }

    private bool MatchesA(float freq, float ampl, float phase)
    {
        return (Mathf.Abs(freq - m_currentCrystal.FreqA) < m_maxFreqDeviation &&
                Mathf.Abs(ampl - m_currentCrystal.AmplitudeA) < m_maxAmplDeviation &&
                Mathf.Abs(phase - m_currentCrystal.PhaseA) < m_maxPhaseDeviation);
    }

    private bool MatchesB(float freq, float ampl, float phase)
    {
        return (Mathf.Abs(freq - m_currentCrystal.FreqB) < m_maxFreqDeviation &&
                Mathf.Abs(ampl - m_currentCrystal.AmplitudeB) < m_maxAmplDeviation &&
                Mathf.Abs(phase - m_currentCrystal.PhaseB) < m_maxPhaseDeviation);
    }

    private Vector3 CalculatePoint(float position, float frequency, float amplitude, float phase)
    {
        var x = Mathf.Lerp(m_pointXMin, m_pointXMax, position);
        float y = Mathf.Lerp(m_pointYMin, m_pointYMax, (Mathf.Sin((phase + position * 2f) * frequency) + 1f) / 2f) * amplitude;
        return new Vector3(x, y, 0f);
    }

    public void OnCrystalSocket(XRBaseInteractable interactor)
    {
        var crystal = interactor.gameObject.GetComponent<Crystal>();
        if (crystal != null)
        {
            m_currentCrystal = crystal;
            m_hasCrystal = true;
            sourceA.pitch = Data.Instance.crystalSoundSettings[crystal.ID].pitchA;
            sourceB.pitch = Data.Instance.crystalSoundSettings[crystal.ID].pitchB;
            DrawScreen();
        }
        else
        {
            m_currentCrystal = null;
            m_hasCrystal = false;
            m_staticVolume = 0f;
            sourceA.volume = sourceB.volume = 0f;
            sourceBuzz.volume = 0f;
        }
    }

    public void OnCrystalUnsocket(XRBaseInteractable interactor)
    {
        m_currentCrystal = null;
        m_hasCrystal = false;
        m_staticVolume = 0f;
        sourceA.volume = sourceB.volume = 0f;
        sourceBuzz.volume = 0f;
    }
}
