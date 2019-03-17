using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NYear.PerformanceTest
{
    class Program
    {
        static void Main(string[] args)
        {


            Console.ReadKey();
        }



    }


    public enum Orm
    {
        SqlSugar,
        Dapper,
        EF,
        ODA,
    }
    enum PerformanceType
    {
        ReadData,
        SQLGenerate
    }
}
