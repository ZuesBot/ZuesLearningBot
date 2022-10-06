// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.5.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Antlr4.Runtime.Misc;
using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using QnABot;
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
        public int score;
        private static readonly MemoryStorage _myStorage = new MemoryStorage();

        public QnABot(IConfiguration configuration, ConversationState conversationState, UserState userState, T dialog)
        {

            var welcomeMsg = configuration["DefaultWelcomeMessage"];
            if (!string.IsNullOrWhiteSpace(welcomeMsg))
                defaultWelcome = welcomeMsg;
            ConversationState = conversationState;
            UserState = userState;
            Dialog = dialog;

            var luisIsConfigured = !string.IsNullOrEmpty(configuration["LuisAppId"]) && !string.IsNullOrEmpty(configuration["LuisAPIKey"]) && !string.IsNullOrEmpty(configuration["LuisAPIHostName"]);
            if (luisIsConfigured)
            {
                var luisApplication = new LuisApplication(
                    configuration["LuisAppId"],
                    configuration["LuisAPIKey"],
                    "https://zeusluisservice.cognitiveservices.azure.com/");
                // Set the recognizer options depending on which endpoint version you want to use.
                // More details can be found in https://docs.microsoft.com/en-gb/azure/cognitive-services/luis/luis-migration-api-v3
                var recognizerOptions = new LuisRecognizerOptionsV3(luisApplication)
                {
                    PredictionOptions = new Bot.Builder.AI.LuisV3.LuisPredictionOptions
                    {
                        IncludeInstanceData = true,
                    }
                };

                var _recognizer = new LuisRecognizer(recognizerOptions);
            }


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
            var response = Convert.ToString(turnContext.Activity.Text);

            if (response == "Quiz" || response == "1-PaaS" || response == "1-IaaS" || response == "1-SaaS" 
                || response == "2-PaaS" || response == "2-IaaS" || response == "2-SaaS"
                || response == "3-PaaS" || response == "3-IaaS" || response == "3-SaaS")
            {
                //SaaS
                if (response == "Quiz")
                {

                    var reply = MessageFactory.Text("**Question:** Office 365 is an example of?");

                    reply.SuggestedActions = new SuggestedActions()
                    {
                        Actions = new List<CardAction>()
                        {
                            new CardAction() { Title = "PaaS", Type = ActionTypes.ImBack, DisplayText = "PaaS", Value = "1-PaaS"},
                            new CardAction() { Title = "IaaS", Type = ActionTypes.ImBack, DisplayText = "IaaS", Value = "1-IaaS"},
                            new CardAction() { Title = "SaaS", Type = ActionTypes.ImBack, DisplayText = "SaaS", Value = "1-SaaS"},

                        },
                    };
                    await turnContext.SendActivityAsync(reply, cancellationToken);
                    
                    return;
                }

                if (response == "1-PaaS" || response == "1-IaaS" || response == "1-SaaS")
                {

                    var quizResponse = turnContext.Activity.Text.ToString();
                    if (quizResponse == "1-SaaS")
                    {
                        var up = new UserProfile();

                        var changes = new Dictionary<string, object>();
                        {
                            up.Score++;
                            changes.Add("UserProfile", up);
                        }
                        await _myStorage.WriteAsync(changes, cancellationToken);
                    }
                    else
                    {
                        var up = new UserProfile();

                        var changes = new Dictionary<string, object>();
                        {
                            up.Score = 0;
                            changes.Add("UserProfile", up);
                        }
                        await _myStorage.WriteAsync(changes, cancellationToken);
                    }
                    //IaaS
                    var reply = MessageFactory.Text("**Question:** Azure Virtual Machine is an example of?");

                    reply.SuggestedActions = new SuggestedActions()
                    {
                        Actions = new List<CardAction>()
                        {
                            new CardAction() { Title = "PaaS", Type = ActionTypes.ImBack, Value = "2-PaaS"},
                            new CardAction() { Title = "IaaS", Type = ActionTypes.ImBack, Value = "2-IaaS"},
                            new CardAction() { Title = "SaaS", Type = ActionTypes.ImBack, Value = "2-SaaS"},

                        },
                    };
                    await turnContext.SendActivityAsync(reply, cancellationToken);
                    
                    return;
                }

                if (response == "2-PaaS" || response == "2-IaaS" || response == "2-SaaS")
                {
                    var quizResponse = turnContext.Activity.Text.ToString();
                    if (quizResponse == "2-IaaS")
                    {
                        string[] profileList = { "UserProfile" };
                        var userProfile = _myStorage.ReadAsync<UserProfile>(profileList).Result?.FirstOrDefault().Value;

                        var changes = new Dictionary<string, object>();
                        {
                            var up = userProfile;
                            up.Score = userProfile.Score + 1;
                            changes.Add("UserProfile", up);
                        }
                        await _myStorage.WriteAsync(changes, cancellationToken);
                    }

                    var reply = MessageFactory.Text("**Question:** Azure App Service is an example of?");

                    reply.SuggestedActions = new SuggestedActions()
                    {
                        Actions = new List<CardAction>()
                            {
                                new CardAction() { Title = "PaaS", Type = ActionTypes.ImBack, Value = "3-PaaS"},
                                new CardAction() { Title = "IaaS", Type = ActionTypes.ImBack, Value = "3-IaaS"},
                                new CardAction() { Title = "SaaS", Type = ActionTypes.ImBack, Value = "3-SaaS"},

                            },
                    };
                    await turnContext.SendActivityAsync(reply, cancellationToken);
                    
                    return;
                }

                if (response == "3-PaaS" || response == "3-IaaS" || response == "3-SaaS")
                {
                    var quizResponse = turnContext.Activity.Text.ToString();
                    if (quizResponse == "3-PaaS")
                    {
                        string[] profileList1 = { "UserProfile" };
                        var userProfile = _myStorage.ReadAsync<UserProfile>(profileList1).Result?.FirstOrDefault().Value;

                        var changes = new Dictionary<string, object>();
                        {
                            var up = userProfile;
                            up.Score = userProfile.Score + 1;
                            changes.Add("UserProfile", up);
                        }
                        await _myStorage.WriteAsync(changes, cancellationToken);
                    }

                    string[] profileList = { "UserProfile" };
                    var myup = _myStorage.ReadAsync<UserProfile>(profileList).Result?.FirstOrDefault().Value;

                    var level = "**Fundamentals**";
                    if (myup.Score == 2)
                        level = "**Associate**";
                    else if(myup.Score == 3)
                        level = "**Expert**";

                    var levelStr = String.Format("Based on your score we reccomend exploring: {0}", level);

                    var reply = MessageFactory.Text(levelStr);

                    reply.SuggestedActions = new SuggestedActions()
                    {
                        Actions = new List<CardAction>()
                            {
                                new CardAction() { Title = "Fundamentals", Type = ActionTypes.ImBack, Value = "AZ-900"},
                                new CardAction() { Title = "Associate", Type = ActionTypes.ImBack, Value = "AZ-104"},
                                new CardAction() { Title = "Expert", Type = ActionTypes.ImBack, Value = "AZ-305"},
                                new CardAction() { Title = "Main Menu", Type = ActionTypes.ImBack, Value = "Main Menu"},

                            },
                    };
                    await turnContext.SendActivityAsync(reply, cancellationToken);
                    return;
                }
            }

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
                            new CardAction() { Title = "Which Level Am I?", Type = ActionTypes.ImBack, Value = "Quiz"}
                        },
                };

                await turnContext.SendActivityAsync(reply, cancellationToken);
            }
            else if (response == "Roadmap")
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
            else
            {


                // Run the Dialog with the new message Activity.
                await Dialog.RunAsync(turnContext, ConversationState.CreateProperty<DialogState>(nameof(DialogState)), cancellationToken);

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
                            new CardAction() { Title = "Which Level Am I?", Type = ActionTypes.ImBack, Value = "Quiz"},
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
