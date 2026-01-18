# StaggeredPanel

The `StaggeredPanel` control is a extensions of the `Panel` control.

The `StaggeredPanel` allows for layout of items in a column approach where an item will be 
added to whichever column has used the least amount of space.

## Properties

| Property          | Type        | Description                                                           |
|-------------------|-------------|-----------------------------------------------------------------------|
| DesiredItemWidth  | double      | The desired width of each column. The width of columns can exceed the DesiredColumnWidth if the HorizontalAlignment is set to Stretch. |
| Padding           | Thickness   | The dimensions of the space between the edge and its child as a Thickness value. |
| HorizontalSpacing | double      | Set the horizontal spacing between the panel's items                  |
| VerticalSpacing   | double      | Set the vertical spacing between the panel's items                    |