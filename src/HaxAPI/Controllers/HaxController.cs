namespace HaxAPI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using HaxLib;
    using HaxLib.Models;

    [Route("api/Hax")]
    public class HaxController : Controller
    {
        private IDatabase database;

        public HaxController(IDatabase database)
        {
            this.database = database;
        }


    }
}
