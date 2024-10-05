using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    float hMove;
    float vMove;

    // Update is called once per frame
    void Update()
    {
        hMove = Input.GetAxis("Horizontal");
        vMove = Input.GetAxis("Vertical");
        Move(hMove, vMove, Input.GetKeyDown(KeyCode.Space));
    }

}
