using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

public class Data : MonoBehaviour
{
    private static Data instance;

    public static Data Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<Data>();
            }

            return instance;
        }
    }

    [System.Serializable]
    public struct CrystalSoundSettings
    {
        [SerializeField]
        public float pitchA;
        [SerializeField]
        public float pitchB;
    }

    [SerializeField]
    public CrystalSoundSettings[] crystalSoundSettings;
}
