﻿using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using Artemis.Models;
using Artemis.Utilities;
using MahApps.Metro.Controls;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;

namespace Artemis.Managers
{
    public static class KeybindManager
    {
        private static readonly List<KeybindModel> KeybindModels = new List<KeybindModel>();

        static KeybindManager()
        {
            InputHook.KeyDownCallback += args => ProcessKey(args, PressType.Down);
            InputHook.KeyUpCallback += args => ProcessKey(args, PressType.Up);
            InputHook.MouseDownCallback += args => ProcessMouse(args, PressType.Down);
            InputHook.MouseUpCallback += args => ProcessMouse(args, PressType.Up);
        }

        private static void ProcessKey(KeyEventArgs keyEventArgs, PressType pressType)
        {
            // Don't trigger if the key itself is a modifier
            if (keyEventArgs.KeyCode == Keys.LShiftKey || keyEventArgs.KeyCode == Keys.RShiftKey ||
                keyEventArgs.KeyCode == Keys.LControlKey || keyEventArgs.KeyCode == Keys.RControlKey ||
                keyEventArgs.KeyCode == Keys.LMenu || keyEventArgs.KeyCode == Keys.RMenu)
                return;

            // Create a WPF ModifierKeys enum
            var modifiers = ModifierKeysFromBooleans(keyEventArgs.Alt, keyEventArgs.Control, keyEventArgs.Shift);

            // Create a HotKey object for comparison
            var hotKey = new HotKey(KeyInterop.KeyFromVirtualKey(keyEventArgs.KeyValue), modifiers);

            foreach (var keybindModel in KeybindModels)
                keybindModel.InvokeIfMatched(hotKey, pressType);
        }

        private static void ProcessMouse(MouseEventArgs mouseEventArgs, PressType pressType)
        {
            foreach (var keybindModel in KeybindModels)
                keybindModel.InvokeIfMatched(mouseEventArgs.Button, pressType);
        }

        public static void AddOrUpdate(KeybindModel keybindModel)
        {
            if (keybindModel == null)
                return;

            var existing = KeybindModels.FirstOrDefault(k => k.Name == keybindModel.Name);
            if (existing != null)
                KeybindModels.Remove(existing);

            KeybindModels.Add(keybindModel);
        }

        public static void Remove(KeybindModel keybindModel)
        {
            if (KeybindModels.Contains(keybindModel))
                KeybindModels.Remove(keybindModel);
        }

        public static void Remove(string name)
        {
            var existing = KeybindModels.FirstOrDefault(k => k.Name == name);
            if (existing != null)
                KeybindModels.Remove(existing);
        }

        public static ModifierKeys ModifierKeysFromBooleans(bool alt, bool control, bool shift)
        {
            // Create a WPF ModifierKeys enum
            var modifiers = ModifierKeys.None;
            if (alt)
                modifiers = ModifierKeys.Alt;
            if (control)
            {
                if (modifiers == ModifierKeys.None)
                    modifiers = ModifierKeys.Control;
                else
                    modifiers |= ModifierKeys.Control;
            }
            if (shift)
            {
                if (modifiers == ModifierKeys.None)
                    modifiers = ModifierKeys.Shift;
                else
                    modifiers |= ModifierKeys.Shift;
            }

            return modifiers;
        }
    }
}
