using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Atc.Wpf.Controls
{
    /// <summary>
    /// GridEx is a extension of the <see cref="Grid" />.
    /// This GridEx supports easy definition of rows and columns.
    /// </summary>
    /// <example>
    /// <![CDATA[
    /// <!-- Example for https://www.wpf-tutorial.com/panels/grid-rows-and-columns
    /// Gives a grid for "3*3" where cell [0,0] is double the size as cell [2,2].
    /// -->
    /// <Grid Rows="2*,1*,1*" Columns="2*,1*,1*">
    /// <!-- grid content -->
    /// </Grid>
    ///
    /// <!-- Example for use of combination as:
    /// - pixels
    /// - auto (size to column content)
    /// - * (star - size proportional to grid) -->
    /// <Grid Rows="2*,Auto,*,66" Columns="2*,*,Auto">
    /// <!-- grid content -->
    /// </Grid>
    /// ]]>
    /// </example>
    [SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "OK.")]
    public class GridEx : Grid
    {
        /// <summary>
        /// The rows dependency property.
        /// </summary>
        private static readonly DependencyProperty RowsProperty = DependencyProperty.Register(
            nameof(Rows),
            typeof(string),
            typeof(GridEx),
            new PropertyMetadata(
                defaultValue: null,
                OnRowsChanged));

        /// <summary>
        /// The columns dependency property.
        /// </summary>
        private static readonly DependencyProperty ColumnsProperty = DependencyProperty.Register(
            nameof(Columns),
            typeof(string),
            typeof(GridEx),
            new PropertyMetadata(
                defaultValue: null,
                OnColumnsChanged));

        /// <summary>
        /// Gets or sets the rows.
        /// </summary>
        /// <value>The rows.</value>
        [Category("Layout")]
        [Description("The rows property.")]
        public string Rows
        {
            get => (string)GetValue(RowsProperty);
            set => SetValue(RowsProperty, value);
        }

        /// <summary>
        /// Gets or sets the columns.
        /// </summary>
        /// <value>The columns.</value>
        [Category("Layout")]
        [Description("The columns property.")]
        public string Columns
        {
            get => (string)GetValue(ColumnsProperty);
            set => SetValue(ColumnsProperty, value);
        }

        /// <summary>
        /// Called when the rows property is changed.
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="args">The <see ref="System.Windows.DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void OnRowsChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            // Get the gridEx.
            if (!(d is GridEx gridEx))
            {
                return;
            }

            // Clear any current rows definitions.
            gridEx.RowDefinitions.Clear();

            // Add each row from the row lengths definition.
            foreach (var rowLength in StringLengthsToGridLengths(gridEx.Rows))
            {
                gridEx.RowDefinitions.Add(
                    new RowDefinition
                    {
                        Height = rowLength,
                    });
            }
        }

        /// <summary>
        /// Called when the columns property is changed.
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void OnColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            // Get the gridEx.
            if (!(d is GridEx gridEx))
            {
                return;
            }

            // Clear any current rows definitions.
            gridEx.ColumnDefinitions.Clear();

            // Add each column from the column lengths definition.
            foreach (var columnLength in StringLengthsToGridLengths(gridEx.Columns))
            {
                gridEx.ColumnDefinitions.Add(
                    new ColumnDefinition
                    {
                        Width = columnLength,
                    });
            }
        }

        /// <summary>
        /// Turns a string of lengths, such as "3*,Auto,2000" into a set of gridLength.
        /// </summary>
        /// <param name="lengths">The string of lengths, separated by commas.</param>
        /// <returns>A list of GridLengths.</returns>
        private static IEnumerable<GridLength> StringLengthsToGridLengths(string lengths)
        {
            // Create the list of GridLengths.
            var gridLengths = new List<GridLength>();

            // If the string is null or empty, this is all we can do.
            if (string.IsNullOrEmpty(lengths))
            {
                return gridLengths;
            }

            // Split the string by comma.
            var sa = lengths.Split(',');

            // Create a grid length converter.
            GridLengthConverter gridLengthConverter = new GridLengthConverter();

            // Use the grid length converter to set each length.
            gridLengths.AddRange(sa.Select(length =>
            {
                var convertFromString = gridLengthConverter.ConvertFromString(length);
                return (GridLength)(GridLength?)convertFromString;
            }));

            return gridLengths;
        }
    }
}