using System;
using System.Collections.Generic;
using System.Linq;
using SmartAddresser.Editor.Core.Models.EntryRules.AddressRules;
using SmartAddresser.Editor.Foundation.EasyTreeView;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace SmartAddresser.Editor.Core.Tools.Addresser.AddressEditor
{
    /// <summary>
    ///     Tree view for the Address Rule Editor.
    /// </summary>
    internal sealed class AddressRuleEditorTreeView : TreeViewBase
    {
        [NonSerialized] private int _currentId;

        public AddressRuleEditorTreeView(AddressEditorTreeViewState state) : base(state)
        {
            showAlternatingRowBackgrounds = true;
            ColumnStates = state.ColumnStates;
            rowHeight = 16;
            Reload();
        }

        public AddressRuleEditorTreeViewItem AddItem(AddressRule rule, int index = -1)
        {
            var item = new AddressRuleEditorTreeViewItem(rule)
            {
                id = _currentId++
            };
            AddItemAndSetParent(item, -1, index);
            return item;
        }

        protected override void CellGUI(int columnIndex, Rect cellRect, RowGUIArgs args)
        {
            var item = (AddressRuleEditorTreeViewItem)args.item;
            var addressableGroup = item.Rule.AddressableGroup;
            switch ((Columns)columnIndex)
            {
                case Columns.Groups:
                    GUI.enabled = addressableGroup != null && !addressableGroup.ReadOnly && item.Rule.Control;
                    GUI.Label(cellRect, GetText(item, columnIndex));
                    break;
                case Columns.Control:
                    GUI.enabled = addressableGroup != null && !addressableGroup.ReadOnly;
                    cellRect.x += cellRect.width / 2 - 7;
                    cellRect.width = 14;
                    item.Rule.Control = GUI.Toggle(cellRect, item.Rule.Control, "");
                    break;
                case Columns.AssetGroups:
                    GUI.enabled = addressableGroup != null && !addressableGroup.ReadOnly && item.Rule.Control;
                    GUI.Label(cellRect, GetText(item, columnIndex));
                    break;
                case Columns.AddressRule:
                    GUI.enabled = addressableGroup != null && !addressableGroup.ReadOnly && item.Rule.Control;
                    GUI.Label(cellRect, GetText(item, columnIndex));
                    break;
                default:
                    throw new NotImplementedException();
            }

            GUI.enabled = true;
        }

        protected override IOrderedEnumerable<TreeViewItem> OrderItems(IList<TreeViewItem> items, int keyColumnIndex,
            bool ascending)
        {
            string KeySelector(TreeViewItem x)
            {
                return GetText((AddressRuleEditorTreeViewItem)x, keyColumnIndex);
            }

            return ascending
                ? items.OrderBy(KeySelector, Comparer<string>.Create(EditorUtility.NaturalCompare))
                : items.OrderByDescending(KeySelector, Comparer<string>.Create(EditorUtility.NaturalCompare));
        }

        protected override string GetTextForSearch(TreeViewItem item, int columnIndex)
        {
            return GetText((AddressRuleEditorTreeViewItem)item, columnIndex);
        }

        protected override bool CanMultiSelect(TreeViewItem item)
        {
            return true;
        }

        protected override bool CanBeParent(TreeViewItem item)
        {
            return false;
        }

        private static string GetText(AddressRuleEditorTreeViewItem item, int columnIndex)
        {
            switch ((Columns)columnIndex)
            {
                case Columns.Groups:
                    return item.Rule.AddressableGroup == null
                        ? "[Missing Reference]"
                        : item.Rule.AddressableGroup.name;
                case Columns.Control:
                    return item.Rule.Control.ToString();
                case Columns.AssetGroups:
                    return item.Rule.AssetGroupDescription.Value;
                case Columns.AddressRule:
                    return item.Rule.AddressProviderDescription.Value;
                default:
                    throw new NotImplementedException();
            }
        }

        public enum Columns
        {
            Groups,
            Control,
            AssetGroups,
            AddressRule
        }
    }
}
