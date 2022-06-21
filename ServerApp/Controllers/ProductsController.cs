using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerApp.Data;
using ServerApp.Models;

namespace ServerApp.Controllers
{
   /// <summary>
   /// 
   /// </summary>
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController:ControllerBase
    {
        
        private readonly SocialContext _context;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public ProductsController(SocialContext context)
        {
            _context=context;
        }
      
      /// <summary>
      /// 
      /// </summary>
      /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetProducts(){
            var products= await _context.Products.ToListAsync();
            return Ok(products);
        }
        
      /// <summary>
      /// 
      /// </summary>
      /// <param name="id"></param>
      /// <returns></returns>
        [HttpGet("{id}")]
         public async Task<ActionResult> GetProduct(int id ){
           var product= await _context.Products.FirstOrDefaultAsync(x=>x.ProductId==id);
           if(product is null){
               return NotFound();
           }
           return Ok(product);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> AddProduct(Product product){
           try {
              await _context.Products.AddAsync(product);
              await _context.SaveChangesAsync();
           }
           catch(Exception ex) {
             return BadRequest(ex.Message);
           }

           return Ok(product);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<ActionResult> UpdateProduct(Product product){
           try {
              _context.Products.Update(product);
              await _context.SaveChangesAsync();
           }
           catch(Exception ex) {
             return BadRequest(ex.Message);
           }

           return Ok(product);
        }

       /// <summary>
       /// 
       /// </summary>
       /// <param name="productId"></param>
       /// <returns></returns>
        [HttpDelete]
        public async Task<ActionResult> DeleteProduct(int productId){
           try {
             var entity= await _context.Products.FindAsync(productId);
             if(entity is null )
                return NotFound("Product can not found!!");
             var ent= _context.Products.Remove(entity);
             await _context.SaveChangesAsync();
             return Ok(entity);
           }
           catch(Exception ex) {
             return BadRequest(ex.Message);
           }

           
        }


    }
}