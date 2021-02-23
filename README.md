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

- Example 1:
```csharp
var item1 = item.SelectItem("Category");

// Same as:
var item2 = ((LinkField)item.Fields["Category"]).TargetItem;
```

- Example 2:
```csharp
var item1 = item.SelectItem("Tags:First");

// Same as:
var item2 = ((MultilistField)item.Fields["Tags"]).GetItems().FirstOrDefault();
```

- Example 3:
```csharp
var item1 = item.SelectItem("Related News:Last");

// Same as:
var item2 = ((MultilistField)item.Fields["Related News"]).GetItems().LastOrDefault();
```

- Example 4:
```csharp
var item1 = item.SelectItem("Ranking[3]");

// Same as:
var item2 = ((MultilistField)item.Fields["Ranking"]).GetItems()[3];
```

- Example 5:
```csharp
var item1 = item.SelectItem("/Data Folder/Metadata");

// Same as:
var item2 = item.Children["Data Folder"].Children["Metadata"];
```

- Example 6:
```csharp
var item1 = item.SelectItem("Related News:First.Category/Data");

// Same as:
var news = ((MultilistField)item.Fields["Related News"]).GetItems().FirstOrDefault();
var item2 = ((LinkField)news.Fields["Category"]).TargetItem.Children["Data"];
```

### SelectAllItems
Returns all items that are hit by the selector.

- Example 1:
```csharp
var items1 = item.SelectAllItems("Tags:*");

// Same as:
var items2 = ((MultilistField)item.Fields["Tags"]).GetItems().ToList();
```

- Example 2:
```csharp
var items1 = item.SelectAllItems("Categories:*.Color");

// Same as:
var items2 = ((MultilistField)item.Fields["Categories"])
    .GetItems()
    .Select(category => ((LinkField)category.Fields["Color"]).TargetItem)
    .ToList();
```

- Example 3:
```csharp
var items1 = item.SelectAllItems("Related News:*/Data");

// Same as:
var items2 = ((MultilistField)item.Fields["Related News"])
    .GetItems()
    .Select(news => news.Children["Data"])
    .ToList();
```

- Example 4:
```csharp
var items1 = item.SelectAllItems("Related News:*/Data.Categories:*.Group");

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
|`.`|Select a item referred in a link field.|`Category.Color.Code`|
|`:First`|Select a first item referred in a multilist field.|`Tags:First.Name`|
|`:Last`|Select a last item referred in a multilist field.|`Tags:Last.Name`|
|`:*`|Select all items referred in a multilist field.|`Tags:*.Name`|
|`[N]`|Select a `N`th item refered in a multilist field.|`Tags[3].Name`|
|`/`|Select a child item.|`Data/Metadata`|

## Author
- Takumi Yamada (xirtardauq@gmail.com)

## License
*Sitecore.ItemSelector* is licensed under the MIT license. See LICENSE.