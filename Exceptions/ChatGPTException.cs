namespace TheOneWhoWritesYourSpeech.Exceptions;

public class ChatGPTException : Exception
{
    public ChatGPTException(string message) : base(message)
    {
    }
}
