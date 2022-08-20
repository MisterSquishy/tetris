using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum DirectionKey
{
    Up,
    Down,
    Left,
    Right,
    None
}

public class Tetromino : MonoBehaviour
{
    Dictionary<DirectionKey, int> pressDurations = new Dictionary<DirectionKey, int>();

    void Start()
    {
        // Default position not valid? Then it's game over
        if (!isValidGridPos()) {
            Debug.Log("GAME OVER");
            Destroy(gameObject);
        }
    }

    DirectionKey GetPressedKey() {
       if (Input.GetKey(KeyCode.UpArrow)) {
            return DirectionKey.Up;
        } else if (Input.GetKey(KeyCode.RightArrow)) {
            return DirectionKey.Right;
        } else if (Input.GetKey(KeyCode.LeftArrow)) {
            return DirectionKey.Left;
        } else if (Input.GetKey(KeyCode.DownArrow)) {
            return DirectionKey.Down;
        }
        return DirectionKey.None;
    }

    void Update()
    {
        if (Time.frameCount % 3 != 0) {
            // 20fps
            return;
        }

        Action rollback = () => {};
        if (Time.frameCount % 60 == 0) {
            transform.position += new Vector3(0, -1, 0);
            rollback = () => {
                transform.position += new Vector3(0, 1, 0);
                Playfield.deleteFullRows();
                FindObjectOfType<Spawner>().spawnNext(); // fixme is it expensive to find this every time?
                enabled = false;
            };
        } else {
            DirectionKey pressedKey = GetPressedKey();
            switch(pressedKey) {
                case DirectionKey.Up:
                    transform.Rotate(0, 0, -90);
                    rollback = () => transform.Rotate(0, 0, 90);
                    break;
                case DirectionKey.Left:
                    transform.position += new Vector3(-1, 0, 0);
                    rollback = () => transform.position += new Vector3(1, 0, 0);
                    break;
                case DirectionKey.Right:
                    transform.position += new Vector3(1, 0, 0);
                    rollback = () => transform.position += new Vector3(-1, 0, 0);
                    break;
                case DirectionKey.Down:
                    transform.position += new Vector3(0, -1, 0);
                    rollback = () => {
                        transform.position += new Vector3(0, 1, 0);
                        Playfield.deleteFullRows();
                        FindObjectOfType<Spawner>().spawnNext(); // fixme is it expensive to find this every time?
                        enabled = false;
                    };
                    break;
            }
        }

        if (isValidGridPos()) {
            updateGrid();
        } else {
            rollback();
        }
    }

    bool isValidGridPos() {        
        foreach (Transform child in transform) {
            Vector2 v = Playfield.roundVec2(child.position);

            // Not inside Border?
            if (!Playfield.insideBorder(v))
                return false;

            // tetromino in grid cell (and not part of same group)?
            if (Playfield.grid[(int)v.x, (int)v.y] != null &&
                Playfield.grid[(int)v.x, (int)v.y].parent != transform)
                return false;
        }
        return true;
    }

    void updateGrid() {
        // Remove old children from grid
        for (int y = 0; y < Playfield.h; ++y)
            for (int x = 0; x < Playfield.w; ++x)
                if (Playfield.grid[x, y] != null)
                    if (Playfield.grid[x, y].parent == transform)
                        Playfield.grid[x, y] = null;

        // Add new children to grid
        foreach (Transform child in transform) {
            Vector2 v = Playfield.roundVec2(child.position);
            Playfield.grid[(int)v.x, (int)v.y] = child;
        }        
    }
}
