<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="AppSecAssignment.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Login</title>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/5.0.0-beta1/css/bootstrap.min.css" integrity="sha512-thoh2veB35ojlAhyYZC0eaztTAUhxLvSZlWrNtlV01njqs/UdY3421Jg7lX0Gq9SRdGVQeL8xeBp9x1IPyL1wQ==" crossorigin="anonymous" />
    <script src="https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/5.0.0-beta1/js/bootstrap.min.js" integrity="sha512-ZvbjbJnytX9Sa03/AcbP/nh9K95tar4R0IAxTS2gh2ChiInefr1r7EpnVClpTWUEN7VarvEsH3quvkY1h0dAFg==" crossorigin="anonymous"></script>
</head>
<body>
    <nav class="navbar navbar-expand-lg navbar-dark bg-dark">
      <div class="container-fluid">
        <a class="navbar-brand" href="#">SITConnect</a>
        <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarSupportedContent" aria-controls="navbarSupportedContent" aria-expanded="false" aria-label="Toggle navigation">
          <span class="navbar-toggler-icon"></span>
        </button>
        <div class="collapse navbar-collapse" id="navbarSupportedContent">
          <ul class="navbar-nav me-auto mb-2 mb-lg-0">
            <li class="nav-item">
              <a class="nav-link active" href="#">Home</a>
            </li>
          </ul>
          <div class="d-flex">
            <a class="me-2 nav-link active" href="Registration.aspx">Register</a>
            <a class="nav-link" href="#">Login</a>
          </div>
        </div>
      </div>
    </nav>
    <form id="form1" runat="server"  defaultbutton="btnSubmit">
        <div class="container-fluid">
                <h2>Login</h2>
                <asp:Label ID="lblErrorMsg" runat="server" EnableViewState="false" ></asp:Label>
                <p>Username : <asp:TextBox ID="tb_userid" runat="server" Height="25px" Width="137px"/></p>
                <p>Password : <asp:TextBox ID="tb_pwd" runat="server" Height="24px" Width="137px" TextMode="Password" /></p>
                <p>
                    <asp:Button ID="btn_register" runat="server" Text="Register" OnClick="btn_register_Click" CssClass="btn btn-primary" />
&nbsp;&nbsp; <asp:Button ID="btnSubmit" runat="server" Text="Login" OnClick="btnSubmit_Click" CssClass="btn btn-primary" />
                <br />
                <br />

                <input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response"/>
                <br />
                <asp:Label ID="lbl_gScore" runat="server"></asp:Label>
                </p>
        </div>
    </form>
        <script src="https://www.google.com/recaptcha/api.js?render=APIKEY"></script>

        <script>
        grecaptcha.ready(function () {
            grecaptcha.execute('APIKEY', { action: 'Login' }).then(function (token) {
                document.getElementById("g-recaptcha-response").value = token;
            });
        });
    </script>
</body>
</html>
