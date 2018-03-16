using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Shibari.Editor
{
    public static class MultiLevelDropDownUtility
    {
        private const string SELECTION_UPDATED_COMMAND_NAME = "SelectionUpdated";
        private static readonly int controlHint = typeof(MultiLevelDropDownUtility).GetHashCode();

        private static int selectionControlID;
        private static string selectedItem;
        private static readonly GenericMenu.MenuFunction2 onSelected = OnSelected;

        private static void DisplayDropDown(Rect position, List<string> entries, string selectedItem)
        {
            var menu = new GenericMenu();

            menu.AddItem(new GUIContent("(None)"), selectedItem == null, onSelected, null);
            menu.AddSeparator("");

            for (int i = 0; i < entries.Count; ++i)
            {
                string menuLabel = entries[i];
                if (string.IsNullOrEmpty(menuLabel))
                    continue;

                var content = new GUIContent(menuLabel);
                menu.AddItem(content, menuLabel == selectedItem, onSelected, menuLabel);
            }

            menu.DropDown(position);
        }

        public static string DrawControl(Rect position, GUIContent label, string selected, List<string> entries)
        {
            if (label != null && label != GUIContent.none)
                position = EditorGUI.PrefixLabel(position, label);

            int controlID = GUIUtility.GetControlID(controlHint, FocusType.Keyboard, position);

            bool triggerDropDown = false;

            switch (Event.current.GetTypeForControl(controlID))
            {
                case EventType.ExecuteCommand:
                    if (Event.current.commandName == SELECTION_UPDATED_COMMAND_NAME)
                    {
                        if (selectionControlID == controlID)
                        {
                            if (selected != selectedItem)
                            {
                                selected = selectedItem;
                                GUI.changed = true;
                            }

                            selectionControlID = 0;
                            selectedItem = null;
                        }
                    }
                    break;

                case EventType.MouseDown:
                    if (GUI.enabled && position.Contains(Event.current.mousePosition))
                    {
                        GUIUtility.keyboardControl = controlID;
                        triggerDropDown = true;
                        Event.current.Use();
                    }
                    break;

                case EventType.KeyDown:
                    if (GUI.enabled && GUIUtility.keyboardControl == controlID)
                    {
                        if (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.Space)
                        {
                            triggerDropDown = true;
                            Event.current.Use();
                        }
                    }
                    break;

                case EventType.Repaint:

                    GUIContent content = new GUIContent
                    {
                        text = selected.Trim()
                    };

                    if (content.text == "")
                        content.text = "(None)";
                    else if (!entries.Contains(selected))
                        content.text += " {Missing}";

                    EditorStyles.popup.Draw(position, content, controlID);
                    break;
            }

            if (triggerDropDown)
            {
                selectionControlID = controlID;
                selectedItem = selected;
                
                DisplayDropDown(position, entries, selectedItem);
            }

            return selected;
        }

        private static void OnSelected(object userData)
        {
            selectedItem = userData as string;

            var selectionUpdatedEvent = EditorGUIUtility.CommandEvent(SELECTION_UPDATED_COMMAND_NAME);
            EditorWindow.focusedWindow.SendEvent(selectionUpdatedEvent);
        }
    }
}