using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UniRx;

namespace UniroidDialog
{
    /// <summary>
    /// Android Native Dialog Manager.
    /// </summary>
    public class UniroidDialog
    {

        private UniroidDialog()
        {
        }

        /// <summary>
        /// the event touched button.
        /// </summary>
        static event Action<int> OnClickEvent = delegate {};

        /// <summary>
        /// constant: positive button touched
        /// </summary>
        private static readonly int TOUCHED_POSITIVE = 1;
        /// <summary>
        /// constant: negative button touched
        /// </summary>
        private static readonly int TOUCHED_NEGATIVE = -1;

        /// <summary>
        /// Show dialog with title and list of buttons.
        /// </summary>
        /// <param name="title">dialog title</param>
        /// <param name="buttonsText">texts for buttons</param>
        /// <returns>the text of touched button</returns>
        public static async UniTask<string> ShowAsyncAlertWithTitleAndListButtons(string title, string[] buttonsText)
        {
            UniroidDialog.ShowListButtons(title, buttonsText);
            int result = await Observable.FromEvent<int>(
                    h => OnClickEvent += h,
                    h => OnClickEvent -= h) 
                .ToUniTask(true);
            return buttonsText[result];
        }

        /// <summary>
        /// Show dialog with title, message and positive button.
        /// </summary>
        /// <param name="title">dialog title</param>
        /// <param name="message">dialog message</param>
        /// <param name="buttonText">text for positive button</param>
        public static async UniTask ShowAsyncAlertWithTitleMessageAndOk(string title, string message,
            string buttonText)
        {
            UniroidDialog.ShowOk(title, message, buttonText);
            await Observable.FromEvent<int>(
                    h => OnClickEvent += h,
                    h => OnClickEvent -= h) 
                .ToUniTask(true);
        }

        /// <summary>
        /// Show dialog with title, message, positive button and negative button.
        /// </summary>
        /// <param name="title">dialog title</param>
        /// <param name="message">dialog message</param>
        /// <param name="positiveButtonsText">text for positive button</param>
        /// <param name="negativeButtonText">text for negative button</param>
        /// <returns>true:touched positive / false:touched negative</returns>
        public static async UniTask<bool> ShowAsyncAlertWithTitleMessageAndOkCancel(string title, string message,
            string positiveButtonsText, string negativeButtonText)
        {
            UniroidDialog.ShowOkCancel(title, message, positiveButtonsText, negativeButtonText);
            int result = await Observable.FromEvent<int>(
                    h => OnClickEvent += h, 
                    h => OnClickEvent -= h) 
                .ToUniTask(true);
            return result == TOUCHED_POSITIVE;
        }

        /// <summary>
        /// show android native dialog with title, message, positive button and negative button.
        /// </summary>
        /// <param name="title">dialog title</param>
        /// <param name="message">dialog message</param>
        /// <param name="buttonsText">[0]: text for positive button / [1]: text for negative button</param>
        /// <returns>true:touched positive / false:touched negative</returns>
        public static async UniTask<bool> ShowAsyncAlertWithTitleMessageAndOkCancel(string title, string message,
            string[] buttonsText)
        {
            return await UniroidDialog.ShowAsyncAlertWithTitleMessageAndOkCancel(title, message, buttonsText[0],
                buttonsText[1]);
        }

        
        /// <summary>
        /// show android native dialog with title and list of buttons
        /// </summary>
        /// <param name="title">dialog title</param>
        /// <param name="buttonsText">texts for buttons</param>
        static void ShowListButtons(string title, string[] buttonsText)
        {
            var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject builder = new AndroidJavaObject("android.app.AlertDialog$Builder", activity);
                builder.Call<AndroidJavaObject>("setTitle", title);
                // message + setItemsは両立不可
                builder.Call<AndroidJavaObject>("setItems", buttonsText,
                    new ButtonClickListener());
                builder.Call<AndroidJavaObject>("setCancelable", false);
                AndroidJavaObject dialog = builder.Call<AndroidJavaObject>("create");
                dialog.Call("show");
            }));
        }
        
        /// <summary>
        /// show android native dialog with title, message and positive button.
        /// </summary>
        /// <param name="title">dialog title</param>
        /// <param name="message">dialog message</param>
        /// <param name="buttonText">text for positive button</param>
        static void ShowOk(string title, string message, string buttonText)
        {
            var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject builder = new AndroidJavaObject("android.app.AlertDialog$Builder", activity);

                builder.Call<AndroidJavaObject>("setTitle", title);
                builder.Call<AndroidJavaObject>("setMessage", message);
                builder.Call<AndroidJavaObject>("setPositiveButton", buttonText, new PositiveClickListener());
                builder.Call<AndroidJavaObject>("setCancelable", false);
                AndroidJavaObject dialog = builder.Call<AndroidJavaObject>("create");
                dialog.Call("show");
            }));
        }

        /// <summary>
        /// show android native dialog with title, message, positive button and negative button.
        /// </summary>
        /// <param name="title">dialog title</param>
        /// <param name="message">dialog message</param>
        /// <param name="positiveButtonsText">text for positive button</param>
        /// <param name="negativeButtonText">text for negative button</param>
        static void ShowOkCancel(string title, string message, string positiveButtonsText, string negativeButtonText)
        {
            var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject builder = new AndroidJavaObject("android.app.AlertDialog$Builder", activity);

                builder.Call<AndroidJavaObject>("setTitle", title);
                builder.Call<AndroidJavaObject>("setMessage", message);
                builder.Call<AndroidJavaObject>("setPositiveButton", positiveButtonsText, new PositiveClickListener());
                builder.Call<AndroidJavaObject>("setNegativeButton", negativeButtonText, new NegativeClickListener());
                builder.Call<AndroidJavaObject>("setCancelable", false);
                AndroidJavaObject dialog = builder.Call<AndroidJavaObject>("create");
                dialog.Call("show");
            }));
        }

        /// <summary>
        /// The listener for list-button touching event.
        /// </summary>
        /// <remarks>https://developer.android.com/reference/android/content/DialogInterface.OnClickListener</remarks>
        private class ButtonClickListener : AndroidJavaProxy
        {
            public ButtonClickListener() : base("android.content.DialogInterface$OnClickListener")
            {
            }

            /// <summary>
            /// 
            /// </summary>
            /// <remarks>https://developer.android.com/reference/android/content/DialogInterface.OnClickListener</remarks>
            /// <param name="dialog"></param>
            /// <param name="value">the position of the item clicked</param>
            public void onClick(AndroidJavaObject dialog, int value)
            {
                //ボタンが押された時に呼び出される
                Debug.Log($"ButtonClickListener onClick. value={value}");
                UniroidDialog.OnClickEvent(value);
            }
        }
        
        /// <summary>
        /// The listener for positive-button touching event.
        /// </summary>
        /// <remarks>https://developer.android.com/reference/android/content/DialogInterface.OnClickListener</remarks>
        private class PositiveClickListener : AndroidJavaProxy
        {
            public PositiveClickListener() : base("android.content.DialogInterface$OnClickListener")
            {
            }

            /// <summary>
            /// This method will be invoked when a button in the dialog is clicked.
            /// </summary>
            /// <remarks>https://developer.android.com/reference/android/content/DialogInterface.OnClickListener</remarks>
            /// <param name="dialog"></param>
            /// <param name="value">DialogInterface.BUTTON_POSITIVE https://developer.android.com/reference/android/content/DialogInterface#BUTTON_POSITIVE </param>
            public void onClick(AndroidJavaObject dialog, int value)
            {
                //ボタンが押された時に呼び出される
                Debug.Log($"ButtonClickListener onClick. value={value}");
                UniroidDialog.OnClickEvent(TOUCHED_POSITIVE);
            }
        }

        /// <summary>
        /// The listener for negative-button touching event.
        /// </summary>
        /// <remarks>https://developer.android.com/reference/android/content/DialogInterface.OnClickListener</remarks>
        private class NegativeClickListener : AndroidJavaProxy
        {
            /// <summary>
            /// constructor
            /// </summary>
            public NegativeClickListener() : base("android.content.DialogInterface$OnClickListener")
            {
            }

            /// <summary>
            /// This method will be invoked when a button in the dialog is clicked.
            /// </summary>
            /// <remarks>https://developer.android.com/reference/android/content/DialogInterface.OnClickListener</remarks>
            /// <param name="dialog"></param>
            /// <param name="value">DialogInterface.BUTTON_NEGATIVE https://developer.android.com/reference/android/content/DialogInterface#BUTTON_NEGATIVE </param>
            public void onClick(AndroidJavaObject dialog, int value)
            {
                //ボタンが押された時に呼び出される
                Debug.Log($"ButtonClickListener onClick. value={value}");
                UniroidDialog.OnClickEvent(TOUCHED_NEGATIVE);
            }
        }
    }
}

