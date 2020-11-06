using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    [SerializeField] private List<Room> m_StartingRooms;
    [SerializeField] private List<Room> m_RandomRoomPool;
    [SerializeField] private float m_RoomSize = 20f;
    
    private Queue<Room> m_RoomQueue;
    private Vector2 m_RoomSpawnPosition = Vector2.zero;

    //Keep reference to main(?) camera
    Camera mainCam;

    // Start is called before the first frame update
    void Start()
    {
        m_RoomQueue = new Queue<Room>();
        mainCam = Camera.main;

        //Create starting rooms
        for(int i = 0; i < m_StartingRooms.Count; i++)
        {
            Room room = Instantiate(m_StartingRooms[i], m_RoomSpawnPosition, Quaternion.identity);
            m_RoomSpawnPosition += Vector2.right * m_RoomSize;
            m_RoomQueue.Enqueue(room);
        }

    }

    private void FixedUpdate()
    {
        //Get top room
        Room room = m_RoomQueue.Peek();

        //Calculate top room's position against camera
        Vector2 displacement = room.transform.position - mainCam.transform.position;
        if (displacement.magnitude >= m_RoomSize) //Good enough to get rid off from m_RoomQueue
        {
            Room roomToDestroy = m_RoomQueue.Dequeue();
            Destroy(roomToDestroy.gameObject);
            CreateRoom();
        }
    }

    void CreateRoom()
    {
        int randomIndex = Random.Range(0, m_RandomRoomPool.Count);
        Room room = Instantiate(m_RandomRoomPool[randomIndex], m_RoomSpawnPosition, Quaternion.identity);
        m_RoomQueue.Enqueue(room);
        m_RoomSpawnPosition += Vector2.right * m_RoomSize;
    }
}
