﻿// ReSharper disable once CheckNamespace
namespace Fluent;

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;
using Fluent.Converters;
using Fluent.Extensions;
using Fluent.Helpers;
using Fluent.Internal;
using Fluent.Internal.KnownBoxes;

/// <summary>
/// Represents spinner control
/// </summary>
[ContentProperty(nameof(Value))]
[TemplatePart(Name = "PART_TextBox", Type = typeof(System.Windows.Controls.TextBox))]
[TemplatePart(Name = "PART_ButtonUp", Type = typeof(RepeatButton))]
[TemplatePart(Name = "PART_ButtonDown", Type = typeof(RepeatButton))]
public class Spinner : RibbonControl, IMediumIconProvider, ISimplifiedRibbonControl
{
    /// <summary>
    /// Occurs when value has been changed
    /// </summary>
    public event RoutedPropertyChangedEventHandler<double>? ValueChanged;

    // Parts of the control (must be in control template)
    private System.Windows.Controls.TextBox? textBox;
    private RepeatButton? buttonUp;
    private RepeatButton? buttonDown;

    #region Properties

    #region SimplifiedSizeDefinition

    /// <inheritdoc />
    public RibbonControlSizeDefinition SimplifiedSizeDefinition
    {
        get => (RibbonControlSizeDefinition)this.GetValue(SimplifiedSizeDefinitionProperty);
        set => this.SetValue(SimplifiedSizeDefinitionProperty, value);
    }

    /// <summary>Identifies the <see cref="SimplifiedSizeDefinition"/> dependency property.</summary>
    public static readonly DependencyProperty SimplifiedSizeDefinitionProperty = RibbonProperties.SimplifiedSizeDefinitionProperty.AddOwner(typeof(Spinner));

    #endregion

    #region MediumIcon

    /// <inheritdoc />
    [Localizability(LocalizationCategory.NeverLocalize)]
    [Localizable(false)]
    public object? MediumIcon
    {
        get => this.GetValue(MediumIconProperty);
        set => this.SetValue(MediumIconProperty, value);
    }

    /// <summary>Identifies the <see cref="MediumIcon"/> dependency property.</summary>
    public static readonly DependencyProperty MediumIconProperty = MediumIconProviderProperties.MediumIconProperty.AddOwner(typeof(Spinner), new PropertyMetadata(LogicalChildSupportHelper.OnLogicalChildPropertyChanged));

    #endregion

    #region Value

    /// <summary>
    /// Gets or sets current value
    /// </summary>
    public double Value
    {
        get => (double)this.GetValue(ValueProperty);
        set => this.SetValue(ValueProperty, value);
    }

    /// <summary>
    /// Using a DependencyProperty as the backing store for Value.
    /// This enables animation, styling, binding, etc...
    /// </summary>
    public static readonly DependencyProperty ValueProperty;

    private static object CoerceValue(DependencyObject d, object? basevalue)
    {
        if (basevalue is null)
        {
            return double.NaN;
        }

        var spinner = (Spinner)d;
        var value = (double)basevalue;
        value = GetLimitedValue(spinner, value);
        return value;
    }

    private static double GetLimitedValue(Spinner spinner, double value)
    {
        value = Math.Max(spinner.Minimum, value);
        value = Math.Min(spinner.Maximum, value);
        return value;
    }

    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var spinner = (Spinner)d;
        spinner.ValueToTextBoxText();

        spinner.ValueChanged?.Invoke(spinner, new RoutedPropertyChangedEventArgs<double>((double)e.OldValue, (double)e.NewValue));
    }

    private void ValueToTextBoxText()
    {
        if (this.textBox is null)
        {
            return;
        }

        var newText = (string)this.TextToValueConverter.ConvertBack(this.Value, typeof(string), this.Format, CultureInfo.CurrentCulture);
        this.textBox.Text = newText;
        this.Text = newText;
    }

    #endregion

    #region Text

    /// <summary>
    /// Gets current text from the spinner
    /// </summary>
    public string? Text
    {
        get => (string?)this.GetValue(TextProperty);
        private set => this.SetValue(TextPropertyKey, value);
    }

    // ReSharper disable once InconsistentNaming
    private static readonly DependencyPropertyKey TextPropertyKey = DependencyProperty.RegisterReadOnly(nameof(Text), typeof(string), typeof(Spinner), new PropertyMetadata());

    /// <summary>Identifies the <see cref="Text"/> dependency property.</summary>
    public static readonly DependencyProperty TextProperty = TextPropertyKey.DependencyProperty;

    #endregion

    #region Increment

    /// <summary>
    /// Gets or sets a value added or subtracted from the value property
    /// </summary>
    public double Increment
    {
        get => (double)this.GetValue(IncrementProperty);
        set => this.SetValue(IncrementProperty, value);
    }

    /// <summary>Identifies the <see cref="Increment"/> dependency property.</summary>
    public static readonly DependencyProperty IncrementProperty =
        DependencyProperty.Register(nameof(Increment), typeof(double), typeof(Spinner), new PropertyMetadata(DoubleBoxes.One));

    #endregion

    #region Minimum

    /// <summary>
    /// Gets or sets minimun value
    /// </summary>
    public double Minimum
    {
        get => (double)this.GetValue(MinimumProperty);
        set => this.SetValue(MinimumProperty, value);
    }

    /// <summary>
    /// Using a DependencyProperty as the backing store for Minimum.
    /// This enables animation, styling, binding, etc...
    /// </summary>
    public static readonly DependencyProperty MinimumProperty;

    private static object CoerceMinimum(DependencyObject d, object? basevalue)
    {
        var spinner = (Spinner)d;
        var value = (double)basevalue!;

        if (spinner.Maximum < value)
        {
            return spinner.Maximum;
        }

        return value;
    }

    private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var spinner = (Spinner)d;
        var value = (double)CoerceValue(d, spinner.Value);

        if (DoubleUtil.AreClose(value, spinner.Value) == false)
        {
            spinner.Value = value;
        }
    }

    #endregion

    #region Maximum

    /// <summary>
    /// Gets or sets maximum value
    /// </summary>
    public double Maximum
    {
        get => (double)this.GetValue(MaximumProperty);
        set => this.SetValue(MaximumProperty, value);
    }

    /// <summary>
    /// Using a DependencyProperty as the backing store for Maximum.
    /// This enables animation, styling, binding, etc...
    /// </summary>
    public static readonly DependencyProperty MaximumProperty;

    private static object? CoerceMaximum(DependencyObject d, object? basevalue)
    {
        var spinner = (Spinner)d;

        if (basevalue is double value
            && spinner.Minimum > value)
        {
            return spinner.Minimum;
        }

        return basevalue;
    }

    private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var spinner = (Spinner)d;
        var value = (double)CoerceValue(d, spinner.Value);

        if (DoubleUtil.AreClose(value, spinner.Value) == false)
        {
            spinner.Value = value;
        }
    }

    #endregion

    #region Format

    /// <summary>
    /// Gets or sets string format of value
    /// </summary>
    public string Format
    {
        get => (string)this.GetValue(FormatProperty);
        set => this.SetValue(FormatProperty, value);
    }

    /// <summary>Identifies the <see cref="Format"/> dependency property.</summary>
    public static readonly DependencyProperty FormatProperty =
        DependencyProperty.Register(nameof(Format), typeof(string), typeof(Spinner), new PropertyMetadata("F1", OnFormatChanged));

    private static void OnFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var spinner = (Spinner)d;
        spinner.ValueToTextBoxText();
    }

    #endregion

    #region Delay

    /// <summary>
    /// Gets or sets the amount of time, in milliseconds,
    /// the Spinner waits while it is pressed before it starts repeating.
    /// The value must be non-negative. This is a dependency property.
    /// </summary>
    public int Delay
    {
        get => (int)this.GetValue(DelayProperty);
        set => this.SetValue(DelayProperty, value);
    }

    /// <summary>Identifies the <see cref="Delay"/> dependency property.</summary>
    public static readonly DependencyProperty DelayProperty =
        DependencyProperty.Register(nameof(Delay), typeof(int), typeof(Spinner),
            new PropertyMetadata(400));

    #endregion

    #region Interval

    /// <summary>
    /// Gets or sets the amount of time, in milliseconds,
    /// between repeats once repeating starts. The value must be non-negative.
    /// This is a dependency property.
    /// </summary>
    public int Interval
    {
        get => (int)this.GetValue(IntervalProperty);
        set => this.SetValue(IntervalProperty, value);
    }

    /// <summary>Identifies the <see cref="Interval"/> dependency property.</summary>
    public static readonly DependencyProperty IntervalProperty =
        DependencyProperty.Register(nameof(Interval), typeof(int), typeof(Spinner), new PropertyMetadata(80));

    #endregion

    #region TextToValueConverter

    /// <summary>
    /// Gets or sets a converter which is used to convert from text to double and from double to text.
    /// </summary>
    public IValueConverter TextToValueConverter
    {
        get => (IValueConverter)this.GetValue(TextToValueConverterProperty);
        set => this.SetValue(TextToValueConverterProperty, value);
    }

    /// <summary>Identifies the <see cref="TextToValueConverter"/> dependency property.</summary>
    public static readonly DependencyProperty TextToValueConverterProperty =
#pragma warning disable WPF0016 // Default value is shared reference type.
        DependencyProperty.Register(nameof(TextToValueConverter), typeof(IValueConverter), typeof(Spinner), new PropertyMetadata(SpinnerTextToValueConverter.DefaultInstance));
#pragma warning restore WPF0016 // Default value is shared reference type.

    #endregion TextToValueConverter

    /// <summary>
    /// Defines whether all text should be select as soon as this control gets focus.
    /// </summary>
    public bool SelectAllTextOnFocus
    {
        get => (bool)this.GetValue(SelectAllTextOnFocusProperty);
        set => this.SetValue(SelectAllTextOnFocusProperty, BooleanBoxes.Box(value));
    }

    /// <summary>Identifies the <see cref="SelectAllTextOnFocus"/> dependency property.</summary>
    public static readonly DependencyProperty SelectAllTextOnFocusProperty =
        DependencyProperty.Register(nameof(SelectAllTextOnFocus), typeof(bool), typeof(Spinner), new PropertyMetadata(BooleanBoxes.FalseBox));

    #region IsSimplified

    /// <summary>
    /// Gets or sets whether or not the ribbon is in Simplified mode
    /// </summary>
    public bool IsSimplified
    {
        get => (bool)this.GetValue(IsSimplifiedProperty);
        private set => this.SetValue(IsSimplifiedPropertyKey, BooleanBoxes.Box(value));
    }

    private static readonly DependencyPropertyKey IsSimplifiedPropertyKey =
        DependencyProperty.RegisterReadOnly(nameof(IsSimplified), typeof(bool), typeof(Spinner), new PropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>Identifies the <see cref="IsSimplified"/> dependency property.</summary>
    public static readonly DependencyProperty IsSimplifiedProperty = IsSimplifiedPropertyKey.DependencyProperty;

    #endregion

    #endregion Properties

    #region Constructors

    /// <summary>
    /// Static constructor
    /// </summary>
    static Spinner()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(Spinner), new FrameworkPropertyMetadata(typeof(Spinner)));

        MaximumProperty = DependencyProperty.Register(nameof(Maximum), typeof(double), typeof(Spinner), new PropertyMetadata(DoubleBoxes.MaxValue, OnMaximumChanged, CoerceMaximum));
        MinimumProperty = DependencyProperty.Register(nameof(Minimum), typeof(double), typeof(Spinner), new PropertyMetadata(DoubleBoxes.Zero, OnMinimumChanged, CoerceMinimum));
        ValueProperty = DependencyProperty.Register(nameof(Value), typeof(double), typeof(Spinner), new FrameworkPropertyMetadata(DoubleBoxes.Zero, OnValueChanged, CoerceValue) { BindsTwoWayByDefault = true });

        KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(Spinner), new FrameworkPropertyMetadata(KeyboardNavigationMode.Once));
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Select all text in the Spinner.
    /// </summary>
    public void SelectAll()
    {
        this.textBox?.SelectAll();
    }

    #endregion

    #region Overrides

    /// <summary>
    /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
    /// </summary>
    public override void OnApplyTemplate()
    {
        if (this.buttonUp is not null)
        {
            this.buttonUp.Click -= this.OnButtonUpClick;
            BindingOperations.ClearAllBindings(this.buttonUp);
        }

        if (this.buttonDown is not null)
        {
            this.buttonDown.Click -= this.OnButtonDownClick;
            BindingOperations.ClearAllBindings(this.buttonDown);
        }

        if (this.textBox is not null)
        {
            this.textBox.LostKeyboardFocus -= this.OnTextBoxLostKeyboardFocus;
            this.textBox.PreviewKeyDown -= this.OnTextBoxPreviewKeyDown;
        }

        // Get template childs
        this.textBox = this.GetTemplateChild("PART_TextBox") as System.Windows.Controls.TextBox;
        this.buttonUp = this.GetTemplateChild("PART_ButtonUp") as RepeatButton;
        this.buttonDown = this.GetTemplateChild("PART_ButtonDown") as RepeatButton;

        // Check template
        if (this.textBox is null
            || this.buttonUp is null
            || this.buttonDown is null)
        {
            Debug.WriteLine("Template for Spinner control is invalid");
            return;
        }

        // Bindings
        Bind(this, this.buttonUp, nameof(this.Delay), RepeatButton.DelayProperty, BindingMode.OneWay);
        Bind(this, this.buttonDown, nameof(this.Delay), RepeatButton.DelayProperty, BindingMode.OneWay);
        Bind(this, this.buttonUp, nameof(this.Interval), RepeatButton.IntervalProperty, BindingMode.OneWay);
        Bind(this, this.buttonDown, nameof(this.Interval), RepeatButton.IntervalProperty, BindingMode.OneWay);

        // Events subscribing
        this.buttonUp.Click += this.OnButtonUpClick;
        this.buttonDown.Click += this.OnButtonDownClick;
        this.textBox.GotFocus += this.HandleTextBoxGotFocus;
        this.textBox.LostKeyboardFocus += this.OnTextBoxLostKeyboardFocus;
        this.textBox.PreviewKeyDown += this.OnTextBoxPreviewKeyDown;

        this.ValueToTextBoxText();
    }

    #endregion

    #region Event Handling

    /// <inheritdoc />
    public override KeyTipPressedResult OnKeyTipPressed()
    {
        if (this.textBox is null)
        {
            return KeyTipPressedResult.Empty;
        }

        this.textBox.SelectAll();
        this.textBox.Focus();

        return new KeyTipPressedResult(true, false);
    }

    private void HandleTextBoxGotFocus(object sender, RoutedEventArgs e)
    {
        if (this.SelectAllTextOnFocus
            && this.textBox is not null)
        {
            // Async because setting the carret happens after focus.
            this.RunInDispatcherAsync(() =>
            {
                this.textBox.SelectAll();
            }, DispatcherPriority.Background);
        }
    }

    /// <summary>
    /// Invoked when an unhandled System.Windows.Input.Keyboard.KeyUp attached event reaches
    /// an element in its route that is derived from this class. Implement this method to add class handling for this event.
    /// </summary>
    /// <param name="e">The System.Windows.Input.KeyEventArgs that contains the event data.</param>
    protected override void OnKeyUp(KeyEventArgs e)
    {
        // Avoid Click invocation (from RibbonControl)
        if (e.Key == Key.Enter
            || e.Key == Key.Space)
        {
            return;
        }

        base.OnKeyUp(e);
    }

    private void OnButtonUpClick(object sender, RoutedEventArgs e)
    {
        this.Value = GetLimitedValue(this, this.Value + this.Increment);
    }

    private void OnButtonDownClick(object sender, RoutedEventArgs e)
    {
        this.Value = GetLimitedValue(this, this.Value - this.Increment);
    }

    private void OnTextBoxLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        this.TextBoxTextToValue();
    }

    private void OnTextBoxPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            this.TextBoxTextToValue();
        }

        if (e.Key == Key.Escape)
        {
            this.ValueToTextBoxText();
        }

        if (e.Key == Key.Enter
            || e.Key == Key.Escape)
        {
            // Move Focus
            this.textBox!.Focusable = false;
            this.Focus();
            this.textBox!.Focusable = true;
            e.Handled = true;
        }

        if (e.Key == Key.Up
            && this.buttonUp is not null)
        {
            this.buttonUp.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        }

        if (e.Key == Key.Down
            && this.buttonDown is not null)
        {
            this.buttonDown.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        }
    }

    private void TextBoxTextToValue()
    {
        var converterParam = new Tuple<string, double>(this.Format, this.Value);
        var newValue = (double)this.TextToValueConverter.Convert(this.textBox!.Text, typeof(double), converterParam, CultureInfo.CurrentCulture);

        this.Value = GetLimitedValue(this, newValue);

        this.ValueToTextBoxText();
    }

    #endregion

    #region Quick Access Item Creating

    /// <summary>
    /// Gets control which represents shortcut item.
    /// This item MUST be syncronized with the original
    /// and send command to original one control.
    /// </summary>
    /// <returns>Control which represents shortcut item</returns>
    public override FrameworkElement CreateQuickAccessItem()
    {
        var spinner = new Spinner();
        this.BindQuickAccessItem(spinner);
        return spinner;
    }

    /// <summary>
    /// This method must be overriden to bind properties to use in quick access creating
    /// </summary>
    /// <param name="element">Toolbar item</param>
    protected virtual void BindQuickAccessItem(FrameworkElement element)
    {
        var spinner = (Spinner)element;

        BindQuickAccessItem(this, element);

        Bind(this, spinner, nameof(this.Value), ValueProperty, BindingMode.TwoWay);
        Bind(this, spinner, nameof(this.Increment), IncrementProperty, BindingMode.OneWay);
        Bind(this, spinner, nameof(this.Minimum), MinimumProperty, BindingMode.OneWay);
        Bind(this, spinner, nameof(this.Maximum), MaximumProperty, BindingMode.OneWay);
        Bind(this, spinner, nameof(this.Format), FormatProperty, BindingMode.OneWay);
        Bind(this, spinner, nameof(this.Delay), DelayProperty, BindingMode.OneWay);
        Bind(this, spinner, nameof(this.Interval), IntervalProperty, BindingMode.OneWay);
    }

    #endregion

    /// <inheritdoc />
    void ISimplifiedStateControl.UpdateSimplifiedState(bool isSimplified)
    {
        this.IsSimplified = isSimplified;
    }

    /// <inheritdoc />
    protected override IEnumerator LogicalChildren
    {
        get
        {
            var baseEnumerator = base.LogicalChildren;
            while (baseEnumerator?.MoveNext() == true)
            {
                yield return baseEnumerator.Current;
            }

            if (this.MediumIcon is not null)
            {
                yield return this.MediumIcon;
            }
        }
    }
}