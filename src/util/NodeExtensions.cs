namespace Mush;

using Chickensoft.GodotNodeInterfaces;

public static class NodeExtensions {
    /// <summary>
    /// Ensure this object is both valid and not about to become invalid
    /// </summary>
    // public static bool IsValid<T>(this T? node) where T : IGodotObject
    // {
    //     return node is not null
    //            && GodotObject.IsInstanceValid(node)
    //            && !node.IsQueuedForDeletion();
    // }

    /// <summary>
    /// QueueFree only if valid
    /// </summary>
    // public static void SafeQueueFree(this INode node)
    // {
    //     if (node.IsValid()) {
    //         node.QueueFree();
    //     }
    // }

    /// <summary>
    /// Use to check inline if an object is valid.
    /// Example:
    /// <code>_myNode.IfValid()?.DoStuff();   // do stuff if the node is valid, else just do not crash</code>
    /// </summary>
    /// <returns>the object, if valid, else null</returns>
    // public static T? IfValid<T>(this T control) where T : IGodotObject
    //     => control.IsValid() ? control : null;
}
