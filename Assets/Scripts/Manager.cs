using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour {

    //Vector2 screenSize; //  画面のサイズ
    //Vector2 outerSize;  //  画面外のサイズ

    const float MARGIN = 5;

    Area screenArea;
    Area outerArea;

    Player player;

    public class Area
    {
        //(2017.12.13)[todo]  ↓この辺とかデバッグ用途なので、最終的にはRectで保持すれば良い
        //何回も同じものを書くよりは楽なので、残すのはありかも？
        public Vector3 lt, rb, rt, lb;

        public Bounds bounds;

        public Area(float w, float h)
        {
            lt = new Vector3(-w / 2, h / 2, 0);
            rb = new Vector3(w / 2, -h / 2, 0);
            rt = new Vector3(rb.x, lt.y, 0);
            lb = new Vector3(lt.x, rb.y, 0);

            bounds = new Bounds(Vector3.zero, new Vector3(w, h, 0));
        }
    }


    // Use this for initialization
    void Start()
    {
		updateArea();
        player = Player.Create(Vector3.zero);

        debugInit();

        StartCoroutine(spawnEnemy());
    }

    // Update is called once per frame
    void Update()
    {
        debugDisp();
    }


    IEnumerator spawnEnemy()
    {
        while(true)
        {
            yield return new WaitForSeconds(1);
            Enemy enemy = Enemy.Spawn(this);
            enemy.transform.SetParent(transform, false);
        }
    }



    void updateArea()
    {
        //  z=0の平面の画面サイズを更新
        Camera cam = Camera.main;

        float aspect = cam.aspect;
        float h = Mathf.Tan(cam.fieldOfView/2*Mathf.Deg2Rad) * (-cam.transform.position.z) * 2; //  カメラが引いた状態で計算するのでZの符号は反転
        float w = aspect * h;
        //screenSize = new Vector2(w, h);
        //outerSize = new Vector2(w + MARGIN * 2, h + MARGIN * 2);

        screenArea = new Area(w, h);
        outerArea = new Area(w+MARGIN*2, h+MARGIN*2);
    }

    public Player getPlayer()
    {
        return player;
    }

    public Bounds getScreenBounds()
    {
        return screenArea.bounds;
    }

    public Bounds getOuterBounds()
    {
        return outerArea.bounds;
    }


    void debugInit()
    {
        gameObject.AddComponent<MeshFilter>();
        MeshRenderer mr = gameObject.AddComponent<MeshRenderer>();
        mr.sharedMaterial = Resources.Load<Material>("Materials/ColorMaterial");
        MaterialPropertyBlock mb = new MaterialPropertyBlock();
        mb.SetColor("_Color", new Color(1, 1, 1, 1));
        mr.SetPropertyBlock(mb);
    }

    void debugDisp()
    {
		//	画面の外枠
        Debug.DrawLine(outerArea.lt, outerArea.rt, Color.red);
        Debug.DrawLine(outerArea.rt, outerArea.rb, Color.red);
        Debug.DrawLine(outerArea.rb, outerArea.lb, Color.red);
        Debug.DrawLine(outerArea.lb, outerArea.lt, Color.red);

		//	カメラに映る範囲
		/*
        Mesh mesh = new Mesh();
        mesh.vertices = new Vector3[] { screenArea.lt, screenArea.rt, screenArea.lb, screenArea.rb };
        mesh.colors = new Color[] { Color.red, Color.green, Color.blue, Color.yellow };
        mesh.triangles = new int[] { 0, 1, 2, 1, 3, 2 };
        GetComponent<MeshFilter>().mesh = mesh;
		*/
    }

}
