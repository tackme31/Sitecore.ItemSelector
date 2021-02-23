using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Sitecore.ItemSelector
{
    public static class ItemExtensions
    {
        public static Item SelectItem(this Item item, string selector)
        {
            Assert.ArgumentNotNull(item, nameof(item));
            Assert.ArgumentNotNullOrEmpty(selector, nameof(selector));

            return item.SelectAllItems(selector).FirstOrDefault();
        }

        public static List<Item> SelectAllItems(this Item item, string selector)
        {
            Assert.ArgumentNotNull(item, nameof(item));
            Assert.ArgumentNotNullOrEmpty(selector, nameof(selector));

            var targetItems = new List<Item>() { item };
            foreach (var itemLink in selector.Split('.').ToList())
            {
                // MultilistField*/Child
                // MultilistField*^
                if (itemLink.Contains('*'))
                {
                    var namePart = itemLink.Split('*')[0];
                    var restPart = itemLink.Split('*')[1];
                    targetItems = targetItems
                        .Select(targetItem => (MultilistField)targetItem?.Fields[namePart])
                        .SelectMany(field => field?.GetItems() ?? new Item[0])
                        .Select(multilistItem => GetTargetItem(multilistItem, restPart))
                        .ToList();
                }
                else
                {
                    targetItems = targetItems.Select(targetItem => GetTargetItem(targetItem, itemLink)).ToList();
                }
            }

            return targetItems.Where(targetItem => targetItem != null).ToList();
        }

        private static Item GetTargetItem(Item item, string itemLink)
        {
            var name = itemLink.Split('/', '^').First();
            var axes = itemLink.Substring(name.Length);
            var result = item;

            // MultilistField[0]
            var match = Regex.Match(name, @"^(?<namePart>\w+?)\[(?<index>\d+)\]$");
            if (match.Success)
            {
                var namePart = match.Groups["namePart"].Value;
                var index = int.Parse(match.Groups["index"].Value);
                var multilistField = (MultilistField)item?.Fields[namePart];
                var items = multilistField?.GetItems();
                result = items?.Length <= index ? null : items?[index];
            }
            // MultilistField:First
            else if (name.EndsWith(":First", StringComparison.InvariantCultureIgnoreCase))
            {
                var namePart = name.Split(':')[0];
                var multilistField = (MultilistField)item?.Fields[namePart];
                result = multilistField?.GetItems().FirstOrDefault();
            }
            // MultilistField:Last
            else if (name.EndsWith(":Last", StringComparison.InvariantCultureIgnoreCase))
            {
                var namePart = name.Split(':')[0];
                var multilistField = (MultilistField)item?.Fields[namePart];
                result = multilistField?.GetItems().LastOrDefault();
            }
            // Link field
            else if (!string.IsNullOrWhiteSpace(name))
            {
                result = ((LinkField)item?.Fields[name])?.TargetItem ?? ((ReferenceField)item?.Fields[name])?.TargetItem;
            }

            // Parent and Children
            if (result != null && !string.IsNullOrEmpty(axes))
            {
                foreach (var child in axes.Split('/'))
                {
                    // 'child' should be "^[^/^]*\\^*$" pattern (e.g. 'Foo', 'Foo^^', '^^^')
                    var childName = child.TrimEnd('^');
                    if (!string.IsNullOrEmpty(childName))
                    {
                        result = result.Children[childName];
                    }

                    // Climb up to parents
                    var climbCount = child.Length - childName.Length;
                    result = Enumerable.Range(0, climbCount).Aggregate(result, (acc, _) => result.Parent);
                }
            }

            return result;
        }
    }
}
