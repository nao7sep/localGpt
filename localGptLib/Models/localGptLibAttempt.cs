namespace localGptLib
{
    public class localGptLibAttempt
    {
        public Guid Id { get; init; }

        public localGptLibUserRequest? UserRequest { get; set; }

        public localGptLibAssistantReply? AssistantReply { get; set; }
    }
}
