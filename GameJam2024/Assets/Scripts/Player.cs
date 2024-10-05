using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Character chara;
    float hMove;
    float vMove;


    private void Awake()
    {
        chara = GetComponent<Character>();
    }

    // Update is called once per frame
    void Update()
    {
        hMove = Input.GetAxis("Horizontal");
        vMove = Input.GetAxis("Vertical");
        chara.Move(hMove, vMove, Input.GetKeyDown(KeyCode.Space));
    }

}
