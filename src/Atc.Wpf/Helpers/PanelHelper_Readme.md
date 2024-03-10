# PanelHelper

The `PanelHelper` enhances the functionality of WPF Panel controls and any controls
derived from the `Panel` class. It serves as a versatile extension, offering additional
capabilities and utilities to streamline the management and manipulation of panel layouts
within your applications.

## Supported Controls

`PanelHelper` seamlessly integrates with a variety of panel controls in 
the WPF framework, including but not limited to:

- Grid
- GridEx
- DockPanel
- PanelEx
- StackPanel
- StaggeredPanel
- TabPanel
- UniformSpacingPanel
- WrapPanel

## Example for PanelHelper usages

```xml
<!-- Example: Set the horizontal spacing to 15 and vertical spacing to 20. -->
<WrapPanel
   atc:PanelHelper.HorizontalSpacing="15"
   atc:PanelHelper.VerticalSpacing="20">

   <!-- panel content -->

</WrapPanel>
```

```xml
<!-- Example: Set the horizontal and vertical spacing to 10. -->
<WrapPanel
   atc:PanelHelper.Spacing="10">

   <!-- panel content -->

</WrapPanel>
```

## The equvalent example with UniformSpacingPanel usages

```xml
<!-- Example: Set the horizontal spacing to 15 and vertical spacing to 20. -->
<UniformSpacingPanel
   HorizontalSpacing="15"
   VerticalSpacing="20">

   <!-- panel content -->

</UniformSpacingPanel>
```

## Properties

| Property          | Type        | Description                                                           |
|-------------------|-------------|-----------------------------------------------------------------------|
| HorizontalSpacing | double      | Set the horizontal spacing between the panel's items                  |
| VerticalSpacing   | double      | Set the vertical spacing between the panel's items                    |
| Spacing           | double      | Set both the  horizontal & vertical spacing between the panel's items |
