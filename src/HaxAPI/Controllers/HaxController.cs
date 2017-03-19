namespace HaxAPI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using HaxLib;
    using HaxLib.Models;
    using Microsoft.AspNetCore.Mvc;

    [Route("[controller]")]
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
                    return new ObjectResult(new { name = user.Name });
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
                return new ObjectResult(new { name = user.ID });
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
        public IActionResult CreateLobby([FromBody] PostRequestBody body)
        {
            if (body.userID != null)
            {
                string lid = this.lobbyManager.CreateLobby((int)body.userID);
                if (lid != null)
                {
                    return new ObjectResult(new { lid = lid });
                }
                else
                {
                    return NotFound();
                }
            }

            return BadRequest();
        }

        [HttpPost("lobbies/join")]
        public IActionResult JoinLobby([FromBody] PostRequestBody body)
        {
            if (body.lobbyID != null && body.userID != null)
            {
                if (this.lobbyManager.JoinLobby(body.lobbyID, (int)body.userID))
                {
                    return Ok();
                }

                return NotFound();
            }

            return BadRequest();
        }

        [HttpPost("lobbies/set")]
        public IActionResult Set([FromBody] PostRequestBody body)
        {
            if (body.lobbyID != null)
            {
                if (body.userID != null)
                {
                    if (body.userPayAmount != null)
                    {
                        if (this.lobbyManager.SetUserPayAmount(body.lobbyID, (int)body.userID, (float)body.userPayAmount))
                        {
                            return Ok();
                        }
                        else
                        {
                            return NotFound();
                        }
                    }
                    else if (body.verified != null)
                    {
                        if (this.lobbyManager.SetUserVerified(body.lobbyID, (int)body.userID, (bool)body.verified))
                        {
                            return Ok();
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
                else if (body.receiptUrl != null)
                {
                    if (this.lobbyManager.SetReceiptUrl(body.lobbyID, body.receiptUrl))
                    {
                        return Ok();
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                else if (body.totalPayAmount != null)
                {
                    if (this.lobbyManager.SetTotalPayAmount(body.lobbyID, (float)body.totalPayAmount))
                    {
                        return Ok();
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                else if (body.hostConfirmed != null)
                {
                    if (this.lobbyManager.SetHostConfirmed(body.lobbyID, (bool)body.hostConfirmed))
                    {
                        return Ok();
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                else if (body.merchantID != null)
                {
                    if (this.lobbyManager.SetMerchant(body.lobbyID, (int)body.merchantID))
                    {
                        return Ok();
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }

            return BadRequest();
        }

        [HttpPost("lobbies/pay/init")]
        public IActionResult InitPayment([FromBody] PostRequestBody body)
        {
            if (body.lobbyID != null)
            {
                if (this.lobbyManager.InitPayment(body.lobbyID))
                {
                    return Ok();
                }

                return NotFound();
            }

            return BadRequest();
        }

        [HttpPost("lobbies/delete")]
        public IActionResult DeleteLobby([FromBody] PostRequestBody body)
        {
            if (body.lobbyID != null)
            {
                if (this.lobbyManager.DeleteLobby(body.lobbyID))
                {
                    return Ok();
                }

                return NotFound();
            }

            return BadRequest();
        }

        [HttpPost("lobbies/pay/begin")]
        public IActionResult Pay([FromBody] PostRequestBody body)
        {
            if (body.lobbyID != null)
            {
                bool? result = this.lobbyManager.Pay(body.lobbyID).Result;
                if (result == null)
                {
                    return NotFound();
                }
                else if ((bool)result)
                {
                    return Ok();
                }

                return NoContent();
            }

            return BadRequest();
        }
    }
}
