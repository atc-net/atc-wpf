using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

// ReSharper disable StringLiteralTypo
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable VirtualMemberNeverOverridden.Global
namespace Atc.Wpf.Controls.W3cSvg
{
    /// <summary>
    /// This is an <see langword="abstract"/> implementation of a markup extension that enables the creation
    /// of <see cref="DrawingImage"/> from SVG sources.
    /// </summary>
    [MarkupExtensionReturnType(typeof(DrawingImage))]
    public abstract class SvgIconBase : MarkupExtension
    {
        private string? applicationName;
        private CultureInfo? culture;

        /// <summary>
        /// Initializes a new instance of the <see cref="SvgIconBase"/>
        /// class with the default parameters.
        /// </summary>
        protected SvgIconBase()
        {
            this.GetAppName();
        }

        public Color? OverrideColor { get; set; }

        /// <summary>
        /// Gets or sets the main culture information used for rendering texts.
        /// </summary>
        /// <value>
        /// An instance of the <see cref="CultureInfo"/> specifying the main
        /// culture information for texts. The default is the English culture.
        /// </value>
        /// <remarks>
        /// <para>
        /// This is the culture information passed to the <see cref="FormattedText"/>
        /// class instance for the text rendering.
        /// </para>
        /// <para>
        /// The library does not currently provide any means of splitting texts
        /// into its multi-language parts.
        /// </para>
        /// </remarks>
        public CultureInfo? CultureInfo
        {
            get => this.culture;
            set
            {
                if (value != null)
                {
                    this.culture = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the project or application name of the target assembly.
        /// </summary>
        /// <value>
        /// A string specifying the application project name.
        /// </value>
        /// <remarks>
        /// This is optional and is only used to resolve the resource Uri at the design time.
        /// </remarks>
        public string? ApplicationName
        {
            get => this.applicationName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    this.GetAppName();
                }
                else
                {
                    this.applicationName = value;
                }
            }
        }

        /// <summary>
        /// This converts the SVG resource specified by the Uri to <see cref="DrawingGroup"/>.
        /// </summary>
        /// <param name="svgSource">A <see cref="Uri"/> specifying the source of the SVG resource.</param>
        /// <returns>A <see cref="DrawingGroup"/> of the converted SVG resource.</returns>
        [SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK - for now.")]
        protected virtual DrawingGroup? GetDrawing(Uri svgSource)
        {
            if (svgSource == null)
            {
                throw new ArgumentNullException(nameof(svgSource));
            }

            string scheme;
            bool designTime = DesignerProperties.GetIsInDesignMode(new DependencyObject());
            if (designTime && !svgSource.IsAbsoluteUri)
            {
                scheme = "pack";
            }
            else
            {
                scheme = svgSource.Scheme;
            }

            if (string.IsNullOrWhiteSpace(scheme))
            {
                return null;
            }

            switch (scheme)
            {
                case "file":
                case "https":
                case "http":
                    using (SvgFileReader reader = new SvgFileReader(this.OverrideColor))
                    {
                        return reader.Read(svgSource);
                    }

                case "pack":
                    var svgStreamInfo = svgSource.ToString().IndexOf("siteoforigin", StringComparison.OrdinalIgnoreCase) != -1
                        ? Application.GetRemoteStream(svgSource)
                        : Application.GetResourceStream(svgSource);

                    var svgStream = svgStreamInfo?.Stream;
                    if (svgStream != null)
                    {
                        string fileExt = Path.GetExtension(svgSource.ToString());
                        bool isCompressed = !string.IsNullOrWhiteSpace(fileExt) &&
                            string.Equals(fileExt, ".svgz", StringComparison.OrdinalIgnoreCase);

                        if (isCompressed)
                        {
                            using var stream = svgStream;
                            using var zipStream = new GZipStream(svgStream, CompressionMode.Decompress);
                            using SvgFileReader reader = new SvgFileReader(this.OverrideColor);
                            var drawGroup = reader.Read(zipStream);
                            if (drawGroup != null)
                            {
                                return drawGroup;
                            }
                        }
                        else
                        {
                            using var stream = svgStream;
                            using SvgFileReader reader = new SvgFileReader(this.OverrideColor);
                            var drawGroup = reader.Read(svgStream);
                            if (drawGroup != null)
                            {
                                return drawGroup;
                            }
                        }
                    }

                    break;
                case "data":
                    var sourceData = svgSource.OriginalString.Replace(" ", string.Empty, StringComparison.Ordinal);
                    int nColon = sourceData.IndexOf(":", StringComparison.OrdinalIgnoreCase);
                    int nSemiColon = sourceData.IndexOf(";", StringComparison.OrdinalIgnoreCase);
                    int nComma = sourceData.IndexOf(",", StringComparison.OrdinalIgnoreCase);
                    string sMimeType = sourceData.Substring(nColon + 1, nSemiColon - nColon - 1);
                    string sEncoding = sourceData.Substring(nSemiColon + 1, nComma - nSemiColon - 1);
                    if (string.Equals(sMimeType.Trim(), "image/svg+xml", StringComparison.OrdinalIgnoreCase)
                        && string.Equals(sEncoding.Trim(), "base64", StringComparison.OrdinalIgnoreCase))
                    {
                        string sContent = SvgFileReader.RemoveWhitespace(sourceData.Substring(nComma + 1));
                        byte[] imageBytes = Convert.FromBase64CharArray(sContent.ToCharArray(), 0, sContent.Length);
                        bool isGZipped = sContent.StartsWith(SvgFileReader.GZipSignature, StringComparison.Ordinal);
                        if (isGZipped)
                        {
                            using var stream = new MemoryStream(imageBytes);
                            using GZipStream zipStream = new GZipStream(stream, CompressionMode.Decompress);
                            using var reader = new SvgFileReader(this.OverrideColor);
                            var drawGroup = reader.Read(zipStream);
                            if (drawGroup != null)
                            {
                                return drawGroup;
                            }
                        }
                        else
                        {
                            using var stream = new MemoryStream(imageBytes);
                            using var reader = new SvgFileReader(this.OverrideColor);
                            var drawGroup = reader.Read(stream);
                            if (drawGroup != null)
                            {
                                return drawGroup;
                            }
                        }
                    }

                    break;
            }

            return null;
        }

        /// <summary>
        /// This converts the SVG resource specified by the Uri to <see cref="DrawingImage"/>.
        /// </summary>
        /// <param name="svgSource">A <see cref="Uri"/> specifying the source of the SVG resource.</param>
        /// <returns>A <see cref="DrawingImage"/> of the converted SVG resource.</returns>
        /// <remarks>
        /// This uses the <see cref="GetDrawing(Uri)"/> method to convert the SVG resource to <see cref="DrawingGroup"/>,
        /// which is then wrapped in <see cref="DrawingImage"/>.
        /// </remarks>
        protected virtual DrawingImage? GetImage(Uri svgSource)
        {
            var drawGroup = this.GetDrawing(svgSource);
            return drawGroup == null
                ? null
                : new DrawingImage(drawGroup);
        }

        [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "OK.")]
        protected void GetAppName()
        {
            try
            {
                var asm = GetEntryAssembly();
                if (asm != null)
                {
                    this.applicationName = asm.GetName().Name;
                }
            }
            catch
            {
                // Dummy
            }
        }

        [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "OK.")]
        protected static Assembly? GetEntryAssembly()
        {
            string xDesProc = "XDesProc"; // WPF designer process
            Assembly? asm = null;
            try
            {
                asm = Assembly.GetEntryAssembly();
                if (asm != null)
                {
                    var assemblyName = asm.GetName().Name;
                    if (string.Equals(assemblyName, xDesProc, StringComparison.OrdinalIgnoreCase))
                    {
                        asm = null;
                    }
                }

                if (asm == null)
                {
                    asm = (
                          from assembly in AppDomain.CurrentDomain.GetAssemblies()
                          where !assembly.IsDynamic
                          let assemblyName = Path.GetFileName(assembly.Location).Trim()
                          where assemblyName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)
                          && !string.Equals(assemblyName, "XDesProc.exe", StringComparison.OrdinalIgnoreCase)
                          select assembly
                          )
                        .FirstOrDefault();

                    if (asm == null)
                    {
                        asm = Application.ResourceAssembly;
                        if (asm != null)
                        {
                            var assemblyName = asm.GetName().Name;
                            if (string.Equals(assemblyName, xDesProc, StringComparison.OrdinalIgnoreCase))
                            {
                                asm = null;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (asm == null)
                {
                    asm = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                           where !assembly.IsDynamic
                           let assemblyName = Path.GetFileName(assembly.Location).Trim()
                           where assemblyName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)
                           && !string.Equals(assemblyName, "XDesProc.exe", StringComparison.OrdinalIgnoreCase)
                           select assembly
                          )
                        .FirstOrDefault();
                }

                Trace.TraceError(ex.ToString());
            }

            return asm;
        }

        [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "OK.")]
        protected static Assembly? GetExecutingAssembly()
        {
            Assembly? asm;
            try
            {
                asm = Assembly.GetExecutingAssembly();
            }
            catch
            {
                asm = Assembly.GetEntryAssembly();
            }

            return asm;
        }
    }
}