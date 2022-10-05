// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.5.0

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using QnABot.Factories;
using QnABot.Factories.bot.Factories;
using QnABot.Helper;

namespace Microsoft.BotBuilderSamples.Bots
{
    public class QnABot<T> : ActivityHandler where T : Microsoft.Bot.Builder.Dialogs.Dialog
    {
        protected readonly BotState ConversationState;
        protected readonly Microsoft.Bot.Builder.Dialogs.Dialog Dialog;
        protected readonly BotState UserState;
        protected string defaultWelcome = "Hello! Thank you for reaching out to <Name here>. I'm Zeus Bot!";
        AdaptiveCardFactory _adaptiveCardFactory;


        public QnABot(IConfiguration configuration, ConversationState conversationState, UserState userState, T dialog)
        {

            var welcomeMsg = configuration["DefaultWelcomeMessage"];
            if (!string.IsNullOrWhiteSpace(welcomeMsg))
                defaultWelcome = welcomeMsg;
            ConversationState = conversationState;
            UserState = userState;
            Dialog = dialog;

        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            await base.OnTurnAsync(turnContext, cancellationToken);

            // Save any state changes that might have occurred during the turn.
            await ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);
            await UserState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            // Run the Dialog with the new message Activity.
            await Dialog.RunAsync(turnContext, ConversationState.CreateProperty<DialogState>(nameof(DialogState)), cancellationToken);
        }

        // This is your entry point to the bot when a end user is added to the chat
        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(defaultWelcome), cancellationToken);
                    await turnContext.SendActivityAsync(MessageFactory.Text("How can I help you ?"), cancellationToken);

                    //Attachment optionsCard;
                    //_adaptiveCardFactory = new AdaptiveCardFactory();
                    //optionsCard = _adaptiveCardFactory.CreateOptionsCard();
                    //await turnContext.SendActivityAsync(MessageFactory.Attachment(optionsCard), cancellationToken);

                    //await turnContext.SendActivityAsync(MessageFactory.Attachment(Cards.GetHeroCard().ToAttachment()), cancellationToken);

                    //var reply = MessageFactory.Text("Which level Azure certification are you looking for?");

                    //reply.SuggestedActions = new SuggestedActions()
                    //{
                    //    Actions = new List<CardAction>()
                    //    {
                    //        new CardAction() { Title = "Beginner", Type = ActionTypes.ImBack, Value = "https://learn.microsoft.com/en-us/certifications/exams/az-900"},
                    //        new CardAction() { Title = "Intermediate", Type = ActionTypes.ImBack, Value = "AZ-204"},
                    //        new CardAction() { Title = "Expert", Type = ActionTypes.ImBack, Value = "AZ-500"},
                    //    },
                    //};

                    //await turnContext.SendActivityAsync(reply, cancellationToken);
                }
            }
        }
    }
}
