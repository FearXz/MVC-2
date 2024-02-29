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
        [HttpPost]
        public ActionResult Details(int? productId, HttpPostedFileBase ImmagineXtra)
        {
            if (ImmagineXtra != null)
            {

                var fileName = Path.GetFileName(ImmagineXtra.FileName);
                var path = Path.Combine("~/Content/Img", fileName);
                var absolutePath = Server.MapPath(path);
                ImmagineXtra.SaveAs(absolutePath);

                string connectionString = ConfigurationManager.ConnectionStrings["MyDb"].ToString();
                SqlConnection conn = new SqlConnection(connectionString);

                try
                {
                    conn.Open();

                    string query = "INSERT INTO ImmaginiXtra (idScarpa,PercorsoImmagine) VALUES (@idScarpe, @PercorsoImmagine)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@idScarpe", productId);
                    cmd.Parameters.AddWithValue("@PercorsoImmagine", path);

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
            return RedirectToAction("Details", new { id = productId });
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
                cmd.Parameters.AddWithValue("@IdScarpa", id);

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

            return RedirectToAction("Index");
        }

        public ActionResult DeleteImgExtra(int id, int productId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyDb"].ToString();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = "DELETE FROM ImmaginiXtra WHERE IdImmagine = @IdImmagine";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@IdImmagine", id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write($"Errore durante il recupero dei dati: {ex.Message}");
            }

            return RedirectToAction("Details", new { id = productId });
        }

        public ActionResult Edit(int? id)
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

                }
                reader.Close();
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
        [HttpPost]
        public ActionResult Edit(Scarpa s, int productId, HttpPostedFileBase ImmagineCopertina)
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

                    string query = "UPDATE Scarpe SET Nome = @Nome, Descrizione = @Descrizione, Prezzo = @Prezzo, ImmagineCopertina = @ImmagineCopertina WHERE IdScarpa = @IdScarpa";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Nome", s.Nome);
                    cmd.Parameters.AddWithValue("@Descrizione", s.Descrizione);
                    cmd.Parameters.AddWithValue("@Prezzo", s.Prezzo);
                    cmd.Parameters.AddWithValue("@ImmagineCopertina", path);
                    cmd.Parameters.AddWithValue("@IdScarpa", productId);

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
            else if (ImmagineCopertina == null)
            {
                string connectionString = ConfigurationManager.ConnectionStrings["MyDb"].ToString();
                SqlConnection conn = new SqlConnection(connectionString);

                try
                {
                    conn.Open();

                    string query = "UPDATE Scarpe SET Nome = @Nome, Descrizione = @Descrizione, Prezzo = @Prezzo WHERE IdScarpa = @IdScarpa";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Nome", s.Nome);
                    cmd.Parameters.AddWithValue("@Descrizione", s.Descrizione);
                    cmd.Parameters.AddWithValue("@Prezzo", s.Prezzo);
                    cmd.Parameters.AddWithValue("@IdScarpa", productId);

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
            return RedirectToAction("Edit", new { id = productId });
        }

        [HttpPost]
        public JsonResult UpdateCoverImage(int idScarpa, string nuovaImmagineCopertina)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyDb"].ToString();
            SqlConnection conn = new SqlConnection(connectionString);

            try
            {
                conn.Open();

                string query = "UPDATE Scarpe SET ImmagineCopertina = @ImmagineCopertina WHERE IdScarpa = @IdScarpa";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ImmagineCopertina", nuovaImmagineCopertina);
                cmd.Parameters.AddWithValue("@IdScarpa", idScarpa);

                cmd.ExecuteNonQuery();

                return Json(new { success = true, message = "Immagine copertina aggiornata con successo" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Errore durante l'aggiornamento dell'immagine copertina: {ex.Message}" }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                conn.Close();
            }
        }


    }
}