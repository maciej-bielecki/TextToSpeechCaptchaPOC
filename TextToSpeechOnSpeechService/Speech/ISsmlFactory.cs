namespace TextToSpeechOnSpeechService.Speech
{
    public interface ISsmlFactory
    {
        string CreateSsmlForText(string text);
    }
}