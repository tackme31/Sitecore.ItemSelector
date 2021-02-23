# Sitecore.ItemSelector
A Sitecore library for getting a link field target with simple syntax.

## Installation
Download a package from [here](https://github.com/xirtardauq/Sitecore.ItemSelector/releases) and install it from your local package source.

## Usage
*Sitecore.ItemSelector* provides the following methods:

- [SelectItem](#user-content-selectitem)
- [SelectAllItems](#user-content-selectallitems)

### SelectItem
Returns a first item that are hit by the selector.

- Example 1: Get an item in link field.
```csharp
var item1 = item.SelectItem("Category.Group");

// Same as:
var item2 = ((LinkField)((LinkField)item.Fields["Category"]).TargetItem.Fields["Group"]).TargetItem;
```

- Example 2: Get an item in multilist field.
```csharp
var item1a = item.SelectItem("Tags:First");
var item1b = item.SelectItem("Related News:Last");
var item1c = item.SelectItem("Ranking[3]");

// Same as:
var item2a = ((MultilistField)item.Fields["Tags"]).GetItems().FirstOrDefault();
var item2b = ((MultilistField)item.Fields["Tags"]).GetItems().LastOfDefault();
var item2c = ((MultilistField)item.Fields["Tags"]).GetItems()[3];
```

- Example 3: Get a child/parent item.
```csharp
var item1a = item.SelectItem("/Data Folder/Metadata");
var item1b = item.SelectItem("^^/Settings");

// Same as:
var item2a = item.Children["Data Folder"].Children["Metadata"];
var item2b = item.Parent.Parent.Children["Settings"];
```

- Example 4: Complex pattern.
```csharp
var item1 = item.SelectItem("Related News:First.Category^/Data");

// Same as:
var news = ((MultilistField)item.Fields["Related News"]).GetItems().FirstOrDefault();
var item2 = ((LinkField)news.Fields["Category"]).TargetItem.Parent.Children["Data"];
```

### SelectAllItems
Returns all items that are hit by the selector.

- Example 1: Get all items in multilist field.
```csharp
var items1 = item.SelectAllItems("Tags*");

// Same as:
var items2 = ((MultilistField)item.Fields["Tags"]).GetItems().ToList();
```

- Example 2: Complex pattern.
```csharp
var items1 = item.SelectAllItems("Related News*/Data.Categories*.Group");

// Same as:
var items2 = ((MultilistField)item.Fields["Related News"])
    .GetItems()
    .Select(news => news.Children["Data"])
    .SelectMany(data => ((MultilistField)data.Fields["Categories"]).GetItems())
    .Select(category => ((LinkField)category.Fields["Group"]).TargetItem)
    .ToList();
```

## Syntax
|Syntax|Description|Example|
|:-|:-|:-|
|`.`|Select a item referred in a link field.|`Category.Group`|
|`:First`|Select a first item referred in a multilist field.|`Tags:First`|
|`:Last`|Select a last item referred in a multilist field.|`Tags:Last`|
|`*`|Select all items referred in a multilist field.|`Tags*`|
|`[N]`|Select a `N`th item refered in a multilist field.|`Tags[3]`|
|`/`|Select a child item.|`Data/Metadata`|
|`^`|Select a parent item.|`Data^`|

## Author
- Takumi Yamada (xirtardauq@gmail.com)

## License
*Sitecore.ItemSelector* is licensed under the MIT license. See LICENSE.