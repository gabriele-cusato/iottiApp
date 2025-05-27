using System.Collections;
using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using IottiMobileApp.Behaviors;
using IottiMobileApp.Classes;
using Microsoft.Maui.Controls;

namespace IottiMobileApp.Controls
{
    public class SmartExpander : ContentView
    {
        #region Bindable Properties

        // Proprietà per il binding dei dati
        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(SmartExpander), null, propertyChanged: OnItemsSourceChanged);

        public static readonly BindableProperty SelectedItemProperty =
            BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(SmartExpander), null, BindingMode.TwoWay, propertyChanged: OnSelectedItemChanged);

        public static readonly BindableProperty HeaderTextProperty =
            BindableProperty.Create(nameof(HeaderText), typeof(string), typeof(SmartExpander), "Seleziona elemento");

        public static readonly BindableProperty PlaceholderTextProperty =
            BindableProperty.Create(nameof(PlaceholderText), typeof(string), typeof(SmartExpander), "Scegli un elemento", propertyChanged: OnPlaceholderTextChanged);

        public static readonly BindableProperty DisplayMemberPathProperty =
            BindableProperty.Create(nameof(DisplayMemberPath), typeof(string), typeof(SmartExpander), null, propertyChanged: OnDisplayMemberPathChanged);

        public static readonly BindableProperty MaxHeightProperty =
            BindableProperty.Create(nameof(MaxHeight), typeof(double), typeof(SmartExpander), 200.0, propertyChanged: OnMaxHeightChanged);

        public static readonly BindableProperty ItemHeightProperty =
            BindableProperty.Create(nameof(ItemHeight), typeof(double), typeof(SmartExpander), 40.0, propertyChanged: OnItemHeightChanged);

        public static readonly BindableProperty ArrowIconProperty =
            BindableProperty.Create(nameof(ArrowIcon), typeof(string), typeof(SmartExpander), "▼", propertyChanged: OnArrowIconChanged);

        // Proprietà pubbliche
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

        public string HeaderText
        {
            get => (string)GetValue(HeaderTextProperty);
            set => SetValue(HeaderTextProperty, value);
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

        public string? SessionKey
        {
            get => (string?)GetValue(SessionKeyProperty);
            set => SetValue(SessionKeyProperty, value);
        }

        public bool EnableSession
        {
            get => (bool)GetValue(EnableSessionProperty);
            set => SetValue(EnableSessionProperty, value);
        }

        public string ArrowIcon
        {
            get => (string)GetValue(ArrowIconProperty);
            set => SetValue(ArrowIconProperty, value);
        }

        private static void OnPlaceholderTextChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is SmartExpander expander && newValue is string placeholderText)
            {
                // Aggiorna il placeholder text e la visualizzazione
                if (expander.SelectedItem == null)
                {
                    if (string.IsNullOrEmpty(placeholderText))
                    {
                        expander._selectedItemLabel.Text = string.Empty;
                        expander._selectedItemLabel.IsVisible = false;
                    }
                    else
                    {
                        expander._selectedItemLabel.Text = placeholderText;
                        expander._selectedItemLabel.IsVisible = true;
                    }
                }
            }
        }

        private static void OnArrowIconChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is SmartExpander expander && newValue is string iconText)
            {
                expander._arrowLabel.Text = iconText;
            }
        }

        #endregion

        #region Helper Methods

        private Page? GetParentPage()
        {
            Element? parent = this.Parent;
            while (parent != null)
            {
                if (parent is Page page)
                    return page;
                parent = parent.Parent;
            }
            return null;
        }

        #endregion

        #region Private Fields
        private Expander _expander = null!;
        private CollectionView _collectionView = null!;
        private Label _selectedItemLabel = null!;
        private Label _arrowLabel = null!;
        #endregion

        #region Events
        public event EventHandler<SelectionChangedEventArgs>? SelectionChanged;
        #endregion

        public SmartExpander()
        {
            CreateControls();
            // Aggiungi l'handler per l'evento ExpandedChanged
            Loaded += OnSmartExpanderLoaded;
        }

        private void OnSmartExpanderLoaded(object? sender, EventArgs e)
        {
            // Precarica il contenuto dell'expander per velocizzare la prima apertura
            if (_expander != null)
            {
                _expander.ExpandedChanged += OnExpandedChanged;
                PreloadExpanderContent();
            }
        }

        private async void PreloadExpanderContent()
        {
            // Forza il rendering del contenuto nascosto per velocizzare la prima apertura
            if (_expander.Content is VisualElement content)
            {
                // Rendi brevemente visibile e poi nascosto per forzare il layout
                var originalOpacity = content.Opacity;
                var originalVisible = content.IsVisible;

                content.Opacity = 0;
                content.IsVisible = true;

                // Attendi un frame per il layout
                await Task.Delay(16);

                // Calcola e imposta l'altezza dinamica
                await CalculateAndSetDynamicHeight();

                // Ripristina stato originale
                content.IsVisible = originalVisible;
                content.Opacity = originalOpacity;
            }
        }

        private async Task CalculateAndSetDynamicHeight()
        {
            if (ItemsSource == null) return;

            // Conta gli elementi nell'ItemsSource
            var itemCount = 0;
            if (ItemsSource is ICollection collection)
            {
                itemCount = collection.Count;
            }
            else
            {
                // Per IEnumerable generici, conta manualmente
                foreach (var item in ItemsSource)
                {
                    itemCount++;
                }
            }

            if (itemCount == 0) return;

            // Calcola l'altezza necessaria (numero elementi × altezza singolo elemento)
            var requiredHeight = itemCount * ItemHeight;

            // Usa il minimo tra altezza necessaria e altezza massima
            var finalHeight = Math.Min(requiredHeight, MaxHeight);

            // Aggiorna le altezze dei controlli
            if (_expander?.Content is Border contentBorder && contentBorder.Content is ScrollView scrollView)
            {
                await Dispatcher.DispatchAsync(() =>
                {
                    contentBorder.HeightRequest = finalHeight;
                    scrollView.HeightRequest = finalHeight;

                    // Aggiorna anche il behavior con la nuova altezza
                    var behavior = _expander.Behaviors.OfType<ExpanderAnimationBehavior>().FirstOrDefault();
                    if (behavior != null)
                    {
                        behavior.MaxExpandedHeight = finalHeight;
                    }
                });
            }

            System.Diagnostics.Debug.WriteLine($"SmartExpander: {itemCount} elementi, altezza calcolata: {finalHeight}px (max: {MaxHeight}px)");
        }

        private void OnExpandedChanged(object? sender, ExpandedChangedEventArgs e)
        {
            // Anima la freccia quando l'expander si apre/chiude
            if (_arrowLabel != null)
            {
                var targetRotation = e.IsExpanded ? 180 : 0;
                _ = _arrowLabel.RotateTo(targetRotation, 350, Easing.CubicInOut);
            }
        }

        // Proprietà pubblica per accedere all'expander interno
        public Expander InternalExpander => _expander;

        // Metodo per chiudere l'expander programmaticamente
        public async Task CloseExpanderSafely()
        {
            if (_expander.IsExpanded)
            {
                var animationBehavior = _expander.Behaviors
                    .OfType<ExpanderAnimationBehavior>()
                    .FirstOrDefault();

                if (animationBehavior != null)
                {
                    // Attendi che eventuali animazioni finiscano
                    var maxWait = TimeSpan.FromMilliseconds(800);
                    var checkInterval = TimeSpan.FromMilliseconds(50);
                    var stopwatch = System.Diagnostics.Stopwatch.StartNew();

                    while (stopwatch.Elapsed < maxWait && animationBehavior.IsAnimating)
                    {
                        await Task.Delay(checkInterval);
                    }
                }

                if (!IsExpanderAnimating())
                {
                    _expander.IsExpanded = false;
                }
            }
        }

        private bool IsExpanderAnimating()
        {
            var animationBehavior = _expander.Behaviors
                .OfType<ExpanderAnimationBehavior>()
                .FirstOrDefault();
            return animationBehavior?.IsAnimating ?? false;
        }

        private void CreateControls()
        {
            // Crea l'header dell'expander
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
                Text = string.IsNullOrEmpty(PlaceholderText) ? string.Empty : PlaceholderText,
                IsVisible = !string.IsNullOrEmpty(PlaceholderText)
            };
            // Applica lo style se esiste
            if (Application.Current?.Resources?.TryGetValue("PickerHeaderTextStyle", out var headerTextStyle) == true && headerTextStyle is Style headerStyle)
            {
                _selectedItemLabel.Style = headerStyle;
            }

            _arrowLabel = new Label
            {
                Text = ArrowIcon, // Usa la proprietà bindabile
                FontSize = 20,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.End,
                TextColor = Colors.Black,
                Margin = new Thickness(0, 0, 5, 0),
                FontFamily = "FASolid" // Font per l'icona
            };
            // Applica lo style se esiste
            if (Application.Current?.Resources?.TryGetValue("PickerArrowStyle", out var arrowStyle) == true && arrowStyle is Style arrowStyleTyped)
            {
                _arrowLabel.Style = arrowStyleTyped;
            }

            headerGrid.Children.Add(_selectedItemLabel);
            headerGrid.Children.Add(_arrowLabel);
            Grid.SetColumn(_selectedItemLabel, 0);
            Grid.SetColumn(_arrowLabel, 1);

            var headerBorder = new Border
            {
                Content = headerGrid
            };
            // Applica lo style se esiste
            if (Application.Current?.Resources?.TryGetValue("PickerBorderStyle", out var borderStyle) == true && borderStyle is Style style)
            {
                headerBorder.Style = style;
            }

            // Crea la CollectionView
            _collectionView = new CollectionView
            {
                SelectionMode = SelectionMode.Single,
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.Fill,
                BackgroundColor = Colors.LightBlue
            };
            _collectionView.SelectionChanged += OnInternalSelectionChanged;

            // Crea il contenuto dell'expander (Border + ScrollView + CollectionView)
            var scrollView = new ScrollView
            {
                Content = _collectionView,
                VerticalScrollBarVisibility = ScrollBarVisibility.Always,
                HeightRequest = MaxHeight
            };

            var contentBorder = new Border
            {
                Content = scrollView,
                HeightRequest = MaxHeight,
                Padding = new Thickness(0)
            };
            // Applica lo style se esiste
            if (Application.Current?.Resources?.TryGetValue("PickerBorderStyle", out var contentBorderStyle) == true && contentBorderStyle is Style contentStyle)
            {
                contentBorder.Style = contentStyle;
            }

            // Crea l'Expander principale
            _expander = new Expander
            {
                Header = headerBorder,
                Content = contentBorder,
                VerticalOptions = LayoutOptions.Start
            };

            // Aggiungi il behavior di animazione
            var animationBehavior = new ExpanderAnimationBehavior
            {
                AnimationDuration = 350,
                MaxExpandedHeight = MaxHeight
            };
            _expander.Behaviors.Add(animationBehavior);

            // Imposta il contenuto del controllo
            Content = _expander;

            // Aggiorna il template della CollectionView
            UpdateItemTemplate();
        }

        private void UpdateItemTemplate()
        {
            if (_collectionView == null) return;

            var template = new DataTemplate(() =>
            {
                var grid = new Grid
                {
                    Padding = new Thickness(10),
                    HeightRequest = ItemHeight
                };

                // Applica lo style se esiste
                if (Application.Current?.Resources?.TryGetValue("SelectableGridStyle", out var gridStyle) == true && gridStyle is Style style)
                {
                    grid.Style = style;
                }

                var label = new Label
                {
                    VerticalOptions = LayoutOptions.Center
                };

                // Applica lo style se esiste
                if (Application.Current?.Resources?.TryGetValue("PickerItemTextStyle", out var itemTextStyle) == true && itemTextStyle is Style itemStyle)
                {
                    label.Style = itemStyle;
                }

                // Binding dinamico basato su DisplayMemberPath
                if (!string.IsNullOrEmpty(DisplayMemberPath))
                {
                    label.SetBinding(Label.TextProperty, new Binding(DisplayMemberPath));
                }
                else
                {
                    // Fallback: usa ToString()
                    label.SetBinding(Label.TextProperty, new Binding("."));
                }

                grid.Children.Add(label);
                return grid;
            });

            _collectionView.ItemTemplate = template;
        }

        private void OnInternalSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection?.FirstOrDefault() is object selectedItem)
            {
                SelectedItem = selectedItem;
                UpdateSelectedItemDisplay();

                // Chiama il metodo standardizzato nel ViewModel per gestire la sessione
                CallViewModelSessionMethod(selectedItem);

                // Chiudi l'expander dopo la selezione
                _ = Task.Run(async () =>
                {
                    await Task.Delay(100);
                    await Dispatcher.DispatchAsync(async () =>
                    {
                        await CloseExpanderSafely();
                    });
                });

                // Propaga l'evento
                SelectionChanged?.Invoke(this, e);
            }
        }

        private void CallViewModelSessionMethod(object selectedItem)
        {
            // Trova la pagina contenitore
            var page = this.GetParentPage();
            if (page?.BindingContext != null)
            {
                var viewModel = page.BindingContext;

                // Cerca il metodo OnSmartExpanderSelectionChanged nel ViewModel
                var method = viewModel.GetType().GetMethod("OnSmartExpanderSelectionChanged",
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                if (method != null)
                {
                    try
                    {
                        method.Invoke(viewModel, new object[] { selectedItem });
                        System.Diagnostics.Debug.WriteLine($"SmartExpander: Chiamato OnSmartExpanderSelectionChanged nel ViewModel con {selectedItem}");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"SmartExpander: Errore chiamando OnSmartExpanderSelectionChanged nel ViewModel: {ex.Message}");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("SmartExpander: Metodo OnSmartExpanderSelectionChanged non trovato nel ViewModel");
                }
            }
        }

        private void UpdateSelectedItemDisplay()
        {
            if (SelectedItem == null)
            {
                if (string.IsNullOrEmpty(PlaceholderText))
                {
                    _selectedItemLabel.Text = string.Empty;
                    _selectedItemLabel.IsVisible = false;
                }
                else
                {
                    _selectedItemLabel.Text = PlaceholderText;
                    _selectedItemLabel.IsVisible = true;
                }
                return;
            }

            // Elemento selezionato - sempre visibile
            _selectedItemLabel.IsVisible = true;

            if (!string.IsNullOrEmpty(DisplayMemberPath))
            {
                // Usa reflection per ottenere il valore della proprietà
                var property = SelectedItem.GetType().GetProperty(DisplayMemberPath);
                if (property != null)
                {
                    var value = property.GetValue(SelectedItem);
                    _selectedItemLabel.Text = value?.ToString() ?? (string.IsNullOrEmpty(PlaceholderText) ? string.Empty : PlaceholderText);
                    return;
                }
            }

            // Fallback: usa ToString()
            _selectedItemLabel.Text = SelectedItem.ToString() ?? (string.IsNullOrEmpty(PlaceholderText) ? string.Empty : PlaceholderText);
        }

        #region Property Changed Handlers

        private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is SmartExpander expander)
            {
                expander._collectionView.ItemsSource = newValue as IEnumerable;

                // Ricalcola l'altezza quando cambia ItemsSource
                _ = Task.Run(async () =>
                {
                    await Task.Delay(100); // Attendi che la CollectionView si aggiorni
                    await expander.CalculateAndSetDynamicHeight();
                });
            }
        }

        private static void OnSelectedItemChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is SmartExpander expander)
            {
                expander._collectionView.SelectedItem = newValue;
                expander.UpdateSelectedItemDisplay();
            }
        }

        private static void OnDisplayMemberPathChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is SmartExpander expander)
            {
                expander.UpdateItemTemplate();
                expander.UpdateSelectedItemDisplay();
            }
        }

        private static void OnMaxHeightChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is SmartExpander expander && newValue is double maxHeight)
            {
                // Ricalcola l'altezza con il nuovo massimo
                _ = Task.Run(async () =>
                {
                    await expander.CalculateAndSetDynamicHeight();
                });
            }
        }

        private static void OnItemHeightChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is SmartExpander expander)
            {
                expander.UpdateItemTemplate();

                // Ricalcola l'altezza con la nuova altezza elemento
                _ = Task.Run(async () =>
                {
                    await expander.CalculateAndSetDynamicHeight();
                });
            }
        }

        #endregion
    }
}