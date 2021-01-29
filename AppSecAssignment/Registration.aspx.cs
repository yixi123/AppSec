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
using System.Drawing;

namespace AppSecAssignment
{
    public partial class Registration : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        static string finalHash;
        static string salt;
        byte[] Key;
        byte[] IV;
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
            ("https://www.google.com/recaptcha/api/siteverify?secret=6LeHeeUZAAAAAPnxEN-P4bd0dy531fUV6TBywIwY&response=" + captchaResponse);

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
            bool notEmpty = true;
            lblErrorMsgFirstName.Text = "";
            if (String.IsNullOrEmpty(tb_firstName.Text.Trim())){
                lblErrorMsgFirstName.Text = "Field is Empty";
                lblErrorMsgFirstName.ForeColor = System.Drawing.Color.Red;
                notEmpty = false;
            }
            lblErrorMsgLastName.Text = "";
            if (String.IsNullOrEmpty(tb_lastName.Text.Trim()))
            {
                lblErrorMsgLastName.Text = "Field is Empty";
                lblErrorMsgLastName.ForeColor = System.Drawing.Color.Red;
                notEmpty = false;
            }
            lblErrorMsgCreditCard.Text = "";
            if (String.IsNullOrEmpty(tb_creditCard.Text.Trim()))
            {
                lblErrorMsgCreditCard.Text = "Field is Empty";
                lblErrorMsgCreditCard.ForeColor = System.Drawing.Color.Red;
                notEmpty = false;
            }
            lblErrorMsgEmail.Text = "";
            if (String.IsNullOrEmpty(tb_email.Text.Trim()))
            {
                lblErrorMsgEmail.Text = "Field is Empty";
                lblErrorMsgEmail.ForeColor = System.Drawing.Color.Red;
                notEmpty = false;
            }
            lbl_pwdchecker.Text = "";
            if (String.IsNullOrEmpty(tb_pwd.Text.Trim()))
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
            lblErrorMsgDOB.Text = "";
            if (String.IsNullOrEmpty(tb_DOB.Text.Trim()))
            {
                lblErrorMsgDOB.Text = "Field is Empty";
                lblErrorMsgDOB.ForeColor = System.Drawing.Color.Red;
                notEmpty = false;
            }


            if (tb_firstName.Text.Trim().Length > 49)
            {
                lblErrorMsgFirstName.Text = "Length of first name should be less than 50 characters";
                lblErrorMsgFirstName.ForeColor = System.Drawing.Color.Red;
                notEmpty = false;
            }

            if (tb_lastName.Text.Trim().Length > 49)
            {
                lblErrorMsgLastName.Text = "Length of last name should be less than 50 characters";
                lblErrorMsgLastName.ForeColor = System.Drawing.Color.Red;
                notEmpty = false;
            }

            if (tb_email.Text.Trim().Length > 99)
            {
                lblErrorMsgEmail.Text = "Length of email should be less than 100 characters";
                lblErrorMsgEmail.ForeColor = System.Drawing.Color.Red;
                notEmpty = false;
            }

            if (notEmpty)
            {
                //string pwd = get value from your Textbox
                string pwd = tb_pwd.Text.ToString().Trim();
                int scores = checkPassword(pwd);
                string cfmpwd = tb_cfmpwd.Text.ToString().Trim();

                bool unique = true;
                SqlConnection connection = new SqlConnection(MYDBConnectionString);
                string sql = "select Email FROM Account WHERE Email=@USERID";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@USERID", tb_email.Text.Trim());
                try
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            if (reader["Email"] != null)
                            {
                                unique = false;
                                break;
                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.ToString());
                }
                finally { connection.Close(); }
                lblErrorMsgEmail.Text = "";
                lblErrorMsgCfmPwd.Text = "";
                lbl_pwdchecker.Text = "";
                bool valid = true;
                if (!unique)
                {
                    lblErrorMsgEmail.Text = "Email is already in use!";
                    lblErrorMsgEmail.ForeColor = System.Drawing.Color.Red;
                }
                if (pwd != cfmpwd)
                {
                    lblErrorMsgCfmPwd.Text = "Password not the same";
                    lblErrorMsgCfmPwd.ForeColor = System.Drawing.Color.Red;
                }
                if (scores < 5)
                {
                    lbl_pwdchecker.Text = "Password not Strong";
                    lbl_pwdchecker.ForeColor = System.Drawing.Color.Red;
                }
                try { Convert.ToDateTime(tb_DOB.Text.Trim()); }
                catch {
                    lblErrorMsgDOB.Text = "Invalid Date";
                    lblErrorMsgDOB.ForeColor = System.Drawing.Color.Red;
                    valid = false;
                }
                if (unique && pwd == cfmpwd && scores >= 5)
                {
                    //Generate random "salt"
                    RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                    byte[] saltByte = new byte[8];
                    //Fills array of bytes with a cryptographically strong sequence of random values.
                    rng.GetBytes(saltByte);
                    salt = Convert.ToBase64String(saltByte);
                    SHA512Managed hashing = new SHA512Managed();
                    string pwdWithSalt = pwd + salt;
                    byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                    finalHash = Convert.ToBase64String(hashWithSalt);
                    RijndaelManaged cipher = new RijndaelManaged();
                    cipher.GenerateKey();
                    Key = cipher.Key;
                    IV = cipher.IV;
                    createAccount();
                }
                
            }
        }
        public void createAccount()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO Account VALUES(@Email, @FirstName, @LastName, @CreditCard, @DOB, @PasswordHash, @PasswordSalt, @IV, @Key, @PasswordAge, null, null, null, null, 0, null)"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@Email", tb_email.Text.Trim());
                            cmd.Parameters.AddWithValue("@FirstName", tb_firstName.Text.Trim());
                            cmd.Parameters.AddWithValue("@LastName", tb_lastName.Text.Trim());
                            cmd.Parameters.AddWithValue("@CreditCard", Convert.ToBase64String(encryptData(tb_creditCard.Text.Trim())));
                            cmd.Parameters.AddWithValue("@DOB", Convert.ToDateTime(tb_DOB.Text.Trim()));
                            cmd.Parameters.AddWithValue("@PasswordHash", finalHash);
                            cmd.Parameters.AddWithValue("@PasswordSalt", salt);
                            cmd.Parameters.AddWithValue("@IV", Convert.ToBase64String(IV));
                            cmd.Parameters.AddWithValue("@Key", Convert.ToBase64String(Key));
                            cmd.Parameters.AddWithValue("@PasswordAge", DateTime.Now);
                            cmd.Connection = con;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                }
                Response.Redirect("Login.aspx", false);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        protected byte[] encryptData(string data)
        {
            byte[] cipherText = null;
            try
            {
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.IV = IV;
                cipher.Key = Key;
                ICryptoTransform encryptTransform = cipher.CreateEncryptor();
                //ICryptoTransform decryptTransform = cipher.CreateDecryptor();
                byte[] plainText = Encoding.UTF8.GetBytes(data);
                cipherText = encryptTransform.TransformFinalBlock(plainText, 0,
               plainText.Length);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { }
            return cipherText;
        }

        protected void btn_back_Click(object sender, EventArgs e)
        {
            Response.Redirect("Login.aspx",false);
        }
    }
}