﻿using Avalonia.WYSWYG.Widgets.ProcessChain.Graph.Designer;
using Avalonia.WYSWYG.Widgets.ProcessChain.Graph.Designer.Layout;
using Avalonia.WYSWYG.Widgets.ProcessChain.Graph.Editor;
using Avalonia.WYSWYG.Widgets.ProcessChain.Graph.Editor.Model;
using Avalonia.WYSWYG.Widgets.ProcessChain.Graph.Editor.MVVM;
using Avalonia.WYSWYG.Widgets.ProcessChain.Graph.Nodes;
using DrawingNodeViewModel = Avalonia.WYSWYG.Widgets.ProcessChain.Graph.Editor.MVVM.DrawingNodeViewModel;
using NodeViewModel = Avalonia.WYSWYG.Widgets.ProcessChain.Graph.Editor.MVVM.NodeViewModel;

namespace Avalonia.WYSWYG.Widgets.ProcessChain.Graph;

public partial class NodeFactory
{
    public IDrawingNode CreateDrawing(string name = null)
    {
        var drawing = new DrawingNodeViewModel
        {
            X = 0,
            Y = 0,
            Name = name,
            Width = 90000,
            Height = 60000,
            Nodes = new ObservableCollection<INode>(),
            Connectors = new ObservableCollection<IConnector>(),
            EnableSnap = true,
            SnapX = 30.0,
            SnapY = 30.0,
            GridCellHeight = 30,
            GridCellWidth = 30,
            EnableGrid = true
        };

        var entryX = SnapHelper.Snap(drawing.Width / 2 - 175, drawing.SnapX);
        var entryY = SnapHelper.Snap(drawing.Height / 2 - 200, drawing.SnapY);

        CreateEntry((entryX, entryY), drawing);

        return drawing;
    }

    private static CustomNodeViewModel CreateViewModel(EmptyNode vm, (double x, double y) position,
        (double width, double height) size)
    {
        var node = new CustomNodeViewModel
        {
            X = position.x,
            Y = position.y,
            Width = size.width,
            Height = size.height,
            Pins = new ObservableCollection<IPin>(),
            Content = vm
        };

        return node;
    }

    private static void CreateEntry((double x, double y) position, IDrawingNode drawing)
    {
        var node = CreateNode(new EntryNode(), position);
        node.IsRemovable = false;
        node.Parent = drawing;

        drawing.Nodes!.Add(node);
    }

    private static double CalculateSinglePin(double width, int pinCount, int i)
    {
        return width / (pinCount + 1) * (i + 1);
    }

    private static void AddPins(
        IEnumerable<PinDescriptor> pins,
        NodeViewModel viewModel, Func<int, (double, double)> positionMapper)
    {
        var pinArray = pins as PinDescriptor[] ??
                       pins.ToArray();

        for (var i = 0; i < pinArray.Length; i++)
        {
            var pin = pinArray[i];
            var (baseX, baseY) = positionMapper(i);

            viewModel.AddPin((baseX, baseY), (PinSize, PinSize), pin.Direction, pin.Alignment,
                pin.Name, pin.MultipleConnections);
        }
    }
}
