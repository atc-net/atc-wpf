global using System;
global using System.Collections;
global using System.Collections.ObjectModel;
global using System.Collections.Specialized;
global using System.ComponentModel;
global using System.ComponentModel.DataAnnotations;
global using System.Diagnostics;
global using System.Diagnostics.CodeAnalysis;
global using System.Globalization;
global using System.IO;
global using System.Runtime.CompilerServices;
global using System.Text;
global using System.Text.Json;
global using System.Windows;
global using System.Windows.Controls;
global using System.Windows.Data;
global using System.Windows.Input;
global using System.Windows.Media;
global using System.Windows.Media.Imaging;
global using System.Windows.Threading;

global using Atc.Helpers;
global using Atc.Serialization.JsonConverters;
global using Atc.Wpf.Collections;
global using Atc.Wpf.Controls.Dialogs;
global using Atc.Wpf.Controls.LabelControls;
global using Atc.Wpf.Controls.LabelControls.Abstractions;
global using Atc.Wpf.Controls.LabelControls.Extractors;
global using Atc.Wpf.Controls.LabelControls.Writers;
global using Atc.Wpf.Controls.Media;
global using Atc.Wpf.Controls.Monitoring;
global using Atc.Wpf.Controls.Notifications;
global using Atc.Wpf.Controls.Options;
global using Atc.Wpf.Controls.Sample;
global using Atc.Wpf.Controls.SettingsControls;
global using Atc.Wpf.Data;
global using Atc.Wpf.FontIcons;
global using Atc.Wpf.FontIcons.ValueConverters;
global using Atc.Wpf.Helpers;
global using Atc.Wpf.Sample.Models;
global using Atc.Wpf.Serialization.JsonConverters;
global using Atc.Wpf.Theming.Helpers;
global using Atc.Wpf.Translation;
global using Atc.Wpf.ValueConverters;
global using Atc.XamlToolkit.Command;
global using Atc.XamlToolkit.Controls.Attributes;
global using Atc.XamlToolkit.Diagnostics;
global using Atc.XamlToolkit.Messaging;
global using Atc.XamlToolkit.Mvvm;

global using ControlzEx.Theming;

global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Logging;
global using Microsoft.Win32;