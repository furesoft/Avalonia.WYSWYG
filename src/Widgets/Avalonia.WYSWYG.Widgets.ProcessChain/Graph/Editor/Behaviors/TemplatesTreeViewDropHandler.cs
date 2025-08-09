﻿using Avalonia.Input;
using Avalonia.VisualTree;
using Avalonia.WYSWYG.Widgets.ProcessChain.Graph.Editor.Model;
using Avalonia.Xaml.Interactions.DragAndDrop;

namespace Avalonia.WYSWYG.Widgets.ProcessChain.Graph.Editor.Behaviors;

public class TemplatesTreeViewDropHandler : DropHandlerBase
{
    private bool Validate<T>(TreeView treeView, DragEventArgs e, object sourceContext, object targetContext,
        bool bExecute) where T : INodeTemplate
    {
        if (sourceContext is not T sourceItem
            || targetContext is not INodeTemplatesHost nodeTemplatesHost
            || nodeTemplatesHost.Templates is null
            || treeView.GetVisualAt(e.GetPosition(treeView)) is not Control targetControl
            || targetControl.DataContext is not T targetItem)
        {
            return false;
        }

        var sourceIndex = nodeTemplatesHost.Templates.IndexOf(sourceItem);
        var targetIndex = nodeTemplatesHost.Templates.IndexOf(targetItem);

        if (e.DragEffects == DragDropEffects.Copy)
        {
            return false;
        }

        if (e.DragEffects == DragDropEffects.Move)
        {
            if (bExecute)
            {
                MoveItem(nodeTemplatesHost.Templates, sourceIndex, targetIndex);
            }

            return true;
        }

        if (e.DragEffects == DragDropEffects.Link)
        {
            if (bExecute)
            {
                SwapItem(nodeTemplatesHost.Templates, sourceIndex, targetIndex);
            }

            return true;
        }

        return false;
    }

    public override bool Validate(object sender, DragEventArgs e, object sourceContext, object targetContext,
        object state)
    {
        if (e.Source is Control && sender is TreeView listBox)
        {
            return Validate<INodeTemplate>(listBox, e, sourceContext, targetContext, false);
        }

        return false;
    }

    public override bool Execute(object sender, DragEventArgs e, object sourceContext, object targetContext,
        object state)
    {
        if (e.Source is Control && sender is TreeView listBox)
        {
            return Validate<INodeTemplate>(listBox, e, sourceContext, targetContext, true);
        }

        return false;
    }
}
