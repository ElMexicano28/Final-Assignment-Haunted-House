using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public InputAction MoveAction;

    Animator m_Animator;

    public AudioClip powerupSound;
    public AudioClip powerdownSound;
    public GameObject audioPlayer;

    public float walkSpeed = 1.0f;
    public float turnSpeed = 20f;

    public bool hasShield;
    public GameObject shieldPrefab;

   
    public float hitCooldown = 1.5f;
    float m_LastHitTime = -Mathf.Infinity;

    Rigidbody m_Rigidbody;
    Vector3 m_Movement;
    Quaternion m_Rotation = Quaternion.identity;

    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        MoveAction.Enable();
        hasShield = false;

        m_Animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        var pos = MoveAction.ReadValue<Vector2>();

        float horizontal = pos.x;
        float vertical = pos.y;

        m_Movement.Set(horizontal, 0f, vertical);
        m_Movement.Normalize();

        bool hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
        bool hasVerticalInput = !Mathf.Approximately(vertical, 0f);
        bool isWalking = hasHorizontalInput || hasVerticalInput;
        m_Animator.SetBool("IsWalking", isWalking);

        if (isWalking && Input.GetKey(KeyCode.LeftShift))
        {
            walkSpeed = 2f;
            Vector3 desiredForward = Vector3.RotateTowards(transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0f);
            m_Rotation = Quaternion.LookRotation(desiredForward);

            m_Rigidbody.MoveRotation(m_Rotation);
            m_Rigidbody.MovePosition(m_Rigidbody.position + m_Movement * walkSpeed * Time.deltaTime);
        }
        else
        {
            walkSpeed = 1.0f;
            Vector3 desiredForward = Vector3.RotateTowards(transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0f);
            m_Rotation = Quaternion.LookRotation(desiredForward);

            m_Rigidbody.MoveRotation(m_Rotation);
            m_Rigidbody.MovePosition(m_Rigidbody.position + m_Movement * walkSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider whatDidIHit)
    {
        if (whatDidIHit.tag == "PowerUp")
        {
            Destroy(whatDidIHit.gameObject);
            //Picked up shield
            if (!hasShield)
            {
                hasShield = true;
                if (shieldPrefab != null)
                {
                    shieldPrefab.SetActive(true);
                }
                PlaySound(1);
            }
            
        }
    }

    //From fighter plane project
    public bool ConsumeShield()
    {
        if (!hasShield)
            return false;

        hasShield = false;
        if (shieldPrefab != null)
            shieldPrefab.SetActive(false);

        PlaySound(2);

        return true;
    }

    public bool TryRegisterHit()
    {
        // Ignore hits during cooldown
        if (Time.time - m_LastHitTime < hitCooldown)
            return false;

        // record time of this hit attempt
        m_LastHitTime = Time.time;

        // If player has shield, consume it
        if (hasShield)
        {
            ConsumeShield();
            return false;
        }

        // No shield and not in cooldown, hit should register
        return true;
    }

    public void PlaySound(int whichSound)
    {
        switch (whichSound)
        {
            case 1:
                audioPlayer.GetComponent<AudioSource>().PlayOneShot(powerupSound);
                break;
            case 2:
                audioPlayer.GetComponent<AudioSource>().PlayOneShot(powerdownSound);
                break;
        }
    }
}