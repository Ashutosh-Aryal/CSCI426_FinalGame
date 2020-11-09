using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour {
    [SerializeField] private List<Room> m_StartingRooms;
    [SerializeField] private List<Room> m_RandomRoomPool;
    
	private const float ROOM_CELL_WIDTH = 20f;
    
    private Queue<Room> m_RoomQueue;
    private Vector2 m_RoomSpawnPosition = Vector2.zero;

    // Keep reference to main(?) camera
    Camera mainCam;

    // Start is called before the first frame update
    void Start() {
        m_RoomQueue = new Queue<Room>();
        mainCam = Camera.main;

        // Create starting rooms
        for(int i = 0; i < m_StartingRooms.Count; i++) {
            Room room = Instantiate(m_StartingRooms[i], m_RoomSpawnPosition, Quaternion.identity);
            m_RoomSpawnPosition += Vector2.right * ROOM_CELL_WIDTH;
            m_RoomQueue.Enqueue(room);
        }
    }

    private void FixedUpdate() {
		// if there are less than 3 rooms then make sure that we spawn more
		while (m_RoomQueue.Count < 3) {
			CreateRoom();
		}

        // Get room closest to being destroyed
        Room room = m_RoomQueue.Peek();

        // Calculate distance from camera to see if it is time to destroy it; destroy if we are a room to the left of the camera
		if (room.transform.position.x < mainCam.transform.position.x) {
			float displacementSqr = (room.transform.position - mainCam.transform.position).sqrMagnitude;
			// basically adjusting for rooms with variable widths
			float deleteThresholdSqr = ((ROOM_CELL_WIDTH * room.NumUnits) / 2) + ROOM_CELL_WIDTH / 2;
			deleteThresholdSqr *= deleteThresholdSqr; // sqr the value
			if (displacementSqr > deleteThresholdSqr) {
				Room roomToDestroy = m_RoomQueue.Dequeue();
				Destroy(roomToDestroy.gameObject);
				CreateRoom();
			}
		}
    }

    void CreateRoom() {
        int randomIndex = Random.Range(0, m_RandomRoomPool.Count);
		Vector2 tempSpawnPosition = m_RoomSpawnPosition + (Vector2.right * (ROOM_CELL_WIDTH * ((m_RandomRoomPool[randomIndex].NumUnits / 2f) - 0.5f)));
		Room room = Instantiate(m_RandomRoomPool[randomIndex], tempSpawnPosition, Quaternion.identity);
		//Room room = Instantiate(m_RandomRoomPool[randomIndex], m_RoomSpawnPosition, Quaternion.identity);
		m_RoomQueue.Enqueue(room);
		m_RoomSpawnPosition += Vector2.right * ROOM_CELL_WIDTH * room.NumUnits;
		//m_RoomSpawnPosition += Vector2.right * ROOM_CELL_WIDTH;
	}
}
