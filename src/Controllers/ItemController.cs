namespace todo.Controllers
{
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Models;
    using System.Configuration;
    using System;
    using System.IO;
    using System.Collections.Generic;

    public class ItemController : Controller
    {

        private void TracePost(string method)
        {
            //Store the posted data
            Request.InputStream.Seek(0, SeekOrigin.Begin);
            string rawPostData = new StreamReader(Request.InputStream).ReadToEnd();
            var telemetry = new Microsoft.ApplicationInsights.TelemetryClient();
            telemetry.TrackTrace(rawPostData,
                           Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Warning,
                           new Dictionary<string, string> { { "PostData", "true" }, { "Method", method } });
        }

        [ActionName("Index")]
        public async Task<ActionResult> IndexAsync()
        {
            //var apiUrl = ConfigurationManager.AppSettings["api"];
            //using (var client = new WebClient())
            //{
            //    using (var stream = client.OpenRead(new Uri(apiUrl + "/api/Values/")))
            //    using (StreamReader reader = new StreamReader(stream))
            //    {
            //        ViewBag.Values = reader.ReadToEnd();
            //    }
            //}

            //var items = await DocumentDBRepository<Item>.GetItemsAsync(d => !d.Completed);
            //return View(items);


            //Fake dependency call for demo
            var telemetry = new Microsoft.ApplicationInsights.TelemetryClient();
            var success = false;
            var startTime = DateTime.UtcNow;
            var timer = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                var apiUrl = ConfigurationManager.AppSettings["api"];
                using (var client = new WebClient())
                {
                    using (var stream = client.OpenRead(new Uri(apiUrl + "/api/Values/")))
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        ViewBag.Values = reader.ReadToEnd();
                    }
                }

                var items = await DocumentDBRepository<Item>.GetItemsAsync(d => !d.Completed);
                return View(items);
            }
            finally
            {
                timer.Stop();
                telemetry.TrackDependency("IndexAsyncFakeDependency", "IndexAsync - Fake Call", startTime, timer.Elapsed, success);
            }

        }

#pragma warning disable 1998
        [ActionName("Create")]
        public async Task<ActionResult> CreateAsync()
        {
            return View();
        }
#pragma warning restore 1998

        [HttpPost]
        [ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync([Bind(Include = "Id,Name,Description,Completed")] Item item)
        {
            TracePost("CreateAsync");
            if (ModelState.IsValid)
            {
                await DocumentDBRepository<Item>.CreateItemAsync(item);
                return RedirectToAction("Index");
            }

            return View(item);
        }

        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAsync([Bind(Include = "Id,Name,Description,Completed")] Item item)
        {
            if (ModelState.IsValid)
            {
                await DocumentDBRepository<Item>.UpdateItemAsync(item.Id, item);
                return RedirectToAction("Index");
            }

            return View(item);
        }

        [ActionName("Edit")]
        public async Task<ActionResult> EditAsync(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Item item = await DocumentDBRepository<Item>.GetItemAsync(id);
            if (item == null)
            {
                return HttpNotFound();
            }

            return View(item);
        }

        [ActionName("Delete")]
        public async Task<ActionResult> DeleteAsync(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Item item = await DocumentDBRepository<Item>.GetItemAsync(id);
            if (item == null)
            {
                return HttpNotFound();
            }

            return View(item);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmedAsync([Bind(Include = "Id")] string id)
        {
            await DocumentDBRepository<Item>.DeleteItemAsync(id);
            return RedirectToAction("Index");
        }

        [ActionName("Details")]
        public async Task<ActionResult> DetailsAsync(string id)
        {
            Item item = await DocumentDBRepository<Item>.GetItemAsync(id);
            return View(item);
        }
    }
}