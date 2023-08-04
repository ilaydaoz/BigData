using BigData.DAL.Dto;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace BigData.Controllers
{
    public class DefaultController : Controller
    {
        private readonly string _connectionString = "Server=DESKTOP-AKOHCIC\\SQLEXPRESS;initial Catalog=CARPLATES;integrated Security=true";
        
        public async Task<IActionResult> Index()
        {
           await using var connection = new SqlConnection(_connectionString);
            var plateMax = (await connection.QueryAsync<PLATEResult>("SELECT TOP 1 SUBSTRING(PLATE, 1, 2) AS plate, COUNT(*) AS count FROM PLATES GROUP BY SUBSTRING(PLATE, 1, 2) ORDER BY count DESC")).FirstOrDefault();
            var plateMin = (await connection.QueryAsync<PLATEResult>("SELECT TOP 1 SUBSTRING(PLATE, 1, 2) AS plate, COUNT(*) AS count FROM PLATES GROUP BY SUBSTRING(PLATE, 1, 2) ORDER BY count ASC")).FirstOrDefault();

            var brandMax = (await connection.QueryAsync<BrandResult>("SELECT TOP 1 BRAND, COUNT(*) AS count FROM PLATES GROUP BY BRAND ORDER BY count DESC")).FirstOrDefault();
            var brandMin = (await connection.QueryAsync<BrandResult>("SELECT TOP 1 BRAND, COUNT(*) AS count FROM PLATES GROUP BY BRAND ORDER BY count ASC")).FirstOrDefault();

            var modelMax = (await connection.QueryAsync<MODELResult>("SELECT TOP 1 MODEL, COUNT(*) AS count FROM PLATES GROUP BY MODEL ORDER BY count DESC")).FirstOrDefault();
            var modelMin = (await connection.QueryAsync<MODELResult>("SELECT TOP 1 MODEL, COUNT(*) AS count FROM PLATES GROUP BY MODEL ORDER BY count ASC")).FirstOrDefault();


            var fuelType = (await connection.QueryAsync<FuelResult>("SELECT TOP 1 FUEL, COUNT(*) AS count FROM PLATES GROUP BY FUEL ORDER BY count DESC")).FirstOrDefault();

           
            var colorType = (await connection.QueryAsync<ColorResult>("SELECT TOP 1 COLOR, COUNT(*) AS count FROM PLATES GROUP BY COLOR ORDER BY count DESC")).FirstOrDefault();

            ViewData["plateMin"] = plateMin.PLATE;
            ViewData["countMinPlate"] = plateMin.Count;
           

            ViewData["brandMin"] = brandMin.BRAND;
            ViewData["countMinBrand"] = brandMin.Count;


            ViewData["modelMax"] = modelMax.MODEL;
            ViewData["countMaxCount"] = modelMin.Count;


            ViewData["fuelType"] = fuelType.FUEL;
            ViewData["fuelTypeCount"] = fuelType.Count;

            ViewData["colorType"] = colorType.Color;
            ViewData["colorTypeCount"] = colorType.Count;

            return View();




        }
        public async Task<IActionResult> Search(string keyword)
        {

            string query = @"
            SELECT TOP 10000 BRAND, SUBSTRING(PLATE, 1, 2) AS PlatePrefix, SHIFTTYPE, FUEL, COLOR
            FROM PLATES
            WHERE BRAND LIKE '%' + @Keyword + '%'
               OR FUEL LIKE '%' + @Keyword + '%'
               OR COLOR LIKE '%' + @Keyword + '%' ";
       

            await using var connection = new SqlConnection(_connectionString);
            connection.Open();

            var searchResults = await connection.QueryAsync<SearchResult>(query, new { Keyword = keyword });

            return Json(searchResults);

        }

    }
}
