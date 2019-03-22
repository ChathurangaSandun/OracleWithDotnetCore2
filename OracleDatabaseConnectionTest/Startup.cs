using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;

namespace OracleDatabaseConnectionTest
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }


            app.Run(async (context) =>
            {
                //Demo: Basic ODP.NET Core application for ASP.NET Core
                // to connect, query, and return results to a web page

                //Create a connection to Oracle			
                string conString = "User Id=xxx;Password=xxx;" +

                //How to connect to an Oracle DB without SQL*Net configuration file
                //  also known as tnsnames.ora.
                "Data Source=xxxxx:xxxx/xxxx;";

                //How to connect to an Oracle DB with a DB alias.
                //Uncomment below and comment above.
                //"Data Source=<service name alias>;";

                using (OracleConnection con = new OracleConnection(conString))
                {
                    using (OracleCommand cmd = con.CreateCommand())
                    {
                        try
                        {
                            con.Open();
                            cmd.BindByName = true;

                            // call sp and get data.
                            OracleCommand objCmd = new OracleCommand();

                            objCmd.Connection = con;

                            objCmd.CommandText = "TESTCHATHURANNGASP";

                            objCmd.CommandType = CommandType.StoredProcedure;
                            objCmd.Parameters.Add("customers", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                            OracleDataReader reader = objCmd.ExecuteReader();
                            while (reader.Read())
                            {
                                await context.Response.WriteAsync("sp first row " + reader.GetString(0) + "\n");
                            }

                            reader.Dispose();
                        }
                        catch (Exception ex)
                        {
                            await context.Response.WriteAsync(ex.Message);
                        }
                    }
                }

            });


            app.UseHttpsRedirection();
            app.UseMvc();
        }

       

    }

    internal class SPParameter
    {
        public SPParameter()
        {
        }

        public string Name { get; set; }
        public OracleDbType DataType { get; set; }
        public ParameterDirection Direction { get; set; }
        public DateTime Value { get; set; }
    }
}
