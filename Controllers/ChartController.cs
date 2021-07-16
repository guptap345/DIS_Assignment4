using DIS_Assignment4.DataAccess;
using DIS_Assignment4.Models;
using DIS_Assignment4.Domain;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.Net.Http;

namespace DIS_Assignment4.Controllers
{
    public class ChartController : Controller
    {
        public ApplicationDbContext _context;

        public ChartController(ApplicationDbContext context)
        {
            _context = context;
        }
        static string api_link = "https://data.cdc.gov/resource/hk9y-quqm.json?$where=`group`=%27By%20Total%27&state=United%20States&age_group=All%20Ages";

        HttpClient httpclient = new HttpClient();

        Covid_Conditions covid_conditions = new Covid_Conditions();

        public ViewResult Chart()
        {
            httpclient.BaseAddress = new Uri(api_link);

            HttpResponseMessage response = httpclient.GetAsync(api_link).GetAwaiter().GetResult();
            DbDomain d = new DbDomain(_context);
            if (d._context.Covid_Conditions_data.ToList().Count == 0)
            {
                d.covidConditionPost(covid_conditions);
            }

            var results = from p in _context.Covid_Conditions_data
                          group p by p.condition_group into g
                          select new { condition_group = g.Key, covid_19_deaths = g.Sum(c => Convert.ToInt64(c.covid_19_deaths)) };

            List<string> ChartLabels = new List<string>();
            ChartLabels = results.Select(p => p.condition_group).ToList();
            List<long> ChartData = new List<long>();
            ChartData = results.Select(p => p.covid_19_deaths).ToList();

            ChartModel Model = new ChartModel
            {
                ChartType = "pie",
                Labels = String.Join(",", ChartLabels.Select(d => "'" + d + "'")),
                Data = String.Join(",", ChartData.Select(d => d)),
                Title = "Covid deaths by Condition Group"
            };
            return View(Model);
        }
    }
}
