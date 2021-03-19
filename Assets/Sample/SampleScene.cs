using System;
using UniroidDialog;
using UnityEngine;
using UnityEngine.UI;

public class SampleScene : MonoBehaviour
{
    [SerializeField, Tooltip("ボタン")]
    Button listButton = null;
    [SerializeField, Tooltip("ボタン")]
    Button OkCancelButton = null;
    [SerializeField, Tooltip("ボタン")]
    Button OkButton = null;
    [SerializeField, Tooltip("テキスト")]
    private Text resultView = null;
    void Start()
    {
        listButton.onClick.AddListener(this.ShowListDialog);
        OkCancelButton.onClick.AddListener(this.ShowOkCancelDialog);
        OkButton.onClick.AddListener(this.ShowOkDialog);
    }

    void Update()
    {
        
    }

    async void ShowListDialog()
    {
        // タッチしたボタンのテキスト
        string result = await UniroidDialog.UniroidDialog.ShowAsyncAlertWithTitleAndListButtons("テスト本文", new string[] {"melon", "grape", "banana", "orange"});
        Debug.Log($"////result={result}");
        resultView.text = $"Clicked button:{result}";
    }

    async void ShowOkCancelDialog()
    {
        // OKボタンをタッチしたか？
        bool result = await UniroidDialog.UniroidDialog.ShowAsyncAlertWithTitleMessageAndOkCancel("テストタイトル","テスト本文", "OK", "キャンセル");
        Debug.Log($"////result={result}");
        resultView.text = $"Clicked OK?:{result}";
    }
    
    async void ShowOkDialog()
    {
        // 待つだけ。
        await UniroidDialog.UniroidDialog.ShowAsyncAlertWithTitleMessageAndOk("テストタイトル","テスト本文", "OK");
        resultView.text = $"OK Clicked!!";
    }
}