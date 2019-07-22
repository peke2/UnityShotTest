using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {


    public static Player Create(Vector3 pos)
    {
        GameObject obj = Instantiate(Resources.Load<GameObject>("Prefabs/Player"));
        Player player;
        player = obj.AddComponent<Player>();
        player.transform.position = pos;

        return player;
    }


    const float MOVE_SPEED_SCALE = 0.5f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        move();
	}

    private void move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if( Mathf.Abs(h) < 0.15f ) h = 0;
        if( Mathf.Abs(v) < 0.15f ) v = 0;

        transform.position += new Vector3(h * MOVE_SPEED_SCALE, v * MOVE_SPEED_SCALE, 0);
    }

}
