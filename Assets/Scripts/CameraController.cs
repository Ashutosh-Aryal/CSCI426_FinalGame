using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraController : MonoBehaviour {

	[SerializeField] private GameObject[] m_ButtonObjects;
    [SerializeField] private GameObject m_CanvasObject;

    [SerializeField] private RectTransform m_TimeTextRectTransform;

	[SerializeField] private Vector2 m_TargetTextPosition;

	[SerializeField] private float m_TextSpeed = 4f;
    [SerializeField] private float m_HorizontalMovementSpeed = 1f;
    [SerializeField] private float m_HorizontalAcceleration = 0.1f;

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
			m_TimeTextRectTransform.position = new Vector2(Mathf.Lerp(m_TimeTextRectTransform.position.x, m_TargetTextPosition.x, m_TextSpeed * Time.deltaTime),
												Mathf.Lerp(m_TimeTextRectTransform.position.y, m_TargetTextPosition.y, m_TextSpeed * Time.deltaTime));

			if (Vector3.SqrMagnitude(m_TimeTextRectTransform.position - new Vector3(m_TargetTextPosition.x, m_TargetTextPosition.y)) < .1){
				break;
			}

			yield return null;
		}

		foreach (var butt in m_ButtonObjects)
			butt.SetActive(true);

		yield break;
	}
}
