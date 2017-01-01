﻿using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using Bot_Application1.Cardatt_achment;
using Bot_Application1.dataBase;
using System.Threading;
using NLPtest;

namespace Bot_Application1.IDialog
{
    [Serializable]
    public class MenuOptionDialog<T> : PromptDialog.PromptChoice<T>
    {
        private T[] options;
        private IDialog<object>[] dialogOptions;
        private ResumeAfter<object> contFunction;
        

        public MenuOptionDialog(T[] options, string prompt, string retry, int attempts, IDialog<object>[] dialogOptions, ResumeAfter<object> contFunction)
            : base (options, prompt, retry,attempts)
        {
            this.dialogOptions = dialogOptions;
            this.contFunction = contFunction;
            this.options = options;
        }

        protected async override Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> message)
        {

        //    var message = await result;

         T result;
          if (this.TryParse(await message, out result))
              {
                var i = 0;
                string resultMA = result as string;
                foreach (var o in options)
                {
                    if (o.Equals(resultMA))
                    {
                        context.Call(dialogOptions[i], contFunction);
                        return;
                    }
                    i++;
                }

                //defualt
                context.Call(dialogOptions[dialogOptions.Length - 1], contFunction);

              }
             else
              {
              --promptOptions.Attempts;
              if (promptOptions.Attempts > 0)
                {
                    await context.PostAsync(this.MakePrompt(context, promptOptions.Retry ?? promptOptions.DefaultRetry, promptOptions.Options));
                    context.Wait(MessageReceivedAsync);
                }
                else
                 {
                  //too many attempts, throw.
                    await context.PostAsync(this.MakePrompt(context, promptOptions.TooManyAttempts));
                                      throw new TooManyAttemptsException(promptOptions.TooManyAttempts);
                 }
             }



        }




      protected override bool TryParse(IMessageActivity message, out T result)
        {
            ConversationController conv = new ConversationController();
            result = conv.FindMatchFromOptions<T>(message.Text, promptOptions.Options);
            return result != null;
        }

    }

}