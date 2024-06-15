using YoutubeExplode;
using YoutubeExplode.Channels;

namespace YtTranscriptSearch.Shared
{
    public sealed class TranscriptSearchParameters
    {
        private TranscriptSearchParameters() { }

        public string? SearchTerms { get; set; }
        public string? ChannelName { get; private set; }
        public Channel? Channel { get; private set; }

        /// <summary>
        /// Create an instance of TranscriptSearchParameters
        /// </summary>
        /// <param name="client">YoutubeClient</param>
        /// <param name="channel">Youtube Channel Name</param>
        /// <param name="searchTerms">Search Terms</param>
        /// <returns>TranscriptSearchParameters</returns>
        public static async Task<TranscriptSearchParameters> CreateAsync(YoutubeClient client, string channel, string searchTerms)
        {
            string? fullUrl;

            if (Uri.TryCreate(channel, new UriCreationOptions(), out Uri? _))
            {
                fullUrl = channel;
            }

            else
            {
                if (channel.StartsWith('@'))
                {
                    fullUrl = $"https://youtube.com/{channel}";
                }

                else
                {
                    fullUrl = $"https://youtube.com/@{channel}";
                }

                if (!Uri.TryCreate(fullUrl, new UriCreationOptions { }, out Uri? _))
                {
                    throw new Exception($"Invalid Uri '{channel}'");
                }
            }

            Channel foundChannel = await client.Channels.GetByHandleAsync(fullUrl);

            return new() { ChannelName = foundChannel.Title, SearchTerms = searchTerms, Channel = foundChannel };
        }
    }
}
