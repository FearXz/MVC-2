using MVC_2.Models;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace MVC_2.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Utente u)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyDb"].ToString();
            SqlConnection conn = new SqlConnection(connectionString);

            try
            {
                conn.Open();

                string query = "SELECT * FROM Utenti WHERE Username = @Username AND Password=@Password";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Username", u.Username);
                cmd.Parameters.AddWithValue("@Password", u.Password);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    Utente utente = new Utente();
                    utente.IdUtente = Convert.ToInt32(reader["IdUtente"]);
                    utente.Username = reader["Username"].ToString();
                    utente.Nome = reader["Nome"].ToString();
                    utente.Cognome = reader["Cognome"].ToString();
                    utente.Email = reader["Email"].ToString();
                    utente.TipoUtente = (bool)reader["TipoUtente"];
                    Session["Utente"] = utente;
                }
                else
                {
                    return RedirectToAction("Login");
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

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Logout()
        {
            Session["Utente"] = null;
            return RedirectToAction("Index", "Home");
        }
    }
}