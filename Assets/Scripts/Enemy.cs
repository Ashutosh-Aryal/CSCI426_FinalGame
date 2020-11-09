using System.Collections;
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

    // Start is called before the first frame update
    void Start() {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update() {
		// dont do anything until on screen
		if (!m_OnScreen)
			return;
        m_Rigidbody2D.velocity = m_Velocity;

		// cleanup any enemies that may have fallen off level
		if (transform.position.y < Camera.main.transform.position.y - RoomGenerator.ROOM_CELL_WIDTH)
			Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision) {

		// if hit by sword, kill enemy
		if (collision.gameObject.tag == "Sword") {

            KillSelf();
            PlayerController playerController = collision.gameObject.GetComponentInParent<PlayerController>();
            playerController.ProcessKill(); 
            return;
		}

		Vector2 normal = collision.contacts[0].normal;

        bool shouldReverseHorizontalVelocity = !Mathf.Approximately(normal.x, 0.0f) || collision.gameObject.tag == "Enemy";

        if (shouldReverseHorizontalVelocity) {
            m_Velocity.x *= -1f; return;
        }

        bool hasTouchedGround = collision.gameObject.tag == "Ground" || normal.y > 0f;

        if(hasTouchedGround) {
            m_InAir = false;
        }
    }

	// handle the death; sounds and/or animations here
	public void KillSelf() {
		
        if(m_DeathSFX)
            m_DeathSFX.Play();
     	
        Destroy(gameObject);
	}

	// activate self when on screen
	private void OnBecameVisible() {
		m_OnScreen = true;
	}
}
