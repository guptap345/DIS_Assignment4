using DIS_Assignment4.DataAccess;
using DIS_Assignment4.Domain;
using DIS_Assignment4.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DIS_Assignment4.Controllers
{
    public class HomeController : Controller
    {
        public ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }
        static string api_link = "https://data.cdc.gov/resource/hk9y-quqm.json?$where=%60group%60=%27By%20Total%27&age_group=All%20Ages";

        HttpClient httpclient = new HttpClient();

        Covid_Conditions covid_conditions = new Covid_Conditions();

        public IActionResult Data(string state = "United States")
        {
            httpclient.BaseAddress = new Uri(api_link);

            HttpResponseMessage response = httpclient.GetAsync(api_link).GetAwaiter().GetResult();

            string covidData = null;

            //var responseTask = httpclient.GetAsync(api_link);
            //responseTask.Wait();
            //var result = responseTask.Result;
            if (response.IsSuccessStatusCode)
            {
                //Get the data from api and store it as string in covidData variable
                covidData = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }
            if (!covidData.Equals(""))
            {
                // JsonConvert is part of the NewtonSoft.Json Nuget package which convert string to json
                covid_conditions.data = JsonConvert.DeserializeObject<List<Covid_Condition>>(covidData);
                
            }

            DbDomain d = new DbDomain(_context);
            //add the data to db
            if (d._context.Covid_Conditions_data.ToList().Count==0)
            {
                //post the data into database
                d.covidConditionPost(covid_conditions);
            }
            ViewBag.State = state;
            return View(d._context.Covid_Conditions_data.ToList());
        }
        public IActionResult Condition(string val)
        {
            if (TempData.Count!= 0 )
            {
                //to display a message to the client like updated or deleted
                ViewBag.Message = TempData["shortMessage"].ToString();
            }
            return View(_context.Covid_Conditions_data.Where(c => c.condition_group == val));



            //var condition_data=d._context.Covid_Conditions_data.Where(c => c.condition_group == val).GroupBy(c => c.condition_group).First();
            //return View(covid_conditions.data);
        }
        public IActionResult Update(string cond)
        {
            DbDomain d = new DbDomain(_context);
            //fetch the records which match the given condition
            var _cov=d._context.Covid_Conditions_data.Where(c => c.condition == cond).First();

            return View(_cov);
        }
        [HttpPost]
        public IActionResult UpdateRecord(Covid_Condition data)
        {
            var rec=_context.Covid_Conditions_data.FirstOrDefault(x => x.CaseId== data.CaseId);

            if(rec!=null)
            {
                rec.covid_19_deaths = data.covid_19_deaths;
                _context.SaveChanges();
            }

            return RedirectToAction("Condition", new { val = rec.condition_group });
        }
        public IActionResult Delete(string cond)
        {
            var rec = _context.Covid_Conditions_data.FirstOrDefault(x => x.condition == cond);
            if (rec != null)
            {
                _context.Covid_Conditions_data.Remove(rec);
                _context.SaveChanges();
                TempData["shortMessage"] = "Deleted Successfully";
            }

            return RedirectToAction("Condition",new { val = rec.condition_group });
        }
    }
}




