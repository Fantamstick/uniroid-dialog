using System;
using UnityEngine;
using UnityEngine.UI;

public class SampleScene : MonoBehaviour
{
    [SerializeField, Tooltip("Button")]
    Button listButton = null;
    [SerializeField, Tooltip("Button")]
    Button OkCancelButton = null;
    [SerializeField, Tooltip("Button")]
    Button OkButton = null;
    [SerializeField, Tooltip("Text")]
    Text resultView = null;
    
    void Start()
    {
        listButton.onClick.AddListener(this.ShowListDialog);
        OkCancelButton.onClick.AddListener(this.ShowOkCancelDialog);
        OkButton.onClick.AddListener(this.ShowOkDialog);
    }

    async void ShowListDialog()
    {
        // タッチしたボタンのテキスト
        string result = await UniroidDialog.UniroidDialog.ShowAsyncAlertWithTitleAndListButtons("Test Description", new string[] {"melon", "grape", "banana", "orange"});
        Debug.Log($"////result={result}");
        resultView.text = $"Clicked button:{result}";
    }

    async void ShowOkCancelDialog()
    {
        // OKボタンをタッチしたか？
        bool result = await UniroidDialog.UniroidDialog.ShowAsyncAlertWithTitleMessageAndOkCancel("Test Title","Description", "OK", "Cancel");
        Debug.Log($"////result={result}");
        resultView.text = $"Clicked OK?:{result}";
    }
    
    async void ShowOkDialog()
    {
        // 待つだけ。
        await UniroidDialog.UniroidDialog.ShowAsyncAlertWithTitleMessageAndOk("Test Title","Description", "OK");
        resultView.text = $"OK Clicked!!";
    }
}