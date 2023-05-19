using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;//追加

public class MonsterMove : MonoBehaviour
{
    public float dist = 10; //視野の届く距離
    public float normalSpeed = 3.0f; //移動速度
    public float chaseSpeed = 5.1f;
    public float sightDegree = 45.0f; //片側の視野角
    public GameObject player; //プレイヤーのゲームオブジェクト
    public float xmin = -10.0f; //ステージのx座標の最小値
    public float xmax = 10.0f; //ステージのx座標の最大値
    public float zmin = -10.0f; //ステージのz座標の最小値
    public float zmax = 10.0f; //ステージのz座標の最大値
    public float pxmin;
    public float pxmax;
    public float pzmin;
    public float pzmax;
    public float bodyHeight = 1.7f; //体の大きさ
    public float eyeHeight = 1.2f; //視点の高さ
    public float swingSpeed = 2.5f; //左右を見渡すスピード(角度)
    public float fallY = -0.5f; //プレイヤーが落下したと判定する座標
    public float catchradius = 1.2f;
    public int stopCount = 1000;
    public float stopDist = 2.0f;
    public GameManager gameManager;
    NavMeshAgent agent;
    private Vector3 toPoint;
    private int state;//0:散策中、1:追跡中、2:見失い中、3:確保後
    private int loststate;//0:左から右設定、1:左から右回転、2:右から左設定、3:右から左回転
    private Vector3 frt;
    private Animator animator;//アニメーター
    private Vector3 prePosition;
    private int sameCount;
    Rigidbody rb;
    /*
     * 参考サイト
     * [Ray系]
     * https://qiita.com/nakabonne/items/e37313bfeab77b94210a
     * http://carving.roguelive.chicappa.jp/?eid=57
     * https://zenn.dev/daichi_gamedev/articles/76e21594ed15ca
     *
     * [NavMesh系]
     * https://nosystemnolife.com/enemy_ai_basic/
     * https://programming.sincoston.com/unity-how-to-use-navmesh-obstacle/
     */

    public class RayDistanceCompare : IComparer<RaycastHit>
    {
        public int Compare(RaycastHit x, RaycastHit y)
        {
            if (x.distance < y.distance)
            {
                return -1;
            }
            if (x.distance > y.distance)
            {
                return 1;
            }
            return 0;
        }
    }

    //レイを発射し、視覚にプレイヤーが写っているかを判断する関数
    bool isRayHit()
    {

        Vector3 dif = player.transform.position - new Vector3(this.transform.position.x, eyeHeight, this.transform.position.z);

        //レイを「this.transform.position(自分の目)」から「dif.normal(プレイヤーのいる方向)」に距離「dist」だけ発射し、当たったものを全て配列hitに記録する
        RaycastHit[] hit = Physics.RaycastAll(new Vector3(this.transform.position.x,eyeHeight,this.transform.position.z),dif.normalized,dist);

        if(hit.Length == 0)
        {
            //視野の届く範囲で何もなければfalseで即終了
            return false;
        }
        else if (hit.Length > 1)
        {
            //当たったものを、距離が近い順に並べ替える
            System.Array.Sort(hit, 0, hit.Length, new RayDistanceCompare());
        }

        //一番近いものについているタグが"Player"なら
        if (hit[0].collider.tag == "Player")
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //視野角内にプレイヤーがいるかを判定する関数(視界に写っているかどうかは分からない)
    bool isInSight()
    {
        float difDeg;

        //前方とプレイヤーの位置の間のなす各を求める
        difDeg = Vector3.Angle(this.transform.forward, Vector3.ProjectOnPlane(player.transform.position, Vector3.up) - Vector3.ProjectOnPlane(this.transform.position, Vector3.up));

        if(difDeg < sightDegree)
        {
            return isRayHit();
        }
        else
        {
            return false;
        }
    }

    //プレイヤーの追跡を開始する関数
    void setPointPlayer()
    {
        toPoint = new Vector3(player.transform.position.x, bodyHeight / 2, player.transform.position.z);
        agent.destination = toPoint;
        agent.speed = chaseSpeed;
        return;
    }

    //ランダムな目的地を生成する関数
    void changePoint()
    {
        float x, z;
        do
        {
            x = Random.Range(xmin, xmax);
            z = Random.Range(zmin, zmax);
        } while (((pxmin < x) && (x < pxmax)) && (pzmin < z) && (z < pzmax));
        toPoint = new Vector3(x, bodyHeight / 2, z);
        agent.destination = toPoint;
        agent.speed = normalSpeed;
        return;
    }

    //止まっているかをチェックする関数
    void stopCheck()
    {
        if (Vector3.Distance(prePosition, this.transform.position) < stopDist)
        {
            sameCount++;
            if (sameCount > stopCount)
            {
                if (state != 3)
                {
                    state = 0;
                }
                changePoint();
                sameCount = 0;
            }

        }
        else
        {
            prePosition = this.transform.position;
            sameCount = 0;
        }
        return;
    }

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        changePoint();
        agent.speed = normalSpeed;
        agent.destination = toPoint;
        state = 0;
        loststate = 0;
        prePosition = this.transform.position;
        sameCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(player.transform.position.y < fallY)
        {
            state = 3;
        }
        switch (state)
        {
            case 0:
                animator.SetInteger("state", 1);
                //Debug.Log("State 0");
                if (isInSight())
                {
                    //視覚内にプレイヤーがいるときの処理
                    setPointPlayer();
                    state = 1;
                }
                else
                {
                    //ランダムな目的地付近に到着したら
                    if (Vector3.Distance(this.transform.position, toPoint) < 5.0f)
                    {
                        //新しいランダムな目的地を設定する
                        changePoint();
                    }
                }
                break;
            case 1:
                animator.SetInteger("state", 2);
                //もし確保距離になったら
                if (Vector3.Distance(this.transform.position, new Vector3(toPoint.x, this.transform.position.y, toPoint.z)) < catchradius)
                {
                    //プレイヤーを視認できたら
                    if (isInSight())
                    {
                        state = 3;
                        gameManager.Attacked();
                        changePoint();
                    }
                    else //見失ったら
                    {
                        state = 2;
                    }
                }
                else
                {
                    //引き続き視野内に捉えていたら
                    if (isInSight())
                    {
                        setPointPlayer();
                    }
                }
                break;
            case 2:
                animator.SetInteger("state", 0);
                if (isInSight()) //見渡してプレイヤーが見つかったら
                {
                    setPointPlayer();
                    state = 1;
                }
                else
                {
                    switch (loststate)
                    {
                        case 0: //基準の向きを決める
                            frt = this.transform.forward;
                            rb.angularVelocity = new Vector3(0.0f, swingSpeed, 0.0f);
                            loststate = 1;
                            break;
                        case 1: //左を向く
                            if (Vector3.SignedAngle(frt, this.transform.forward, this.transform.up) >= 90.0f)
                            {
                                rb.angularVelocity = new Vector3(0.0f, -swingSpeed, 0.0f);
                                loststate = 2;
                            }
                            else
                            {
                                rb.angularVelocity = new Vector3(0.0f, swingSpeed, 0.0f);
                            }
                            break;
                        case 2: //右を向く
                            if (Vector3.SignedAngle(frt, this.transform.forward, this.transform.up) <= -90.0f)
                            {
                                rb.angularVelocity = new Vector3(0.0f, 0.0f, 0.0f);
                                loststate = 0;
                                state = 0;
                            }
                            else
                            {
                                rb.angularVelocity = new Vector3(0.0f, -swingSpeed, 0.0f);
                            }
                            break;
                    }
                }
                break;
            case 3: //確保したら
                animator.SetInteger("state", 1);
                if (Vector3.Distance(this.transform.position, toPoint) < 5.0f)
                {
                    //新しいランダムな目的地を設定する
                    changePoint();
                }
                break;
        }
        stopCheck();
    }
}
