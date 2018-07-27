using UnityEngine;
using System.Collections;

public class L2DPlayerController : MonoBehaviour {

    private Transform player;
    public float speed = 15f;

    bool isDragging = false;
    Vector2 offset;

    // Use this for initialization
    void Start() {
        player = this.transform;
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            offset = Camera.main.ScreenToWorldPoint(Input.mousePosition) - player.position;
            isDragging = true;

            // Released the touch screen
        }
        else if (Input.GetMouseButtonUp(0)) {
            offset = Vector2.zero;
            isDragging = false;
        }

        if (isDragging) {
            Vector2 destination = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - offset;
            player.position = Vector2.Lerp(player.position, destination, speed * Time.deltaTime);
        }
    }
}
