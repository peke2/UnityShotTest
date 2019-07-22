using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : EnemyBase {

    public static Enemy Spawn(Manager manager)
    {
        GameObject enemyBase = Instantiate(Resources.Load<GameObject>("Prefabs/Enemy"));

        Enemy enemy = enemyBase.AddComponent<Enemy>();
        enemy.init(manager);

        return enemy;
    }


    int duration;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += direction * speed;

        if( updateLife() )
        {
            duration += 1;
            if (duration == 60)
            {
                duration = 0;
                Shot.Spawn(manager, transform.position);
            }
        }
    }




    void init(Manager manager)
    {
        this.manager = manager;

        initPos();
        initDirection();

        speed = Random.Range(0.1f, 0.4f);
        duration = 0;
    }

    void initPos()
    {
        Bounds outerBounds = manager.getOuterBounds();
        Bounds screenBounds = manager.getScreenBounds();

        float x = Random.Range(outerBounds.min.x, outerBounds.max.x);
        float y = Random.Range(outerBounds.min.y, outerBounds.max.y);

        float range;
        float len;
        float offset;

        //  横か縦どちらかの座標を画面外領域にマッピング
        //  画面外の枠内で座標を生成する
        if (Random.Range(0, 2) == 0)
        {
            range = outerBounds.max.x - screenBounds.max.x;
            len = outerBounds.min.x - outerBounds.max.x;
            if (x >= 0) offset = screenBounds.max.x;
            else        offset = screenBounds.min.x;
            x = offset + x / (len / 2) * range;
        }
        else
        {
            range = outerBounds.max.y - screenBounds.max.y;
            len = outerBounds.max.y - outerBounds.min.y;
            if (y >= 0) offset = screenBounds.max.y;
            else        offset = screenBounds.min.y;
            y = offset + y / (len / 2) * range;
        }

        transform.position = new Vector3(x, y, 0);
    }

    void initDirection()
    {
        //  45°をベースに±15°
        float x, y;
        x = (0 > transform.position.x) ? 1 : -1;
        y = (0 > transform.position.y) ? 1 : -1;

        float rot = Random.Range(-15f, 15f) * Mathf.Deg2Rad;
        float rx, ry;

        rx = x * Mathf.Cos(rot) - y * Mathf.Sin(rot);
        ry = y * Mathf.Sin(rot) + y * Mathf.Cos(rot);

        direction = new Vector3(rx, ry, 0);
    }
}
