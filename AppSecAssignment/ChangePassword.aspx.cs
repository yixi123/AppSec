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
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace AppSecAssignment
{
    public partial class ChangePassword : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        static string finalHash;
        static string salt;
        string userID = null;
        string currentHash;
        string currentSalt;
        string h1;
        string s1;
        string h2;
        string s2;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] != null && Session["AuthToken"] != null && Request.Cookies["AuthToken"] != null)
            {
                if (!Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                {
                    Response.Redirect("Login.aspx", false);
                }
                else
                {
                    userID = (string)Session["userID"];
                    lblUserId.Text = userID;
                    DateTime pwdAge;
                    DateTime.TryParse(getPwdhistory(userID, "PasswordAge"), out pwdAge);
                    if (pwdAge.AddMinutes(15) < DateTime.Now)
                    {
                        lblMsg.Text = "Must change password after 15 mins";
                        lblMsg.ForeColor = System.Drawing.Color.Red;
                    } else if (pwdAge.AddMinutes(5) > DateTime.Now)
                    {
                        lblMsg.Text = "Cannot change password within 5 mins from the last change of password";
                        lblMsg.ForeColor = System.Drawing.Color.Red;
                    } else
                    {
                        lblMsg.Text = "Password can be change";
                        lblMsg.ForeColor = System.Drawing.Color.Green;
                    }
                }
            }
            else
            {
                Response.Redirect("Login.aspx", false);
            }
        }

        protected void btn_back_Click(object sender, EventArgs e)
        {
            Response.Redirect("Success.aspx");
        }

        private int checkPassword(string password)
        {
            int score = 0;

            if (password.Length < 8)
            {
                return 1;
            }
            else
            {
                score = 1;
            }

            if (Regex.IsMatch(password, "[a-z]"))
            {
                score++;
            }

            if (Regex.IsMatch(password, "[A-Z]"))
            {
                score++;
            }

            if (Regex.IsMatch(password, "[0-9]"))
            {
                score++;
            }

            if (Regex.IsMatch(password, "[^a-zA-Z0-9]"))
            {
                score++;
            }
            return score;
        }

        protected void btn_submit_Click(object sender, EventArgs e)
        {
            lblErrorMsgCfmPwd.Text = "";
            lbl_pwdchecker.Text = "";
            lblErrorMsgPwd.Text = "";
            lblMsg.Text = "";
            DateTime pwdAge; 
            DateTime.TryParse(getPwdhistory(userID, "PasswordAge"), out pwdAge);
            if (pwdAge.AddMinutes(5) > DateTime.Now)
            {
                lblMsg.Text = "Cannot change password within 5 mins from the last change of password";
                lblMsg.ForeColor = System.Drawing.Color.Red;
            }
            else
            {
                bool notEmpty = true;
                lblErrorMsgPwd.Text = "";
                if (String.IsNullOrEmpty(tb_pwd.Text.Trim()))
                {
                    lblErrorMsgPwd.Text = "Field is Empty";
                    lblErrorMsgPwd.ForeColor = System.Drawing.Color.Red;
                    notEmpty = false;
                }
                lbl_pwdchecker.Text = "";
                if (String.IsNullOrEmpty(tb_newpwd.Text.Trim()))
                {
                    lbl_pwdchecker.Text = "Field is Empty";
                    lbl_pwdchecker.ForeColor = System.Drawing.Color.Red;
                    notEmpty = false;
                }
                lblErrorMsgCfmPwd.Text = "";
                if (String.IsNullOrEmpty(tb_cfmpwd.Text.Trim()))
                {
                    lblErrorMsgCfmPwd.Text = "Field is Empty";
                    lblErrorMsgCfmPwd.ForeColor = System.Drawing.Color.Red;
                    notEmpty = false;
                }


                if (notEmpty)
                {
                    //string pwd = get value from your Textbox
                    string pwd = tb_pwd.Text.ToString().Trim();
                    string newpwd = tb_newpwd.Text.ToString().Trim();
                    int scores = checkPassword(newpwd);
                    string cfmpwd = tb_cfmpwd.Text.ToString().Trim();

                    currentHash = getDBHash(userID);
                    currentSalt = getDBSalt(userID);
                    bool valid = validatePassword(currentHash, currentSalt, pwd);

                    if (valid && newpwd == cfmpwd && scores >= 5 && newpwd != pwd)
                    {
                        h1 = getPwdhistory(userID, "PasswordHashHistory1");
                        s1 = getPwdhistory(userID, "PasswordSaltHistory1");
                        h2 = getPwdhistory(userID, "PasswordHashHistory2");
                        s2 = getPwdhistory(userID, "PasswordSaltHistory2");

                        bool valid1 = validatePassword(h1, s1, newpwd);
                        bool valid2 = validatePassword(h2, s2, newpwd);
                        if (valid1 || valid2)
                        {
                            lbl_pwdchecker.Text = "You should not use the same password as previous 2 password you used";
                            lbl_pwdchecker.ForeColor = System.Drawing.Color.Red;
                        }
                        else
                        {
                            SHA512Managed hashing = new SHA512Managed();
                            //Generate random "salt"
                            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                            byte[] saltByte = new byte[8];
                            //Fills array of bytes with a cryptographically strong sequence of random values.
                            rng.GetBytes(saltByte);
                            salt = Convert.ToBase64String(saltByte);
                            string pwdWithSalt = newpwd + salt;
                            byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                            finalHash = Convert.ToBase64String(hashWithSalt);
                            updatePassword();
                        }

                    }
                    if (newpwd != cfmpwd)
                    {
                        lblErrorMsgCfmPwd.Text = "Password not the same";
                        lblErrorMsgCfmPwd.ForeColor = System.Drawing.Color.Red;
                    }
                    if (scores < 5)
                    {
                        lbl_pwdchecker.Text = "Password not Strong";
                        lbl_pwdchecker.ForeColor = System.Drawing.Color.Red;
                    }
                    if (!valid)
                    {
                        lblErrorMsgPwd.Text = "Password is incorrect. Please try again.";
                        lblErrorMsgPwd.ForeColor = System.Drawing.Color.Red;
                    }
                    else if (newpwd == pwd)
                    {
                        lbl_pwdchecker.Text = "New Password should not be same as current password";
                        lbl_pwdchecker.ForeColor = System.Drawing.Color.Red;
                    }
                }
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
        public void updatePassword()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE Account SET [PasswordHash]=@PasswordHash, [PasswordSalt]=@PasswordSalt," +
                        "[PasswordHashHistory1]=@PasswordHashHistory1, [PasswordSaltHistory1]=@PasswordSaltHistory1," +
                        "[PasswordHashHistory2]=@PasswordHashHistory2, [PasswordSaltHistory2]=@PasswordSaltHistory2, [PasswordAge]=@PasswordAge WHERE [Email]=@USERID"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@PasswordHash", finalHash);
                            cmd.Parameters.AddWithValue("@PasswordSalt", salt);
                            cmd.Parameters.AddWithValue("@PasswordHashHistory1", currentHash);
                            cmd.Parameters.AddWithValue("@PasswordSaltHistory1", currentSalt);
                            cmd.Parameters.AddWithValue("@PasswordHashHistory2", h1 == null ? "null" : h1);
                            cmd.Parameters.AddWithValue("@PasswordSaltHistory2", s1 == null ? "null" : s1);
                            cmd.Parameters.AddWithValue("@PasswordAge", DateTime.Now);
                            cmd.Parameters.AddWithValue("@USERID", userID);
                            cmd.Connection = con;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                }
                ScriptManager.RegisterStartupScript(this, this.GetType(),
                "Alert",
                "alert('Password change sucessfully');window.location ='Success.aspx';",
                true);

                //Response.Redirect("Success.aspx", false);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        protected string getPwdhistory(string userid,string item)
        {
            string s = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select "+item+" FROM ACCOUNT WHERE Email=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader[item] != null)
                        {
                            if (reader[item] != DBNull.Value)
                            {
                                s = reader[item].ToString();
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
        protected bool validatePassword(string dbHash, string dbSalt,string password)
        {
            bool valid = false;
            SHA512Managed hashing = new SHA512Managed();
            try
            {
                if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                {
                    string pwdWithSalt = password + dbSalt;
                    byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                    string userHash = Convert.ToBase64String(hashWithSalt);
                    if (userHash.Equals(dbHash))
                    {
                        valid = true;
                        
                    }
                }
                return valid;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally {  }
        }

        protected void btn_logout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();
            Response.Redirect("Login.aspx", false);

            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);
            }
            if (Request.Cookies["AuthToken"] != null)
            {
                Response.Cookies["AuthToken"].Value = string.Empty;
                Response.Cookies["AuthToken"].Expires = DateTime.Now.AddMonths(-20);
            }
        }
    }
}