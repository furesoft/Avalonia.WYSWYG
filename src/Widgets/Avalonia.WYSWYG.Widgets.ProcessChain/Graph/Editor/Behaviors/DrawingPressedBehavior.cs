﻿using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.WYSWYG.Widgets.ProcessChain.Graph.Editor.Model;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.WYSWYG.Widgets.ProcessChain.Graph.Editor.Behaviors;

public class DrawingPressedBehavior : Behavior<ItemsControl>
{
    protected override void OnAttached()
    {
        base.OnAttached();

        if (AssociatedObject is not null)
        {
            AssociatedObject.AddHandler(InputElement.PointerPressedEvent, Pressed,
                RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
        }
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();

        if (AssociatedObject is not null)
        {
            AssociatedObject.RemoveHandler(InputElement.PointerPressedEvent, Pressed);
        }
    }

    private void Pressed(object sender, PointerPressedEventArgs e)
    {
        if (e.Handled)
        {
            return;
        }

        if (AssociatedObject?.DataContext is not IDrawingNode drawingNode)
        {
            return;
        }

        if (e.Source is Control {DataContext: IPin})
        {
            return;
        }

        var info = e.GetCurrentPoint(AssociatedObject);
        var (x, y) = e.GetPosition(AssociatedObject);

        if (info.Properties.IsLeftButtonPressed)
        {
            drawingNode.DrawingLeftPressed(x, y);
        }
        else if (info.Properties.IsRightButtonPressed)
        {
            drawingNode.DrawingRightPressed(x, y);
        }
    }
}
