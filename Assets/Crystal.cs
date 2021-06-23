using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal : MonoBehaviour
{
    [SerializeField]
    private float freqA, freqB;
    [SerializeField]
    private float amplA, amplB;
    [SerializeField]
    private float phaseA, phaseB;

    [SerializeField]
    private int id;

    public float FreqA => freqA;
    public float AmplitudeA => amplA;
    public float PhaseA => phaseA;

    public float FreqB => freqB;
    public float AmplitudeB => amplB;
    public float PhaseB => phaseB;

    public int ID => id;
}
