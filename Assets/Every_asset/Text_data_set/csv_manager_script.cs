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
        // CSV 파일 로드
        string filePath = Application.dataPath + "/Every_asset/Text_data_set/" + csvFileName;
        StreamReader streamReader = new StreamReader(filePath);

        // CSV 파일 파싱
        while (!streamReader.EndOfStream)
        {
            string line = streamReader.ReadLine();
            line = line.Replace("##", "\n"); // "##"을 줄바꿈으로 대체
            string[] values = line.Split(';');

            // 빈 값을 제외하고 추가
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
