using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ȯ�� �����ؼ� �־�� �� �������� ������ Ŭ����. �ڿ� ReturnScore�Լ��� �����ϸ� �˾Ƽ� ��� ��
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
        //���⼭ ���� ���� © ��
    }
}
