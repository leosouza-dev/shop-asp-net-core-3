using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;

namespace Shop.Controllers
{
    [ApiController]
    [Route("categories")]
    public class CategoryController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Category>>> Get([FromServices]DataContext context)
        {
            try
            {
                var lista = await context.Categories.AsNoTracking().ToListAsync();
                return Ok(lista);             
            }
            catch (System.Exception)
            {
                return BadRequest(new { message = "Não foi possivel recuperar a lista de categoria" });
            }
        }

        [HttpGet]
        [Route("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<Category>> GetById(int id, [FromServices]DataContext context)
        {
            try
            {
                var categoria = await context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);

                if(categoria == null)
                    return NotFound();

                return Ok(categoria);        
            }
            catch (System.Exception)
            {
                return BadRequest(new { message = "Não foi possivel recuperar a categoria" });
            }
        }

        [HttpPost]
        [Route("")]
        [Authorize]
        public async Task<ActionResult<Category>> Post([FromBody]Category category, [FromServices]DataContext context)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            try
            {
                context.Categories.Add(category);
                await context.SaveChangesAsync();
                return Ok(category);
                
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possivel criar a categoria" });
            }

        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<Category>> Put(int id, [FromBody]Category category, [FromServices]DataContext context)
        {

            if(category.Id != id)
            {
                return NotFound(new {message = "Categoria não encontrada"});
            }

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            try
            {
                // context.Entry<Category>(category).State = EntityState.Modified;
                context.Categories.Update(category);
                await context.SaveChangesAsync();
                return Ok(category);
                
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possivel atualizar a categoria" });
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<Category>> Delete(int id, [FromServices]DataContext context)
        {
            var modelo = await context.Categories.FirstOrDefaultAsync(c => c.Id == id);

            if(modelo == null)
            {
                return NotFound(new { message = "Não foi possivel encontrar a categoria" });
            }

            try
            {
                context.Categories.Remove(modelo);
                await context.SaveChangesAsync();
                return Ok(new { message = "Categoria removida com sucesso!" });               
            }
            catch (System.Exception)
            {
                
                return BadRequest(new { message = "Não foi possivel Deletar a categoria" });

            }

        }
    }
}