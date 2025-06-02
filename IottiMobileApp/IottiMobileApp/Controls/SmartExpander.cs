using System.Collections;
using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using IottiMobileApp.Classes;
using Microsoft.Maui.Controls;

namespace IottiMobileApp.Controls
{
    public class SmartExpander : ContentView
    {
        #region Bindable Properties
        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(SmartExpander), null, propertyChanged: OnItemsSourceChanged);

        public static readonly BindableProperty SelectedItemProperty =
            BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(SmartExpander), null, BindingMode.TwoWay, propertyChanged: OnSelectedItemChanged);

        public static readonly BindableProperty PlaceholderTextProperty =
            BindableProperty.Create(nameof(PlaceholderText), typeof(string), typeof(SmartExpander), "Scegli elemento", propertyChanged: OnPlaceholderTextChanged);

        public static readonly BindableProperty DisplayMemberPathProperty =
            BindableProperty.Create(nameof(DisplayMemberPath), typeof(string), typeof(SmartExpander), null, propertyChanged: OnDisplayMemberPathChanged);

        public static readonly BindableProperty MaxHeightProperty =
            BindableProperty.Create(nameof(MaxHeight), typeof(double), typeof(SmartExpander), 200.0);

        public static readonly BindableProperty ItemHeightProperty =
            BindableProperty.Create(nameof(ItemHeight), typeof(double), typeof(SmartExpander), 40.0, propertyChanged: OnItemHeightChanged);

        public static readonly BindableProperty ArrowIconProperty =
            BindableProperty.Create(nameof(ArrowIcon), typeof(string), typeof(SmartExpander), "▼");

        public IEnumerable? ItemsSource
        {
            get => (IEnumerable?)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public object? SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public string PlaceholderText
        {
            get => (string)GetValue(PlaceholderTextProperty);
            set => SetValue(PlaceholderTextProperty, value);
        }

        public string? DisplayMemberPath
        {
            get => (string?)GetValue(DisplayMemberPathProperty);
            set => SetValue(DisplayMemberPathProperty, value);
        }

        public double MaxHeight
        {
            get => (double)GetValue(MaxHeightProperty);
            set => SetValue(MaxHeightProperty, value);
        }

        public double ItemHeight
        {
            get => (double)GetValue(ItemHeightProperty);
            set => SetValue(ItemHeightProperty, value);
        }

        public string ArrowIcon
        {
            get => (string)GetValue(ArrowIconProperty);
            set => SetValue(ArrowIconProperty, value);
        }
        #endregion

        #region Private Fields
        private Grid _mainContainer = null!;
        private Border _headerBorder = null!;
        private Border _contentBorder = null!;
        private CollectionView _collectionView = null!;
        private Label _selectedItemLabel = null!;
        private Label _arrowLabel = null!;
        private bool _isExpanded = false;
        private bool _isAnimating = false;
        #endregion

        #region Events
        public event EventHandler<SelectionChangedEventArgs>? SelectionChanged;
        #endregion

        public bool IsExpanded => _isExpanded;

        public SmartExpander()
        {
            CreateControls();
        }

        private void CreateControls()
        {
            _mainContainer = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto }
                }
            };

            CreateHeader();
            CreateContent();

            Grid.SetRow(_headerBorder, 0);
            Grid.SetRow(_contentBorder, 1);

            _mainContainer.Children.Add(_headerBorder);
            _mainContainer.Children.Add(_contentBorder);

            Content = _mainContainer;
            UpdateItemTemplate();
        }

        private void CreateHeader()
        {
            var headerGrid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Auto }
                },
                Padding = new Thickness(10)
            };

            _selectedItemLabel = new Label
            {
                VerticalOptions = LayoutOptions.Center,
                Text = PlaceholderText
            };

            _arrowLabel = new Label
            {
                Text = ArrowIcon,
                FontSize = 20,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.End,
                FontFamily = "FASolid"
            };

            headerGrid.Children.Add(_selectedItemLabel);
            headerGrid.Children.Add(_arrowLabel);
            Grid.SetColumn(_selectedItemLabel, 0);
            Grid.SetColumn(_arrowLabel, 1);

            _headerBorder = new Border { Content = headerGrid };

            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += OnHeaderTapped;
            _headerBorder.GestureRecognizers.Add(tapGesture);
        }

        private void CreateContent()
        {
            _collectionView = new CollectionView
            {
                SelectionMode = SelectionMode.Single,
                BackgroundColor = Colors.LightBlue
            };
            _collectionView.SelectionChanged += OnInternalSelectionChanged;

            var scrollView = new ScrollView
            {
                Content = _collectionView,
                VerticalScrollBarVisibility = ScrollBarVisibility.Always
            };

            _contentBorder = new Border
            {
                Content = scrollView,
                IsVisible = false,
                HeightRequest = 0
            };
        }

        private async void OnHeaderTapped(object? sender, EventArgs e)
        {
            if (_isAnimating) return;

            _isAnimating = true;
            try
            {
                if (_isExpanded)
                    await CollapseAsync();
                else
                    await ExpandAsync();
            }
            finally
            {
                _isAnimating = false;
            }
        }

        private async Task ExpandAsync()
        {
            if (_isExpanded) return;

            _isExpanded = true;
            var targetHeight = CalculateRequiredHeight();

            _contentBorder.Opacity = 0;
            _contentBorder.IsVisible = true;

            var heightAnim = new Animation(v => _contentBorder.HeightRequest = v, 0, targetHeight);
            var fadeAnim = new Animation(v => _contentBorder.Opacity = v, 0, 1);
            var arrowAnim = new Animation(v => _arrowLabel.Rotation = v, 0, 180);

            var combined = new Animation();
            combined.Add(0, 1, heightAnim);
            combined.Add(0.2, 1, fadeAnim);
            combined.Add(0, 1, arrowAnim);

            combined.Commit(this, "Expand", 16, 400, Easing.CubicOut);
            await Task.Delay(400);
        }

        private async Task CollapseAsync()
        {
            if (!_isExpanded) return;

            _isExpanded = false;
            var currentHeight = _contentBorder.HeightRequest;

            var heightAnim = new Animation(v => _contentBorder.HeightRequest = v, currentHeight, 0);
            var fadeAnim = new Animation(v => _contentBorder.Opacity = v, 1, 0);
            var arrowAnim = new Animation(v => _arrowLabel.Rotation = v, 180, 0);

            var combined = new Animation();
            combined.Add(0.2, 1, heightAnim);
            combined.Add(0, 0.8, fadeAnim);
            combined.Add(0, 1, arrowAnim);

            combined.Commit(this, "Collapse", 16, 350, Easing.CubicIn, (v, c) =>
            {
                _contentBorder.IsVisible = false;
                _contentBorder.Opacity = 1;
                _contentBorder.HeightRequest = 0;
            });

            await Task.Delay(350);
        }

        public async Task CloseExpanderSafely()
        {
            if (_isExpanded && !_isAnimating)
                await CollapseAsync();
        }

        private double CalculateRequiredHeight()
        {
            if (ItemsSource == null) return MaxHeight;

            var count = ItemsSource is ICollection collection ? collection.Count : ItemsSource.Cast<object>().Count();
            return Math.Min(count * ItemHeight, MaxHeight);
        }

        private void UpdateItemTemplate()
        {
            if (_collectionView == null) return;

            _collectionView.ItemTemplate = new DataTemplate(() =>
            {
                var grid = new Grid
                {
                    Padding = new Thickness(10),
                    HeightRequest = ItemHeight
                };

                var label = new Label { VerticalOptions = LayoutOptions.Center };

                if (!string.IsNullOrEmpty(DisplayMemberPath))
                    label.SetBinding(Label.TextProperty, new Binding(DisplayMemberPath));
                else
                    label.SetBinding(Label.TextProperty, new Binding("."));

                grid.Children.Add(label);
                return grid;
            });
        }

        private void OnInternalSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection?.FirstOrDefault() is object selectedItem)
            {
                SelectedItem = selectedItem;
                UpdateSelectedItemDisplay();
                CallViewModelMethod(selectedItem);

                _ = Task.Run(async () =>
                {
                    await Task.Delay(100);
                    await Dispatcher.DispatchAsync(async () => await CloseExpanderSafely());
                });

                SelectionChanged?.Invoke(this, e);
            }
        }

        private void CallViewModelMethod(object selectedItem)
        {
            var page = GetParentPage();
            var method = page?.BindingContext?.GetType().GetMethod("OnSmartExpanderSelectionChanged",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            try
            {
                method?.Invoke(page.BindingContext, new[] { selectedItem });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SmartExpander method call error: {ex.Message}");
            }
        }

        private Page? GetParentPage()
        {
            Element? parent = Parent;
            while (parent != null)
            {
                if (parent is Page page) return page;
                parent = parent.Parent;
            }
            return null;
        }

        private void UpdateSelectedItemDisplay()
        {
            if (_selectedItemLabel == null) return;

            if (SelectedItem == null)
            {
                _selectedItemLabel.Text = PlaceholderText;
                return;
            }

            if (!string.IsNullOrEmpty(DisplayMemberPath))
            {
                var property = SelectedItem.GetType().GetProperty(DisplayMemberPath);
                _selectedItemLabel.Text = property?.GetValue(SelectedItem)?.ToString() ?? PlaceholderText;
            }
            else
            {
                _selectedItemLabel.Text = SelectedItem.ToString() ?? PlaceholderText;
            }
        }

        #region Property Changed Handlers
        private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is SmartExpander expander && expander._collectionView != null)
                expander._collectionView.ItemsSource = newValue as IEnumerable;
        }

        private static void OnSelectedItemChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is SmartExpander expander)
            {
                if (expander._collectionView != null)
                    expander._collectionView.SelectedItem = newValue;
                expander.UpdateSelectedItemDisplay();
            }
        }

        private static void OnPlaceholderTextChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is SmartExpander expander && expander._selectedItemLabel != null)
                expander.UpdateSelectedItemDisplay();
        }

        private static void OnDisplayMemberPathChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is SmartExpander expander)
            {
                expander.UpdateItemTemplate();
                expander.UpdateSelectedItemDisplay();
            }
        }

        private static void OnItemHeightChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is SmartExpander expander)
                expander.UpdateItemTemplate();
        }
        #endregion
    }
}