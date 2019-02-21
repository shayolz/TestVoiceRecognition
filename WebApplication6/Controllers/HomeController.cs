using System;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace WebApplication6.Controllers
{
    public class HomeController : Controller
    {
        private SpeechSynthesizer sSynth = new SpeechSynthesizer();
        private PromptBuilder pBuilder = new PromptBuilder();
        private WMPLib.WindowsMediaPlayer player = new WMPLib.WindowsMediaPlayer();
        private SpeechRecognitionEngine sRecognize = new SpeechRecognitionEngine();
        private bool blockMic = false;

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "aa";
            return View();
        }

        public async Task CallAsync()
        {
            Choices sList = new Choices();
            sList.Add(new string[] {"veloce", "lento", "canzoneuno", "canzone uno", "stop", "normale", "avanti", "indietro", "pausa", "canzonedue", "canzone due", "chi è la più bella","la più bella"});

            Grammar gr = new Grammar(new GrammarBuilder(sList));

            try
            {
                sRecognize.RequestRecognizerUpdate();
                sRecognize.LoadGrammar(gr);
                sRecognize.SpeechRecognized += sRecognize_SpeechRecognized;
                sRecognize.SetInputToDefaultAudioDevice();
                sRecognize.RecognizeAsync(RecognizeMode.Multiple);
            }
            catch (Exception)
            {
                ViewBag.Message = "not recognized";
            }

            ViewBag.Message = "aa";
        }

        private void sRecognize_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (blockMic)
                return;

            try
            {
                if (e.Result.Text == "la più bella" || e.Result.Text == "chi è la più bella") {
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
            }
            catch (Exception)
            {
                throw;
            }

            blockMic = false;
        }

        public async Task<int> VoiceMessage(string message)
        {
            try
            {
                await Task.Run(() => NewMethod1(message));

                return 1;
            }
            catch (Exception e)
            {
                blockMic = false;
                return 0;
            }
        }

        private void NewMethod1(string message)
        {
            pBuilder.ClearContent();
            pBuilder.AppendText(message);
            sSynth.Speak(pBuilder);
        }

        public async Task<ActionResult> Contact()
        {
            try
            {
                pBuilder.ClearContent();
                pBuilder.AppendText("Hello Simone");
                sSynth.Speak(pBuilder);

                ViewBag.Message = pBuilder;

                return View();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}