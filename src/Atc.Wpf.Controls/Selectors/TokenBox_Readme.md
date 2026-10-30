# 🏷️ TokenBox

A text field where the user types to pick several values; each pick becomes a removable chip ("token") in the field.

## 🔍 Overview

`TokenBox` looks like a text box. The selected values are small input `Chip`s followed by the caret. Typing opens a suggestion pop-up below the field with the matching items (the typed text in bold, already-picked items left out); `Enter`, `Tab`, a separator or a click picks the highlighted suggestion.

Use it where a check-box drop-down does not fit:

- **Large lists** (5,000 users, all countries): typing "jen" is faster than scrolling a check list.
- **Open-ended values** (tags, labels, e-mail addresses): with `AllowCreate`, text that matches no item becomes a new token.
- **Pasting a list** ("a; b; c" from Excel or an e-mail): pasted text is split on the separators and on new lines, and every part becomes a token in one change.

For huge or remote lists, `SuggestionsProvider` replaces the filtering of `Items` with an asynchronous lookup that runs after a short pause in typing.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Selectors;
```

## 🚀 Usage

### Pick from a known list

```xml
<atc:TokenBox
    atc:TextBoxHelper.Watermark="Assign people"
    Items="{Binding Path=Users}"
    SelectedKeys="{Binding Path=AssignedUserKeys}" />
```

### Free tags

```xml
<atc:TokenBox
    atc:TextBoxHelper.Watermark="Add tags"
    AllowCreate="True"
    AllowEditTokens="True"
    Items="{Binding Path=KnownTags}"
    MaxSelectionCount="10"
    SelectedKeys="{Binding Path=Tags}"
    ShowClearButton="True" />
```

### Validate new values

```xml
<atc:TokenBox
    AllowCreate="True"
    CreateValidator="{Binding Path=ValidateEmail}"
    SelectedKeys="{Binding Path=Recipients}" />
```

```csharp
public Func<string, string?> ValidateEmail
    => x => x.Contains('@') ? null : "Not an e-mail address";
```

A refused value stays in the field with the theme's error border; `ErrorText` holds the message (also the tooltip).

### Assign the key of a new value

```csharp
tokenBox.TokenCreating += (_, e) =>
{
    if (e.Text.Length > 30)
    {
        e.Cancel = true;      // refuse it; the text stays in the field
        return;
    }

    e.Key = "tag:" + e.Text;  // default: the text itself
};
```

### Asynchronous lookup

```xml
<atc:TokenBox
    SelectedKeys="{Binding Path=AssignedUsers}"
    SuggestionDelay="0:0:0.25"
    SuggestionsProvider="{Binding Path=LookupUsers}" />
```

```csharp
public Func<string, CancellationToken, Task<IReadOnlyList<KeyValuePair<string, string>>>> LookupUsers
    => async (text, cancellationToken) => await userService.SearchAsync(text, cancellationToken);
```

While a lookup waits or runs, the pop-up shows "Please wait...". A new keystroke cancels the previous lookup, and a result for older text is dropped even if the provider ignores the token. A picked suggestion is added to `Items`.

### One line

```xml
<atc:TokenBox
    Items="{Binding Path=Countries}"
    MaxVisibleTokens="3"
    SelectedKeys="{Binding Path=SelectedCountries}"
    TokensWrap="False" />
```

## ⚙️ Properties

| Property | Type | Default | Description |
|---|---|---|---|
| `Items` | `Dictionary<string, string>` | empty | The known values, key → display value |
| `SelectedKeys` | `ObservableCollection<string>` | empty | The token keys in token order; two-way by default, updated in place |
| `Text` | `string` | `""` | The typed text (two-way) |
| `AllowCreate` | `bool` | `false` | Text that matches no item becomes a new token on `Enter`, `Tab` or a separator |
| `CreateValidator` | `Func<string, string?>?` | `null` | Returns an error text to refuse a new value |
| `Separators` | `string` | `",;"` | Typing one commits the typed text; pasted text is split on them and on new lines |
| `FilterMode` | `TokenFilterMode` | `Contains` | `StartsWith`, `Contains` or `Custom` |
| `CustomFilter` | `Func<string, KeyValuePair<string, string>, bool>?` | `null` | Used when `FilterMode = Custom` |
| `MinimumPrefixLength` | `int` | `1` | Characters before suggestions show; `0` shows them on focus |
| `MaxSuggestions` | `int` | `50` | The most rows in the pop-up |
| `TokensWrap` | `bool` | `true` | `true`: the field grows in height; `false`: one line that scrolls while focused and ends in "+N" when not |
| `MaxVisibleTokens` | `int?` | `null` | When not focused, the most tokens before a "+N" chip; all show again on focus |
| `MaxSelectionCount` | `int?` | `null` | At the limit typing is blocked and the field shows "{0} of max {1}". `null` (no limit) or at least 1; a lower value throws an `ArgumentException`. |
| `FixedKeys` | `ObservableCollection<string>` | empty | Tokens that cannot be removed (no ×; `Backspace` and Clear skip them) |
| `AllowEditTokens` | `bool` | `false` | Double-click a created token to edit it (`Enter` / a click elsewhere commits, `Esc` cancels) |
| `ShowClearButton` | `bool` | `false` | An × at the end of the field removes every token that is not fixed (one change) |
| `ItemTemplate` | `DataTemplate?` | `null` | A suggestion row; data context `TokenSuggestion` (`Key`, `DisplayValue`, `TextBeforeMatch`, `MatchedText`, `TextAfterMatch`) |
| `TokenTemplate` | `DataTemplate?` | `null` | A token; data context `MultiSelectChipItem` (`Key`, `DisplayValue`, `CanRemove`, `RemoveCommand`) |
| `SuggestionsProvider` | `Func<string, CancellationToken, Task<IReadOnlyList<KeyValuePair<string, string>>>>?` | `null` | Asynchronous lookup that replaces the filtering of `Items` |
| `SuggestionDelay` | `TimeSpan` | 250 ms | The pause in typing before `SuggestionsProvider` runs |
| `IsDropDownOpen` | `bool` | `false` | Whether the suggestion pop-up is open (two-way) |
| `MaxDropDownHeight` | `double` | as `ComboBox` | The pop-up's maximum height (the list is virtualized) |
| `ErrorText` | `string` | `""` | Read-only: the `CreateValidator` error for the typed text |
| `HasError` | `bool` | `false` | Read-only: whether `ErrorText` is set |
| `TextBoxHelper.Watermark` | `string` | — | Shown while there are no tokens |

## 🔧 Methods

| Method | Description |
|---|---|
| `ClearTokens()` | Removes every token that is not fixed and clears the typed text (one change) |

## 📢 Events

| Event | Args | When |
|---|---|---|
| `SelectedKeysChanged` | `ValueChangedEventArgs<IReadOnlyList<string>>` | Once per pick, removal, paste, edit, Clear, or `Items` change that drops tokens |
| `TokenCreating` | `TokenCreatingEventArgs` (`Text`, settable `Key`, `Cancel`) | Before a value that matches no item becomes a token |

## ⌨️ Keyboard

| Key | Action |
|---|---|
| `↓` / `↑` | Move in the suggestions (opens them when closed) |
| `Enter`, `Tab`, a separator | Pick the highlighted suggestion, or create (when allowed) |
| `Backspace` in an empty text | Select the last removable token; a second `Backspace` removes it |
| `←` / `→` at the text start | Move the selection across the tokens; `Delete` removes the selected one |
| `Ctrl+V` | Paste and split |
| `Esc` | Close the suggestions; a second `Esc` clears the typed text |

## ♿ UI Automation

- The field is a **combo box**: its Value is the typed text, and `ExpandCollapse` opens or closes the suggestions. Setting a Value that contains a separator or a new line is handled as a paste.
- Its children are the **token list** (AutomationId `Tokens`, a list whose items are named by their display value, each with a "Remove {name}" button), the **edit** control the user types in, the **Clear** button (when shown) and, while open, the **suggestion list** (AutomationId `Suggestions`, with the `Selection` pattern; selecting a row picks it).

## 📝 Notes

- When `SelectedKeys` is data-bound, the keys dropped (or ticked) because `Items` changed are written once the bindings have settled (one dispatcher pass later), so a DataContext swap never writes into the previous view model's collection; unbound, it happens at once.
- **Matching typed text:** a part matches an item when it equals its display value (case-insensitive) or its key.
- **Highlight:** without `AllowCreate` the first suggestion is highlighted, so `Enter` picks it. With `AllowCreate` only a suggestion equal to the typed text is highlighted, so `Enter` creates the typed value instead of picking a longer one (use `↓` to pick a suggestion).
- **Unknown text:** without `AllowCreate`, text that matches nothing stays in the field; pasted parts that match nothing stay there too (joined by the first separator).
- **Created values:** the key is the text itself unless `TokenCreating` sets another; the pair is added to `Items`. While `AllowCreate` is set, keys in `SelectedKeys` that are not in `Items` show as their key.
- **Duplicates** are ignored (by key, or by display value ignoring case); pasting "a, a, b" adds a and b.
- **Items changes** drop tokens whose keys are gone (one event), except created values and values picked from `SuggestionsProvider`. With `AllowCreate`, keys that were never in `Items` (free values set from the view model) are kept too.
- **Limit:** a paste past `MaxSelectionCount` adds tokens up to the limit and drops the rest.
- **Token order** follows `SelectedKeys`; new tokens are appended.
- The texts ("+{0}", "Remove {0}", "{0} of max {1}", "No matches", "Please wait...", "Clear") come from the shared `Atc.Wpf.Resources.Miscellaneous` resources (English, Danish, German).

## 🔗 Related Controls

- **LabelTokenBox** - The form field around this control (label, mandatory marker, validation)
- **MultiSelectComboBox** - 3–1,000 known items in a drop-down with check boxes
- **CheckListBox** - An always-visible check list
- **CheckTreeView** / **TreeSelectComboBox** - Hierarchies with tri-state check boxes
- **ChipGroup** - 2–15 short options as toggle chips
- **Segmented** - 2–6 very short options as a joined button row
- **DualListSelector** - Building an ordered list out of a pool (two lists with transfer buttons)
- **CardSelector** - Few, visual choices

## 🎮 Sample Application

See the sample application under **Wpf.Controls > Selectors > TokenBox** for a known list with an event log (its options can be switched in the side panel), free tags with a limit, editing and a Clear button, e-mail validation, an asynchronous lookup of 10,000 users, one line vs wrap with `MaxVisibleTokens`, fixed tokens and a disabled field.
