# Sitecore.ItemFieldSelector
A Sitecore library for getting an item's field with simple syntax.

## Installation
Download a package from [here](https://github.com/xirtardauq/Sitecore.ItemFieldSelector/releases) and install it from your local package source.

## Usage
*Sitecore.ItemFieldSelector* provides the following methods:

- [SelectField](#user-content-selectfield)
- [SelectAllFields](#user-content-selectallfields)

### SelectField
Returns a first field that are hit by the selector.

```csharp
// Get a "Category" field's "Name" field.
var field1 = item.SelectField("Category.Name");

// Get a "Tags" field's a first item's "Name" field.
var field2 = item.SelectField("Tags:First.Name");

// Get a "Related News" field's a last item's "Title" field.
var field3 = item.SelectField("Related News:Last.Title");

// Get a "Ranking" field's a third item's "First Name" field.
var field4 = item.SelectField("Ranking[3].First Name");

// Get a "Related News" field's a first item's "Category" field's a "Data" child item's a "Name" field.
var field5 = item.SelectField("Related News:First.Category/Data.Name");
```

### SelectAllFields
Returns all fields that are hit by the selector.

```csharp
// Get a "Name" field of all items that are referred in a "Categories" Multilist field.
var fields1 = item.SelectAllFields("Categories:*.Name");

// Get a "Title" field of a "Data" child item for all items referred in a "Related News" field.
var fields2 = item.SelectAllFields("Related News:*/Data.Title");
```

## Syntax
|Syntax|Description|Example|
|:-|:-|:-|
|`.`|Select a field referred in a link field.|`Category.Color.Code`|
|`:First`|Select a first item referred in a multilist field.|`Tags:First.Name`|
|`:Last`|Select a last item referred in a multilist field.|`Tags:Last.Name`|
|`:*`|Select all items referred in a multilist field.|`Tags:*.Name`|
|`[N]`|Select a `N`th item refered in a multilist field.|`Tags[3].Name`|
|`/`|Select a child item.|`Data/Metadata.Title`|

## Author
- Takumi Yamada (xirtardauq@gmail.com)

## License
*Sitecore.ItemFieldSelector* is licensed under the MIT license. See LICENSE.