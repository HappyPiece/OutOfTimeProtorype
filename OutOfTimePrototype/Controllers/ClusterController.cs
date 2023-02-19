using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OutOfTimePrototype.DAL;
using OutOfTimePrototype.DAL.Models;
using OutOfTimePrototype.DTO;
using OutOfTimePrototype.Services.Interfaces;

namespace OutOfTimePrototype.Controllers
{
    [Route("api/cluster")]
    [ApiController]
    public class ClusterController : ControllerBase
    {
        private readonly OutOfTimeDbContext _outOfTimeDbContext;
        private readonly IClusterService _clusterService;

        public ClusterController(OutOfTimeDbContext outOfTimeDbContext, IClusterService clusterService)
        {
            _outOfTimeDbContext = outOfTimeDbContext;
            _clusterService = clusterService;
        }

        [HttpPost, Route("create")]
        public async Task<IActionResult> CreateCluster(ClusterDto clusterDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await _outOfTimeDbContext.Clusters.AnyAsync(x => x.Number == clusterDto.Number))
            {
                return BadRequest("Cluster with such number already exists");
            }

            if (clusterDto.SuperClusterNumber != null)
                if (!await _outOfTimeDbContext.Clusters.AnyAsync(x => x.Number == clusterDto.SuperClusterNumber))
                {
                    return BadRequest("Cluster, specified as parent, doesn't exist");
                }

            await _outOfTimeDbContext.AddAsync(new Cluster
            {
                Number = clusterDto.Number,
                SuperCluster = await _outOfTimeDbContext.Clusters.SingleOrDefaultAsync(x => x.Number == clusterDto.SuperClusterNumber)
            });

            await _outOfTimeDbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPost, Route("{number}/edit")]
        public async Task<IActionResult> EditCluster(string number, ClusterEditDto clusterEditDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (number != clusterEditDto.Number && await _outOfTimeDbContext.Clusters.AnyAsync(x => x.Number == clusterEditDto.Number))
            {
                return BadRequest("Cluster with such number already exists");
            }

            if (clusterEditDto.SuperClusterNumber != null)
                if (!await _outOfTimeDbContext.Clusters.AnyAsync(x => x.Number == clusterEditDto.SuperClusterNumber))
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
                if (clusterEditDto.Number != null)
                {
                    cluster.Number = clusterEditDto.Number;
                }
                if (clusterEditDto.SuperClusterNumber != null)
                {
                    cluster.SuperCluster = await _outOfTimeDbContext.Clusters.SingleOrDefaultAsync(x => x.Number == clusterEditDto.SuperClusterNumber);
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

            var response = clusters.Select(x => new ClusterDto { Number = x.Number, SuperClusterNumber = x.SuperCluster?.Number });

            return Ok(response);
        }

        [HttpGet, Route("{number}")]
        public async Task<IActionResult> GetCluster(string number)
        {
            var cluster = await _outOfTimeDbContext.Clusters.Include(x => x.SuperCluster).SingleOrDefaultAsync(x => x.Number == number);
            if (cluster == null)
            {
                return NotFound("Cluster doesn't exist");
            }

            return Ok(new ClusterDto { Number = cluster.Number, SuperClusterNumber = cluster.SuperCluster?.Number });
        }

        [HttpGet, Route("{number}/super")]
        public async Task<IActionResult> GetSuper(string number)
        {
            var cluster = await _outOfTimeDbContext.Clusters.Include(x => x.SuperCluster).SingleOrDefaultAsync(x => x.Number == number);
            if (cluster == null)
            {
                return NotFound("Cluster doesn't exist");
            }

            var super = await _clusterService.GetSuperClusters(cluster);

            return Ok(super.Select(x => new ClusterDto { Number = x.Number, SuperClusterNumber = x.SuperCluster?.Number }));
        }
    }
}
