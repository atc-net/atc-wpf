using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Atc.Wpf.Controls.W3cSvg;
using Atc.Wpf.Controls.W3cSvg.FileLoaders;

// ReSharper disable ConvertSwitchStatementToSwitchExpression
// ReSharper disable InvertIf
// ReSharper disable SwitchStatementHandlesSomeKnownEnumValuesWithDefault
// ReSharper disable SwitchStatementMissingSomeEnumCasesNoDefault
namespace Atc.Wpf.Controls
{
    /// <summary>
    /// This is the SVG image view control.
    /// The image control can either load the image from a file <see cref="SetImage(string)"/> or by
    /// setting the <see cref="Drawing"/> object through <see cref="SetImage(Drawing)"/>, which allows
    /// multiple controls to share the same drawing instance.
    /// </summary>
    [SuppressMessage("Style", "IDE0066:Convert switch statement to expression", Justification = "OK.")]
    public class SvgImage : Control
    {
        private readonly TranslateTransform translateTransform = new TranslateTransform();
        private readonly ScaleTransform scaleTransform = new ScaleTransform();
        private Drawing? drawing;
        private SvgRender? svgRender;
        private Action<SvgRender>? loadImage;

        private static new readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(
            nameof(Background),
            typeof(Brush),
            typeof(SvgImage),
            new FrameworkPropertyMetadata(
                default(Brush),
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
                OnBackgroundChanged));

        private static readonly DependencyProperty ControlSizeTypeProperty = DependencyProperty.Register(
            nameof(ControlSizeType),
            typeof(ControlSizeType),
            typeof(SvgImage),
            new FrameworkPropertyMetadata(
                ControlSizeType.ContentToSizeNoStretch,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
                OnControlSizeTypeChanged));

        private static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
            nameof(Source),
            typeof(string),
            typeof(SvgImage),
            new FrameworkPropertyMetadata(
                defaultValue: null,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
                OnSourceChanged));

        private static readonly DependencyProperty FileSourceProperty = DependencyProperty.Register(
            nameof(FileSource),
            typeof(string),
            typeof(SvgImage),
            new PropertyMetadata(OnFileSourceChanged));

        private static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register(
            nameof(ImageSource),
            typeof(Drawing),
            typeof(SvgImage),
            new FrameworkPropertyMetadata(
                defaultValue: null,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
                OnImageSourceChanged));

        private static readonly DependencyProperty UseAnimationsProperty = DependencyProperty.Register(
            nameof(UseAnimations),
            typeof(bool),
            typeof(SvgImage),
            new PropertyMetadata(defaultValue: true));

        private static readonly DependencyProperty OverrideColorProperty = DependencyProperty.Register(
            nameof(OverrideColor),
            typeof(Color?),
            typeof(SvgImage),
            new PropertyMetadata(propertyChangedCallback: null));

        private static readonly DependencyProperty OverrideStrokeWidthProperty = DependencyProperty.Register(
            nameof(OverrideStrokeWidth),
            typeof(double?),
            typeof(SvgImage),
            new FrameworkPropertyMetadata(
                default,
                FrameworkPropertyMetadataOptions.AffectsRender,
                OverrideStrokeWidthPropertyChanged));

        public static readonly DependencyProperty CustomBrushesProperty = DependencyProperty.Register(
            nameof(CustomBrushes),
            typeof(Dictionary<string, Brush>),
            typeof(SvgImage),
            new FrameworkPropertyMetadata(
                default,
                FrameworkPropertyMetadataOptions.AffectsRender,
                CustomBrushesPropertyChanged));

        public static readonly DependencyProperty ExternalFileLoaderProperty = DependencyProperty.Register(
            nameof(ExternalFileLoader),
            typeof(IExternalFileLoader),
            typeof(SvgImage),
            new PropertyMetadata(FileSystemLoader.Instance));

        static SvgImage()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(SvgImage),
                new FrameworkPropertyMetadata(typeof(SvgImage)));
            ClipToBoundsProperty.OverrideMetadata(
                typeof(SvgImage),
                new FrameworkPropertyMetadata(defaultValue: true));
            SnapsToDevicePixelsProperty.OverrideMetadata(
                typeof(SvgImage),
                new FrameworkPropertyMetadata(defaultValue: true));
        }

        public SvgImage()
        {
            this.ClipToBounds = true;
            this.SnapsToDevicePixels = true;
        }

        public new Brush? Background
        {
            get => (Brush)GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }

        public ControlSizeType ControlSizeType
        {
            get => (ControlSizeType)GetValue(ControlSizeTypeProperty);
            set => SetValue(ControlSizeTypeProperty, value);
        }

        public string Source
        {
            get => (string)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        public string FileSource
        {
            get => (string)GetValue(FileSourceProperty);
            set => SetValue(FileSourceProperty, value);
        }

        public Drawing ImageSource
        {
            get => (Drawing)GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        public bool UseAnimations
        {
            get => (bool)GetValue(UseAnimationsProperty);
            set => SetValue(UseAnimationsProperty, value);
        }

        public Color? OverrideColor
        {
            get => (Color?)GetValue(OverrideColorProperty);
            set => SetValue(OverrideColorProperty, value);
        }

        public double? OverrideStrokeWidth
        {
            get => (double?)GetValue(OverrideStrokeWidthProperty);
            set => SetValue(OverrideStrokeWidthProperty, value);
        }

        [SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "OK.")]
        public Dictionary<string, Brush> CustomBrushes
        {
            get => (Dictionary<string, Brush>)GetValue(CustomBrushesProperty);
            set => SetValue(CustomBrushesProperty, value);
        }

        public IExternalFileLoader ExternalFileLoader
        {
            get => (IExternalFileLoader)GetValue(ExternalFileLoaderProperty);
            set => SetValue(ExternalFileLoaderProperty, value);
        }

        internal Svg? Svg => this.svgRender?.Svg;

        public void ReRenderSvg()
        {
            if (this.svgRender is not null)
            {
                var svgDrawing = this.svgRender.CreateDrawing(this.svgRender.Svg);
                this.SetImage(svgDrawing);
            }
        }

        public void SetImage(string svgFileName)
        {
            this.loadImage = render =>
            {
                this.SetImage(render.LoadDrawing(svgFileName));
            };

            if (!this.IsInitialized &&
                !DesignerProperties.GetIsInDesignMode(this))
            {
                return;
            }

            this.InitializeSvgRender();

            this.loadImage(this.svgRender);
            this.loadImage = null;
        }

        public void SetImage(Stream svgStream)
        {
            this.loadImage = render =>
            {
                this.SetImage(render.LoadDrawing(svgStream));
            };

            if (!this.IsInitialized &&
                !DesignerProperties.GetIsInDesignMode(this))
            {
                return;
            }

            this.InitializeSvgRender();

            this.loadImage(svgRender);
            this.loadImage = null;
        }

        public void SetImage(Drawing svgDrawing)
        {
            this.drawing = svgDrawing;
            this.InvalidateVisual();
            if (this.drawing is not null)
            {
                this.InvalidateMeasure();
            }

            this.ReCalculateImageSize();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            if (this.loadImage is null)
            {
                return;
            }

            this.InitializeSvgRender();

            this.loadImage(this.svgRender);
            this.loadImage = null;
            var brushesFromSvg = new Dictionary<string, Brush>(StringComparer.Ordinal);
            if (this.svgRender.Svg is not null)
            {
                foreach (var (key, value) in svgRender.Svg.PaintServers.GetServers())
                {
                    brushesFromSvg[key] = value.GetBrush();
                }
            }

            this.CustomBrushes = brushesFromSvg;
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            this.ReCalculateImageSize();
            this.InvalidateVisual();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (drawingContext is null)
            {
                throw new ArgumentNullException(nameof(drawingContext));
            }

            if (this.drawing is null)
            {
                return;
            }

            if (this.Background is not null)
            {
                // Notice TemplateBinding background must be removed from the Border in the default template (or remove Border from the template)
                // Border renders the background AFTER the child render has been called
                // http://social.msdn.microsoft.com/Forums/en-US/wpf/thread/1575d2af-8e86-4085-81b8-a8bf24268e51/?prof=required
                drawingContext.DrawRectangle(
                    this.Background,
                    pen: null,
                    new Rect(0, 0, this.ActualWidth, this.ActualHeight));
            }

            drawingContext.PushTransform(this.translateTransform);
            drawingContext.PushTransform(this.scaleTransform);
            drawingContext.DrawDrawing(this.drawing);
            drawingContext.Pop();
            drawingContext.Pop();
        }

        protected override Size MeasureOverride(Size constraint)
        {
            var size = base.MeasureOverride(constraint);
            if (this.ControlSizeType == ControlSizeType.SizeToContent &&
                this.drawing is not null &&
                !this.drawing.Bounds.Size.IsEmpty)
            {
                size = this.drawing.Bounds.Size;
            }

            if (constraint.Width > 0 && constraint.Width < size.Width)
            {
                size.Width = constraint.Width;
            }

            if (constraint.Height > 0 && constraint.Height < size.Height)
            {
                size.Height = constraint.Height;
            }

            return size;
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            var size = base.ArrangeOverride(arrangeBounds);
            if (this.ControlSizeType == ControlSizeType.SizeToContent &&
                this.drawing is not null &&
                !this.drawing.Bounds.Size.IsEmpty)
            {
                size = this.drawing.Bounds.Size;
            }

            if (arrangeBounds.Width > 0 && arrangeBounds.Width < size.Width)
            {
                size.Width = arrangeBounds.Width;
            }

            if (arrangeBounds.Height > 0 && arrangeBounds.Height < size.Height)
            {
                size.Height = arrangeBounds.Height;
            }

            return size;
        }

        private static void OnBackgroundChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (obj is not SvgImage svgImage)
            {
                return;
            }

            svgImage.ReRenderSvg();
        }

        private static void OnControlSizeTypeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (obj is not SvgImage svgImage)
            {
                return;
            }

            svgImage.ReCalculateImageSize();
        }

        private static void OnSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (obj is not SvgImage svgImage)
            {
                return;
            }

            var uri = args.NewValue.ToString();
            var resource = Application.GetResourceStream(new Uri(uri, UriKind.Relative));
            if (resource is not null)
            {
                svgImage.SetImage(resource.Stream);
            }
        }

        private static void OnFileSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (obj is not SvgImage svgImage)
            {
                return;
            }

            var uri = args.NewValue.ToString();
            using var fileStream = new FileStream(uri, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var memoryStream = (MemoryStream)fileStream.CopyToStream();
            svgImage.SetImage(memoryStream);
        }

        private static void OnImageSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (obj is not SvgImage svgImage)
            {
                return;
            }

            svgImage.SetImage(args.NewValue as Drawing);
        }

        private static void OverrideStrokeWidthPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (obj is not SvgImage svgImage ||
                args.NewValue is not double newStrokeWidth ||
                svgImage.svgRender is null)
            {
                return;
            }

            svgImage.svgRender.OverrideStrokeWidth = newStrokeWidth;
            svgImage.InvalidateVisual();
            svgImage.ReRenderSvg();
        }

        private static void CustomBrushesPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (obj is not SvgImage svgImage ||
                args.NewValue is not Dictionary<string, Brush> newBrushes)
            {
                return;
            }

            if (svgImage.svgRender != null)
            {
                if (svgImage.svgRender.CustomBrushes != null)
                {
                    var newCustomBrushes = new Dictionary<string, Brush>(svgImage.svgRender.CustomBrushes, StringComparer.Ordinal);
                    foreach (var (key, value) in newBrushes)
                    {
                        newCustomBrushes[key] = value;
                    }

                    svgImage.svgRender.CustomBrushes = newCustomBrushes;
                }
                else
                {
                    svgImage.svgRender.CustomBrushes = newBrushes;
                }
            }

            svgImage.InvalidateVisual();
            svgImage.ReRenderSvg();
        }

        private void InitializeSvgRender() =>
            this.svgRender = new SvgRender
            {
                ExternalFileLoader = this.ExternalFileLoader,
                OverrideColor = this.OverrideColor,
                CustomBrushes = this.CustomBrushes,
                OverrideStrokeWidth = this.OverrideStrokeWidth,
                UseAnimations = this.UseAnimations,
            };

        [SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK - for now.")]
        private void ReCalculateImageSize()
        {
            if (this.drawing == null)
            {
                return;
            }

            var rect = this.drawing.Bounds;
            switch (this.ControlSizeType)
            {
                case ControlSizeType.None:
                    this.scaleTransform.ScaleX = 1;
                    this.scaleTransform.ScaleY = 1;
                    switch (this.HorizontalContentAlignment)
                    {
                        case HorizontalAlignment.Center:
                            this.translateTransform.X = (this.ActualWidth / 2) - (rect.Width / 2) - rect.Left;
                            break;
                        case HorizontalAlignment.Right:
                            this.translateTransform.X = this.ActualWidth - rect.Right;
                            break;
                        default:
                            // Move to left by default
                            this.translateTransform.X = -rect.Left;
                            break;
                    }

                    switch (this.VerticalContentAlignment)
                    {
                        case VerticalAlignment.Center:
                            this.translateTransform.Y = (this.ActualHeight / 2) - (rect.Height / 2);
                            break;
                        case VerticalAlignment.Bottom:
                            this.translateTransform.Y = this.ActualHeight - rect.Height - rect.Top;
                            break;
                        default:
                            // Move to top by default
                            this.translateTransform.Y = -rect.Top;
                            break;
                    }

                    break;
                case ControlSizeType.ContentToSizeNoStretch:
                    this.SizeToContentNoStretch();
                    break;
                case ControlSizeType.ContentToSizeStretch:
                    double xScale = this.ActualWidth / rect.Width;
                    double yScale = this.ActualHeight / rect.Height;
                    this.scaleTransform.CenterX = rect.Left;
                    this.scaleTransform.CenterY = rect.Top;
                    this.scaleTransform.ScaleX = xScale;
                    this.scaleTransform.ScaleY = yScale;

                    // Move to top-left by default
                    this.translateTransform.X = -rect.Left;
                    this.translateTransform.Y = -rect.Top;
                    break;
                case ControlSizeType.SizeToContent when rect.Width > this.ActualWidth || rect.Height > this.ActualHeight:
                    this.SizeToContentNoStretch();
                    break;
                case ControlSizeType.SizeToContent:
                    this.scaleTransform.CenterX = rect.Left;
                    this.scaleTransform.CenterY = rect.Top;
                    this.scaleTransform.ScaleX = 1;
                    this.scaleTransform.ScaleY = 1;

                    // Move to top-left by default
                    this.translateTransform.X = -rect.Left;
                    this.translateTransform.Y = -rect.Top;
                    break;
                default:
                    throw new SwitchExpressionException(this.ControlSizeType);
            }
        }

        private void SizeToContentNoStretch()
        {
            var rect = this.drawing!.Bounds;
            double xScale = this.ActualWidth / rect.Width;
            double yScale = this.ActualHeight / rect.Height;
            double scale = xScale;
            if (scale > yScale)
            {
                scale = yScale;
            }

            this.scaleTransform.CenterX = rect.Left;
            this.scaleTransform.CenterY = rect.Top;
            this.scaleTransform.ScaleX = scale;
            this.scaleTransform.ScaleY = scale;

            this.translateTransform.X = -rect.Left;
            if (scale < xScale)
            {
                switch (this.HorizontalContentAlignment)
                {
                    case HorizontalAlignment.Center:
                        double width = rect.Width * scale;
                        this.translateTransform.X = (this.ActualWidth / 2) - (width / 2) - rect.Left;
                        break;
                    case HorizontalAlignment.Right:
                        this.translateTransform.X = this.ActualWidth - (rect.Right * scale);
                        break;
                }
            }

            this.translateTransform.Y = -rect.Top;
            if (scale < yScale)
            {
                switch (this.VerticalContentAlignment)
                {
                    case VerticalAlignment.Center:
                        double height = rect.Height * scale;
                        this.translateTransform.Y = (this.ActualHeight / 2) - (height / 2) - rect.Top;
                        break;
                    case VerticalAlignment.Bottom:
                        this.translateTransform.Y = this.ActualHeight - (rect.Height * scale) - rect.Top;
                        break;
                }
            }
        }
    }
}