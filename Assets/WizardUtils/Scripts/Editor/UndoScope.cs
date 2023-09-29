using System;
using UnityEditor;

public class UndoScope : IDisposable
{
    private int undoGroup;

    public UndoScope(string text)
    {
        Undo.SetCurrentGroupName(text);
        undoGroup = Undo.GetCurrentGroup();
    }

    public void Dispose()
    {
        Undo.CollapseUndoOperations(undoGroup);
    }
}