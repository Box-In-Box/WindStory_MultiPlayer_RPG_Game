using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkManager : MonoBehaviour
{
    public static TalkManager instance;
    Dictionary<int, string[]> talkData;

    void Awake()
    {
        if (instance == null) { DontDestroyOnLoad(gameObject); instance = this; }
        else Destroy(this.gameObject);

        this.talkData = new Dictionary<int, string[]>();
        GenerateData();
    }

    void GenerateData()
    {
        talkData.Add(1000, new string[] { "�ȳ��ϼ��� ������ø���� �մϴ�.\n\n\n ������ : dgf0000@naver.com" });

        talkData.Add(1000 + 10, new string[] { "���� �������� ������ ������ �͵� ������ �켱 �⺻�� Ȯ���ؾ� ���� �ʰڴ°�?\n\n�Դٰ� �׷��� �Ǽ����� ������ �ٴϴٰ� ���Ͷ� ������ ��� �±� �ʻ��̶��.\n������~"
            , "���� ���� ������ ���⸦ ���״� �켱 ���� �ո����� ���͸� �Ѹ����� ����ϰ� ���Գ�.\n\n �׷� ���� ���̻� �ڳ׸� ������ �ʰڳ�.\n ��� �� �� �� ���ڳ�?" });
        talkData.Add(1000 + 11, new string[] { "���� �ո����� \"��������\"�� �Ѹ��� ��ƺ���..\n �Ƿ��� �־�� ������ ������ �ʰڳ�?" });
        talkData.Add(1000 + 12, new string[] { "���� ���͸� ����ϰ� �Ա���\n\n���� ���� ������ �ʰڳ� ������ ���Գ�" });

        talkData.Add(1100, new string[] { "����...���� �����ٰ� �̲����� �׸� �ٸ��� ��ġ�� ���ҽ��ϴ�.\n\n���� ���� �� �ͼ� �̰� ���� ���̶�..."
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
