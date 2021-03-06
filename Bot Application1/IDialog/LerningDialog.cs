﻿using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using NLP.Models;
using Model.dataBase;
using Bot_Application1.Controllers;
using Model;
using Model.Models;
using Bot_Application1.Exceptions;
using Bot_Application1.Models;
using System.Threading;

namespace Bot_Application1.IDialog
{
    [Serializable]
    public class LerningDialog : AbsDialog<string>
    {
        public override UserContext getDialogContext()
        {
            UserContext.dialog = "LerningDialog";
            return UserContext;
        }


        public override async Task StartAsync(IDialogContext context)
        {
            getDialogsVars(context);
            if(StudySession == null || StudySession.Category == null)
            {
                await chooseSubject(context);
                return;
            }
            else
            {
                var question = conv().getPhrase(Pkey.shouldWeContinue);
              
                await context.Forward<Boolean,string[]>(new YesNoQuestionDialog(), shouldWeContinue, question, new System.Threading.CancellationToken());
                return;
            }


        }

        private async Task chooseSubject(IDialogContext context)
        {

            StudySession.CurrentQuestion = null;
            await writeMessageToUser(context, conv().getPhrase(Pkey.letsLearn));

            IMessageActivity message;
            //choose study subject menu gallery
            if (context.Activity.ChannelId != "telegram")
            {
                await writeMessageToUser(context, conv().getPhrase(Pkey.chooseStudyUnits));
                message = context.MakeMessage();
                foreach (var m in edc().getStudyCategory())
                {
                    var action = new CardAction(type: "imBack", value: m, title: m);
        var hc = new HeroCard(title: m, images: getImage(m), tap: action, buttons:
            new CardAction[] { action });
        message.Attachments.Add(hc.ToAttachment());
                    message.AttachmentLayout = "carousel";

                }
            }else{
                await createMenuOptions(context, conv().getPhrase(Pkey.chooseStudyUnits)[0], edc().getStudyCategory(), StartLearning);
                return;
            }


            context.UserData.RemoveValue("studySession");
            StudySession = new StudySession();
            User.UserLastSession = context.Activity.Timestamp;
            setDialogsVars(context);
            await context.PostAsync(message);
            updateRequestTime(context);
            context.Wait(StartLearning);
            return;
        }


        private async Task shouldWeContinue(IDialogContext context, IAwaitable<Boolean> result)
        {
            var cont = await result;
            if (cont)
            {
                await intreduceQuestion(context);
            }else
            {
                await writeMessageToUser(context, conv().getPhrase(Pkey.letsStartOver));
                StudySession = new StudySession();
                setDialogsVars(context);
                await chooseSubject(context);
            }
        }

        private CardImage[] getImage(string m)
        {
            MediaController mc = new MediaController();
            var key = edc().getRamdomImg(m);
            var urlAdd = mc.getFileUrl(key);
            var cardImg = new CardImage(url: urlAdd);
            return new CardImage[] { cardImg };
        }





        public async virtual Task StartLearning(IDialogContext context, IAwaitable<object> result)
        {

            await StartLearning(context, stringToMessageActivity(context,await result as string));
        }



        public async virtual Task StartLearning(IDialogContext context, IAwaitable<IMessageActivity> result)
        {

            await checkOutdatedMessage<IMessageActivity>(context, StartLearning, result);

            string message = null;
            var res = await result;
            //if (res is string)
            //{
            //    message = res as string;
            //}
            //else
            //{
                var r =  res as IMessageActivity;
                message = r.Text;
            //     }

            var option = conv().FindMatchFromOptions(edc().getStudyCategory(), message);
           if (option != null )
            {
                StudySession.Category = option;
                setDialogsVars(context);

                try
                {
                    edc().getQuestion();
                    setDialogsVars(context);
                }
                catch(CategoryOutOfQuestionException ex)
                {
                    await CategoryOutOfQuestion(context);
                }
              
      
                await writeMessageToUser(context, conv().getPhrase(Pkey.areUReaddyToLearn));
                await writeMessageToUser(context, conv().getPhrase(Pkey.firstQuestion));
                await intreduceQuestion(context);
                return;
            }else
            {
                await writeMessageToUser(context, conv().getPhrase(Pkey.NotAnOption, textVar: message));
                await chooseSubject(context);
            }
        }

        private async Task CategoryOutOfQuestion(IDialogContext context)
        {
            if(StudySession.QuestionAsked.Count > 0)
            {

                await context.Forward(new YesNoQuestionDialog(), CategoryOutOfQuestionRes, conv().getPhrase(Pkey.restartSession), new CancellationToken());

            }
            else
            {
                await writeMessageToUser(context, conv().getPhrase(Pkey.SubjectNotAvialable));
                await StartAsync(context);
                return;
            }
        }

        private async Task CategoryOutOfQuestionRes(IDialogContext context, IAwaitable<bool> result)
        {
            var res = await result;
            await writeMessageToUser(context, conv().getPhrase(Pkey.ok));
            if (res)
            {
                StudySession.QuestionAsked = new System.Collections.Generic.List<IQuestion>();
                setDialogsVars(context);
                await chooseSubject(context);
            }
            else
            {
                await StartAsync(context);

            }

            return;

        }

        private async Task intreduceQuestion(IDialogContext context)
        {
            try
            {
                getDialogsVars(context);
                edc().getNextQuestion();
                setDialogsVars(context);
            }
            catch (CategoryOutOfQuestionException ex)
            {
                await CategoryOutOfQuestion(context);
                return;
            }

            if (StudySession.CurrentQuestion != null)
            {

                try
                {
                    context.Call(new QuestionDialog(), questionSummery);
                    return;
                }
               catch (StopSessionException ex)
                {
                    context.Done("EndSession");
                    return;
                }
                catch (menuException Exception)
                {
                    context.Done("menu");
                    return;

                }
                catch (Exception ex)
                {
                    await writeMessageToUser(context, conv().getPhrase(Pkey.innerException));
                    await generalExceptionError(context, ex);
                    context.Done("menu");
                    return;
                }
            }
            else
            {
                await EndOfLearningSession(context);
                return;
            }
        }

        private async Task questionSummery(IDialogContext context, IAwaitable<string> result)
        {
            try {
                var res = await result;
                getDialogsVars(context);
            }
            catch (EndOfLearningSessionException ex)
            {
                context.Done("menu");
                return;
            }
            catch (menuException ex)
            {
                await chooseSubject(context);
                return;
            }
            catch (StopSessionException ex) 
            {
                context.Done("learningSession");
                return;
            }
            catch (sessionBreakException ex)
            {
                await suggestBreak(context);
                return;
            }
           
            catch (Exception ex)
            {
                await generalExceptionError(context, ex);
                await intreduceQuestion(context);
                return;
            }

            if (StudySession.QuestionAsked.Count < StudySession.SessionLength - 1)
            {
                await writeMessageToUser(context, conv().getPhrase(Pkey.moveToNextQuestion));
                await writeMessageToUser(context, conv().getPhrase(Pkey.beforAskQuestion));
                await intreduceQuestion(context);
            }
            else
            {
                await suggestBreak(context);
            }
        }

        

        public async Task suggestBreak(IDialogContext context)
        {
            await context.Forward(new YesNoQuestionDialog(), takeAbreak, conv().getPhrase(Pkey.suggestBreak),new CancellationToken());
        }

        public async Task takeAbreak(IDialogContext context, IAwaitable<bool> result)
        {
            getDialogsVars(context);
            StudySession.SessionLength = StudySession.QuestionAsked.Count + 3;
            setDialogsVars(context);
            var cont = await result;
            if (cont)
            {
                await writeMessageToUser(context, conv().getPhrase(Pkey.takeAbreak));
                await writeMessageToUser(context, conv().getPhrase(Pkey.uselessLink));
                var msg = context.MakeMessage();
                var media = conv().getMediaMessage("useless");
                await writeMessageToUser(context, media.value.Split('|'));
                await writeMessageToUser(context, conv().getPhrase(Pkey.imWaiting));
                updateRequestTime(context);
                context.Wait(continuAfterBreak);
                return;
            }
            else
            {
                await writeMessageToUser(context, conv().getPhrase(Pkey.ok));
                await writeMessageToUser(context, conv().getPhrase(Pkey.letsContinueWitoutBreak));
                await intreduceQuestion(context);
                return;
            }
    

        }

        public async Task continuAfterBreak(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var mes = await result;
            if (mes.Timestamp > Request.AddSeconds(10))
            {
                await writeMessageToUser(context, conv().getPhrase(Pkey.letsContinue));
                await intreduceQuestion(context);
            }
            else{
                context.Wait(continuAfterBreak);
                return;
            }
        }


        public async Task EndOfLearningSession(IDialogContext context)
        {
            await writeMessageToUser(context, conv().endOfSession());
            await writeMessageToUser(context, conv().getPhrase(Pkey.endOfSession));

            getDialogsVars(context);
        //    edc().saveUserSession();

            context.Done("menu");
        }


    }
}