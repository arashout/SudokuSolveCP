using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace ReactSudoku.Controllers
{
    [Route("api")]
    public class SudokuController : Controller
    {
        [Route("[action]/{gridString}")]
        [HttpGet()]
        public IEnumerable<string> Solve(string gridString){
            var cpb = new CPBoard(gridString);
            var sr = CPSolver.Solve(cpb);
            return sr.CPB.GetTiles();
        }

        [Route("[action]/{gridString}")]
        [HttpGet()]
        public IEnumerable<string> Constrain(string gridString){
            var cpb = new CPBoard(gridString);
            CPSolver.InitializeConstraints(cpb);
            return cpb.GetTiles();
        }
    }
}
