using DataTypes;
using Newtonsoft.Json;
using System;
using System.Web.Http;
using System.Web.Http.Cors;
using WebAPIClient.APICalls;

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
                Job job = JobAPI.GetById(pk);
                AgentEnvironment.SetJob(job);
                JobLogic.StartJob(job);
            }
        }

        [Route("api/machine/giveadmin/{pk}")]
        [HttpPut]
        public void PutAdmin(int pk)
        {
            Job job = JobAPI.GetById(pk);
            AgentEnvironment.SetJob(job);
            JobLogic.StartAdminJob(job);
        }

        [Route("api/machine/getstatus")]
        [HttpGet]
        public IHttpActionResult GetStatus()
        {
            return Ok(JsonConvert.SerializeObject(AgentEnvironment.HasTask));
        }

        [Route("api/machine/kill")]
        [HttpPost]
        public void Kill()
        {
            AgentLogic.Kill();
        }

        [Route("api/machine/shutdown")]
        [HttpPost]
        public void Shutdown()
        {
            AgentLogic.Shutdown();
        }
    }
}