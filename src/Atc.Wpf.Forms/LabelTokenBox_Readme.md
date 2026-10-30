# 🏷️ LabelTokenBox

A labeled form field around `TokenBox`: a text field where the user types to pick several values (each a removable chip), with a label, mandatory marker, information text and validation.

## 🔍 Overview

`LabelTokenBox` puts a `TokenBox` in the standard `LabelContent` layout used by the other `Label*` controls. `IsMandatory` means at least one token is required. It implements `ILabelTokenBox`, so it works in `LabelControlsForm` (`GetKeyValues()` returns the token keys as a list, which `LabelControlsFormToModelWriter` writes back to a `List<string>` / `IList<string>` property) and can be created with `LabelControlFactory.CreateLabelTokenBox(...)`.

## 📍 Namespace

```csharp
using Atc.Wpf.Forms;
```

## 🚀 Usage

```xml
<atc:LabelTokenBox
    AllowCreate="True"
    Items="{Binding Path=KnownTags}"
    LabelText="Tags"
    MaxSelectionCount="10"
    SelectedKeys="{Binding Path=Tags}"
    WatermarkText="Add tags" />
```

```xml
<atc:LabelTokenBox
    InformationText="Pick at least one person."
    IsMandatory="True"
    LabelText="Assigned to"
    Orientation="Vertical"
    SelectedKeys="{Binding Path=AssignedUsers}"
    SuggestionsProvider="{Binding Path=LookupUsers}" />
```

```csharp
var control = LabelControlFactory.CreateLabelTokenBox(
    groupIdentifier: "Issue",
    labelText: "Labels",
    isReadOnly: false,
    isMandatory: true,
    items: knownLabels,
    selectedKeys: ["bug"],
    allowCreate: true);
```

## ⚙️ Properties

Besides the common label properties (`LabelText`, `IsMandatory`, `InformationText`, `ValidationText`, `Orientation`, `LabelPosition`, `LabelWidthNumber`, `ContentMinHeight`, …):

| Property | Type | Default | Description |
|---|---|---|---|
| `Items` | `Dictionary<string, string>` | empty | The known values, key → display value |
| `SelectedKeys` | `ObservableCollection<string>` | empty | The token keys; two-way by default, updated in place |
| `WatermarkText` | `string` | `""` | Shown in the empty field |
| `AllowCreate` | `bool` | `false` | Text that matches no item becomes a new token |
| `CreateValidator` | `Func<string, string?>?` | `null` | Returns an error text to refuse a new value |
| `Separators` | `string` | `",;"` | Commit characters; pasted text is split on them and on new lines |
| `FilterMode` | `TokenFilterMode` | `Contains` | `StartsWith`, `Contains` or `Custom` |
| `CustomFilter` | `Func<string, KeyValuePair<string, string>, bool>?` | `null` | Used when `FilterMode = Custom` |
| `MinimumPrefixLength` | `int` | `1` | Characters before suggestions show; `0` shows them on focus |
| `MaxSuggestions` | `int` | `50` | The most rows in the pop-up |
| `TokensWrap` | `bool` | `true` | `false`: one line that scrolls |
| `MaxVisibleTokens` | `int?` | `null` | When not focused, the most tokens before "+N" |
| `MaxSelectionCount` | `int?` | `null` | Token limit |
| `FixedKeys` | `ObservableCollection<string>` | empty | Tokens that cannot be removed |
| `AllowEditTokens` | `bool` | `false` | Double-click a created token to edit it |
| `ShowClearButton` | `bool` | `false` | An × that removes every token that is not fixed |
| `ItemTemplate` | `DataTemplate?` | `null` | A suggestion row (data context: `TokenSuggestion`) |
| `TokenTemplate` | `DataTemplate?` | `null` | A token (data context: `MultiSelectChipItem`) |
| `SuggestionsProvider` | `Func<string, CancellationToken, Task<IReadOnlyList<KeyValuePair<string, string>>>>?` | `null` | Asynchronous lookup that replaces the filtering of `Items` |
| `SuggestionDelay` | `TimeSpan` | 250 ms | The pause in typing before the lookup runs |
| `MaxDropDownHeight` | `double` | as `ComboBox` | The pop-up's maximum height |

## 📢 Events

| Event | Args | When |
|---|---|---|
| `SelectedKeysChanged` | `ValueChangedEventArgs<IReadOnlyList<string>>` | The tokens changed through the field (or an `Items` change) and the field is valid |
| `SelectedKeysChangedInvalid` | `ValueChangedEventArgs<IReadOnlyList<string>>` | The tokens changed through the field and the field is invalid (mandatory with no tokens, or more tokens than `MaxSelectionCount`) |
| `TokenCreating` | `TokenCreatingEventArgs` | Before a value that matches no item becomes a token (set `Key`, or `Cancel`) |

## 📝 Notes

- `IsValid()` validates now and sets `ValidationText`: "Field is required" when mandatory and there are no tokens, "Select at most {0}" (translated) when `SelectedKeys` set from code holds more keys than `MaxSelectionCount` (the keys are kept; the field itself never goes beyond the limit).
- `SelectedKeys` binds two-way by default: bind it to a property with a setter, or use `Mode=OneWay`.
- Typing, pasting, keyboard and UI Automation behaviour comes from `TokenBox`; see `TokenBox_Readme.md`.

## 🔗 Related Controls

- **TokenBox** - The field without the label
- **LabelMultiSelectComboBox** - 3–1,000 known items in a drop-down with check boxes
- **LabelCheckListBox** - An always-visible check list
- **CheckTreeView** / **TreeSelectComboBox** - Hierarchies with tri-state check boxes
- **LabelChipGroup** - 2–15 short options as toggle chips
- **Segmented** - 2–6 very short options as a joined button row
- **DualListSelector** - Building an ordered list out of a pool (two lists with transfer buttons)
- **CardSelector** - Few, visual choices

## 🎮 Sample Application

See the sample application under **Wpf.Forms > Label Controls > LabelTokenBox**, including mandatory fields, a limit and one-line mode.
