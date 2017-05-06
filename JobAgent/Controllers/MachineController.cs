using System;
using System.Web.Http;
using Newtonsoft.Json;
using System.Web.Http.Cors;

namespace JobAgent.Controllers
{
    [EnableCors("*", "*", "*")]
    public class MachineController: ApiController
    {
        [Route("api/machine/print/{message}")]
        [HttpGet]
        public void Put(string message)
        {
            Console.WriteLine(message);
        }

        [Route("api/machine/give/{pk}")]
        [HttpPut]
        public void Put(int pk)
        {
            if (!AgentEnvironment.HasTask)
            {
                AgentEnvironment.HasTask = true;
                JobLogic.StartJob(pk);
            }
        }

        [Route("api/machine/giveadmin/{pk}")]
        [HttpPut]
        public void PutAdmin(int pk)
        {
            JobLogic.StartAdminJob(pk);
        }

        [Route("api/machine/getstatus")]
        [HttpGet]
        public IHttpActionResult GetStatus()
        {
            return Ok(JsonConvert.SerializeObject(AgentEnvironment.HasTask));
        }
    }
}