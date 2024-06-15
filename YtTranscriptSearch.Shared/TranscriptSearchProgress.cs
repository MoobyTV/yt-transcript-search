namespace YtTranscriptSearch.Shared
{
    public record TranscriptSearchProgress
    {
        public string? VideoTitle { get; set; }
        public Exception? Exception { get; set; }
        public TranscriptSearchMatch[]? Matches { get; set; }
    }
}
