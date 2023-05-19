using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ItemDistance : MonoBehaviour
{
    public GameObject player;
    public float dowsingRadius; //�ǉ� : �_�E�W���O�Ɉ��������鋗����public�ϐ���
    public float getRadius; //�ǉ� : �l���ł��鋗����public�ϐ���
    private GameObject MMObj; //�ύX : private�ő��ϐ���
    private MapManager mapmanagerscript; //�ύX : private�ő��ϐ���
    private GameObject PMObj; //�ǉ� : PocketManager�p�̃Q�[���I�u�W�F�N�g�^�ϐ�
    private PocketManager pocketmanagerscript; //�ǉ� : PocketManager.cs�p�̃X�N���v�g�ϐ�
    private DowsingExecutor dowsingExecutor;

    // Start is called before the first frame update
    /*
     * �Q�l�T�C�g
     * https://futabazemi.net/unity/script_var/
     * https://qiita.com/Akarinnn/items/00f58b7cbc7c5d659d92
     * https://futabazemi.net/notes/script_function/
     * https://chnr.hatenablog.com/entry/2015/02/04/093853
     */

    void Start()
    {
        MMObj = GameObject.Find("MapManager"); //�ύX : Start()�Őݒ�
        mapmanagerscript = MMObj.GetComponent<MapManager>(); //�ύX : Start()�Őݒ�
        PMObj = GameObject.Find("PocketManager"); //�ύX : Start()�Őݒ�
        pocketmanagerscript = PMObj.GetComponent<PocketManager>(); //�ύX : Start()�Őݒ�
        dowsingExecutor = GameObject.Find("XR Origin").GetComponent<DowsingExecutor>();
    }

    // Update is called once per frame
    void Update()
    {
        int i;
        Transform[] lessthan = new Transform[50]; //�ύX : �z��̃T�C�Y��50��
        Transform[] between = new Transform[50]; //�ύX : �z��̃T�C�Y��50��
        Transform[] morethan = new Transform[50]; //�ύX : �z��̃T�C�Y��50��

        for (i = 0; i < mapmanagerscript.ItemList.Length; i++) //�ύX : ���[�v�͈͂�Itemlist[]�̃T�C�Y�ɍ��킹��
        {
            lessthan[i] = null;
            between[i] = null;
            morethan[i] = null;
        }

        int l, b, m;
        float dist;

        for (i = 0,l = -1,b = -1,m = -1; i < mapmanagerscript.ItemList.Length; i++)//�ύX : ���[�v�͈͂�Itemlist[]�̃T�C�Y�ɍ��킹��
        {
            if(mapmanagerscript.ItemList[i] == null)
            {
                continue;
            }
            dist = Vector3.Distance(mapmanagerscript.ItemList[i].transform.position, player.transform.position);
            if (dist < getRadius) //�ύX : ���������public�ϐ��ōs��
            {
                l++;
                lessthan[l] = mapmanagerscript.ItemList[i].transform;

                pocketmanagerscript.PickUp(mapmanagerscript.ItemList[i].GetComponent<ItemManager>().taken()); //�ǉ� : taken()���Ăяo���A�߂�l��pickup()�ɓ�����
                Destroy(mapmanagerscript.ItemList[i]);//�ύX : �l���A�C�e����Destroy()
            }
            else if(dist < dowsingRadius) //�ύX : ���������public�ϐ��ōs��
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
