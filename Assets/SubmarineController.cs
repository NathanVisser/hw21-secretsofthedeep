using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class SubmarineController : MonoBehaviour
{
    [SerializeField]
    private Rigidbody m_rigidbody;

    [SerializeField]
    private Transform m_subBody;

    private AxisDragInteractable m_speedLever;
    private DialInteractable m_levelLever;
    private DialInteractable m_directionLever;

    [SerializeField]
    private float m_elevationForce = 100f;
    [SerializeField]
    private float m_thrustForce = 100f;
    [SerializeField]
    private float m_turnForce = 100f;
    
    private float m_elevationAmount = 0f;
    private float m_thrustAmount = 0f;
    private float m_turnAmount = 0f;

    private bool m_active = false;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DelayedStart());
    }

    private IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(2f);
        m_rigidbody.isKinematic = false;
        GetComponent<CapsuleCollider>().enabled = true;
        m_active = true;
    }


    void FixedUpdate()
    {
        m_subBody.position = transform.position;
        m_subBody.rotation = transform.rotation;
    }

    public void Unlock()
    {
        StartCoroutine(DelayedStart());

    }

    // Update is called once per frame
    void Update()
    {
        if(!m_active)
            return;
        

        //m_thrustAmount = Input.GetAxis("Vertical");
        //m_turnAmount = Input.GetAxis("Horizontal");

        if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            m_elevationAmount += 1f;
        }
        if (Input.GetKeyUp(KeyCode.Keypad8))
        {
            m_elevationAmount -= 1f;
        }

        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            m_elevationAmount -= 1f;
        }
        if (Input.GetKeyUp(KeyCode.Keypad2))
        {
            m_elevationAmount += 1f;
        }

        m_rigidbody.AddRelativeForce(new Vector3(-m_thrustAmount * m_thrustForce, m_elevationAmount * m_elevationForce) * Time.deltaTime);
        m_rigidbody.AddRelativeTorque(new Vector3(0f, m_turnAmount * m_turnForce, 0f) * Time.deltaTime);
    }

    public void SetSpeed(int value)
    {
        m_thrustAmount = Mathf.Clamp((float) value / 10f - 1f, -1f, 1f);
        Debug.Log("Speed: " + value + " total: " + m_thrustAmount);
    }

    public void SetElevation(int value)
    {
        m_elevationAmount = Mathf.Clamp((float) value - 1f, -1f, 1f);
        Debug.Log("Elevation: " + value + " total: " + m_elevationAmount);

    }

    public void SetSteering(float value)
    {

    }
}
