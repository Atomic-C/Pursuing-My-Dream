using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    // Enum that holds the direction the next room need to have a door
    public OpeningDirection oppositeDirection;

    // Timer to destroy the room spawn point, to optimize scene game objects ammount
    public float waitTime = 4f;

    // Int that controls the size of the dungeon
    public int roomQuantity = 8;

    // Class that holds all room types
    private RoomTemplates templates;

    // Int that holds the randomized number, used to instantiate the respective room type
    private int random;

    // Bool that prevents the spawning of duplicate rooms
    private bool spawned = false;

    private void Start()
    {
        // Destroy the spawn point attached to this code after the waitTime passes
        Destroy(gameObject, waitTime);

        // Grabs the room templates from the game object tagged with "Rooms"
        templates = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplates>();

        // Calls the Spawn function after the defined delay
        Invoke("Spawn", 0.1f);
    }

    // Function that spawns the next room
    void Spawn()
    {
        // Skip the code below if the room already spawned
        if (spawned == false)
        {
            switch (oppositeDirection)
            {
                case OpeningDirection.TOP:
                    // Need to spawn a room with a TOP door
                    random = Random.Range(0, templates.topRooms.Length);
                    Instantiate(templates.topRooms[random], transform.position, Quaternion.identity);
                    break;
                case OpeningDirection.BOTTOM:
                    // Need to spawn a room with a BOTTOM door
                    random = Random.Range(0, templates.bottomRooms.Length);
                    Instantiate(templates.bottomRooms[random], transform.position, Quaternion.identity);
                    break;
                case OpeningDirection.LEFT:
                    // Need to spawn a room with a LEFT door
                    random = Random.Range(0, templates.leftRooms.Length);
                    Instantiate(templates.leftRooms[random], transform.position, Quaternion.identity);
                    break;
                case OpeningDirection.RIGHT:
                    // Need to spawn a room with a RIGHT door
                    random = Random.Range(0, templates.rightRooms.Length);
                    Instantiate(templates.rightRooms[random], transform.position, Quaternion.identity);
                    break;
                case OpeningDirection.NONE:
                    break;
            }
            // Room already spawned
            spawned = true;
        }
    }

    // If a spawn point collides with another
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("SpawnPoint"))
         {
            
            if(other.GetComponent<RoomSpawner>().spawned == false && spawned == false)
            {
                Instantiate(templates.closedRoom, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
            spawned = true;
        }
    }

    public enum OpeningDirection
    {
        TOP,
        BOTTOM,
        LEFT,
        RIGHT,
        NONE
    }

}
