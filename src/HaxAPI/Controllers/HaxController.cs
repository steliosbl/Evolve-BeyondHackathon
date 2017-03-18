namespace HaxAPI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using HaxLib;
    using HaxLib.Models;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    public class HaxController : Controller
    {
        private IDatabase database;
        private ILobbyManager lobbyManager;

        public HaxController(IDatabase database, ILobbyManager manager)
        {
            this.database = database;
            this.lobbyManager = manager;
        }

        [HttpGet("users")]
        public IActionResult GetUserById([RequiredFromQuery] string id)
        {
            int userID;
            if (int.TryParse(id, out userID))
            {
                var user = this.database.Get(userID);
                if (user != null)
                {
                    return new ObjectResult(user.Name);
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet("users")]
        public IActionResult GetUserByname([RequiredFromQuery] string name)
        {
            var user = this.database.Get(name);
            if (user != null)
            {
                return new ObjectResult(user.ID);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("lobbies")]
        public IActionResult GetLobbyInfo([RequiredFromQuery] string id)
        {
            var lobby = this.lobbyManager.GetLobby(id);
            if (lobby != null)
            {
                return new ObjectResult(lobby);
            }

            return NotFound();
        }

        [HttpPost("lobbies/new")]
        public IActionResult CreateLobby([FromBody] string userID)
        {
            int id;
            if (int.TryParse(userID, out id))
            {
                string lid = this.lobbyManager.CreateLobby(id);
                if (lid != null)
                {
                    return new ObjectResult(new { lid = lid });
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
