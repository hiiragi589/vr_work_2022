using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ItemDistance : MonoBehaviour
{
    public GameObject player;
    public float dowsingRadius; //追加 : ダウジングに引っかかる距離をpublic変数化
    public float getRadius; //追加 : 獲得できる距離をpublic変数化
    private GameObject MMObj; //変更 : privateで大域変数化
    private MapManager mapmanagerscript; //変更 : privateで大域変数化
    private GameObject PMObj; //追加 : PocketManager用のゲームオブジェクト型変数
    private PocketManager pocketmanagerscript; //追加 : PocketManager.cs用のスクリプト変数
    private DowsingExecutor dowsingExecutor;

    // Start is called before the first frame update
    /*
     * 参考サイト
     * https://futabazemi.net/unity/script_var/
     * https://qiita.com/Akarinnn/items/00f58b7cbc7c5d659d92
     * https://futabazemi.net/notes/script_function/
     * https://chnr.hatenablog.com/entry/2015/02/04/093853
     */

    void Start()
    {
        MMObj = GameObject.Find("MapManager"); //変更 : Start()で設定
        mapmanagerscript = MMObj.GetComponent<MapManager>(); //変更 : Start()で設定
        PMObj = GameObject.Find("PocketManager"); //変更 : Start()で設定
        pocketmanagerscript = PMObj.GetComponent<PocketManager>(); //変更 : Start()で設定
        dowsingExecutor = GameObject.Find("XR Origin").GetComponent<DowsingExecutor>();
    }

    // Update is called once per frame
    void Update()
    {
        int i;
        Transform[] lessthan = new Transform[50]; //変更 : 配列のサイズを50に
        Transform[] between = new Transform[50]; //変更 : 配列のサイズを50に
        Transform[] morethan = new Transform[50]; //変更 : 配列のサイズを50に

        for (i = 0; i < mapmanagerscript.ItemList.Length; i++) //変更 : ループ範囲をItemlist[]のサイズに合わせる
        {
            lessthan[i] = null;
            between[i] = null;
            morethan[i] = null;
        }

        int l, b, m;
        float dist;

        for (i = 0,l = -1,b = -1,m = -1; i < mapmanagerscript.ItemList.Length; i++)//変更 : ループ範囲をItemlist[]のサイズに合わせる
        {
            if(mapmanagerscript.ItemList[i] == null)
            {
                continue;
            }
            dist = Vector3.Distance(mapmanagerscript.ItemList[i].transform.position, player.transform.position);
            if (dist < getRadius) //変更 : 距離判定をpublic変数で行う
            {
                l++;
                lessthan[l] = mapmanagerscript.ItemList[i].transform;

                pocketmanagerscript.PickUp(mapmanagerscript.ItemList[i].GetComponent<ItemManager>().taken()); //追加 : taken()を呼び出し、戻り値をpickup()に投げる
                Destroy(mapmanagerscript.ItemList[i]);//変更 : 獲得アイテムをDestroy()
            }
            else if(dist < dowsingRadius) //変更 : 距離判定をpublic変数で行う
            {
                b++;
                between[b] = mapmanagerscript.ItemList[i].transform;
            }
            else
            {
                m++;
                morethan[m] = mapmanagerscript.ItemList[i].transform;
            }
        }
        dowsingExecutor.DowsingHaptic(between);
    }
}
