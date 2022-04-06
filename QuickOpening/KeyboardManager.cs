using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace QuickOpening
{
    public static class KeyboardManager
    {
        #region DLL

        [DllImport("user32", SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32", SetLastError = true)]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        #endregion

        public static event EventHandler<HotKeyEventArgs> HotKeyPressed;

        private static int _id = 0;
        private static volatile MessageWindow _wnd;
        private static volatile IntPtr _hwnd;
        delegate void RegisterHotKeyDelegate(IntPtr hwnd, int id, uint modifiers, uint key);
        delegate void UnRegisterHotKeyDelegate(IntPtr hwnd, int id);
        private static ManualResetEvent _windowReadyEvent = new ManualResetEvent(false);
        static public int GetCount() => _id;

        static KeyboardManager()
        {
            Thread messageLoop = new Thread(delegate ()
            {
                Application.Run(new MessageWindow());
            });
            messageLoop.Name = "MessageLoopThread";
            messageLoop.IsBackground = true;
            messageLoop.Start();
        }
        private static void OnHotKeyPressed(HotKeyEventArgs e)
        {
            if (HotKeyPressed != null) HotKeyPressed(null, e);
        }

        #region register

        public static int RegisterHotKey(Keys key, KeyModifiers modifiers)
        {
            _windowReadyEvent.WaitOne();
            int id = System.Threading.Interlocked.Increment(ref _id);
            _wnd.Invoke(new RegisterHotKeyDelegate(RegisterHotKeyInternal), _hwnd, id, (uint)modifiers, (uint)key);
            return id;
        }

        private static void RegisterHotKeyInternal(IntPtr hwnd, int id, uint modifiers, uint key)
        {
            RegisterHotKey(hwnd, id, modifiers, key);
        }

        #endregion

        #region unregister

        public static void UnregisterHotKey(int id)
        {
            _wnd.Invoke(new UnRegisterHotKeyDelegate(UnRegisterHotKeyInternal), _hwnd, id);
        }

        private static void UnRegisterHotKeyInternal(IntPtr hwnd, int id)
        {
            UnregisterHotKey(_hwnd, id);
        }

        #endregion

        #region Form

        private class MessageWindow : Form
        {
            private const int WM_HOTKEY = 0x312;
            public MessageWindow()
            {
                _wnd = this;
                _hwnd = this.Handle;
                _windowReadyEvent.Set();
            }
            protected override void WndProc(ref Message m)
            {
                if (m.Msg == WM_HOTKEY)
                {
                    HotKeyEventArgs e = new HotKeyEventArgs(m.LParam);
                    KeyboardManager.OnHotKeyPressed(e);
                }

                base.WndProc(ref m);
            }
            protected override void SetVisibleCore(bool value)
            {
                // Ensure the window never becomes visible
                base.SetVisibleCore(false);
            }
        }
        
        #endregion

        #region HotKeyEventArgs

        public class HotKeyEventArgs : EventArgs
        {
            public readonly Keys Key;
            public readonly KeyModifiers Modifiers;

            public HotKeyEventArgs(Keys key, KeyModifiers modifiers)
            {
                this.Key = key;
                this.Modifiers = modifiers;
            }
            public HotKeyEventArgs(IntPtr hotKeyParam)
            {
                uint param = (uint)hotKeyParam.ToInt64();
                Key = (Keys)((param & 0xffff0000) >> 16);
                Modifiers = (KeyModifiers)(param & 0x0000ffff);
            }
        }

        #endregion

        public enum KeyModifiers
        {
            Alt = 1,
            Control = 2,
            Shift = 4,
            Windows = 8,
            NoRepeat = 0x4000
        }
    }
}
