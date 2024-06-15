using System.Web;
using YoutubeExplode;
using YoutubeExplode.Playlists;
using YoutubeExplode.Videos.ClosedCaptions;
using System.Collections.Specialized;

namespace YtTranscriptSearch.Shared
{
    public class TranscriptSearch(YoutubeClient Youtube, IList<TranscriptSearchMatch> Matches, IProgress<TranscriptSearchProgress> Progress)
    {
        public async Task SearchAsync(TranscriptSearchParameters searchParams, CancellationToken cancellationToken)
        {
            TranscriptSearchProgress currentProgress = new();

            try
            {
                await foreach (PlaylistVideo videoUpload in Youtube.Channels.GetUploadsAsync(searchParams.Channel.Id, cancellationToken))
                {
                    try
                    {
                        currentProgress = new TranscriptSearchProgress
                        {
                            VideoTitle = videoUpload.Title
                        };

                        NameValueCollection queryStringBuilder = HttpUtility.ParseQueryString(videoUpload.Url);
                        queryStringBuilder.Remove("list");

                        string? clipTimestampUrl = queryStringBuilder.ToString();

                        ClosedCaptionManifest manifest = await Youtube.Videos.ClosedCaptions.GetManifestAsync(videoUpload.Id, cancellationToken);
                        ClosedCaptionTrackInfo trackInfo = manifest.GetByLanguage("en");
                        ClosedCaptionTrack track = await Youtube.Videos.ClosedCaptions.GetAsync(trackInfo, cancellationToken);     
                        
                        TranscriptSearchMatch[] foundMatches = track.Captions
                            .Where(caption => caption.Text.Contains(searchParams.SearchTerms, StringComparison.InvariantCultureIgnoreCase))
                            .Select(x => new TranscriptSearchMatch(videoUpload.Title, x.Text, $"{x.Offset.Hours}:{x.Offset.Minutes}:{x.Offset.Seconds}", $"{clipTimestampUrl}&t={(int)x.Offset.TotalSeconds}s"))
                            .ToArray();

                        if (foundMatches.Length > 0)
                        {
                            for (int i = 0; i < foundMatches.Length; i++)
                            {
                                TranscriptSearchMatch foundMatch = foundMatches[i];
                                Matches.Add(foundMatch);
                            }
                        }

                        currentProgress.Matches = foundMatches;
                    }

                    catch (TaskCanceledException)
                    {
                        return;
                    }

                    catch (Exception ex)
                    {
                        currentProgress.Exception = ex;
                    }

                    Progress.Report(currentProgress);
                }
            }

            catch (TaskCanceledException)
            {
                return;
            }
        }
    }
}