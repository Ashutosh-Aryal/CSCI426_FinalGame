﻿using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraController : MonoBehaviour {

	[SerializeField] private GameObject[] m_ButtonObjects;
    [SerializeField] private GameObject m_CanvasObject;

    [SerializeField] private RectTransform m_TimeTextRectTransform;
    [SerializeField] private RectTransform m_TimeTextRectGoalTransform;

	[SerializeField] private float m_TextSpeed = 4f;
    [SerializeField] private float m_HorizontalMovementSpeed = 1f;
    [SerializeField] private float m_HorizontalAcceleration = 0.1f;

	[SerializeField] private RoomGenerator m_RoomGen;
	[SerializeField] private Lava m_Lava;

	private const float MAX_HORIZONTAL_SPEED = 4.8f;

    private bool m_ShowingFinalScore = false;
    private bool m_IsPlayerDead = false;

    // Update is called once per frame
    void Update() {
		if (m_IsPlayerDead && Input.GetKeyDown(KeyCode.R))
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);

		m_HorizontalMovementSpeed = Mathf.Clamp(m_HorizontalMovementSpeed, 0.0f, MAX_HORIZONTAL_SPEED);

		if (!m_IsPlayerDead) {
			transform.Translate(m_HorizontalMovementSpeed * Time.deltaTime, 0, 0);
			m_HorizontalMovementSpeed += m_HorizontalAcceleration * Time.deltaTime;
		}
		else {
			var lava = GetComponentInChildren<Lava>();
			if (lava == null) return;
			lava.transform.parent = null;
			lava.m_HorizontalMovementSpeed = Mathf.Max(m_HorizontalMovementSpeed, 10f);
		}
	}

	// shows death screen
	public void ShowFinalScore() {

		m_IsPlayerDead = true;
		m_CanvasObject.GetComponent<UI>().StopTimer();
		StartCoroutine("CoShowFinalScore");
		Debug.Log("Final score shown");
	}

	IEnumerator CoShowFinalScore() {

		if (m_ShowingFinalScore) { yield break; }
		m_ShowingFinalScore = true;
		
		while (true) {
			m_TimeTextRectTransform.anchoredPosition = Vector2.Lerp(m_TimeTextRectTransform.anchoredPosition, m_TimeTextRectGoalTransform.anchoredPosition, m_TextSpeed * Time.deltaTime);
			/*new Vector2(Mathf.Lerp(m_TimeTextRectTransform.anchoredPosition.x, m_TimeTextRectGoalTransform.anchoredPosition.x, m_TextSpeed * Time.deltaTime),
												Mathf.Lerp(m_TimeTextRectTransform.anchoredPosition.y, m_TimeTextRectGoalTransform.anchoredPosition.y, m_TextSpeed * Time.deltaTime));*/

			if (Vector2.SqrMagnitude(m_TimeTextRectTransform.anchoredPosition - m_TimeTextRectGoalTransform.anchoredPosition) < .1){
				break;
			}

			yield return null;
		}

		foreach (var butt in m_ButtonObjects)
			butt.SetActive(true);

		yield break;
	}

	IEnumerator CoFadeOutMusic() {
		yield break;
	}

	public void SlowDown(float SlowPercentage) { 
		m_HorizontalMovementSpeed *= 1 - SlowPercentage;
		if (m_RoomGen)
			m_RoomGen.MarkSlowdown();
		// show lava effect
		if (m_Lava)
			m_Lava.EnableSlowdownEffect();
	}
}
