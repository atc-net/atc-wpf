# 🔄 TransitioningContentControl

A `ContentControl` that animates the transition between old and new content using visual states.

## 🔍 Overview

`TransitioningContentControl` keeps the previous content in one presenter and the new content in another, and plays a visual-state storyboard every time `Content` changes. The storyboard is selected by the `Transition` property (for example slide left/right/up/down), or by a custom visual state name. When the transition completes, the previous content is released and the `TransitionCompleted` event is raised. It is typically used for page/view navigation regions.

## 📍 Namespace

```csharp
using Atc.Wpf.Theming.Controls;
```

## 🚀 Usage

### Basic Example

```xml
<atc:TransitioningContentControl
    Content="{Binding CurrentView}"
    Transition="Left" />
```

### Required Template Structure

The control relies on its control template for the animations. A template must contain:

- a `ContentPresenter` named `PreviousContentPresentationSite` and one named `CurrentContentPresentationSite`
- a `VisualStateGroup` named `PresentationStates` with a `Hidden` state and a state for each transition used (for example `DefaultTransition`)

```xml
<ControlTemplate TargetType="{x:Type atc:TransitioningContentControl}">
    <Grid>
        <VisualStateManager.VisualStateGroups>
            <VisualStateGroup x:Name="PresentationStates">
                <VisualState x:Name="Hidden" />
                <VisualState x:Name="DefaultTransition">
                    <Storyboard>
                        <DoubleAnimation
                            Storyboard.TargetName="CurrentContentPresentationSite"
                            Storyboard.TargetProperty="Opacity"
                            From="0"
                            To="1"
                            Duration="0:0:0.3" />
                        <DoubleAnimation
                            Storyboard.TargetName="PreviousContentPresentationSite"
                            Storyboard.TargetProperty="Opacity"
                            From="1"
                            To="0"
                            Duration="0:0:0.3" />
                    </Storyboard>
                </VisualState>
            </VisualStateGroup>
        </VisualStateManager.VisualStateGroups>
        <ContentPresenter x:Name="PreviousContentPresentationSite" />
        <ContentPresenter x:Name="CurrentContentPresentationSite" />
    </Grid>
</ControlTemplate>
```

### Custom Transition

```xml
<atc:TransitioningContentControl
    Content="{Binding CurrentView}"
    CustomVisualStatesName="MyFadeTransition"
    Transition="Custom">
    <atc:TransitioningContentControl.CustomVisualStates>
        <VisualState x:Name="MyFadeTransition">
            <Storyboard>
                <!-- animations targeting the presentation sites -->
            </Storyboard>
        </VisualState>
    </atc:TransitioningContentControl.CustomVisualStates>
</atc:TransitioningContentControl>
```

### Replaying the Transition from Code

```csharp
// Replays the current transition even though the content did not change
myTransitioningControl.ReloadTransition();

// Stops a running transition and releases the previous content
myTransitioningControl.AbortTransition();
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Transition` | `TransitionType` | `Default` | Transition to play when content changes (inherited by child elements) |
| `IsTransitioning` | `bool` | `false` | Whether a transition is running (read-only; setting it externally throws) |
| `RestartTransitionOnContentChange` | `bool` | `false` | Restart the transition when content changes while a transition is still running |
| `CustomVisualStates` | `ObservableCollection<VisualState>?` | empty collection | Extra visual states added to the `PresentationStates` group when the template is applied |
| `CustomVisualStatesName` | `string` | `"CustomTransition"` | Name of the visual state used when `Transition` is `Custom` |

## 📊 TransitionType Values

| Value | Visual State Name |
|-------|-------------------|
| `Default` | `DefaultTransition` |
| `Normal` | `Normal` |
| `Up` | `UpTransition` |
| `Down` | `DownTransition` |
| `Right` | `RightTransition` |
| `RightReplace` | `RightReplaceTransition` |
| `Left` | `LeftTransition` |
| `LeftReplace` | `LeftReplaceTransition` |
| `Custom` | value of `CustomVisualStatesName` |

## ⚡ Events

| Event | Type | Description |
|-------|------|-------------|
| `TransitionCompleted` | `RoutedEventHandler` | Raised when a transition storyboard has completed |

## 🔧 Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `ReloadTransition()` | `void` | Replays the current transition without changing the content |
| `AbortTransition()` | `void` | Goes to the `Hidden` state, clears `IsTransitioning` and releases the previous content |

## 📝 Notes

- The library does not currently ship a default style/template for `TransitioningContentControl`; supply a template with the structure shown above
- When the template is applied and the visual state for the current `Transition` cannot be found, `Transition` is reset to `Default` and an `AtcAppsException` is thrown
- Changing `Transition` to a value whose visual state does not exist reverts to the previous value
- A transition only starts when both presentation sites exist in the template
- `TransitionCompleted` is a plain CLR event (not a routed event), although it uses `RoutedEventHandler`

## 🔗 Related Controls

- **ContentControlEx** - Content control with additional content/text casing support used by theme templates
- **NiceWindow** - Themed window often hosting navigation regions

## 🎮 Sample Application

No dedicated sample yet.
