namespace localGptLib
{
    public class localGptLibModelSettings
    {
        // https://platform.openai.com/docs/api-reference/chat/create

        // https://platform.openai.com/docs/models#gpt-4o

        public string ChatModel { get; set; } = "gpt-4o";

        // https://platform.openai.com/docs/api-reference/images/create

        // https://platform.openai.com/docs/models#dall-e

        /// <summary>
        /// "dall-e-3" or "dall-e-2".
        /// </summary>
        public string ImagesModel { get; set; } = "dall-e-3";

        /// <summary>
        /// "hd" or "standard".
        /// </summary>
        public string ImagesQuality { get; set; } = "hd";

        /// <summary>
        /// "1024x1024" or "1792x1024" or "1024x1792" (supposing nobody would use DALL-E 2).
        /// </summary>
        public string ImagesSize { get; set; } = "1024x1024";

        /// <summary>
        /// "vivid" or "natural".
        /// </summary>
        public string ImagesStyle { get; set; } = "vivid";
    }
}
