using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;//�ǉ�

public class MonsterMove : MonoBehaviour
{
    public float dist = 10; //����̓͂�����
    public float normalSpeed = 3.0f; //�ړ����x
    public float chaseSpeed = 5.1f;
    public float sightDegree = 45.0f; //�Б��̎���p
    public GameObject player; //�v���C���[�̃Q�[���I�u�W�F�N�g
    public float xmin = -10.0f; //�X�e�[�W��x���W�̍ŏ��l
    public float xmax = 10.0f; //�X�e�[�W��x���W�̍ő�l
    public float zmin = -10.0f; //�X�e�[�W��z���W�̍ŏ��l
    public float zmax = 10.0f; //�X�e�[�W��z���W�̍ő�l
    public float pxmin;
    public float pxmax;
    public float pzmin;
    public float pzmax;
    public float bodyHeight = 1.7f; //�̂̑傫��
    public float eyeHeight = 1.2f; //���_�̍���
    public float swingSpeed = 2.5f; //���E�����n���X�s�[�h(�p�x)
    public float fallY = -0.5f; //�v���C���[�����������Ɣ��肷����W
    public float catchradius = 1.2f;
    public int stopCount = 1000;
    public float stopDist = 2.0f;
    public GameManager gameManager;
    NavMeshAgent agent;
    private Vector3 toPoint;
    private int state;//0:�U�����A1:�ǐՒ��A2:���������A3:�m�ی�
    private int loststate;//0:������E�ݒ�A1:������E��]�A2:�E���獶�ݒ�A3:�E���獶��]
    private Vector3 frt;
    private Animator animator;//�A�j���[�^�[
    private Vector3 prePosition;
    private int sameCount;
    Rigidbody rb;
    /*
     * �Q�l�T�C�g
     * [Ray�n]
     * https://qiita.com/nakabonne/items/e37313bfeab77b94210a
     * http://carving.roguelive.chicappa.jp/?eid=57
     * https://zenn.dev/daichi_gamedev/articles/76e21594ed15ca
     *
     * [NavMesh�n]
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

    //���C�𔭎˂��A���o�Ƀv���C���[���ʂ��Ă��邩�𔻒f����֐�
    bool isRayHit()
    {

        Vector3 dif = player.transform.position - new Vector3(this.transform.position.x, eyeHeight, this.transform.position.z);

        //���C���uthis.transform.position(�����̖�)�v����udif.normal(�v���C���[�̂������)�v�ɋ����udist�v�������˂��A�����������̂�S�Ĕz��hit�ɋL�^����
        RaycastHit[] hit = Physics.RaycastAll(new Vector3(this.transform.position.x,eyeHeight,this.transform.position.z),dif.normalized,dist);

        if(hit.Length == 0)
        {
            //����̓͂��͈͂ŉ����Ȃ����false�ő��I��
            return false;
        }
        else if (hit.Length > 1)
        {
            //�����������̂��A�������߂����ɕ��בւ���
            System.Array.Sort(hit, 0, hit.Length, new RayDistanceCompare());
        }

        //��ԋ߂����̂ɂ��Ă���^�O��"Player"�Ȃ�
        if (hit[0].collider.tag == "Player")
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //����p���Ƀv���C���[�����邩�𔻒肷��֐�(���E�Ɏʂ��Ă��邩�ǂ����͕�����Ȃ�)
    bool isInSight()
    {
        float difDeg;

        //�O���ƃv���C���[�̈ʒu�̊Ԃ̂Ȃ��e�����߂�
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

    //�v���C���[�̒ǐՂ��J�n����֐�
    void setPointPlayer()
    {
        toPoint = new Vector3(player.transform.position.x, bodyHeight / 2, player.transform.position.z);
        agent.destination = toPoint;
        agent.speed = chaseSpeed;
        return;
    }

    //�����_���ȖړI�n�𐶐�����֐�
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

    //�~�܂��Ă��邩���`�F�b�N����֐�
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
                    //���o���Ƀv���C���[������Ƃ��̏���
                    setPointPlayer();
                    state = 1;
                }
                else
                {
                    //�����_���ȖړI�n�t�߂ɓ���������
                    if (Vector3.Distance(this.transform.position, toPoint) < 5.0f)
                    {
                        //�V���������_���ȖړI�n��ݒ肷��
                        changePoint();
                    }
                }
                break;
            case 1:
                animator.SetInteger("state", 2);
                //�����m�ۋ����ɂȂ�����
                if (Vector3.Distance(this.transform.position, new Vector3(toPoint.x, this.transform.position.y, toPoint.z)) < catchradius)
                {
                    //�v���C���[�����F�ł�����
                    if (isInSight())
                    {
                        state = 3;
                        gameManager.Attacked();
                        changePoint();
                    }
                    else //����������
                    {
                        state = 2;
                    }
                }
                else
                {
                    //��������������ɑ����Ă�����
                    if (isInSight())
                    {
                        setPointPlayer();
                    }
                }
                break;
            case 2:
                animator.SetInteger("state", 0);
                if (isInSight()) //���n���ăv���C���[������������
                {
                    setPointPlayer();
                    state = 1;
                }
                else
                {
                    switch (loststate)
                    {
                        case 0: //��̌��������߂�
                            frt = this.transform.forward;
                            rb.angularVelocity = new Vector3(0.0f, swingSpeed, 0.0f);
                            loststate = 1;
                            break;
                        case 1: //��������
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
                        case 2: //�E������
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
            case 3: //�m�ۂ�����
                animator.SetInteger("state", 1);
                if (Vector3.Distance(this.transform.position, toPoint) < 5.0f)
                {
                    //�V���������_���ȖړI�n��ݒ肷��
                    changePoint();
                }
                break;
        }
        stopCheck();
    }
}
