using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


namespace dojodachi.Controllers
{
    //using Newtonsoft.Json;
// Somewhere in your namespace, outside other classes
        public static class SessionExtensions
        {
            // We can call ".SetObjectAsJson" just like our other session set methods, by passing a key and a value
            public static void SetObjectAsJson(this ISession session, string key, object value)
            {
                // This helper function simply serializes theobject to JSON and stores it as a string in session
                session.SetString(key, JsonConvert.SerializeObject(value));
            }
            
            // generic type T is a stand-in indicating that we need to specify the type on retrieval
            public static T GetObjectFromJson<T>(this ISession session, string key)
            {
                string value = session.GetString(key);
                // Upone retrieval the object is deserialized based on the type we specified
                return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
            }
        }
    public class Dachi : Controller
    {
        
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            if(HttpContext.Session.GetObjectFromJson<dachiinfo>("Dojodachi") == null)
            {
                HttpContext.Session.SetObjectAsJson("Dojodachi", new dachiinfo());
            }

            ViewBag.Dojodachi = HttpContext.Session.GetObjectFromJson<dachiinfo>("Dojodachi");
            ViewBag.Message = "You got a new Dojodachi!";
            ViewBag.GameStatus = "running";
            ViewBag.Reaction = "";
            return View();
        }

        [HttpPost]
        [Route("performAction")]
        public IActionResult PerformAction(string action)
        {
            dachiinfo edit_dachi = HttpContext.Session.GetObjectFromJson<dachiinfo>("Dojodachi");
            Random RandObject = new Random();
            ViewBag.GameStatus = "running";
            switch(action)
            {
                case "feed":
                    if(edit_dachi.meals > 0){
                        edit_dachi.meals -= 1;
                        if(RandObject.Next(0, 4) > 0)
                        {
                            edit_dachi.fullness += RandObject.Next(5, 11);
                            ViewBag.Reaction = ":)";
                            ViewBag.Message = "Dojodachi enjoyed the meal!";
                        }
                        else
                        {
                            ViewBag.Reaction = ":(";
                            ViewBag.Message = "Dojodachi didn't like the food very much...";
                        }
                    }
                    else
                    {
                        ViewBag.Reaction = ":(";
                        ViewBag.Message = "You don't have any food for your Dojodachi!";
                    }
                    break;
                case "play":
                    if(edit_dachi.energy > 4)
                    {
                        edit_dachi.energy -= 5;
                        if(RandObject.Next(0, 4) > 0)
                        {
                            edit_dachi.happiness += RandObject.Next(5, 11);
                            ViewBag.Reaction = ":)";
                            ViewBag.Message = "Dojodachi had fun playing!";
                        }
                        else
                        {
                            ViewBag.Reaction = ":(";
                            ViewBag.Message = "Looks like Dojodachi didn't want to play...";
                        }
                    }
                    else
                    {
                        ViewBag.Reaction = ":(";
                        ViewBag.Message = "Not enough energy...";
                    }

                    break;
                case "work":
                    if(edit_dachi.energy > 4)
                    {
                        edit_dachi.energy -= 5;
                        edit_dachi.meals += RandObject.Next(1, 4);
                        ViewBag.Reaction = ":)";
                        ViewBag.Message = "You worked hard to feed your Dojodachi!";
                    }
                    else
                    {
                        ViewBag.Reaction = ":(";
                        ViewBag.Message = "Not enough energy...";
                    }
                    break;
                case "sleep":
                    edit_dachi.energy += 15;
                    edit_dachi.fullness -= 5;
                    edit_dachi.happiness -= 5;
                    ViewBag.Reaction = ":)";
                    ViewBag.Message = "Dojodachi seems well rested.";
                    break;

            }
            if(edit_dachi.fullness < 1 || edit_dachi.happiness < 1)
            {
                ViewBag.Reaction = "X(";
                ViewBag.Message = "Oh no! Your Dojodachi has died...";
                ViewBag.GameStatus = "over";
            }
            else if(edit_dachi.fullness > 99 && edit_dachi.happiness > 99)
            {
                ViewBag.Reaction = ":D";
                ViewBag.Message = "Congratulations! You win!";
                ViewBag.GameStatus = "over";
            }
            HttpContext.Session.SetObjectAsJson("Dojodachi", edit_dachi);
            ViewBag.Dojodachi = edit_dachi;
            return View("Index");
        }

        [HttpGet]
        [Route("reset")]
        public IActionResult Reset()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}