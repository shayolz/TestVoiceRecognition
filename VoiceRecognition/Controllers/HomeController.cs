using System;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace VoiceRecognition.Controllers
{
    public class HomeController : Controller
    {
        private readonly SpeechSynthesizer sSynth = new SpeechSynthesizer();
        private readonly PromptBuilder pBuilder = new PromptBuilder();
        private readonly WMPLib.WindowsMediaPlayer player = new WMPLib.WindowsMediaPlayer();
        private readonly SpeechRecognitionEngine sRecognize = new SpeechRecognitionEngine();
        private bool blockMic = false;

        public ActionResult Index()
        {
            return View();
        }

        public async Task CallAsync()
        {
            Choices sList = new Choices();
            sList.Add(new string[] { "veloce", "lento", "canzoneuno", "canzone uno", "stop", "normale", "avanti", "indietro", "pausa", "canzonedue", "canzone due", "chi è la più bella", "la più bella" });

            Grammar gr = new Grammar(new GrammarBuilder(sList));

            try
            {
                sRecognize.SetInputToDefaultAudioDevice();
                sRecognize.RequestRecognizerUpdate();
                sRecognize.LoadGrammar(gr);
                sRecognize.SpeechRecognized += Recognize_SpeechRecognized;
                sRecognize.SetInputToDefaultAudioDevice();
                sRecognize.RecognizeAsync(RecognizeMode.Multiple);
            }
            catch (Exception ex)
            {
                ViewBag.Message = "not recognized";
            }

            ViewBag.Message = "Fine ascolto";
        }

        private void Recognize_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (blockMic)
                return;
            if (e.Result.Text == "la più bella" || e.Result.Text == "chi è la più bella")
            {
                VoiceMessage("Ovviamente la più bella del mondo è JOYA");
            }
            if (e.Result.Text == "lento")
            {
                VoiceMessage("piu' lento");

                var pactualRate = player.settings.rate;
                player.settings.rate = pactualRate - 0.1;
            }
            else if (e.Result.Text == "veloce")
            {
                VoiceMessage("piu' veloce");

                var pactualRate = player.settings.rate;
                player.settings.rate = pactualRate + 0.1;
            }
            else if (e.Result.Text == "normale")
            {
                player.settings.rate = 1.0;
            }
            else if (e.Result.Text == "avanti")
            {
                VoiceMessage("avanti 10 secondi");

                player.controls.currentPosition += 10;
            }
            else if (e.Result.Text == "indietro")
            {
                VoiceMessage("indietro 10 secondi");

                player.controls.currentPosition -= 10;
            }
            else if (e.Result.Text == "canzoneuno" || e.Result.Text == "canzone uno")
            {
                VoiceMessage("avvio Canzone uno");

                player.controls.stop();
                var basePath = AppDomain.CurrentDomain.BaseDirectory;
                player.URL = basePath + "Content\\musicTest.mp3";
                player.controls.play();
            }
            else if (e.Result.Text == "canzonedue" || e.Result.Text == "canzone due")
            {
                player.controls.stop();
                var basePath = AppDomain.CurrentDomain.BaseDirectory;
                player.URL = basePath + "Content\\musicTest2.mp3";
                player.controls.play();
            }
            else if (e.Result.Text == "stop")
            {
                VoiceMessage("canzone fermata");

                player.controls.stop();
            }
            else if (e.Result.Text == "pausa")
            {
                VoiceMessage("pausa canzone");

                player.controls.pause();
            }

            blockMic = false;
        }

        public int VoiceMessage(string message)
        {
            try
            {
                AddMessage(message);

                return 1;
            }
            catch (Exception e)
            {
                blockMic = false;
                return 0;
            }
        }

        private void AddMessage(string message)
        {
            pBuilder.ClearContent();
            pBuilder.AppendText(message);
            sSynth.Speak(pBuilder);
        }

        public async Task HelloSimone()
        {
            pBuilder.ClearContent();
            pBuilder.AppendText("Hello Simone");
            sSynth.Speak(pBuilder);
            ViewBag.Message = pBuilder;
        }
    }
}