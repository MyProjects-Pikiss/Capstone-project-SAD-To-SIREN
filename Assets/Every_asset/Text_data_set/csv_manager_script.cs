using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;

public class csv_manager_script : MonoBehaviour
{
    public void SetCsvFileName(string fileName)
    {
        csvFileName = fileName;
    }

    [SerializeField]
    private string csvFileName;
    public TextMeshProUGUI textObj;

    private List<string> dialogStrings = new List<string>();

    private void Start()
    {
        LoadCsv();
        Typing_manager_script.instance.Typing(dialogStrings.ToArray(), textObj);
        StartCoroutine(WaitForDialogEnd());
    }

    IEnumerator WaitForDialogEnd()
    {
        while (!Typing_manager_script.isDialogEnd)
        {
            yield return null;
        }
    }

    private void LoadCsv()
    {
        // CSV ���� �ε�
        string filePath = Application.dataPath + "/Every_asset/Text_data_set/" + csvFileName;
        StreamReader streamReader = new StreamReader(filePath);

        // CSV ���� �Ľ�
        while (!streamReader.EndOfStream)
        {
            string line = streamReader.ReadLine();
            line = line.Replace("##", "\n"); // "##"�� �ٹٲ����� ��ü
            string[] values = line.Split(';');

            // �� ���� �����ϰ� �߰�
            foreach (string value in values)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    dialogStrings.Add(value);
                }
            }
        }
        streamReader.Close();
    }
}
