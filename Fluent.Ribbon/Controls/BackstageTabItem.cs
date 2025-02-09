// ReSharper disable once CheckNamespace
namespace Fluent;

using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Fluent.Helpers;
using Fluent.Internal.KnownBoxes;

/// <summary>
/// Represents backstage tab item
/// </summary>
[TemplatePart(Name = "PART_Header", Type = typeof(FrameworkElement))]
public class BackstageTabItem : ContentControl, IHeaderedControl, IKeyTipedControl, ILogicalChildSupport
{
    internal FrameworkElement? HeaderContentHost { get; private set; }

    #region Icon

    /// <summary>
    /// Gets or sets Icon for the element
    /// </summary>
    [Localizability(LocalizationCategory.NeverLocalize)]
    [Localizable(false)]
    public object? Icon
    {
        get => this.GetValue(IconProperty);
        set => this.SetValue(IconProperty, value);
    }

    /// <summary>Identifies the <see cref="Icon"/> dependency property.</summary>
    public static readonly DependencyProperty IconProperty = RibbonControl.IconProperty.AddOwner(typeof(BackstageTabItem), new PropertyMetadata(LogicalChildSupportHelper.OnLogicalChildPropertyChanged));

    #endregion

    /// <inheritdoc />
    public string? KeyTip
    {
        get => (string?)this.GetValue(KeyTipProperty);
        set => this.SetValue(KeyTipProperty, value);
    }

    /// <summary>
    /// Dependency property for <see cref="KeyTip"/>
    /// </summary>
    public static readonly DependencyProperty KeyTipProperty = Fluent.KeyTip.KeysProperty.AddOwner(typeof(BackstageTabItem));

    /// <summary>
    /// Gets or sets a value indicating whether the tab is selected
    /// </summary>
    [Bindable(true)]
    [Category("Appearance")]
    public bool IsSelected
    {
        get => (bool)this.GetValue(IsSelectedProperty);
        set => this.SetValue(IsSelectedProperty, BooleanBoxes.Box(value));
    }

    /// <summary>
    /// Dependency property for <see cref="IsSelected"/>
    /// </summary>
    public static readonly DependencyProperty IsSelectedProperty =
        Selector.IsSelectedProperty.AddOwner(typeof(BackstageTabItem),
            new FrameworkPropertyMetadata(BooleanBoxes.FalseBox,
                FrameworkPropertyMetadataOptions.Journal |
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault |
                FrameworkPropertyMetadataOptions.AffectsParentMeasure,
                OnIsSelectedChanged));

    /// <summary>
    /// Gets parent tab control
    /// </summary>
    internal BackstageTabControl? TabControlParent => ItemsControlHelper.ItemsControlFromItemContainer(this) as BackstageTabControl;

    /// <summary>
    /// Gets or sets tab items text
    /// </summary>
    public object? Header
    {
        get => this.GetValue(HeaderProperty);
        set => this.SetValue(HeaderProperty, value);
    }

    /// <summary>Identifies the <see cref="Header"/> dependency property.</summary>
    public static readonly DependencyProperty HeaderProperty = RibbonControl.HeaderProperty.AddOwner(typeof(BackstageTabItem), new PropertyMetadata(LogicalChildSupportHelper.OnLogicalChildPropertyChanged));

    /// <inheritdoc />
    public DataTemplate? HeaderTemplate
    {
        get => (DataTemplate?)this.GetValue(HeaderTemplateProperty);
        set => this.SetValue(HeaderTemplateProperty, value);
    }

    /// <summary>Identifies the <see cref="HeaderTemplate"/> dependency property.</summary>
    public static readonly DependencyProperty HeaderTemplateProperty = RibbonControl.HeaderTemplateProperty.AddOwner(typeof(BackstageTabItem), new PropertyMetadata());

    /// <inheritdoc />
    public DataTemplateSelector? HeaderTemplateSelector
    {
        get => (DataTemplateSelector?)this.GetValue(HeaderTemplateSelectorProperty);
        set => this.SetValue(HeaderTemplateSelectorProperty, value);
    }

    /// <summary>Identifies the <see cref="HeaderTemplateSelector"/> dependency property.</summary>
    public static readonly DependencyProperty HeaderTemplateSelectorProperty = RibbonControl.HeaderTemplateSelectorProperty.AddOwner(typeof(BackstageTabItem), new PropertyMetadata());

    /// <summary>
    /// Static constructor
    /// </summary>
    static BackstageTabItem()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(BackstageTabItem), new FrameworkPropertyMetadata(typeof(BackstageTabItem)));

        KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(BackstageTabItem), new FrameworkPropertyMetadata(KeyboardNavigationMode.Local));
        KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(BackstageTabItem), new FrameworkPropertyMetadata(KeyboardNavigationMode.Cycle));
    }

    #region Overrides

    /// <inheritdoc />
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        this.HeaderContentHost = this.GetTemplateChild("PART_Header") as FrameworkElement;
    }

    /// <inheritdoc />
    protected override void OnContentChanged(object oldContent, object newContent)
    {
        base.OnContentChanged(oldContent, newContent);

        if (this.IsSelected
            && this.TabControlParent is not null)
        {
            this.TabControlParent.SelectedContent = newContent;
        }
    }

    /// <inheritdoc />
    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonDown(e);

        if (e.Handled)
        {
            return;
        }

        if (ReferenceEquals(e.Source, this)
            || this.IsSelected == false)
        {
            this.IsSelected = true;
        }
    }

    /// <inheritdoc />
    protected override void OnGotFocus(RoutedEventArgs e)
    {
        base.OnGotFocus(e);

        this.IsSelected = true;
    }

    #endregion

    #region Private methods

    // Handles IsSelected changed
    private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var container = (BackstageTabItem)d;
        var newValue = (bool)e.NewValue;

        if (newValue)
        {
            container.OnSelected(new RoutedEventArgs(Selector.SelectedEvent, container));
        }
        else
        {
            container.OnUnselected(new RoutedEventArgs(Selector.UnselectedEvent, container));
        }
    }

    #endregion

    /// <summary>
    /// Handles selected event
    /// </summary>
    /// <param name="e">The event data.</param>
    protected virtual void OnSelected(RoutedEventArgs e)
    {
        this.Focus();

        this.HandleIsSelectedChanged(e);
    }

    /// <summary>
    /// Handles unselected event
    /// </summary>
    /// <param name="e">The event data.</param>
    protected virtual void OnUnselected(RoutedEventArgs e)
    {
        this.HandleIsSelectedChanged(e);
    }

    #region Event handling

    /// <summary>
    /// Handles IsSelected changed
    /// </summary>
    /// <param name="e">The event data.</param>
    private void HandleIsSelectedChanged(RoutedEventArgs e)
    {
        this.RaiseEvent(e);
    }

    #endregion

    /// <inheritdoc />
    public KeyTipPressedResult OnKeyTipPressed()
    {
        this.IsSelected = true;

        return KeyTipPressedResult.Empty;
    }

    /// <inheritdoc />
    public void OnKeyTipBack()
    {
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

            if (this.Header is not null)
            {
                yield return this.Header;
            }
        }
    }

    /// <inheritdoc />
    protected override AutomationPeer OnCreateAutomationPeer() => new Fluent.Automation.Peers.RibbonBackstageTabItemAutomationPeer(this);
}