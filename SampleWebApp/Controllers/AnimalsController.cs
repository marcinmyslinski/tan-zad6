using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SampleWebApp.Models;
using SampleWebApp.Services;
using System.Net;
using System.Threading.Tasks;

namespace SampleWebApp
{
    [Route("api/animals")]
    [ApiController]
    public class AnimalsController : ControllerBase
    {

        private readonly IDatabaseService _dbService;
        private readonly IDatabaseService2 _dbService2;


        public AnimalsController(IDatabaseService dbService, IDatabaseService2 dbService2)
        {
            _dbService = dbService;
            _dbService2 = dbService2;

        }

        [HttpGet]
        public IActionResult GetAnimals()
        {
            return Ok(_dbService.GetAnimals());
        }

        [HttpPost]
        public IActionResult AddAnimals(Models.Animal a)
        {
            var res =  Ok(_dbService2.AddAnimals(a));
            if (res == null)
            {
                return StatusCode(400);
            }
            else
            {
                return StatusCode(200);
            }
        }

        [HttpPut("{index}")]

        public async Task<IActionResult> UpdateAnimals(string index, Models.Animal a2)
        {
            var res= Ok(await _dbService2.ChangeAnimalAsync(index, a2));
            if (res.Value.Equals(1))
            {
                return StatusCode(200);
            }
            else if (res.Value.Equals(2))
            {
                return StatusCode(404);
            }
            else
            {
                return StatusCode(400);
            }
        }

        [HttpDelete("{index}")]
        public async Task<IActionResult> DeleteAnimals(string index)
        {
            var res = Ok(await _dbService2.DeleteAnimalAsync(index));
            if (res.Value.Equals(1))
            {
                return StatusCode(200);
            }
            else if (res.Value.Equals(2))
            {
                return StatusCode(404);
            }
            else
            {
                return StatusCode(400);
            }
        }

    }
}
