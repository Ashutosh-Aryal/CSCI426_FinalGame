using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour {

	[HideInInspector] public float m_HorizontalMovementSpeed = 0f;

	private bool m_HasCameraStopped = false;

    // Update is called once per frame
    void Update() {

		if (m_HasCameraStopped)
			return;
		
		transform.Translate(m_HorizontalMovementSpeed * Time.deltaTime, 0, 0);

		bool isLavaHorizontalPositionPastCamera = transform.position.x >= Camera.main.transform.position.x;

		if (isLavaHorizontalPositionPastCamera)
			m_HasCameraStopped = true;
		
	}

    private void OnTriggerEnter2D(Collider2D collision) {
        
		if (collision.tag == "Enemy") {
            Destroy(collision.gameObject);
        } else if (collision.gameObject.tag == "Player") {
			collision.gameObject.GetComponent<PlayerController>().KillPlayer();
		}
    }
}
