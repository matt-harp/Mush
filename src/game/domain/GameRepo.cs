namespace Mush;

using System;
using Godot;

public interface IGameRepo : IDisposable {
}

public class GameRepo : IGameRepo {
    private bool _disposed;


    private void Dispose(bool disposing) {
        if (_disposed) {
            return;
        }

        if (disposing) {
            // dispose managed objects
            GD.Print("game disposing");
        }

        _disposed = true;
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
