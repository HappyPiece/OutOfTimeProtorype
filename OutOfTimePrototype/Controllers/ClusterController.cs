using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OutOfTimePrototype.DAL;
using OutOfTimePrototype.DAL.Models;
using OutOfTimePrototype.DTO;

namespace OutOfTimePrototype.Controllers
{
    [Route("api/cluster")]
    [ApiController]
    public class ClusterController : ControllerBase
    {
        private readonly OutOfTimeDbContext _outOfTimeDbContext;

        public ClusterController(OutOfTimeDbContext outOfTimeDbContext)
        {
            _outOfTimeDbContext = outOfTimeDbContext;
        }

        [HttpPost, Route("create")]
        public async Task<IActionResult> CreateCluster(ClusterDTO clusterDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await _outOfTimeDbContext.Clusters.AnyAsync(x => x.Number == clusterDTO.Number))
            {
                return BadRequest("Cluster with such number already exists");
            }

            if (clusterDTO.SuperClusterNumber != null)
                if (!await _outOfTimeDbContext.Clusters.AnyAsync(x => x.Number == clusterDTO.SuperClusterNumber))
                {
                    return BadRequest("Cluster, specified as parent, doesn't exist");
                }

            await _outOfTimeDbContext.AddAsync(new Cluster
            {
                Number = clusterDTO.Number,
                SuperCluster = await _outOfTimeDbContext.Clusters.SingleOrDefaultAsync(x => x.Number == clusterDTO.SuperClusterNumber)
            });

            await _outOfTimeDbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPost, Route("{number}/edit")]
        public async Task<IActionResult> EditCluster(string number, ClusterEditDTO clusterEditDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (number != clusterEditDTO.Number && await _outOfTimeDbContext.Clusters.AnyAsync(x => x.Number == clusterEditDTO.Number))
            {
                return BadRequest("Cluster with such number already exists");
            }

            if (clusterEditDTO.SuperClusterNumber != null)
                if (!await _outOfTimeDbContext.Clusters.AnyAsync(x => x.Number == clusterEditDTO.SuperClusterNumber))
                {
                    return BadRequest("Cluster, specified as parent, doesn't exist");
                }

            var cluster = await _outOfTimeDbContext.Clusters.SingleOrDefaultAsync(x => x.Number == number);
            if(cluster == null)
            {
                return NotFound("Cluster doesn't exist");
            }
            else
            {
                if (clusterEditDTO.Number != null)
                {
                    cluster.Number = clusterEditDTO.Number;
                }
                if (clusterEditDTO.SuperClusterNumber != null)
                {
                    cluster.SuperCluster = await _outOfTimeDbContext.Clusters.SingleOrDefaultAsync(x => x.Number == clusterEditDTO.SuperClusterNumber);
                }
            }

            await _outOfTimeDbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete, Route("{number}/delete")]
        public async Task<IActionResult> DeleteCluster(string number)
        {
            var cluster = await _outOfTimeDbContext.Clusters.SingleOrDefaultAsync(x => x.Number == number);
            if (cluster == null)
            {
                return NotFound("Cluster doesn't exist");
            }
            else
            {
                _outOfTimeDbContext.Clusters.Remove(cluster);
            }

            await _outOfTimeDbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllClusters()
        {
            var clusters = await _outOfTimeDbContext.Clusters.ToListAsync();

            var responce = clusters.Select(x => new ClusterDTO { Number = x.Number, SuperClusterNumber = x.SuperCluster?.Number });

            return Ok(responce);
        }

        [HttpGet, Route("{number}")]
        public async Task<IActionResult> GetCluster(string number)
        {
            var cluster = await _outOfTimeDbContext.Clusters.Include(x => x.SuperCluster).SingleOrDefaultAsync(x => x.Number == number);
            if (cluster == null)
            {
                return NotFound("Cluster doesn't exist");
            }

            return Ok(new ClusterDTO { Number = cluster.Number, SuperClusterNumber = cluster.SuperCluster?.Number });
        }
    }
}
