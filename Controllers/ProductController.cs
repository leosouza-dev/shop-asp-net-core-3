using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;

namespace Shop.Controllers
{
    [ApiController]
    [Route("products")]
    public class ProductController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Product>>> Get([FromServices]DataContext context)
        {
            try
            {
                var products = await context.Products.Include(p => p.Category).AsNoTracking().ToListAsync(); //o include é um join no sqlServer

                if(products == null)
                    return NotFound();

                return Ok(products);           
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possivel recuperar a lista de produtos"});
            }
        }

        [HttpGet]
        [Route("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<Product>> GetById(int id, [FromServices]DataContext context)
        {
            try
            {
                // var product = await context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
                var product = await context.Products.Include(p => p.Category).AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

                if(product == null)
                    return NotFound(new { message = "Produto não encontrado"});

                return Ok(product);              
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possivel recuperar o produtos"});
            }
        }

        [HttpGet]
        [Route("categories/{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Product>>> GetByCategory(int id, [FromServices]DataContext context)
        {
            try
            {
                var products = await context.Products.Where(p => p.CategoryId == id).ToListAsync();
                return Ok(products);
            }
            catch (Exception)
            {
                return BadRequest(new {message = "Não foi possivel encontrar a lista de produtos dessa categoria"});
            }
        }

        [HttpPost]
        [Route("")]
        [Authorize]
        public async Task<ActionResult<Product>> Post([FromBody]Product product, [FromServices]DataContext context)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                context.Products.Add(product);
                await context.SaveChangesAsync();
                return Ok(product);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possivel criar o produto"});
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<Product>> Put(int id, [FromBody]Product product, [FromServices]DataContext context)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                if(product.Id != id)
                    return NotFound(new { message = "Produto não encontrado"});

                context.Products.Update(product);
                await context.SaveChangesAsync();
                return Ok(product);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possivel atualizar o produto"});
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<Product>> Delete(int id, [FromServices]DataContext context)
        {
            try
            {
                var product = await context.Products.FirstOrDefaultAsync(p => p.Id == id);

                if(product == null)
                    return NotFound(new { message = "Produto não encontrado"});

                context.Products.Remove(product);
                await context.SaveChangesAsync();
                return Ok(new { message = "Produto removido com sucesso"});
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possivel remover o produto"});
            }
        }
    }
}