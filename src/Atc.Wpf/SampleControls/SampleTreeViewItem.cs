using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Atc.Wpf.SampleControls
{
    public class SampleTreeViewItem : TreeViewItem
    {
        /// <summary>
        /// The sample path property.
        /// </summary>
        public static readonly DependencyProperty SamplePathProperty = DependencyProperty.Register(
            nameof(SamplePath),
            typeof(string),
            typeof(SampleTreeViewItem),
            new UIPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the sample path.
        /// </summary>
        /// <value>
        /// The sample path.
        /// </value>
        public string SamplePath
        {
            get => (string)this.GetValue(SamplePathProperty);
            set => this.SetValue(SamplePathProperty, value);
        }

        /// <summary>
        /// Provides class handling for a MouseLeftButtonDown event.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            if (string.IsNullOrEmpty(this.SamplePath))
            {
                this.SetCurrentValue(IsExpandedProperty, !this.IsExpanded);
            }
            else
            {
                this.SetCurrentValue(IsSelectedProperty, true);
            }

            e.Handled = true;
        }
    }
}