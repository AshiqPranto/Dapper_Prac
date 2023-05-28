using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace DapperCrudPrac.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuperHeroController : ControllerBase
    {
        private readonly IConfiguration _config;

        public SuperHeroController(IConfiguration config) 
        {
            _config = config;
        }
        [HttpGet]
        public async Task<ActionResult<List<SuperHero>>> GetAllSuperHeroes()
        {
            var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var heroes = await SelectAllSuperHeroes(connection);
            return Ok(heroes);
        }
        private static async Task<IEnumerable<SuperHero>> SelectAllSuperHeroes(SqlConnection connection)
        {
            var heroes = await connection.QueryAsync<SuperHero>("select * from superheroes");
            return heroes;
        }

        [HttpGet("{HeroId}")]
        public async Task<ActionResult<SuperHero>> GetSuperHero(int HeroId)
        {
            var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var hero = await connection.QueryFirstAsync<SuperHero>("select * from superheroes where id = @id", new { id = HeroId });
            return Ok(hero);
        }

        [HttpPost]
        public async Task<ActionResult<List<SuperHero>>> CreateSuperHero(SuperHero newSuperHero)
        {
            var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("insert into superheroes(name,firstname,lastname,place)" +
                "values (@Name,@FirstName,@LastName,@Place)", newSuperHero);
            return Ok(await SelectAllSuperHeroes(connection));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteSuperHero(int id)
        {
            var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("Delete from superheroes where ID = @Id", new { Id = id });
            return Ok();
        }

        [HttpPut]
        public async Task<ActionResult> UpdateSuperHero(SuperHero newSuperHero)
        {
            var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("update superheroes set name=@name, firstname=@firstname,lastname=@lastname,place=@place where id=@id", newSuperHero);
            return Ok();
        }
    }
}
