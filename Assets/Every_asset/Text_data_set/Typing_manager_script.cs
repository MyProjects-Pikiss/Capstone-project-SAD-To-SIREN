using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Typing_manager_script : MonoBehaviour
{
    public static Typing_manager_script instance;

    public delegate void SentenceReachedAction(int sentenceNumber);
    public event SentenceReachedAction OnSentenceReached;

    public float timeForCharacter; //0.08�� �⺻.

    public float timeForCharacter_Fast; //0.03�� ���� �ؽ�Ʈ.

    float characterTime; // ���� ����Ǵ� ���ڿ� �ӵ�.

    //�ӽ� ����Ǵ� ��ȭ ������Ʈ�� ��ȭ����.
    string[] dialogsSave;
    TextMeshProUGUI tmpSave;

    public static bool isDialogEnd;

    bool isTypingEnd = false; //Ÿ������ �����°�?
    int dialogNumber = 0; //��ȭ ���� ����.

    float timer; //���������� ���ư��� �ð� Ÿ�̸�

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        timer = timeForCharacter;
        characterTime = timeForCharacter;
    }

    public void Typing(string[] dialogs, TextMeshProUGUI textObj)
    {
        isDialogEnd = false;
        dialogsSave = dialogs;
        tmpSave = textObj;
        if (dialogNumber < dialogs.Length)
        {
            char[] chars = dialogs[dialogNumber].ToCharArray(); //�޾ƿ� ���̾� �α׸� char�� ��ȯ.
            StartCoroutine(Typer(chars, textObj)); //���۷����� �Ѱܺ��°� �׽�Ʈ �غ���.
        }
        else
        {
            //������ �������Ƿ� �ٸ� ������ ���� �غ�... ���̾�α� �ʱ�ȭ, ���̾�α� ���̺�� Ƽ���� ���̺� �ʱ�ȭ
            tmpSave.text = "";
            isDialogEnd = true; // ȣ���ڴ� ���̾˷α� ���带 ���� ���� ������ �������ָ� ��.
            dialogsSave = null;
            tmpSave = null;
            dialogNumber = 0;
        }
        OnSentenceReached?.Invoke(dialogNumber);
    }

    public void GetInputDown()
    {
        //��ǲ�� �������� -> �ؽ�Ʈ�� �������̸� ������ ����ǰ� �ؽ�Ʈ�� �����Ǿ������� ���� �ؽ�Ʈ�� �Ѿ.
        //�׸��� ��ǲ�� ĵ���Ǹ� �ٽ� ���ڿ� �ӵ��� ����ȭ ���Ѿ���.
        if (dialogsSave != null)
        {
            if (isTypingEnd)
            {
                tmpSave.text = ""; //����ִ� ���� �Ѱܼ� �ʱ�ȭ. 
                Typing(dialogsSave, tmpSave);
            }
            else
            {
                characterTime = timeForCharacter_Fast; //���� ���� �ѱ�.
            }
        }
    }

    public void GetInputUp()
    {
        //��ǲ�� ��������.
        if (dialogsSave != null)
        {
            characterTime = timeForCharacter;
        }
    }

    IEnumerator Typer(char[] chars, TextMeshProUGUI textObj)
    {
        int currentChar = 0;
        int charLength = chars.Length;
        isTypingEnd = false;

        while (currentChar < charLength)
        {
            if (timer >= 0)
            {
                yield return null;
                timer -= Time.deltaTime;
            }
            else
            {
                textObj.text += chars[currentChar].ToString();
                currentChar++;
                timer = characterTime; //Ÿ�̸� �ʱ�ȭ
            }
        }
        if (currentChar >= charLength)
        {
            isTypingEnd = true;
            dialogNumber++;
            yield break;
        }
    }
}