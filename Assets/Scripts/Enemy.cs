﻿using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Rigidbody2D m_Rigidbody2D;

    private bool m_IsAlive = true;
    private bool m_InAir = false;
	private bool m_OnScreen = false;

    [SerializeField] private Vector2 m_Velocity;
    [SerializeField] private AudioSource m_DeathSFX;

    SpriteRenderer m_SpriteRenderer;
    BoxCollider2D m_BoxCollider2D;
    [SerializeField] Material m_DeathMaterial;
    [SerializeField] float m_TimeTillDeath = .8f;
    float m_CurrDeathTime = 0f;
    bool m_IsDying = false;
    Vector3 m_LastPositionAlive;

    // Start is called before the first frame update
    void Start() {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_BoxCollider2D = GetComponent<BoxCollider2D>();
        m_CurrDeathTime = 0f;
    }

    // Update is called once per frame
    void Update() {
        if(m_IsDying)
        {
            transform.position = m_LastPositionAlive;
            m_CurrDeathTime += Time.deltaTime;
            float dissolveAmount = Mathf.Lerp(3.5f, -2.5f, m_CurrDeathTime / m_TimeTillDeath);
            Debug.Log("dissolve amount: " + dissolveAmount);
            m_DeathMaterial.SetFloat("_DissolveAmount", dissolveAmount);
            if(m_CurrDeathTime >= m_TimeTillDeath)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            // dont do anything until on screen
            if (!m_OnScreen)
                return;
            m_Rigidbody2D.velocity = m_Velocity;

            // cleanup any enemies that may have fallen off level
            if (transform.position.y < Camera.main.transform.position.y - RoomGenerator.ROOM_CELL_WIDTH)
                Destroy(gameObject);
        }

    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if(!m_IsDying)
        {
            // if hit by sword, kill enemy
            if (collision.gameObject.tag == "Sword")
            {

                KillSelf();
                PlayerController playerController = collision.gameObject.GetComponentInParent<PlayerController>();
                playerController.ProcessKill();
                return;
            }

            Vector2 normal = collision.contacts[0].normal;

            bool shouldReverseHorizontalVelocity = !Mathf.Approximately(normal.x, 0.0f) || collision.gameObject.tag == "Enemy";

            if (shouldReverseHorizontalVelocity)
            {
                m_Velocity.x *= -1f; return;
            }

            bool hasTouchedGround = collision.gameObject.tag == "Ground" || normal.y > 0f;

            if (hasTouchedGround)
            {
                m_InAir = false;
            }
        }

    }

	// handle the death; sounds and/or animations here
	public void KillSelf() {
		
        if(m_DeathSFX)
            m_DeathSFX.Play();

        m_IsDying = true;
        m_SpriteRenderer.material = m_DeathMaterial;
        m_DeathMaterial.SetFloat("_DissolveAmount", 3.5f);
        m_BoxCollider2D.enabled = false;
        m_LastPositionAlive = transform.position;
        //Destroy(gameObject);
	}

	// activate self when on screen
	private void OnBecameVisible() {
		m_OnScreen = true;
	}
}
