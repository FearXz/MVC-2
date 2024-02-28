using MVC_2.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Web;
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

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            string connectionString = ConfigurationManager.ConnectionStrings["MyDb"].ToString();
            SqlConnection conn = new SqlConnection(connectionString);
            Scarpa s = new Scarpa();

            try
            {
                conn.Open();

                string query = "SELECT * FROM Scarpe WHERE Attivo=1 AND IdScarpa = @IdScarpe";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@IdScarpe", id);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    s.IdScarpa = Convert.ToInt32(reader["IdScarpa"]);
                    s.Nome = reader["Nome"].ToString();
                    s.Descrizione = reader["Descrizione"].ToString();
                    s.Prezzo = Convert.ToDecimal(reader["Prezzo"]);
                    s.ImmagineCopertina = reader["ImmagineCopertina"].ToString();
                    s.ImmagineNome = reader["ImmagineNome"].ToString();
                    s.Attivo = Convert.ToBoolean(reader["Attivo"]);
                }
                reader.Close();

                string queryImmagini = "SELECT * FROM ImmaginiXtra WHERE IdScarpa = @IdScarpa";
                SqlCommand cmdImmagini = new SqlCommand(queryImmagini, conn);
                cmdImmagini.Parameters.AddWithValue("@IdScarpa", s.IdScarpa);

                SqlDataReader readerImmagini = cmdImmagini.ExecuteReader();

                while (readerImmagini.Read())
                {
                    ImmagineXtra img = new ImmagineXtra();
                    img.IdImmagine = Convert.ToInt32(readerImmagini["IdImmagine"]);
                    img.IdScarpa = Convert.ToInt32(readerImmagini["IdScarpa"]);
                    img.PercorsoImmagine = readerImmagini["PercorsoImmagine"].ToString();

                    s.ListaImmagini.Add(img);
                }
                readerImmagini.Close();


            }
            catch (Exception ex)
            {
                Response.Write($"Errore durante il recupero dei dati: {ex.Message}");
            }
            finally
            {
                conn.Close();
            }

            return View(s);
        }

        public ActionResult CreateProduct()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateProduct(Scarpa s, HttpPostedFileBase ImmagineCopertina)
        {
            if (ImmagineCopertina != null)
            {

                var fileName = Path.GetFileName(ImmagineCopertina.FileName);
                var path = Path.Combine("~/Content/Img", fileName);
                var absolutePath = Server.MapPath(path);
                ImmagineCopertina.SaveAs(absolutePath);

                string connectionString = ConfigurationManager.ConnectionStrings["MyDb"].ToString();
                SqlConnection conn = new SqlConnection(connectionString);

                try
                {
                    conn.Open();

                    string query = "INSERT INTO Scarpe (Nome, Descrizione, Prezzo, ImmagineCopertina) VALUES (@Nome, @Descrizione, @Prezzo, @ImmagineCopertina)";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Nome", s.Nome);
                    cmd.Parameters.AddWithValue("@Descrizione", s.Descrizione);
                    cmd.Parameters.AddWithValue("@Prezzo", s.Prezzo);
                    cmd.Parameters.AddWithValue("@ImmagineCopertina", path);

                    cmd.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    Response.Write($"Errore durante il recupero dei dati: {ex.Message}");
                }
                finally
                {
                    conn.Close();
                }
            }
            else
            {
                Response.Write("Errore durante il caricamento dell'immagine");
            }
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            string connectionString = ConfigurationManager.ConnectionStrings["MyDb"].ToString();
            SqlConnection conn = new SqlConnection(connectionString);

            try
            {
                conn.Open();

                string query = "UPDATE Scarpe SET Attivo = 0 WHERE IdScarpa = @IdScarpa";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@IdScarpa", s.IdScarpa);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Response.Write($"Errore durante il recupero dei dati: {ex.Message}");
            }
            finally
            {
                conn.Close();
            }

            return View();
        }
    }
}