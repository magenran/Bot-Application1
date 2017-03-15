﻿using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;

using Model.dataBase;
using Microsoft.Bot.Connector;
using System.Threading;
using NLP;
using Bot_Application1.Controllers;
using NLP.Models;
using Newtonsoft.Json.Linq;
using Model;
using Model.Models;
using Microsoft.Bot.Builder.ConnectorEx;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Bot_Application1.IDialog;
using Bot_Application1.Models;

namespace Bot_Application1.IDialog
{

    [Serializable]
    public abstract class AbsDialog<T> : IDialog<T>
    {

        private IUser user;
        private StudySession studySession;
        private DateTime request = DateTime.UtcNow;
        private UserContext userContext = new UserContext("abs");
        //IDialogContext context;

        internal void updateRequestTime()
        {
            request = DateTime.UtcNow;
        }

        internal void updateRequestTime(IDialogContext context)
        {
            request = context.Activity.Timestamp.Value;
        }


        public void getUser(IDialogContext context)
            {
                User thisUser = User as User;
                context.UserData.TryGetValue<User>("user", out thisUser);
                user = thisUser;
            }

        public void setUser(IDialogContext context)
        {
                User thisUser = user as User;
                context.UserData.SetValue<User>("user",thisUser);
            
        }

        public void getStudySession(IDialogContext context)
        {
            context.UserData.TryGetValue<StudySession>("studySession", out studySession);
        }

        public EducationController edc()
        {
            return new EducationController(User, StudySession, null);
        }


        public void setStudySession(IDialogContext context)
        {
            context.UserData.SetValue<StudySession>("studySession", studySession);
        }



        internal DateTime Request
        {
            get
            {
                return request;
            }
        }

        public IUser User
        {
            get
            {
                return user;
            }

            set
            {
                user = value;
            }
        }

        public StudySession StudySession
        {
            get
            {
                return studySession;
            }

            set
            {
                studySession = value;
            }
        }

        public UserContext UserContext
        {
            get
            {
                return userContext;
            }

            set
            {
                userContext = value;
            }
        }

        internal async Task writeMessageToUser(IDialogContext context, string[] newMessage)
        {
            var typingReplay = context.MakeMessage();
            typingReplay.Type = ActivityTypes.Typing;
            var msgList = newMessage.ToList();
            msgList.ForEach(x => x.Trim());
            foreach (var m in msgList)
            {
                var m2 = m.Trim();
                if (m2.Contains('|'))
                {


                    await context.PostAsync(typingReplay);
                    //| is a sign for new line
                  //  await writeMessageToUser(context,m.Split('|').Where(x=> x.Trim().Length > 0 ).Select(x=> "\U00002705" + x).ToArray());
                    await writeMessageToUser(context, m2.Split('|'));
                }
                else
                {
                    if (m2.Length > 400)
                    {
                        await context.PostAsync(typingReplay);

                        var idx = m2.IndexOf(' ', 385);
                        if (idx > 0)
                        {
                            var str1 = m2.Substring(0, idx);
                            var str2 = m2.Substring(idx);
                            await writeMessageToUser(context, new string[] { str1, str2 });
                        } else
                        {

                        }
                    } else
                    {
                        //facebook cuts messages from 300 chars


                        //send message
                        if (m2 != null && m2 != "")
                        {
                            typingTime(context);
                            Thread.Sleep(m2.Length * 30); //writing time
                            await context.PostAsync(m2);
                        }
                    }
                }

            }
        }


        public static async Task postImageToUser(IDialogContext context, string mediaKey)
        {
            MediaController mc = new MediaController();
            var url = mc.getFileUrl(mediaKey);
            var cardImg = new CardImage(url: url);
            var img = new Attachment();
            img.ContentType = "image/png";
            img.ContentUrl = url;
            img.Name = mediaKey;
            var message = context.MakeMessage();
            message.Attachments.Add(img);
            await context.PostAsync(message);
        }


        public virtual async Task createMenuOptions(IDialogContext context, string title, string[] options, ResumeAfter<object> resume)
        {
            if(context.Activity.ChannelId == "facebook")
            {
                await createQuickReplay(context, title, options, resume);
            }
            else
            {
                await createRMenuOptions(context, title, options, resume);
            }
        }

        public async virtual Task createQuickReplay(IDialogContext context,string title, string[] options, ResumeAfter<IMessageActivity> resume)
        {

            //     await writeMessageToUser(context, new string[] { title });
           
            var reply = context.MakeMessage();
            var channelData = new JObject();
            var quickReplies = new JArray();

            
            var qrList = new List<FacebookQuickReply>();
            foreach (var s in options)
            {
                var r = new FacebookQuickReply("text", s, s);
                qrList.Add(r);
            }

          
            var message = new FacebookMessage(title, qrList);
            reply.ChannelData = message;



            await context.PostAsync(reply);

            updateRequestTime(context);
            context.Wait(resume);

        }

   
        public async virtual Task createRMenuOptions(IDialogContext context, string title, string[] options, ResumeAfter<string> resume)
        {

        //    await writeMessageToUser(context, new string[] { title });

         

            var menu = new PromptDialog.PromptChoice<string>(
              options,
             title,
             conv().getPhrase(Pkey.wrongOption)[0],
             3);

            context.Call(menu, resume);

        }

        internal void typingTime(IDialogContext context)
        {
            var message = context.MakeMessage();
            message.Type = ActivityTypes.Typing;
            context.PostAsync(message);
        }


        public ConversationController conv()
        {

            return new ConversationController(User, StudySession);
        }


        public async Task<bool> checkOutdatedMessage<A,R>(IDialogContext context,ResumeAfter<R> resume, IAwaitable<A> message)
        {
            var mes = await message;
            if (context.Activity.Timestamp <= Request)
            {
             //   mes.Summary = getDialogContext();
                context.Wait(resume);
                //await context.Forward<IMessageActivity, IMessageActivity>(new SideDialog(), resume, mes, CancellationToken.None);
                return true;
            }
            return false;
        }

        public abstract UserContext getDialogContext();

        public virtual Task StartAsync(IDialogContext context)
        {
            throw new NotImplementedException();
        }
        public async Task waitForUserInputToContinu(IDialogContext context, ResumeAfter<IMessageActivity> resume)
        {
            context.Wait(resume);
        }


        internal  AwaitableFromItem<IMessageActivity> stringToMessageActivity(IDialogContext context,string message)
        {
            var messageActivity = context.MakeMessage();
            messageActivity.Text = message;
            var awaitble = new AwaitableFromItem<IMessageActivity>(messageActivity);
            return awaitble;
        }
    }




}