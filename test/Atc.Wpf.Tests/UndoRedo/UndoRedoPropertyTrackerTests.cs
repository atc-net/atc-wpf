namespace Atc.Wpf.Tests.UndoRedo;

public sealed class UndoRedoPropertyTrackerTests : IDisposable
{
    private readonly UndoRedoService service = new();
    private readonly UndoRedoPropertyTracker sut;

    public UndoRedoPropertyTrackerTests()
    {
        sut = new UndoRedoPropertyTracker(service);
    }

    [Fact]
    public void Track_ChangeProperty_CreatesUndoCommand()
    {
        var obj = new TrackableObject { Name = "Alice" };
        sut.Track(obj);

        obj.Name = "Bob";

        Assert.Single(service.UndoCommands);
        Assert.Equal("Change Name", service.UndoCommands[0].Description);
    }

    [Fact]
    public void Undo_ReversesPropertyChange()
    {
        var obj = new TrackableObject { Name = "Alice" };
        sut.Track(obj);

        obj.Name = "Bob";
        service.Undo();

        Assert.Equal("Alice", obj.Name);
    }

    [Fact]
    public void Redo_ReAppliesPropertyChange()
    {
        var obj = new TrackableObject { Name = "Alice" };
        sut.Track(obj);

        obj.Name = "Bob";
        service.Undo();
        service.Redo();

        Assert.Equal("Bob", obj.Name);
    }

    [Fact]
    public void Untrack_StopsRecording()
    {
        var obj = new TrackableObject { Name = "Alice" };
        sut.Track(obj);
        sut.Untrack(obj);

        obj.Name = "Bob";

        Assert.Empty(service.UndoCommands);
    }

    [Fact]
    public void PropertyFilter_OnlyTracksSpecifiedProperties()
    {
        var obj = new TrackableObject { Name = "Alice", Age = 30 };
        sut.Track(obj, "Name");

        obj.Age = 40;
        Assert.Empty(service.UndoCommands);

        obj.Name = "Bob";
        Assert.Single(service.UndoCommands);
    }

    [Fact]
    public void Dispose_CleansUpAllSubscriptions()
    {
        var obj = new TrackableObject { Name = "Alice" };
        sut.Track(obj);
        sut.Dispose();

        obj.Name = "Bob";

        Assert.Empty(service.UndoCommands);
    }

    [Fact]
    public void MultipleChanges_CreatesMultipleCommands()
    {
        var obj = new TrackableObject { Name = "A" };
        sut.Track(obj);

        obj.Name = "B";
        obj.Name = "C";

        Assert.Equal(2, service.UndoCommands.Count);
    }

    [Fact]
    public void UndoMultipleChanges_RestoresEachStep()
    {
        var obj = new TrackableObject { Name = "A" };
        sut.Track(obj);

        obj.Name = "B";
        obj.Name = "C";

        service.Undo();
        Assert.Equal("B", obj.Name);

        service.Undo();
        Assert.Equal("A", obj.Name);
    }

    [Fact]
    public void Track_SameObjectTwice_IsNoOp()
    {
        var obj = new TrackableObject { Name = "Alice" };
        sut.Track(obj);
        sut.Track(obj);

        obj.Name = "Bob";

        Assert.Single(service.UndoCommands);
    }

    [Fact]
    public void Track_NullTarget_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => sut.Track(null!));
    }

    [Fact]
    public void Untrack_NullTarget_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => sut.Untrack(null!));
    }

    [Fact]
    public void Constructor_NullService_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new UndoRedoPropertyTracker(null!));
    }

    [Fact]
    public void SameValueAssignment_DoesNotCreateCommand()
    {
        var obj = new TrackableObject { Name = "Alice" };
        sut.Track(obj);

        // TrackableObject raises PropertyChanged even if value is the same,
        // but the tracker should detect equal values and skip.
        obj.Name = "Alice";

        Assert.Empty(service.UndoCommands);
    }

    [Fact]
    public void NullPropertyValue_TrackedCorrectly()
    {
        var obj = new TrackableObject { Name = "Alice" };
        sut.Track(obj);

        obj.Name = null!;
        Assert.Single(service.UndoCommands);

        service.Undo();
        Assert.Equal("Alice", obj.Name);
    }

    public void Dispose()
    {
        sut.Dispose();
    }

    private sealed class TrackableObject : INotifyPropertyChanged
    {
        private string? name;
        private int age;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string? Name
        {
            get => name;
            set
            {
                name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            }
        }

        public int Age
        {
            get => age;
            set
            {
                age = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Age)));
            }
        }
    }
}