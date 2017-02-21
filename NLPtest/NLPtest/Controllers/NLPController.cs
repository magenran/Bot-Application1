using NLPtest;
using System.Collections.Generic;
using System;
using NLPtest.view;
using NLPtest.WorldObj;

public class NLPControler // : INLPControler
{
	
	static NLPControler instance;
    static object syncLock = new object();
    MorfAnalizer ma = null;
    SemanticAnalizer sa = new SemanticAnalizer();

    private NLPControler()
    {
        this.ma = new MorfAnalizer();
    }

    public static NLPControler getInstence()
    {
        lock (syncLock)
        {
            if (instance != null)
            {
                return instance;
            }else
            {
                   var nlp =  new NLPControler();
           //     var nlp = new NLPControlerTestStub();
             //   nlp.Initialize();
                return nlp;
            }
        }
     
    }

    public ContentList testAnalizer(string inputText,out string log)
    {
        log = "";
        //    var a = MorfAnalizer.createSentence(inputText);
        // var context = new TextContext();
        var sen = ma.meniAnalize(inputText);

        //  sa.findGufContext(sen);
        log += "intent:" + getUserIntent(inputText) + Environment.NewLine;

          ContentList input = new ContentList();
        List<WorldObject> sentence = new List<WorldObject>();
        List<WorldObject> last = new List<WorldObject>();
        string logTemp;

        List<ITemplate> context = new List<ITemplate>();
        var sentences = sa.findGufContext(sen, context);

        foreach (var s in sentences)
        {
            sentence = sa.findTemplate(s.ToArray(),out logTemp);
            log += logTemp;
            last = sentence;
            input.Add(sentence);
        }
        
        return input;

    }

    public UserIntent getUserIntent(string str)
    {
        return sa.getUserIntent(str);
    }


    public  string getClass(string text){
		return ma.getClass(text);
	}

	

        public string getName(string inputText){
		return ma.getName(inputText);

	}

	 public string GetGender(string text){
		return ma.GetGender(text);
	}
	
        public string GetGeneralFeeling(string text){
		return ma.GetGeneralFeeling(text);
	}


    public List<WorldObject> Analize(string text)
    {
        return Analize(text, null);
    }

    public List<WorldObject> Analize(string text, string systemAnswerText)
    {
        // var context = new TextContext();
        var textAnlz = ma.meniAnalize(text);
        List<WorldObject> input = new List<WorldObject>();
        List<WorldObject> sentence = new List<WorldObject>();
        List<WorldObject> last = new List<WorldObject>();
        List < List < ITemplate >> sentences;
        List<ITemplate> context = new List<ITemplate>();


        if (systemAnswerText != null)
        {
            var contextAnlz = ma.meniAnalize(systemAnswerText);
            //create context 
            var contextSentences = sa.findGufContext(contextAnlz, context);
            contextSentences.ForEach(x => context.AddRange(x));
             sentences = sa.findGufContext(textAnlz, context);
        }else
        {
             sentences = sa.findGufContext(textAnlz, context);
        }
       
        string logTemp;

        foreach (var s in sentences)
        {
            sentence = sa.findTemplate(s.ToArray(), out logTemp);
            last = sentence;
            input.AddRange(sentence);
        }

        return input;
    }
}
        
	
