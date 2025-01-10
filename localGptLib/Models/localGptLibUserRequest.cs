namespace localGptLib
{
    public class localGptLibUserRequest
    {
        public localGptLibContentType Type { get; set; }

        public required string Prompt { get; set; }

        public DateTime CreatedAtUtc { get; set; }
    }
}
