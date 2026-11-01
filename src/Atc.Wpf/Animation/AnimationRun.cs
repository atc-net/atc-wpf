namespace Atc.Wpf.Animation;

/// <summary>
/// One call of an <see cref="AnimationExtensions"/> helper: the clocks it applies and the task that reports
/// their end.
/// </summary>
/// <remarks>
/// <para>
/// The task completes when every clock completes, or as soon as a newer run takes over one of the same
/// properties (the run is then superseded and its finishing action is skipped). Cancellation stops the clocks
/// where they are and cancels the task.
/// </para>
/// <para>
/// Each running run is stored per animated object, which keeps its clocks alive: WPF holds root clocks weakly,
/// so a clock that a newer animation detached from the property would otherwise be collected before it
/// raised <see cref="Clock.Completed"/>, and the task would never complete.
/// </para>
/// </remarks>
internal sealed class AnimationRun
{
    private static readonly ConditionalWeakTable<DependencyObject, Dictionary<DependencyProperty, AnimationRun>> RunningByTarget = new();

    private readonly IReadOnlyList<AnimationTarget> targets;
    private readonly Action? onFinished;
    private readonly TaskCompletionSource completion = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly List<AnimationClock> clocks = [];
    private CancellationTokenRegistration cancellationRegistration;
    private int pendingClocks;
    private bool isDone;

    public AnimationRun(
        IReadOnlyList<AnimationTarget> targets,
        Action? onFinished)
    {
        this.targets = targets;
        this.onFinished = onFinished;
    }

    /// <summary>
    /// Gets the task that completes when the run finishes or is superseded, or is cancelled.
    /// </summary>
    public Task Completion => completion.Task;

    /// <summary>
    /// Applies the clocks. Must be called on the targets' dispatcher thread.
    /// </summary>
    /// <param name="dispatcher">The dispatcher that owns the targets.</param>
    /// <param name="cancellationToken">Stops the run and cancels <see cref="Completion"/>.</param>
    public void Start(
        Dispatcher dispatcher,
        CancellationToken cancellationToken)
    {
        foreach (var target in targets)
        {
            var running = RunningByTarget.GetOrCreateValue((DependencyObject)target.Target);
            if (running.TryGetValue(target.Property, out var previous) && previous != this)
            {
                previous.Supersede();
            }

            running[target.Property] = this;
        }

        pendingClocks = targets.Count;
        foreach (var target in targets)
        {
            var clock = target.Animation.CreateClock();
            clock.Completed += OnClockCompleted;
            clocks.Add(clock);
            target.Target.ApplyAnimationClock(target.Property, clock, HandoffBehavior.SnapshotAndReplace);
        }

        if (cancellationToken.CanBeCanceled)
        {
            cancellationRegistration = cancellationToken.Register(() =>
            {
                if (dispatcher.CheckAccess())
                {
                    Cancel(cancellationToken);
                }
                else
                {
                    _ = dispatcher.BeginInvoke(() => Cancel(cancellationToken));
                }
            });
        }
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "A failing finishing action faults the task instead of escaping into the timing loop.")]
    private void OnClockCompleted(
        object? sender,
        EventArgs e)
    {
        if (isDone || --pendingClocks > 0)
        {
            return;
        }

        End();
        try
        {
            onFinished?.Invoke();
            completion.TrySetResult();
        }
        catch (Exception ex)
        {
            completion.TrySetException(ex);
        }
    }

    private void Supersede()
    {
        if (isDone)
        {
            return;
        }

        End();
        completion.TrySetResult();
    }

    private void Cancel(CancellationToken cancellationToken)
    {
        if (isDone)
        {
            return;
        }

        End();
        completion.TrySetCanceled(cancellationToken);

        for (var i = 0; i < targets.Count; i++)
        {
            var target = targets[i];
            var owner = (DependencyObject)target.Target;
            var current = owner.GetValue(target.Property);
            clocks[i].Controller?.Remove();
            owner.SetValue(target.Property, current);
        }
    }

    private void End()
    {
        isDone = true;
        cancellationRegistration.Dispose();

        foreach (var target in targets)
        {
            if (RunningByTarget.TryGetValue((DependencyObject)target.Target, out var running) &&
                running.TryGetValue(target.Property, out var current) &&
                current == this)
            {
                running.Remove(target.Property);
            }
        }
    }
}