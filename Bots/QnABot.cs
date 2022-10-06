// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.5.0

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Antlr4.Runtime.Misc;
using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using QnABot.Factories;
using QnABot.Factories.bot.Factories;
//using QnABot.Helper;

namespace Microsoft.BotBuilderSamples.Bots
{
    public class QnABot<T> : ActivityHandler where T : Microsoft.Bot.Builder.Dialogs.Dialog
    {
        protected readonly BotState ConversationState;
        protected readonly Microsoft.Bot.Builder.Dialogs.Dialog Dialog;
        protected readonly BotState UserState;
        protected string defaultWelcome = "Hello! Welcome to Azure certification agent.  Im happy to assist you with your Azure journey.";
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

            var response = Convert.ToString(turnContext.Activity.Text);
            if (response == "No")
            {
                await turnContext.SendActivityAsync(MessageFactory.Text("Thanks for the chat.  Have a great day and i hope we speak again!"), cancellationToken);
            }
            else if (response == "Main Menu")
            {
                var reply = MessageFactory.Text("Lets begin by choosing to view courses by skill level, viewing the Azure certification course path or if you already know your question you can type it in at any time:");

                reply.SuggestedActions = new SuggestedActions()
                {
                    Actions = new List<CardAction>()
                        {
                            new CardAction() { Title = "Fundamentals", Type = ActionTypes.ImBack, Value = "AZ-900"},
                            new CardAction() { Title = "Associate", Type = ActionTypes.ImBack, Value = "AZ-104"},
                            new CardAction() { Title = "Expert", Type = ActionTypes.ImBack, Value = "AZ-305"},
                            new CardAction() { Title = "Azure Roadmap", Type = ActionTypes.ImBack, Value = "Roadmap"},
                        },
                };

                await turnContext.SendActivityAsync(reply, cancellationToken);
            }
            else
            {
                if (response == "Roadmap")
                {

                    var roadMap_heroCard = new HeroCard
                    {
                        Title = "Azure Certification Roadmap",
                        Subtitle = "Course roadmap",
                        Text = "This roadmap will give you guidance on a certification path that best suits your needs",
                        Images = new List<CardImage> { new CardImage("https://learn.microsoft.com/en-us/certifications/posts/images/azure-apps-and-infrastructure.jpg") },
                        //Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Get Started", value: "https://docs.microsoft.com/bot-framework") },
                    };

                    await turnContext.SendActivityAsync(MessageFactory.Attachment(roadMap_heroCard.ToAttachment()), cancellationToken);
                }


                var moreHelp = MessageFactory.Text("Is there anything else i can help you with today?");

                moreHelp.SuggestedActions = new SuggestedActions()
                {
                    Actions = new List<CardAction>()
                        {
                            //new CardAction() { Title = "Yes", Type = ActionTypes.ImBack, Value = "Yes"},
                            new CardAction() { Title = "No", Type = ActionTypes.ImBack, Value = "No"},
                            new CardAction() { Title = "Main Menu", Type = ActionTypes.ImBack, Value = "Main Menu"},
                        },
                };

                await turnContext.SendActivityAsync(moreHelp, cancellationToken);
            }
        }

        //protected override async Task OnEventActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        //{


        //}



        // This is your entry point to the bot when a end user is added to the chat
        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(defaultWelcome), cancellationToken);
                    //await turnContext.SendActivityAsync(MessageFactory.Text("How can I help you?"), cancellationToken);

                    //Attachment optionsCard;
                    //_adaptiveCardFactory = new AdaptiveCardFactory();
                    //optionsCard = _adaptiveCardFactory.CreateOptionsCard();
                    //await turnContext.SendActivityAsync(MessageFactory.Attachment(optionsCard), cancellationToken);

                    //await turnContext.SendActivityAsync(MessageFactory.Attachment(Cards.GetHeroCard().ToAttachment()), cancellationToken);

                    var reply = MessageFactory.Text("Lets begin by choosing to view courses by skill level, viewing the Azure certification course path or if you already know your question you can type it in at any time:");

                    

                    reply.SuggestedActions = new SuggestedActions()
                    {
                        Actions = new List<CardAction>()
                        {
                            new CardAction() { Title = "Fundamentals", Type = ActionTypes.ImBack, Value = "AZ-900"},
                            new CardAction() { Title = "Associate", Type = ActionTypes.ImBack, Value = "AZ-104"},
                            new CardAction() { Title = "Expert", Type = ActionTypes.ImBack, Value = "AZ-305"},
                            new CardAction() { Title = "Azure Roadmap", Type = ActionTypes.ImBack, Value = "Roadmap"},
                        },
                    };

                    await turnContext.SendActivityAsync(reply, cancellationToken);

                    //reply = MessageFactory.Text("Is there anything else i can help you with today?");

                    //reply.SuggestedActions = new SuggestedActions()
                    //{
                    //    Actions = new List<CardAction>()
                    //    {
                    //        new CardAction() { Title = "Yes", Type = ActionTypes.ImBack, Value = "Yes"},
                    //        new CardAction() { Title = "No", Type = ActionTypes.ImBack, Value = "No"},
                    //    },
                    //};

                    //await turnContext.SendActivityAsync(reply, cancellationToken);


                    Console.WriteLine("");



                }

            }
        }
    }
}
