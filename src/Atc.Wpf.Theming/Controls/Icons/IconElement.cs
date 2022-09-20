namespace Atc.Wpf.Theming.Controls.Icons
{
    /// <summary>
    /// Represents the base class for an icon UI element.
    /// </summary>
    public abstract class IconElement : Control
    {
        private bool isForegroundPropertyDefaultOrInherited = true;

        protected IconElement()
        {
            // Empty
        }

        static IconElement()
        {
            ForegroundProperty.OverrideMetadata(
                typeof(IconElement),
                new FrameworkPropertyMetadata(
                    SystemColors.ControlTextBrush,
                    FrameworkPropertyMetadataOptions.Inherits,
                    (sender, e) => ((IconElement)sender).OnForegroundPropertyChanged(e)));
        }

        protected void OnForegroundPropertyChanged(
            DependencyPropertyChangedEventArgs e)
        {
            var baseValueSource = DependencyPropertyHelper.GetValueSource(this, e.Property).BaseValueSource;
            isForegroundPropertyDefaultOrInherited = baseValueSource <= BaseValueSource.Inherited;
            UpdateInheritsForegroundFromVisualParent();
        }

        protected override void OnVisualParentChanged(
            DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);
            UpdateInheritsForegroundFromVisualParent();
        }

        private void UpdateInheritsForegroundFromVisualParent()
        {
            InheritsForegroundFromVisualParent
                = isForegroundPropertyDefaultOrInherited
                  && Parent is not null
                  && VisualParent is not null
                  && Parent != VisualParent;
        }

        internal static readonly DependencyPropertyKey InheritsForegroundFromVisualParentPropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(InheritsForegroundFromVisualParent),
            typeof(bool),
            typeof(IconElement),
            new PropertyMetadata(
                BooleanBoxes.FalseBox,
                (sender, e) => ((IconElement)sender).OnInheritsForegroundFromVisualParentPropertyChanged(e)));

        /// <summary>Identifies the <see cref="InheritsForegroundFromVisualParent"/> dependency property.</summary>
        public static readonly DependencyProperty InheritsForegroundFromVisualParentProperty = InheritsForegroundFromVisualParentPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets whether that this element inherits the <see cref="Control.Foreground"/> form the <see cref="Visual.VisualParent"/>.
        /// </summary>
        public bool InheritsForegroundFromVisualParent
        {
            get => (bool)GetValue(InheritsForegroundFromVisualParentProperty);
            protected set => SetValue(InheritsForegroundFromVisualParentPropertyKey, BooleanBoxes.Box(value));
        }

        protected virtual void OnInheritsForegroundFromVisualParentPropertyChanged(
            DependencyPropertyChangedEventArgs e)
        {
            ArgumentNullException.ThrowIfNull(e);

            if (e.OldValue == e.NewValue)
            {
                return;
            }

            if (e.NewValue is true)
            {
                SetBinding(
                    VisualParentForegroundProperty,
                    new Binding
                    {
                        Path = new PropertyPath(TextElement.ForegroundProperty),
                        Source = VisualParent,
                    });
            }
            else
            {
                ClearValue(VisualParentForegroundProperty);
            }
        }

        private static readonly DependencyProperty VisualParentForegroundProperty = DependencyProperty.Register(
            nameof(VisualParentForeground),
            typeof(Brush),
            typeof(IconElement),
            new PropertyMetadata(
                default(Brush),
                (sender, e) => ((IconElement)sender).OnVisualParentForegroundPropertyChanged(e)));

        protected Brush? VisualParentForeground
        {
            get => (Brush?)GetValue(VisualParentForegroundProperty);
            set => SetValue(VisualParentForegroundProperty, value);
        }

        protected virtual void OnVisualParentForegroundPropertyChanged(
            DependencyPropertyChangedEventArgs e)
        {
        }
    }
}