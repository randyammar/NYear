using NYear.PerformanceTest.ORMTest;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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


        static void ReadDataTest()
        {
            Stopwatch ODAsw = new Stopwatch();
            ODAsw.Start();
            for (int i = 0; i < 10; i++)
                ODATest.ReadData();
            ODAsw.Stop();

            Stopwatch Sugarsw = new Stopwatch();
            Sugarsw.Start();
            for (int i = 0; i < 10; i++)
                SqlSugarTest.ReadData();
            Sugarsw.Stop();

            Stopwatch EFsw = new Stopwatch();
            EFsw.Start();
            for (int i = 0; i < 10; i++)
                EFTest.ReadData();
            EFsw.Stop();

            Stopwatch Dappersw = new Stopwatch();
            Dappersw.Start();
            for (int i = 0; i < 10; i++)
                DapperTest.ReadData();
            Dappersw.Stop();

            Console.WriteLine(string.Format("ODA :{0} , Sugar {1}, EF :{2} ,Dapper :{3} ",
                ODAsw.ElapsedMilliseconds.ToString(),
                Sugarsw.ElapsedMilliseconds.ToString(),
                EFsw.ElapsedMilliseconds.ToString(),
                Dappersw.ElapsedMilliseconds.ToString()
               ));
        }
        static void PagingTest()
        {
            Stopwatch ODAsw = new Stopwatch();
            ODAsw.Start();
            for (int i = 0; i < 10000; i++)
                ODATest.Paging();
            ODAsw.Stop();

            Stopwatch Sugarsw = new Stopwatch();
            Sugarsw.Start();
            for (int i = 0; i < 10000; i++)
                SqlSugarTest.Paging();
            Sugarsw.Stop();

            Stopwatch EFsw = new Stopwatch();
            EFsw.Start();
            for (int i = 0; i < 10000; i++)
                EFTest.Paging();
            EFsw.Stop();

            Stopwatch Dappersw = new Stopwatch();
            Dappersw.Start();
            for (int i = 0; i < 10000; i++)
                DapperTest.Paging();
            Dappersw.Stop();

            Console.WriteLine(string.Format("ODA :{0} , Sugar {1}, EF :{2} ,Dapper :{3} ",
                ODAsw.ElapsedMilliseconds.ToString(),
                Sugarsw.ElapsedMilliseconds.ToString(),
                EFsw.ElapsedMilliseconds.ToString(),
                Dappersw.ElapsedMilliseconds.ToString()
               ));
        }
        static void SqlTest()
        {
            Stopwatch ODAsw = new Stopwatch();
            ODAsw.Start();
            for (int i = 0; i < 10000; i++)
                ODATest.Sql();
            ODAsw.Stop();

            Stopwatch Sugarsw = new Stopwatch();
            Sugarsw.Start();
            for (int i = 0; i < 10000; i++)
                SqlSugarTest.Sql();
            Sugarsw.Stop();

            Stopwatch EFsw = new Stopwatch();
            EFsw.Start();
            for (int i = 0; i < 10000; i++)
                EFTest.Sql();
            EFsw.Stop();

            Stopwatch Dappersw = new Stopwatch();
            Dappersw.Start();
            for (int i = 0; i < 10000; i++)
                DapperTest.Sql();
            Dappersw.Stop();

            Console.WriteLine(string.Format("ODA :{0} , Sugar {1}, EF :{2} ,Dapper :{3} ",
                ODAsw.ElapsedMilliseconds.ToString(),
                Sugarsw.ElapsedMilliseconds.ToString(),
                EFsw.ElapsedMilliseconds.ToString(),
                Dappersw.ElapsedMilliseconds.ToString()
               ));
        }
        static void ByIdTest()
        {
            Stopwatch ODAsw = new Stopwatch();
            ODAsw.Start();
            for (int i = 0; i < 10000; i++)
                ODATest.GetById();
            ODAsw.Stop();

            Stopwatch Sugarsw = new Stopwatch();
            Sugarsw.Start();
            for (int i = 0; i < 10000; i++)
                SqlSugarTest.GetById();
            Sugarsw.Stop();

            Stopwatch EFsw = new Stopwatch();
            EFsw.Start();
            for (int i = 0; i < 10000; i++)
                EFTest.GetById();
            EFsw.Stop();

            Stopwatch Dappersw = new Stopwatch();
            Dappersw.Start();
            for (int i = 0; i < 10000; i++)
                DapperTest.GetById();
            Dappersw.Stop();

            Console.WriteLine(string.Format("ODA :{0} , Sugar {1}, EF :{2} ,Dapper :{3} ",
                ODAsw.ElapsedMilliseconds.ToString(),
                Sugarsw.ElapsedMilliseconds.ToString(),
                EFsw.ElapsedMilliseconds.ToString(),
                Dappersw.ElapsedMilliseconds.ToString()
               ));
        }


        static void Test(OrmType Orm, PerformanceType Per,int Times)
        {
            new Action(() => 
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();

                

                sw.Stop(); 
                Console.WriteLine(string.Format("{0} runn {1} testing {2} times total spand {3} milliseconds", Enum.GetName( Orm.GetType(), Orm), Enum.GetName(Per.GetType(), Per), Times, sw.ElapsedMilliseconds)); 
            }).BeginInvoke(null, null);
        }

        static void Test(OrmType Orm, PerformanceType Per, int TestTimes,int ExecuteTimes)
        {
            new Action(() =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();



                sw.Stop();
                Console.WriteLine(string.Format("{0} runn {1} testing {2} times total spand {3} milliseconds", Enum.GetName(Orm.GetType(), Orm), Enum.GetName(Per.GetType(), Per), TestTimes, sw.ElapsedMilliseconds));
            }).BeginInvoke(null, null);
        }


        static void Execute(string TesstName, int Times, Action TestFunc)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < Times; i++)
                TestFunc();
            sw.Stop();
            Console.WriteLine(string.Format("{0} run {1} times spand {2} milliseconds", TesstName, Times, sw.ElapsedMilliseconds));
        }
    }

    public enum OrmType
    {
        SqlSugar,
        Dapper,
        EF,
        ODA,
    }
    enum PerformanceType
    {
        ReadData,
        Paging, 
        Sql,
        ById

    }
}
