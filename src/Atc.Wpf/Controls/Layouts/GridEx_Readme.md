# GridEx

The `GridEx` control is a extensions of the `Grid` control.

## Example for GridEx usages

```xml
<atc:GridEx Rows="2*,1*,1*" Columns="2*,1*,1,*">

	<Button>Button 1</Button>
	<Button Grid.Column="1">Button 2</Button>
	<Button Grid.Column="2">Button 3</Button>
	<Button Grid.Row="1">Button 4</Button>
	<Button Grid.Column="1" Grid.Row="1">Button 5</Button>
	<Button Grid.Column="2" Grid.Row="1">Button 6</Button>
	<Button Grid.Row="2">Button 7</Button>
	<Button Grid.Column="1" Grid.Row="2">Button 8</Button>
	<Button Grid.Column="2" Grid.Row="2">Button 9</Button>

</atc:GridEx>
```

## The equvalent example with Grid usages

```xml
<Grid>
	<Grid.ColumnDefinitions>
		<ColumnDefinition Width="2*" />
		<ColumnDefinition Width="1*" />
		<ColumnDefinition Width="1*" />
	</Grid.ColumnDefinitions>
	<Grid.RowDefinitions>
		<RowDefinition Height="2*" />
		<RowDefinition Height="1*" />
		<RowDefinition Height="1*" />
	</Grid.RowDefinitions>

	<Button>Button 1</Button>
	<Button Grid.Column="1">Button 2</Button>
	<Button Grid.Column="2">Button 3</Button>
	<Button Grid.Row="1">Button 4</Button>
	<Button Grid.Column="1" Grid.Row="1">Button 5</Button>
	<Button Grid.Column="2" Grid.Row="1">Button 6</Button>
	<Button Grid.Row="2">Button 7</Button>
	<Button Grid.Column="1" Grid.Row="2">Button 8</Button>
	<Button Grid.Column="2" Grid.Row="2">Button 9</Button>

</Grid>
```

> Note: ColumnDefinitions and RowDefinitions is reduced to Columns and Rows in GridEx :heart_eyes:

## Properties

| Property | Type        | Description                                                          |
|----------|-------------|----------------------------------------------------------------------|
| Rows     | string      | Comma seperated **Height** and allow `Auto`, `pixel`, `*`, `number*` |
| Columns  | string      | Comma seperated **Width** and allow `Auto`, `pixel`, `*`, `number*`  |
