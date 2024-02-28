using MVC_2.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace MVC_2.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyDb"].ToString();
            SqlConnection conn = new SqlConnection(connectionString);

            List<Scarpa> listaScarpe = new List<Scarpa>();

            try
            {
                conn.Open();

                string query = "SELECT * FROM Scarpe WHERE Attivo=1";

                SqlCommand cmd = new SqlCommand(query, conn);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Scarpa s = new Scarpa();
                    s.IdScarpa = Convert.ToInt32(reader["IdScarpa"]);
                    s.Nome = reader["Nome"].ToString();
                    s.Descrizione = reader["Descrizione"].ToString();
                    s.Prezzo = Convert.ToDecimal(reader["Prezzo"]);
                    s.ImmagineCopertina = reader["ImmagineCopertina"].ToString();
                    s.ImmagineNome = reader["ImmagineNome"].ToString();
                    s.Attivo = Convert.ToBoolean(reader["Attivo"]);
                    listaScarpe.Add(s);
                }
            }
            catch (Exception ex)
            {
                Response.Write($"Errore durante il recupero dei dati: {ex.Message}");
            }
            finally
            {
                conn.Close();
            }
            return View(listaScarpe);
        }

    }
}