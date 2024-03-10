# UniformSpacingPanel

The `UniformSpacingPanel` control is a extensions of the `Panel` control.

## Example for UniformSpacingPanel usages

```xml
<!-- Example: Set the horizontal spacing to 15 and vertical spacing to 20. -->
<UniformSpacingPanel
   HorizontalSpacing="15"
   VerticalSpacing="20"
   Orientation="Horizontal">

   <!-- panel content -->

</UniformSpacingPanel>
```

## The equvalent example with PanelHelper usages

```xml
<!-- Example: Set the horizontal spacing to 15 and vertical spacing to 20. -->
<WrapPanel
   atc:PanelHelper.HorizontalSpacing="15"
   atc:PanelHelper.VerticalSpacing="20"
   Orientation="Horizontal">

   <!-- panel content -->

</WrapPanel>
```


## Properties

| Property          | Type        | Description                                                           |
|-------------------|-------------|-----------------------------------------------------------------------|
| HorizontalSpacing | double      | Set the horizontal spacing between the panel's items                  |
| VerticalSpacing   | double      | Set the vertical spacing between the panel's items                    |
| Spacing           | double      | Set both the  horizontal & vertical spacing between the panel's items |
| Orientation       | Orientation | Set the orientation for rendering order                               |
