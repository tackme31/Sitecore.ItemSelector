using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Sitecore.ItemFieldSelector
{
    public static class ItemExtensions
    {
        public static Field SelectField(this Item item, string selector)
        {
            Assert.ArgumentNotNull(item, nameof(item));
            Assert.ArgumentNotNullOrEmpty(selector, nameof(selector));

            return item.SelectAllFields(selector).FirstOrDefault();
        }

        public static List<Field> SelectAllFields(this Item item, string selector)
        {
            Assert.ArgumentNotNull(item, nameof(item));
            Assert.ArgumentNotNullOrEmpty(selector, nameof(selector));

            var itemLinks = selector.Split('.').ToList();
            var lastField = itemLinks.Last();
            var targetItems = new List<Item>() { item };

            foreach (var itemLink in itemLinks.GetRange(0, itemLinks.Count - 1))
            {
                // MultilistField:*/Child
                var match = Regex.Match(itemLink, @"^(?<namePart>\w+?):\*(?<restPart>.*)$");
                if (match.Success)
                {
                    var namePart = match.Groups["namePart"].Value;
                    var restPart = match.Groups["restPart"].Value;
                    targetItems = targetItems
                        .Select(i => (MultilistField)i?.Fields[namePart])
                        .SelectMany(field => field?.GetItems() ?? Array.Empty<Item>())
                        .Select(i => GetTargetItem(i, restPart))
                        .ToList();
                }
                else
                {
                    targetItems = targetItems.Select(i => GetTargetItem(i, itemLink)).ToList();
                }
            }

            return targetItems
                .Select(targetItem => targetItem?.Fields[lastField])
                .Where(field => field != null)
                .ToList();
        }

        private static Item GetTargetItem(Item item, string itemLink)
        {
            var nameAndChildren = itemLink.Split('/').ToList();
            var name = nameAndChildren.First();
            var children = nameAndChildren.GetRange(1, nameAndChildren.Count - 1);
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

            // ChildItem1/ChildItem2
            if (result != null && children.Any())
            {
                return children.Aggregate(result, (acc, n) => acc?.Children[n]);
            }

            return result;
        }
    }
}
