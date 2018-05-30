using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 테스트용 코드


struct st
{
    public int a;
    public float b;
    public st(int a, float b)
    {
        this.a = a;
        this.b = b;
    }
};

class cl
{
    public int a;
    float b;
    public cl(int a, float b)
    {
        this.a = a;
        this.b = b;
    }
}

/*
 * 함수 원형 (리턴, 매개변수)이 같으면
 * 다른 함수여도 같은 델리게이트 이벤트에 등록 가능
 * void test(), void test2() 처럼
 */
public class TestScript : MonoBehaviour {

    private static TestScript instance;

    public delegate void testDelegate(int num);
    public delegate float a();
    public a c;
    public event testDelegate BulletProperty;
    public testDelegate t1;

    public float degree;
    public float GetDir() { return degree; }


    public GameObject contactPointObj;

    private st s1;
    private st s2;
    private st s3;
    private List<st> stList;

    private cl c1;
    private cl c2;
    private cl c3;
    private List<cl> clList;

    public GameObject effectObj;

    public void TestPointerfunc(a b)
    {
        c = b;
    }

    public static TestScript Instance { get { return instance; } }


    void Awake()
    {

        if (instance == null)
            instance = this;
        else if (instance != null)
            Destroy(gameObject);
    }

    // Use this for initialization
    void Start () {


        //for(int i = 0; i < 100; i++)
        //{
        //    Debug.Log(Random.Range(0f, 1f));
        //}


        //ListTest();
        //Debug.Log(Abc(2));
        //Debug.Log(Abc(2f));
        degree = 1.5f;
        TestPointerfunc(GetDir);
        //Debug.Log("함수 넘기기 : " + c()); // 1.5
        //BulletProperty = test;

        // 델리게이트 테스트
        /*
        BulletProperty = TestStr;
        BulletProperty = Test;
        BulletProperty += TestStr;
        BulletProperty += Test;

        BulletProperty(10);
        BulletProperty(15);

        Bridge : 10
        B : 10
        Bridge : 10
        Bridge : 15
        B : 15
        Bridge : 15
        */
        //t1 += TestStr;
        TestInhrt();

        stList = new List<st>
        {
            new st(1, 1.5f),
            new st(2, 2.5f),
            new st(3, 3.5f)
        };

        clList = new List<cl>
        {
            new cl(10, 10.5f),
            new cl(20, 20.5f),
            new cl(30, 30.5f)
        };

        s1 = stList[0];
        s2 = stList[0];
        s1.a += 2;
        //stList[1].a += 3; 에러, list 이용해서 Struct 관리하게 되면 
        // struct의 각각의 값에 직접적인 접근 불가능
        clList[0].a += 5;
        /*
        Debug.Log("s1 : " + s1.a);
        Debug.Log("s2 : " + s2.a);
        Debug.Log("s3 : " + s3.a);
        Debug.Log("stList[0] : " + stList[0]);

        Debug.Log("c1 : " + c1.a);
        Debug.Log("c2 : " + c2.a);
        Debug.Log("c3 : " + c3.a);
        Debug.Log("clList[0] : " + clList[0]);*/
    }

    // Update is called once per frame
    //void Update () {
    //}

    public void CreateContactObj(Vector3 pos)
    {
        GameObject createdObj;
        createdObj = Instantiate(contactPointObj);
        createdObj.GetComponent<Transform>().position = pos;
    }

    public void CreateEffect(int id, Vector3 pos)
    {
        if (id < 0) return;
        GameObject createdObj;
        createdObj = Instantiate(effectObj);
        createdObj.SetActive(true);
        createdObj.GetComponent<Effect>().Init(id, pos);
    }



    public void ListTest()
    {
        WeaponBuff a = new WeaponBuff();
        WeaponBuff b = new WeaponBuff();
        WeaponBuff c = new WeaponBuff();
        WeaponBuff d = new WeaponBuff();
        WeaponBuff e = new WeaponBuff();
        List<WeaponBuff> wList = new List<WeaponBuff>();
        wList.Add(a);
        wList.Add(b);
        wList.Add(c);
        wList.Add(d);
        if (a == b)
        {
            Debug.Log("a == b");
        }
        else
        {
            Debug.Log("a != b"); 
        }
        // output : a != b"
        int t = 0;
        int t2 = 0;
        int t3 = 0;
        // 내부적으로 compare로 비교해서 값 비교 되는 걸로 넣어야 될듯 클래스 안됨
        //Debug.Log(t = wList.BinarySearch(c));
        //Debug.Log(t2 = wList.BinarySearch(e));

        // 바이너리서치 오름 차순이어야됨
        List<int> aa = new List<int>(){1, 2, 3, 4, 5, 6};
        //Debug.Log(t = aa.BinarySearch(1)); // 0
        //Debug.Log(t2 = aa.BinarySearch(5)); // 4
        //Debug.Log(t3 = aa.BinarySearch(8)); // 2
    }

    public int Abc(int b)
    {
        return 5;
    }

    public float Abc(float b)
    {
        return 5.5f;
    }

    public void Test(int num)
    {
        Debug.Log("Bridge : " + num);
    }

    public void TestStr(int num)
    {
        Debug.Log("B : " + num);
    }

    private Parent p = new Child(); // 업 캐스팅

    public void TestInhrt()
    {
        // p.Fun(); -> Child function
        // p.AA(); -> Child AA function
    }
}

// 상속, override test
public abstract class Parent
{
    protected float a;
    // virtual 함수는 부모 클래스에서 구현 꼭 적어야 되고
    // 자식에서는 override 해도 되고 안 해도 됨.
    public virtual void Fun()
    {
        Debug.Log("Parent function");
    }

    // protected abstract void AA(); 로 하면
    // 다른데서 p.AA 안뜸. 상속 관계 있는 곳에서만 가능.
    public abstract void AA();
    /*{
        abstract는 부모클래스에서 구현 적으면 오류.
        그대신 자식클래스에서 무조건 override 해야됨.
    }*/

}

public class Child : Parent
{
    public void Create(int n)
    {
        Debug.Log("Child " + n);
    }
    
    public override void Fun()
    {
        Debug.Log("Child function");
    }

    public override void AA()
    {
        Debug.Log("Child AA function");
    }
}

public class Child2 : Parent
{
    
    public void Create(int a, int b)
    {
        Debug.Log("Child2 " + a + ", " + b);
    }

    public override void AA()
    {
        Debug.Log("Child2 AA function");
    }
}

public abstract class TestAbstract
{
}

public interface ITestInterface
{
    /* 안되는 것들
     * 접근 제한자, abstract, virual 안됨.
     * public void Walk();
     * private void Walk2();
     * protected void Walk3();
     * public abstract void Sleep();
     * public virtual void Sleep2()
        {
        Debug.Log("Sleep2");
        }
     */

    void Run();

    
}





/*
 * 무기 강화 요소
 *  - 패시브 아이템
 *  - 시간제 버프
 *  
 *  
 * 1. 단순 수치 강화
 * 
 *  - 공격력
 *  - 공격속도
 *  - 사거리
 *  - 총알 크기
 *  - 총알 속도
 *  
 *  2. 총알 속성 부여 ( 충돌, update, delete 속성부여) 
 *  - 상태이상 공격 추가(독, 화상, 빙결 공격)
 *   고려할 점 : 상태 이상 공격도 상태 이상 효과가 쌔질 수도 있는 거고 아예 없던 상태 이상 공격이 추가 될 수도 있는건데
 * 
 * 
 * 
 * 
 * ----------------------
 * 
 * onwer를 돕는 동료 추가 형
 * 
 * 각각의 ai로 onwer기준으로 enamy를 공격
 *  - 용병
 *  - 동료
 */
