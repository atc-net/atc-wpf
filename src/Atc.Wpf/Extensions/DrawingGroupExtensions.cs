namespace Atc.Wpf.Extensions;

public static class DrawingGroupExtensions
{
    /// <summary>
    /// Applies the transform.
    /// </summary>
    /// <param name="objDrawing">The object drawing.</param>
    /// <param name="objTransform">The object transform.</param>
    public static void ApplyTransform(
        this DrawingGroup objDrawing,
        Transform objTransform)
    {
        ArgumentNullException.ThrowIfNull(objDrawing);

        // Apply the translation directly if the transform is a TranslateTransform
        if (objTransform is TranslateTransform transform)
        {
            transform.X -= objDrawing.Bounds.X;
            transform.Y -= objDrawing.Bounds.Y;
        }

        // Determine how to apply the new transform based on the existing one
        switch (objDrawing.Transform)
        {
            case null:
                // If there's no existing transform, set the new one directly
                objDrawing.Transform = objTransform;
                break;
            case TransformGroup transformGroup:
                // If the existing transform is a group, add the new transform to it
                transformGroup.Children.Add(objTransform);
                break;
            default:
                // If there's an existing transform, but it's not a group, create a new group
                objDrawing.Transform = new TransformGroup
                {
                    Children =
                    {
                        objDrawing.Transform,
                        objTransform,
                    },
                };
                break;
        }
    }
}