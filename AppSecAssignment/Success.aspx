<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Success.aspx.cs" Inherits="AppSecAssignment.Success" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Success</title>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/5.0.0-beta1/css/bootstrap.min.css" integrity="sha512-thoh2veB35ojlAhyYZC0eaztTAUhxLvSZlWrNtlV01njqs/UdY3421Jg7lX0Gq9SRdGVQeL8xeBp9x1IPyL1wQ==" crossorigin="anonymous" />
    <script src="https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/5.0.0-beta1/js/bootstrap.min.js" integrity="sha512-ZvbjbJnytX9Sa03/AcbP/nh9K95tar4R0IAxTS2gh2ChiInefr1r7EpnVClpTWUEN7VarvEsH3quvkY1h0dAFg==" crossorigin="anonymous"></script>
</head>
<body>
    
    <form id="form1" runat="server">
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
                  <asp:LinkButton ID="LinkButton1" runat="server" OnClick="btn_logout_Click">Logout</asp:LinkButton>
              </div>
            </div>
          </div>
        </nav>
        <div>
            <div>
            <h2>User Profile</h2>
            <br />
            User ID : <asp:Label ID="lbl_userID" runat="server"></asp:Label>
            <br />
                <br />
                First Name : <asp:Label ID="lbl_firstName" runat="server"></asp:Label>
            <br />
            <br />
                Last Name : <asp:Label ID="lbl_lastName" runat="server"></asp:Label>
                <br />
            <br />
                Date of Birth : <asp:Label ID="lbl_DOB" runat="server"></asp:Label>
                <br />
            <br />
            Credit Card : <asp:Label ID="lbl_creditCard" runat="server"></asp:Label>
                <br />
                <br />
                <asp:Button ID="btn_changePwd" runat="server" Text="Change Password" OnClick="btn_changePwd_Click" CssClass="btn btn-secondary" />
&nbsp;&nbsp;&nbsp;
                <asp:Button ID="btn_logout" runat="server" OnClick="btn_logout_Click" Text="Logout" CssClass="btn btn-secondary" />
            </div>
        </div>
    </form>
</body>
</html>
