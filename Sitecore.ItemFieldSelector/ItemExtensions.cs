using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using System;
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

            var itemLinks = selector.Split('.').ToList();
            var targetItem = itemLinks.GetRange(0, itemLinks.Count - 1).Aggregate(item, GetTargetItem);
            var lastField = itemLinks.Last();
            return targetItem.Fields[lastField];
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
                var multilistField = (MultilistField)item.Fields[namePart];
                result = multilistField.GetItems()[index];
            }
            // MultilistField:First
            else if (name.EndsWith(":First", StringComparison.InvariantCultureIgnoreCase))
            {
                var namePart = name.Split(':')[0];
                var multilistField = (MultilistField)item.Fields[namePart];
                result = multilistField.GetItems().First();
            }
            // MultilistField:Last
            else if (name.EndsWith(":Last", StringComparison.InvariantCultureIgnoreCase))
            {
                var namePart = name.Split(':')[0];
                var multilistField = (MultilistField)item.Fields[namePart];
                result = multilistField.GetItems().Last();
            }
            // Link field
            else if (!string.IsNullOrWhiteSpace(name))
            {
                result = ((LinkField)item.Fields[name]).TargetItem ?? ((ReferenceField)item.Fields[name]).TargetItem;
            }

            // ChildItem1/ChildItem2
            if (children.Any())
            {
                return children.Aggregate(result, (acc, n) => acc.Children[n]); ;
            }

            return result;
        }
    }
}
