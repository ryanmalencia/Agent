using System.Web.Http;
using System;
using System.Web.Http.Cors;
using DataTypes;

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
                Logic.StartJob(pk);
            }
        }
    }
}