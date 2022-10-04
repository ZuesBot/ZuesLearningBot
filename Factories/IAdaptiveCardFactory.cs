using Microsoft.Bot.Schema;

namespace QnABot.Factories
{
    public interface IAdaptiveCardFactory
    {
        Attachment CreateOptionsCard();
        Attachment CreateTextCard();
        Attachment CreateCombinationCard(string message);
    }
}
