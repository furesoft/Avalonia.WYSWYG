// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using AdaptiveCards;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.WYSWYG.Widgets.AdaptiveCards.Renderer.Helpers;
using FluentIcons.Avalonia;
using FluentIcons.Common;
using Iciclecreek.Avalonia.Controls.Media;
using LibVLCSharp.Shared;

namespace Avalonia.WYSWYG.Widgets.AdaptiveCards.Renderer;

public static class AdaptiveMediaRenderer
{
    private static readonly List<string> _supportedMimeTypes = new()
    {
        "video/mp4",
        "audio/mp4",
        "audio/mpeg"
    };

    private static readonly List<string> _supportedAudioMimeTypes = new()
    {
        "audio/mp4",
        "audio/mpeg"
    };

    public static Control Render(AdaptiveMedia media, AdaptiveRenderContext context)
    {
        // If host doesn't support interactivity or no media source is provided
        // just return the poster image if present
        if (!context.Config.SupportsInteractivity || media.Sources.Count == 0) return GetPosterImage(media, context);

        var mediaSource = GetMediaSource(media, context);
        if (mediaSource == null) return null;

        // Main element to return

        var uiMedia = new Grid();

        #region Thumbnail button

        var mediaConfig = context.Config.Media;
        var uiThumbnailButton = new Grid
        {
            Name = "thumbnailButton",
            IsVisible = true
        };

        /* Poster Image */

        // A poster container is necessary to handle background color and opacity mask
        // in case poster image is not found or does not exist
        var uiPosterContainer = new Grid
        {
            Background = _controlBackgroundColor
        };

        var uiPosterImage = GetPosterImage(media, context);
        if (uiPosterImage != null) uiPosterContainer.Children.Add(uiPosterImage);

        uiThumbnailButton.Children.Add(uiPosterContainer);

        // Play button
        var uiPlayButton = RenderThumbnailPlayButton(context);
        uiThumbnailButton.Children.Add(uiPlayButton);

        // Mouse hover handlers to signify playable media element
        uiThumbnailButton.PointerEntered += (sender, e) => { uiPlayButton.Opacity = 1.0; };
        uiThumbnailButton.PointerExited += (sender, e) => { uiPlayButton.Opacity = 0.2; };

        #endregion

        uiMedia.Children.Add(uiThumbnailButton);

        // Play the media
        uiPlayButton.PointerReleased += (sender, e) =>
        {
            if (mediaConfig.AllowInlinePlayback)
            {
                var videoView = new VideoView
                {
                    Content = new MediaPlayerControls
                    {
                        Background = context.GetColorBrush(context.Config.ContainerStyles.Emphasis.BackgroundColor),
                        Foreground = context.GetColorBrush(context.Config.ContainerStyles.Emphasis.ForegroundColors
                            .Accent.Default)
                    },
                    ZIndex = 100
                };

                videoView.MediaPlayer.EnableHardwareDecoding = true;

                videoView.MediaPlayer.MediaChanged += (sender, e) =>
                {
                    e.Media.ParsedChanged += (sender2, e2) =>
                    {
                        Dispatcher.UIThread.Invoke(() =>
                        {
                            // resize viewer height to match layout width respecting aspect ratio 
                            // we assume width is correct and calculate height based on aspect ratio
                            var videoTrack = videoView.MediaPlayer.Media.Tracks
                                .Where(t => t.TrackType == TrackType.Video).FirstOrDefault().Data.Video;
                            var ratio = videoTrack.Height / (double) videoTrack.Width;
                            var parent = videoView.Parent as Grid;
                            videoView.Height = parent.Bounds.Width * ratio;
                        });
                    };
                };
                uiMedia.Children.Add(videoView);
                videoView.Source = mediaSource.Url;
                videoView.MediaPlayerViewModel.Play();
            }
            // Raise an event to send the media to host
            else
            {
                context.ClickMedia(uiPosterContainer, new(media));

                // Prevent nested events from triggering
                e.Handled = true;
            }
        };

        return uiMedia;
    }

    private static Control RenderThumbnailPlayButton(AdaptiveRenderContext context)
    {
        var playButtonSize = 70;

        // Wrap in a Viewbox to control width, height, and aspect ratio
        var uiPlayButton = new Viewbox
        {
            Width = playButtonSize,
            Height = playButtonSize,
            Stretch = Stretch.Fill,
            Margin = _marginThickness,
            Opacity = 0.2
        };

        var mediaConfig = context.Config.Media;
        if (!string.IsNullOrEmpty(mediaConfig.PlayButton)
            && context.Config.ResolveFinalAbsoluteUri(mediaConfig.PlayButton) != null)
        {
            // Try to use provided image from host config
            var content = new Image
            {
                Height = playButtonSize
            };

            var finalUri = context.Config.ResolveFinalAbsoluteUri(mediaConfig.PlayButton);
            content.SetImageSource(finalUri, context);

            uiPlayButton.Child = content;
        }
        else
        {
            // Otherwise, use the default play symbol
            uiPlayButton.Child = new SymbolIcon
            {
                Symbol = Symbol.Play,
                IsFilled = true,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
                //Background = context.GetColorBrush(context.RenderArgs.ForegroundColors.Light.Default),
                //Foreground = context.GetColorBrush(context.RenderArgs.ForegroundColors.Dark.Default),
            };
        }

        return uiPlayButton;
    }


    /**
     * Get poster image from either card payload or host config
     */
    private static Image GetPosterImage(AdaptiveMedia media, AdaptiveRenderContext context)
    {
        Image uiPosterImage = null;
        if (!string.IsNullOrEmpty(media.Poster) && context.Config.ResolveFinalAbsoluteUri(media.Poster) != null)
        {
            // Use the provided poster
            uiPosterImage = new();
            var finalUri = context.Config.ResolveFinalAbsoluteUri(media.Poster);
            uiPosterImage.SetImageSource(finalUri, context);
        }
        else if (!string.IsNullOrEmpty(context.Config.Media.DefaultPoster)
                 && context.Config.ResolveFinalAbsoluteUri(context.Config.Media.DefaultPoster) != null)
        {
            // Use the default poster from host
            uiPosterImage = new();
            var finalUri = context.Config.ResolveFinalAbsoluteUri(context.Config.Media.DefaultPoster);
            uiPosterImage.SetImageSource(finalUri, context);
        }

        return uiPosterImage;
    }

    /**
     * Get the first media URI with a supported mime type
     */
    private static AdaptiveMediaSource GetMediaSource(AdaptiveMedia media, AdaptiveRenderContext context)
    {
        // Check if sources contain an invalid mix of MIME types (audio and video)
        bool? isLastMediaSourceAudio = null;
        foreach (var source in media.Sources)
            if (!isLastMediaSourceAudio.HasValue)
            {
                isLastMediaSourceAudio = IsAudio(source);
            }
            else
            {
                if (IsAudio(source) != isLastMediaSourceAudio.Value)
                {
                    // If there is one pair of sources with different MIME types,
                    // it's an invalid mix and a warning should be logged
                    context.Warnings.Add(new(-1, "A Media element contains an invalid mix of MIME type"));
                    return null;
                }

                isLastMediaSourceAudio = IsAudio(source);
            }

        // Return the first supported source with not-null URI
        var isAllHttps = true;
        AdaptiveMediaSource httpsSource = null;
        foreach (var source in media.Sources)
            if (_supportedMimeTypes.Contains(source.MimeType))
            {
                var finalMediaUri = context.Config.ResolveFinalAbsoluteUri(source.Url);
                if (finalMediaUri != null)
                {
                    // Since https is not supported by WPF,
                    // try to use non-https sources first
                    if (finalMediaUri.Scheme != "https")
                    {
                        isAllHttps = false;
                        return source;
                    }

                    if (httpsSource == null)
                    {
                        httpsSource = source;
                    }
                }
            }

        // If all sources are https, log a warning and return the first one
        if (isAllHttps)
        {
            context.Warnings.Add(new(-1,
                "All sources have unsupported https scheme. The host would be responsible for playing the media."));
            return httpsSource;
        }

        // No valid source is found
        context.Warnings.Add(new(-1, "A Media element does not have any valid source"));
        return null;
    }

    private static bool IsAudio(AdaptiveMediaSource mediaSource)
    {
        return _supportedAudioMimeTypes.Contains(mediaSource.MimeType);
    }

    private enum MediaState
    {
        NotStarted = 0,
        IsPlaying,
        IsPaused
    }

    #region Appearance Config

    // Element height and margin
    // NOTE: Child height + child margin * 2 = panel height (50)
    private static readonly int _childHeight = 40;
    private static readonly int _childMargin = 5;
    private static readonly int _panelHeight = _childHeight + _childMargin * 2;
    private static readonly Thickness _marginThickness = new(_childMargin, _childMargin, _childMargin, _childMargin);

    // Use contrasting colors for foreground and background
    private static readonly SolidColorBrush _controlForegroundColor = new(Colors.White);

    private static readonly SolidColorBrush _controlBackgroundColor = new(Colors.Gray)
    {
        Opacity = 0.5
    };

    private static readonly FontFamily _symbolFontFamily = new("Segoe UI Symbol");

    #endregion
}