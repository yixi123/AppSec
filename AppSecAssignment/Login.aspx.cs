using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace AppSecAssignment
{
    public partial class Login : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] != null && Session["AuthToken"] != null && Request.Cookies["AuthToken"] != null)
            {
                if (Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                {
                    Response.Redirect("Success.aspx", false);
                }

            }
        }

        public bool ValidateCaptcha()
        {
            bool result = true;

            string captchaResponse = Request.Form["g-recaptcha-response"];

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create
            ("https://www.google.com/recaptcha/api/siteverify?secret=APIKEY&response=" + captchaResponse);

            try
            {
                //Codes to receive the Response in JSON format from Google Server
                using (WebResponse wResponse = req.GetResponse())
                {
                    using (StreamReader readStream = new StreamReader(wResponse.GetResponseStream()))
                    {
                        string jsonResponse = readStream.ReadToEnd();
                        lbl_gScore.Text = jsonResponse.ToString();
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        MyObject jsonObject = js.Deserialize<MyObject>(jsonResponse);
                        result = Convert.ToBoolean(jsonObject.success);
                    }
                }
                return result;
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }
        public class MyObject
        {
            public string success { get; set; }
            public List<string> ErrorMessage { get; set; }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string pwd = tb_pwd.Text.ToString().Trim();
            string userid = tb_userid.Text.ToString().Trim();
            DateTime loginFailureTime = getLoginFailureTime(userid);
            if(loginFailureTime.AddMinutes(20) < DateTime.Now)
            {
                updateLoginFailure(userid, 0);
            }

            int loginFailure = int.Parse(getLoginFailure(userid));
            if(loginFailure >= 3)
            {
                lblErrorMsg.Text = "Account have been lockout. Please try again later.";
                lblErrorMsg.ForeColor = System.Drawing.Color.Red;
            }
            else
            {
                SHA512Managed hashing = new SHA512Managed();
                string dbHash = getDBHash(userid);
                string dbSalt = getDBSalt(userid);
                try
                {
                    if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                    {
                        string pwdWithSalt = pwd + dbSalt;
                        byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                        string userHash = Convert.ToBase64String(hashWithSalt);
                        if (userHash.Equals(dbHash))
                        {
                            Session["UserID"] = userid;
                            string guid = Guid.NewGuid().ToString();
                            Session["AuthToken"] = guid;
                            Response.Cookies.Add(new HttpCookie("AuthToken", guid));
                            
                            updateLoginFailure(userid, 0);
                            Response.Redirect("Success.aspx", false);
                        }
                        else
                        {
                            updateLoginFailure(userid, loginFailure+1);
                            lblErrorMsg.Text = "Userid or password is not valid. Please try again.";
                            lblErrorMsg.ForeColor = System.Drawing.Color.Red;

                        }
                    }
                    else
                    {
                        lblErrorMsg.Text = "Userid or password is not valid. Please try again.";
                        lblErrorMsg.ForeColor = System.Drawing.Color.Red;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.ToString());
                }
                finally { }
            }
            
        }

        protected string getDBHash(string userid)
        {
            string h = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select PasswordHash FROM Account WHERE Email=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["PasswordHash"] != null)
                        {
                            if (reader["PasswordHash"] != DBNull.Value)
                            {
                                h = reader["PasswordHash"].ToString();
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return h;
        }
        protected string getDBSalt(string userid)
        {
            string s = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select PASSWORDSALT FROM ACCOUNT WHERE Email=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["PASSWORDSALT"] != null)
                        {
                            if (reader["PASSWORDSALT"] != DBNull.Value)
                            {
                                s = reader["PASSWORDSALT"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return s;
        }

        protected void btn_register_Click(object sender, EventArgs e)
        {
            Response.Redirect("Registration.aspx");
        }

        protected string getLoginFailure(string userid)
        {
            string s = "0";
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select LoginFailure FROM ACCOUNT WHERE Email=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["LoginFailure"] != null)
                        {
                            if (reader["LoginFailure"] != DBNull.Value)
                            {
                                s = reader["LoginFailure"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return s;
        }
        protected DateTime getLoginFailureTime(string userid)
        {
            DateTime s = new DateTime();
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select LoginFailureTime FROM ACCOUNT WHERE Email=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["LoginFailureTime"] != null)
                        {
                            if (reader["LoginFailureTime"] != DBNull.Value)
                            {
                                DateTime.TryParse(reader["LoginFailureTime"].ToString(), out s);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return s;
        }

        protected void updateLoginFailure(string userid, int loginFailure)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE Account SET [LoginFailure]=@LoginFailure, [LoginFailureTime]=@LoginFailureTime WHERE [Email]=@USERID"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@LoginFailure", loginFailure.ToString());
                            cmd.Parameters.AddWithValue("@LoginFailureTime", DateTime.Now);
                            cmd.Parameters.AddWithValue("@USERID", userid);
                            cmd.Connection = con;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

    }
}
