using UnityEngine;

namespace Fp.Utility
{
    public static class UniClipboard
    {
        private static IBoard _board;

        private static IBoard Board
        {
            get
            {
                if (_board == null)
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                    _board = new AndroidBoard();
#elif UNITY_IOS && !UNITY_TVOS && !UNITY_EDITOR
                    _board = new IOSBoard ();
#else
                    _board = new StandardBoard();
#endif
                }

                return _board;
            }
        }

        public static void SetText(string str)
        {
            Board.SetText(str);
        }

        public static string GetText()
        {
            return Board.GetText();
        }
    }

    internal interface IBoard
    {
        void SetText(string str);
        string GetText();
    }

    internal class StandardBoard : IBoard
    {
#region IBoard Implementation

        public void SetText(string str)
        {
            GUIUtility.systemCopyBuffer = str;
        }

        public string GetText()
        {
            return GUIUtility.systemCopyBuffer;
        }

#endregion
    }

#if UNITY_IOS && !UNITY_TVOS
class IOSBoard : IBoard {
    [DllImport("__Internal")]
    static extern void SetText_ (string str);
    [DllImport("__Internal")]
    static extern string GetText_();

    public void SetText(string str){
        if (Application.platform != RuntimePlatform.OSXEditor) {
            SetText_ (str);
        }
    }

    public string GetText(){
        return GetText_();
    }
}
#endif

#if UNITY_ANDROID
    internal class AndroidBoard : IBoard
    {
#region IBoard Implementation

        public void SetText(string str)
        {
            GetClipboardManager().Call("setText", str);
        }

        public string GetText()
        {
            return GetClipboardManager().Call<string>("getText");
        }

#endregion

        private AndroidJavaObject GetClipboardManager()
        {
            var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            var staticContext = new AndroidJavaClass("android.content.Context");
            var service = staticContext.GetStatic<AndroidJavaObject>("CLIPBOARD_SERVICE");
            return activity.Call<AndroidJavaObject>("getSystemService", service);
        }
    }
#endif
}