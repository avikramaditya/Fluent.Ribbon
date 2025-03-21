﻿// ReSharper disable once CheckNamespace
namespace Fluent;

using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using Fluent.Helpers;
using Fluent.Internal.KnownBoxes;

/// <summary>
/// Represents Fluent UI specific RadioButton
/// </summary>
[ContentProperty(nameof(Header))]
public class RadioButton : System.Windows.Controls.RadioButton, IRibbonControl, IQuickAccessItemProvider, ILargeIconProvider, IMediumIconProvider, ISimplifiedRibbonControl
{
    #region Properties

    #region Size

    /// <inheritdoc />
    public RibbonControlSize Size
    {
        get => (RibbonControlSize)this.GetValue(SizeProperty);
        set => this.SetValue(SizeProperty, value);
    }

    /// <summary>Identifies the <see cref="Size"/> dependency property.</summary>
    public static readonly DependencyProperty SizeProperty = RibbonProperties.SizeProperty.AddOwner(typeof(RadioButton));

    #endregion

    #region SizeDefinition

    /// <inheritdoc />
    public RibbonControlSizeDefinition SizeDefinition
    {
        get => (RibbonControlSizeDefinition)this.GetValue(SizeDefinitionProperty);
        set => this.SetValue(SizeDefinitionProperty, value);
    }

    /// <summary>Identifies the <see cref="SizeDefinition"/> dependency property.</summary>
    public static readonly DependencyProperty SizeDefinitionProperty = RibbonProperties.SizeDefinitionProperty.AddOwner(typeof(RadioButton));

    #endregion

    #region SimplifiedSizeDefinition

    /// <inheritdoc />
    public RibbonControlSizeDefinition SimplifiedSizeDefinition
    {
        get => (RibbonControlSizeDefinition)this.GetValue(SimplifiedSizeDefinitionProperty);
        set => this.SetValue(SimplifiedSizeDefinitionProperty, value);
    }

    /// <summary>Identifies the <see cref="SimplifiedSizeDefinition"/> dependency property.</summary>
    public static readonly DependencyProperty SimplifiedSizeDefinitionProperty = RibbonProperties.SimplifiedSizeDefinitionProperty.AddOwner(typeof(RadioButton));

    #endregion

    #region KeyTip

    /// <inheritdoc />
    public string? KeyTip
    {
        get => (string?)this.GetValue(KeyTipProperty);
        set => this.SetValue(KeyTipProperty, value);
    }

    /// <summary>
    /// Using a DependencyProperty as the backing store for Keys.
    /// This enables animation, styling, binding, etc...
    /// </summary>
    public static readonly DependencyProperty KeyTipProperty = Fluent.KeyTip.KeysProperty.AddOwner(typeof(RadioButton));

    #endregion

    #region Header

    /// <inheritdoc />
    public object? Header
    {
        get => this.GetValue(HeaderProperty);
        set => this.SetValue(HeaderProperty, value);
    }

    /// <summary>Identifies the <see cref="Header"/> dependency property.</summary>
    public static readonly DependencyProperty HeaderProperty = RibbonControl.HeaderProperty.AddOwner(typeof(RadioButton), new PropertyMetadata(LogicalChildSupportHelper.OnLogicalChildPropertyChanged));

    /// <inheritdoc />
    public DataTemplate? HeaderTemplate
    {
        get => (DataTemplate?)this.GetValue(HeaderTemplateProperty);
        set => this.SetValue(HeaderTemplateProperty, value);
    }

    /// <summary>Identifies the <see cref="HeaderTemplate"/> dependency property.</summary>
    public static readonly DependencyProperty HeaderTemplateProperty = RibbonControl.HeaderTemplateProperty.AddOwner(typeof(RadioButton), new PropertyMetadata());

    /// <inheritdoc />
    public DataTemplateSelector? HeaderTemplateSelector
    {
        get => (DataTemplateSelector?)this.GetValue(HeaderTemplateSelectorProperty);
        set => this.SetValue(HeaderTemplateSelectorProperty, value);
    }

    /// <summary>Identifies the <see cref="HeaderTemplateSelector"/> dependency property.</summary>
    public static readonly DependencyProperty HeaderTemplateSelectorProperty = RibbonControl.HeaderTemplateSelectorProperty.AddOwner(typeof(RadioButton), new PropertyMetadata());

    #endregion

    #region Icon

    /// <inheritdoc />
    [Localizability(LocalizationCategory.NeverLocalize)]
    [Localizable(false)]
    public object? Icon
    {
        get => this.GetValue(IconProperty);
        set => this.SetValue(IconProperty, value);
    }

    /// <summary>Identifies the <see cref="Icon"/> dependency property.</summary>
    public static readonly DependencyProperty IconProperty = RibbonControl.IconProperty.AddOwner(typeof(RadioButton), new PropertyMetadata(LogicalChildSupportHelper.OnLogicalChildPropertyChanged));

    #endregion

    #region LargeIcon

    /// <inheritdoc />
    public object? LargeIcon
    {
        get => this.GetValue(LargeIconProperty);
        set => this.SetValue(LargeIconProperty, value);
    }

    /// <summary>Identifies the <see cref="LargeIcon"/> dependency property.</summary>
    public static readonly DependencyProperty LargeIconProperty = LargeIconProviderProperties.LargeIconProperty.AddOwner(typeof(RadioButton), new PropertyMetadata(LogicalChildSupportHelper.OnLogicalChildPropertyChanged));

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
    public static readonly DependencyProperty MediumIconProperty = MediumIconProviderProperties.MediumIconProperty.AddOwner(typeof(RadioButton), new PropertyMetadata(LogicalChildSupportHelper.OnLogicalChildPropertyChanged));

    #endregion

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
        DependencyProperty.RegisterReadOnly(nameof(IsSimplified), typeof(bool), typeof(RadioButton), new PropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>Identifies the <see cref="IsSimplified"/> dependency property.</summary>
    public static readonly DependencyProperty IsSimplifiedProperty = IsSimplifiedPropertyKey.DependencyProperty;

    #endregion

    #endregion Properties

    #region Constructors

    /// <summary>
    /// Static constructor
    /// </summary>
    static RadioButton()
    {
        var type = typeof(RadioButton);
        DefaultStyleKeyProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(type));
        ContextMenuService.Attach(type);
        ToolTipService.Attach(type);
    }

    /// <summary>
    /// Default constructor
    /// </summary>
    public RadioButton()
    {
        ContextMenuService.Coerce(this);
    }

    #endregion

    #region Quick Access Item Creating

    /// <inheritdoc />
    public virtual FrameworkElement CreateQuickAccessItem()
    {
        var button = new RadioButton();

        RibbonControl.Bind(this, button, nameof(this.IsChecked), IsCheckedProperty, BindingMode.TwoWay);
        button.Click += (sender, e) => this.RaiseEvent(e);
        RibbonControl.BindQuickAccessItem(this, button);

        return button;
    }

    /// <inheritdoc />
    public bool CanAddToQuickAccessToolBar
    {
        get => (bool)this.GetValue(CanAddToQuickAccessToolBarProperty);
        set => this.SetValue(CanAddToQuickAccessToolBarProperty, BooleanBoxes.Box(value));
    }

    /// <summary>Identifies the <see cref="CanAddToQuickAccessToolBar"/> dependency property.</summary>
    public static readonly DependencyProperty CanAddToQuickAccessToolBarProperty = RibbonControl.CanAddToQuickAccessToolBarProperty.AddOwner(typeof(RadioButton), new PropertyMetadata(BooleanBoxes.TrueBox, RibbonControl.OnCanAddToQuickAccessToolBarChanged));

    #endregion

    #region Implementation of IKeyTipedControl

    /// <inheritdoc />
    public KeyTipPressedResult OnKeyTipPressed()
    {
        this.OnClick();

        return KeyTipPressedResult.Empty;
    }

    /// <inheritdoc />
    public void OnKeyTipBack()
    {
    }

    #endregion

    /// <inheritdoc />
    void ISimplifiedStateControl.UpdateSimplifiedState(bool isSimplified)
    {
        this.IsSimplified = isSimplified;
    }

    /// <inheritdoc />
    void ILogicalChildSupport.AddLogicalChild(object child)
    {
        this.AddLogicalChild(child);
    }

    /// <inheritdoc />
    void ILogicalChildSupport.RemoveLogicalChild(object child)
    {
        this.RemoveLogicalChild(child);
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

            if (this.Icon is not null)
            {
                yield return this.Icon;
            }

            if (this.MediumIcon is not null)
            {
                yield return this.MediumIcon;
            }

            if (this.LargeIcon is not null)
            {
                yield return this.LargeIcon;
            }

            if (this.Header is not null)
            {
                yield return this.Header;
            }
        }
    }

    /// <inheritdoc />
    protected override AutomationPeer OnCreateAutomationPeer() => new Fluent.Automation.Peers.RibbonRadioButtonAutomationPeer(this);
}