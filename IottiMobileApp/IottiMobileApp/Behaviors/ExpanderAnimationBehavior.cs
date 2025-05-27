using System;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;

namespace IottiMobileApp.Behaviors
{
    public class ExpanderAnimationBehavior : Behavior<Expander>
    {
        public uint AnimationDuration { get; set; } = 350;
        public double MaxExpandedHeight { get; set; } = 300; // Altezza massima prima dello scroll

        private bool _isAnimating;
        private double _lastMeasuredHeight;
        private Expander? _attachedExpander;
        private DateTime _lastClickTime = DateTime.MinValue;
        private readonly TimeSpan _clickDebounceTime = TimeSpan.FromMilliseconds(100);

        // Proprietà pubblica per controllare lo stato dell'animazione
        public bool IsAnimating => _isAnimating;

        protected override void OnAttachedTo(Expander expander)
        {
            base.OnAttachedTo(expander);
            _attachedExpander = expander;
            expander.ExpandedChanged += OnExpandedChanged;

            // Inizializza il contenuto
            if (expander.Content is View content)
            {
                InitializeContent(content);
            }
        }

        protected override void OnDetachingFrom(Expander expander)
        {
            expander.ExpandedChanged -= OnExpandedChanged;
            _attachedExpander = null;
            base.OnDetachingFrom(expander);
        }

        private void InitializeContent(View content)
        {
            // Imposta lo stato iniziale
            content.Opacity = 0;
            content.IsVisible = false;
            content.InputTransparent = true;

            // Se il contenuto è wrappato in un Border, trova la CollectionView interna
            if (content is Border border && border.Content is CollectionView collectionView)
            {
                SetupCollectionView(collectionView);
                // Imposta anche l'altezza del Border wrapper
                border.HeightRequest = MaxExpandedHeight;
            }
            else if (content is CollectionView directCollectionView)
            {
                SetupCollectionView(directCollectionView);
            }
            else if (content is ScrollView scrollView)
            {
                SetupScrollView(scrollView);
            }
            else
            {
                // Per altri contenuti, imposta altezza generica
                SetupGenericContent(content);
            }
        }

        //metodi di setup

        private void SetupCollectionView(CollectionView collectionView)
        {
            // IMPORTANTE: Imposta altezza fissa per limitare l'espansione
            collectionView.HeightRequest = MaxExpandedHeight;

            // Assicurati che sia scrollabile verticalmente
            collectionView.VerticalScrollBarVisibility = ScrollBarVisibility.Always;

            // Imposta il layout per essere contenuto nell'altezza specificata
            collectionView.VerticalOptions = LayoutOptions.Fill;
            collectionView.HorizontalOptions = LayoutOptions.Fill;
        }

        private void SetupScrollView(ScrollView scrollView)
        {
            // Imposta altezza massima per ScrollView
            scrollView.HeightRequest = MaxExpandedHeight;
            scrollView.VerticalScrollBarVisibility = ScrollBarVisibility.Always;
            scrollView.VerticalOptions = LayoutOptions.Fill;
        }

        private void SetupGenericContent(View content)
        {
            // Per contenuti generici, imposta un'altezza massima
            if (content.HeightRequest < 0) // Se non ha altezza specifica
            {
                content.HeightRequest = MaxExpandedHeight;
            }
            else if (content.HeightRequest > MaxExpandedHeight)
            {
                content.HeightRequest = MaxExpandedHeight;
            }
        }

        //handler dell'evento di click sul picker
        private async void OnExpandedChanged(object? sender, ExpandedChangedEventArgs e)
        {
            if (sender is not Expander expander || expander.Content is not View content)
                return;

            // Debounce: ignora click troppo ravvicinati
            var now = DateTime.Now;
            if (now - _lastClickTime < _clickDebounceTime)
            {
                // Ripristina stato senza animazione
                await expander.Dispatcher.DispatchAsync(() =>
                {
                    if (_attachedExpander != null)
                    {
                        _attachedExpander.ExpandedChanged -= OnExpandedChanged;
                        _attachedExpander.IsExpanded = !e.IsExpanded;
                        _attachedExpander.ExpandedChanged += OnExpandedChanged;
                    }
                });
                return;
            }

            // Se c'è già un'animazione in corso, ignora completamente il click
            if (_isAnimating)
            {
                // Ripristina lo stato precedente dell'expander
                await expander.Dispatcher.DispatchAsync(() =>
                {
                    if (_attachedExpander != null)
                    {
                        // Temporaneamente rimuovi l'event handler per evitare loop
                        _attachedExpander.ExpandedChanged -= OnExpandedChanged;
                        _attachedExpander.IsExpanded = !e.IsExpanded;
                        _attachedExpander.ExpandedChanged += OnExpandedChanged;
                    }
                });
                return;
            }

            _lastClickTime = now;

            // Disabilita ulteriori interazioni
            DisableExpanderInteraction(expander, true);

            try
            {
                // Avvia animazione
                await AnimateExpansion(expander, content, e.IsExpanded);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Animation error: {ex.Message}");
            }
            finally
            {
                // Riabilita interazioni
                DisableExpanderInteraction(expander, false);
            }
        }

        //blocco l'animazione se ce n'è una in corso
        private void DisableExpanderInteraction(Expander expander, bool disable)
        {
            // Disabilita l'header per prevenire click durante animazione
            if (expander.Header is View header)
            {
                header.InputTransparent = disable;
            }
        }

        //metodo di controllo per quale animazione avviare
        private async Task AnimateExpansion(Expander expander, View content, bool isExpanding)
        {
            if (_isAnimating)
                return;

            _isAnimating = true;

            try
            {
                // Cancella animazioni precedenti
                content.CancelAnimations();
                var arrow = expander.FindByName<Label>("ArrowLabel");
                arrow?.CancelAnimations();

                if (isExpanding)
                {
                    await AnimateOpen(content, arrow);
                }
                else
                {
                    await AnimateClose(content, arrow);
                }
            }
            catch (Exception ex)
            {
                // Log dell'errore se necessario
                System.Diagnostics.Debug.WriteLine($"Animation error: {ex.Message}");
            }
            finally
            {
                _isAnimating = false;
            }
        }

        //animazione per l'aperta del picker
        private async Task AnimateOpen(View content, Label? arrow)
        {
            // Disabilita input durante l'animazione
            content.InputTransparent = true;

            // Misura il contenuto se necessario
            if (_lastMeasuredHeight <= 0)
            {
                await MeasureContent(content);
            }

            // Calcola l'offset di partenza
            double startOffset = Math.Min(_lastMeasuredHeight, MaxExpandedHeight) + 20;

            // Imposta stato iniziale
            content.TranslationY = -startOffset;
            content.Opacity = 0;
            content.IsVisible = true;

            // Animazioni parallele con easing diversificati
            var contentSlide = content.TranslateTo(0, 0, AnimationDuration, Easing.CubicOut);
            var contentFade = content.FadeTo(1, (uint)(AnimationDuration * 0.8), Easing.SinOut);
            var arrowRotate = arrow?.RotateTo(180, AnimationDuration, Easing.CubicInOut);

            // Attendi tutte le animazioni
            if (arrowRotate != null)
            {
                await Task.WhenAll(contentSlide, contentFade, arrowRotate);
            }
            else
            {
                await Task.WhenAll(contentSlide, contentFade);
            }

            // Riabilita input
            content.InputTransparent = false;
        }

        //animazione per il collapse del picker
        private async Task AnimateClose(View content, Label? arrow)
        {
            // Disabilita input immediatamente
            content.InputTransparent = true;

            // Calcola offset di uscita
            double endOffset = Math.Min(_lastMeasuredHeight, MaxExpandedHeight) + 20;

            // Animazioni parallele
            var contentSlide = content.TranslateTo(0, -endOffset, AnimationDuration, Easing.CubicIn);
            var contentFade = content.FadeTo(0, (uint)(AnimationDuration * 0.6), Easing.SinIn);
            var arrowRotate = arrow?.RotateTo(0, AnimationDuration, Easing.CubicInOut);

            // Attendi tutte le animazioni
            if (arrowRotate != null)
            {
                await Task.WhenAll(contentSlide, contentFade, arrowRotate);
            }
            else
            {
                await Task.WhenAll(contentSlide, contentFade);
            }

            // Nascondi completamente
            content.IsVisible = false;
        }

        //metodo per calcolare l'altezza degli elementi
        private async Task MeasureContent(View content)
        {
            // Forza una misurazione del contenuto
            try
            {
                // Rendi temporaneamente visibile per la misurazione
                var wasVisible = content.IsVisible;
                var wasOpaque = content.Opacity;

                content.IsVisible = true;
                content.Opacity = 0; // Invisibile ma misurato

                // Attendi il layout
                await Task.Delay(100); // Aumentato per dare più tempo al layout

                // Ottieni l'altezza, ma rispetta sempre il massimo
                var measuredHeight = content.Height > 0 ? content.Height : MaxExpandedHeight;
                _lastMeasuredHeight = Math.Min(measuredHeight, MaxExpandedHeight);

                // Per CollectionView, forza sempre l'altezza massima
                if (content is CollectionView)
                {
                    _lastMeasuredHeight = MaxExpandedHeight;
                }

                // Ripristina stato
                content.IsVisible = wasVisible;
                content.Opacity = wasOpaque;

                Console.WriteLine($"Misurato contenuto: altezza={measuredHeight}, limitata a={_lastMeasuredHeight}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore misurazione: {ex.Message}");
                // Fallback
                _lastMeasuredHeight = MaxExpandedHeight;
            }
        }
    }
}