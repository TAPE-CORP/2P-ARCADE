using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 점수 환산 관련해서 있어야 할 정보들을 래핑한 클래스. 뒤에 ReturnScore함수에 대입하면 알아서 출력 됨
/// </summary>
public class Data
{
    public int id;
    public string name;
    public float value;
}

public class DataHandler : MonoBehaviour
{
    public float score;
    public Data scoreInfo = new Data();
    public TReturn ReturnScore<TReturn>(Data info)
    {
        return default(TReturn);
        //여기서 구현 로직 짤 것
    }
}
