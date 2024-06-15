using System.Text;

namespace YtTranscriptSearch.Shared
{
    public static class Extensions
    {
        public static void Save(this IList<TranscriptSearchMatch> matches, string fileName)
        {
            StringBuilder fileContent = new($"video,timestamp,text,url{Environment.NewLine}");

            for (int i = 0; i < matches.Count; i++)
            {
                TranscriptSearchMatch match = matches[i];
                fileContent.AppendLine($"\"{match.Video}\",{match.Timestamp},\"{match.Text}\",{match.Url}");
            }

            File.WriteAllText(fileName, fileContent.ToString());
        }
    }
}
