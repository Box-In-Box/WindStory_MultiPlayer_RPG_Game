using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkManager : MonoBehaviour
{
    Dictionary<int, string[]> talkData;

    void Awake()
    {
        this.talkData = new Dictionary<int, string[]>();
        GenerateData();
    }

    void GenerateData()
    {
        talkData.Add(1000, new string[] { "���� �������� ������ ������ �͵� ������ �켱 �⺻�� Ȯ���ؾ� ���� �ʰڴ°�?\n\n�Դٰ� �׷��� �Ǽ����� ������ �ٴϴٰ� ���Ͷ� ������ ��� �±� �ʻ��̶��.\n������~"
            , "���� ���� ������ ���⸦ ���״� �켱 ���� �ո����� ���͸� �Ѹ����� ����ϰ� ���Գ�.\n\n �׷� ���� ���̻� �ڳ׸� ������ �ʰڳ�.\n ��� �� �� �� ���ڳ�?" });
        talkData.Add(1001, new string[] { "����...���� �����ٰ� �̲����� �׸� �ٸ��� ��ġ�� ���ҽ��ϴ�.\n\n���� ���� �� �ͼ� �̰� ���� ���̶�..."
            , "�����ڴ�...���⼭ ���ݸ� �� ���� ���� ���� �ִµ� ������������ ���� �Ƴ����Լ� ���� ���� �޾Ƽ� ���� ������ �ֽðڽ��ϱ�?\n\n���� ��Ź�帳�ϴ�. �ƴϱ� ���ٸ�! ��.��" });
    }

    public string GetTalk(int id, int talkIndex)
    {
        if (talkIndex == talkData[id].Length)
            return null;
        else
            return talkData[id][talkIndex];
    }
}
