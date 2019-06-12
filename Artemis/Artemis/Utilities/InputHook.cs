﻿using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using Gma.System.MouseKeyHook;
using NLog;

namespace Artemis.Utilities
{
    public static class InputHook
    {
        public delegate void KeyCallbackHandler(KeyEventArgs e);

        public delegate void MouseCallbackHandler(MouseEventArgs e);

        private static IKeyboardMouseEvents _globalHook;

        public static void Start()
        {
            _globalHook = Hook.GlobalEvents();

            // When hitting breakpoints all user input freezes for ~10 seconds due to Windows waiting on the hooks to time out.
            // By simply not hooking when the debugger is attached this no longer happens but keybinds obviously wont work.
            if (Debugger.IsAttached)
            {
                LogManager.GetCurrentClassLogger().Fatal("Debugger attached so not enabling any global hooks, keybinds won't work!");
                return;
            }

            _globalHook.KeyDown += GlobalHookOnKeyDown;
            _globalHook.KeyUp += GlobalHookOnKeyUp;
            _globalHook.MouseDown += GlobalHookOnMouseDown;
            _globalHook.MouseUp += GlobalHookOnMouseUp;
        }

        public static void Stop()
        {
            if (_globalHook == null)
                return;

            _globalHook.KeyDown -= GlobalHookOnKeyDown;
            _globalHook.KeyUp -= GlobalHookOnKeyUp;
            _globalHook.MouseDown -= GlobalHookOnMouseDown;
            _globalHook.MouseUp -= GlobalHookOnMouseUp;
            _globalHook.Dispose();
            _globalHook = null;
        }

        private static async void GlobalHookOnMouseDown(object sender, MouseEventArgs e)
        {
            await Task.Factory.StartNew(() => { MouseDownCallback?.Invoke(e); });
        }

        private static async void GlobalHookOnMouseUp(object sender, MouseEventArgs e)
        {
            await Task.Factory.StartNew(() => { MouseUpCallback?.Invoke(e); });
        }

        private static async void GlobalHookOnKeyDown(object sender, KeyEventArgs e)
        {
            await Task.Factory.StartNew(() => { KeyDownCallback?.Invoke(e); });
        }

        private static async void GlobalHookOnKeyUp(object sender, KeyEventArgs e)
        {
            await Task.Factory.StartNew(() => { KeyUpCallback?.Invoke(e); });
        }

        public static event KeyCallbackHandler KeyDownCallback;
        public static event KeyCallbackHandler KeyUpCallback;
        public static event MouseCallbackHandler MouseDownCallback;
        public static event MouseCallbackHandler MouseUpCallback;
    }
}
