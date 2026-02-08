# 🔽 SplitButton

A button with a primary action and a dropdown menu for secondary actions.

## 🔍 Overview

`SplitButton` combines a primary action button with a dropdown arrow that opens a popup with additional options. Clicking the main area executes the primary command, while clicking the dropdown arrow reveals secondary content. The control supports keyboard navigation and MVVM command binding.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Buttons;
```

## 🚀 Usage

### Basic SplitButton

```xml
<buttons:SplitButton Content="Save" Command="{Binding SaveCommand}">
    <buttons:SplitButton.DropdownContent>
        <StackPanel>
            <MenuItem Header="Save As..." Command="{Binding SaveAsCommand}" />
            <MenuItem Header="Save All" Command="{Binding SaveAllCommand}" />
            <Separator />
            <MenuItem Header="Export..." Command="{Binding ExportCommand}" />
        </StackPanel>
    </buttons:SplitButton.DropdownContent>
</buttons:SplitButton>
```

### With Dropdown Template

```xml
<buttons:SplitButton Content="New" Command="{Binding NewItemCommand}">
    <buttons:SplitButton.DropdownContent>
        <StackPanel MinWidth="150">
            <Button Content="New Document" Margin="4" />
            <Button Content="New Folder" Margin="4" />
            <Button Content="New Project" Margin="4" />
        </StackPanel>
    </buttons:SplitButton.DropdownContent>
</buttons:SplitButton>
```

### 🎨 Custom Corner Radius

```xml
<buttons:SplitButton Content="Actions" CornerRadius="4">
    <buttons:SplitButton.DropdownContent>
        <StackPanel>
            <MenuItem Header="Action 1" />
            <MenuItem Header="Action 2" />
        </StackPanel>
    </buttons:SplitButton.DropdownContent>
</buttons:SplitButton>
```

### Controlled Dropdown

```xml
<buttons:SplitButton
    Content="Options"
    IsDropdownOpen="{Binding IsMenuOpen, Mode=TwoWay}">
    <buttons:SplitButton.DropdownContent>
        <StackPanel>
            <MenuItem Header="Option A" />
            <MenuItem Header="Option B" />
        </StackPanel>
    </buttons:SplitButton.DropdownContent>
</buttons:SplitButton>
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Command` | `ICommand?` | `null` | Command executed on primary button click |
| `CommandParameter` | `object?` | `null` | Parameter passed to the command |
| `DropdownContent` | `object?` | `null` | Content displayed in the dropdown popup |
| `DropdownContentTemplate` | `DataTemplate?` | `null` | Template for dropdown content |
| `IsDropdownOpen` | `bool` | `false` | Whether the dropdown is open (two-way bindable) |
| `CornerRadius` | `CornerRadius` | `0` | Corner radius for the button |

## 📡 Events

| Event | Type | Description |
|-------|------|-------------|
| `Click` | `RoutedEventHandler` | Raised when the primary button area is clicked |

## ⌨️ Keyboard Navigation

| Key | Action |
|-----|--------|
| `Enter` / `Space` | Execute primary action |
| `Alt+Down` / `F4` | Open dropdown |
| `Escape` | Close dropdown |

## 📝 Notes

- The primary button and dropdown arrow are separate click targets
- `Click` event and `Command` both fire on primary button click
- The dropdown popup auto-closes when clicking outside
- `IsDropdownOpen` syncs with the popup's open state

## 🔗 Related Controls

- **ImageButton** - Button with an image icon
- **ImageToggledButton** - Button that toggles between two images

## 🎮 Sample Application

See the SplitButton sample in the Atc.Wpf.Sample application under **Wpf.Controls > Buttons > SplitButton** for interactive examples.
