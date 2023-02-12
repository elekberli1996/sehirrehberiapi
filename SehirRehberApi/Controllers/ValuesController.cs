using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SehirRehberApi.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SehirRehberApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private DataContext _dataContext;

        public ValuesController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet]
        public ActionResult GetValues()
        {
            var values =_dataContext.Values.ToList();
            return Ok(values);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id)
        {
            var value = await _dataContext.Values.FirstOrDefaultAsync(v => v.Id == id);
            return Ok(value);
        }
    }
}
