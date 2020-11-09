using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour {
	public float FloatAmplitude = .5f;
	public float FloatSpeed = .5f;

	private float m_originHeight;
	private bool m_goingUp = true;

    // Start is called before the first frame update
    void Start() {
		m_originHeight = transform.position.y;
    }

    // Update is called once per frame
    void Update() {
		// float up and down
        if (m_goingUp) {
			transform.Translate(Vector2.up * FloatSpeed * Time.deltaTime);
			if (transform.position.y - m_originHeight > FloatAmplitude)
				m_goingUp = false;
		}
		else {
			transform.Translate(Vector2.down * FloatSpeed * Time.deltaTime);
			if (m_originHeight - transform.position.y > FloatAmplitude)
				m_goingUp = true;
		}
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.tag == "Player" && GetCollected(collision))
			Destroy(gameObject);
	}

	// return true if the collectable should be destroyed
	protected virtual bool GetCollected(Collider2D collision) {
		return true;
	}
}
